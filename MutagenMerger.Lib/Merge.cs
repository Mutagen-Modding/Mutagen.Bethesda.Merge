using System.Collections.Generic;
using Mutagen.Bethesda;

namespace MutagenMerger.Lib
{
    public static class MergeExtensions
    {
        public static void MergeMods<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
            this LoadOrder<IModListing<TModGetter>> loadOrder,
            ILinkCache<TMod, TModGetter> linkCache, List<ModKey> modsToMerge, TMod outputMod)
            where TMod : class, TModGetter, IMod
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable
            where TMajorRecord : class, TMajorRecordGetter, IMajorRecord
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            var processedKeys = new HashSet<FormKey>();
            
            foreach (var listing in loadOrder.PriorityOrder)
            {
                if (listing.Mod == null) continue;
                if (!listing.Enabled) continue;

                if (!modsToMerge.Contains(listing.ModKey)) continue;

                foreach (var rec in listing.Mod.EnumerateMajorRecordContexts<TMajorRecord, TMajorRecordGetter>(
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
                        rec.DuplicateIntoAsNewRecord(outputMod, rec.Record.EditorID);
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
