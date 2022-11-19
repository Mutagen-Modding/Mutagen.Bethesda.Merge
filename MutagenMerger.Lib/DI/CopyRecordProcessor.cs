using System;
using System.Collections.Generic;
using System.Linq;
using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using MutagenMerger.Lib.DI.GameSpecifications;

namespace MutagenMerger.Lib.DI;

public class CopyRecordProcessor<TMod, TModGetter>
    where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
    where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
{
    private readonly Dictionary<ObjectKey, ICopyOverride<TMod, TModGetter>> _copyOverrides;

    public CopyRecordProcessor(ICopyOverride<TMod, TModGetter>[] copyOverrides)
    {
        _copyOverrides = copyOverrides
            // .GroupBy(x => x.ObjectKey)
            // .ToDictionary(x => x.Key, x => x.First());
        .ToDictionary(x => x.ObjectKey, x => x);
    }

    public void CopyRecords(
        MergeState<TMod, TModGetter> mergeState)
    {
        foreach (var rec in mergeState.Mods.Where(mod => mergeState.ModsToMerge.Contains(mod.ModKey))
                     .WinningOverrideContexts<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>(mergeState
                         .LinkCache))
        {
            if (_copyOverrides.TryGetValue(rec.Record.Registration.ObjectKey, out var copyOverride))
            {
                copyOverride.HandleCopyFor(mergeState, rec);
            }
            else if (rec.ModKey == rec.Record.FormKey.ModKey)
            {
                DuplicateAsNewRecord(mergeState, rec);
            }
            else
            {
                CopyAsOverride(mergeState, rec);
            }

        }

        Console.WriteLine();
    }

    private void DuplicateAsNewRecord(
        MergeState<TMod, TModGetter> mergeState,
        IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> rec)
    {
        //record is not an override so we can just duplicate it
        var duplicated = rec.DuplicateIntoAsNewRecord(mergeState.OutgoingMod, rec.Record.EditorID);


        Console.WriteLine("          Renumbering Record [" + rec.Record.FormKey.ModKey.Name + "] " +
                          rec.Record.FormKey.IDString() + " to [" + mergeState.OutgoingMod.ModKey.Name + "] " +
                          duplicated.FormKey.IDString());
        mergeState.Mapping.Add(rec.Record.FormKey, duplicated.FormKey);
    }

    private void CopyAsOverride(
        MergeState<TMod, TModGetter> mergeState,
        IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> rec)
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
        if (mergeState.ModsToMerge.Contains(rec.Record.FormKey.ModKey))
        {
            if (mergeState.Mapping.ContainsKey(rec.Record.FormKey))
                throw new NotImplementedException();

            var duplicate = rec.DuplicateIntoAsNewRecord(mergeState.OutgoingMod, rec.Record.EditorID);
            Console.WriteLine("          Renumbering Record[" + rec.Record.FormKey.ModKey.Name + "] " +
                              rec.Record.FormKey.IDString() + " to [" + mergeState.OutgoingMod.ModKey.Name + "] " +
                              duplicate.FormKey.IDString());
            mergeState.Mapping.Add(rec.Record.FormKey, duplicate.FormKey);
        }
        else
        {
            Console.WriteLine("          Copying Override Record[" + rec.Record.FormKey.ModKey.Name + "] " +
                              rec.Record.FormKey.IDString());
            rec.GetOrAddAsOverride(mergeState.OutgoingMod);
        }
    }
}
