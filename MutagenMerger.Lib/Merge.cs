using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;

namespace MutagenMerger.Lib
{
    public static class MergeExtensions
    {
        public static void MergeMods<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
            this IEnumerable<TModGetter> mods,
            IEnumerable<ModKey> modsToMerge,
            TMod outputMod,
            out Dictionary<FormKey, FormKey> mapping)
            where TMod : class, TModGetter, IMod
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable
            where TMajorRecord : class, TMajorRecordGetter, IMajorRecord
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            //var processedKeys = new HashSet<FormKey>();

            // Just in case user gave us a lazy IEnumerable
            var modsCached = mods.ToArray();

            var modsToMergeKeys = modsToMerge.ToHashSet();

            var linkCache = modsCached.ToImmutableLinkCache();

            mapping = new Dictionary<FormKey, FormKey>();

            foreach (var rec in modsCached
                .WinningOverrideContexts<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(linkCache))
            {
                //don't deal with records from plugins we don't want to merge
                if (!modsToMergeKeys.Contains(rec.ModKey)) continue;
                
                if (rec.ModKey == rec.Record.FormKey.ModKey)
                {
                    //record is not an override so we can just duplicate it
                    var duplicated = rec.DuplicateIntoAsNewRecord(outputMod, rec.Record.EditorID);
                    mapping.Add(rec.Record.FormKey, duplicated.FormKey);
                }
                else
                {
                    /*
                     * Record is an override so we have to make sure that we add the override to the output mod
                     * if the original plugin is not to be merged or duplicate the record if we also want to merge
                     * the original plugin.
                     * We duplicate the overwritten record and not the original one because we are looping over all
                     * WinningOverrideContexts meaning we never get the original one if it's overwritten somewhere
                     * and we also don't want "outdated" records (only overrides in that case).
                     * This explanation probably still won't make sense but whatever.
                     */
                    if (modsToMergeKeys.Contains(rec.Record.FormKey.ModKey))
                    {
                        if (mapping.ContainsKey(rec.Record.FormKey))
                            throw new NotImplementedException();
                        
                        var duplicate = rec.DuplicateIntoAsNewRecord(outputMod, rec.Record.EditorID);
                        mapping.Add(rec.Record.FormKey, duplicate.FormKey);
                    }
                    else
                    {
                        rec.GetOrAddAsOverride(outputMod);
                    }
                }
            }
            
            outputMod.RemapLinks(mapping);
        }
    }
}
