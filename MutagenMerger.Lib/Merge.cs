using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;

namespace MutagenMerger.Lib
{
    public static class MergeExtensions
    {
        public static void MergeMods<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
            this IEnumerable<TModGetter> modsToMerge,
            ILinkCache<TMod, TModGetter> linkCache, 
            TMod outputMod,
            out HashSet<FormKey> brokenKeys)
            where TMod : class, TModGetter, IMod
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable
            where TMajorRecord : class, TMajorRecordGetter, IMajorRecord
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            var processedKeys = new HashSet<FormKey>();
            brokenKeys = new HashSet<FormKey>();

            // Just in case user gave us a lazy IEnumerable
            var modsCached = modsToMerge.ToArray();

            var modsToMergeKeys = modsCached.Select(x => x.ModKey).ToHashSet();
            
            foreach (var listing in modsCached)
            {
                if (!modsToMergeKeys.Contains(listing.ModKey)) continue;

                foreach (var rec in listing.EnumerateMajorRecordContexts<TMajorRecord, TMajorRecordGetter>(
                    linkCache))
                {
                    /*
                     * this is true when we add an override to the output mod and then encounter another override or
                     * the original record
                     */
                    if (processedKeys.Contains(rec.Record.FormKey)) continue;
                    
                    if (rec.ModKey == rec.Record.FormKey.ModKey)
                    {
                        //record comes from the current mod, we can duplicate it
                        try
                        {
                            rec.DuplicateIntoAsNewRecord(outputMod, rec.Record.EditorID);
                        }
                        catch (Exception)
                        {
                            brokenKeys.Add(rec.Record.FormKey);
                            //Debugger.Break();
                        }
                    }
                    else
                    {
                        //record does not come from the current mod and overrides a record from another mod
                        rec.GetOrAddAsOverride(outputMod);
                    }

                    processedKeys.Add(rec.Record.FormKey);
                }
            }
        }
    }
}
