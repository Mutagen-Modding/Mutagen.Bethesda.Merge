using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Loqui;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib
{
    internal static class Merge<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>,
        IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter
    {
        static SkyrimSpecifications skyrimSpecifications = new SkyrimSpecifications();

        // static Fallout4Specifications fallout4Specifications = new Fallout4Specifications();
        // static OblivionSpecifications oblivionSpecifications = new OblivionSpecifications();
        static IReadOnlyCollection<ObjectKey> blacklist = new SkyrimSpecifications().BlacklistedCopyTypes; //.Join();

        public static void DoMerge(MergeState<TMod, TModGetter> state)
        {
            CopyRecords(state);

            state.OutgoingMod.RemapLinks(state.Mapping);

            Directory.CreateDirectory(state.OutputPath.Directory ?? "");
            state.OutgoingMod.WriteToBinary(state.OutputPath, Utils.SafeBinaryWriteParameters);

            HandleAssets(state);

            HandleScripts(state);
        }
        
        public static void HandleScripts(
            MergeState<TMod, TModGetter> mergeState)
        {
            // Console.WriteLine("HandleScript");
        }

        public static void HandleAssets(MergeState<TMod, TModGetter> mergeState)
        {
            AssetMerge<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>.Handle(mergeState);
        }

        public static void CopyRecords(
            MergeState<TMod, TModGetter> mergeState)
        {
            foreach (var rec in mergeState.Mods
                         .WinningOverrideContexts<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(mergeState
                             .LinkCache))
            {
                if (blacklist.Contains(rec.Record.Registration.ObjectKey))
                {
                    HandleDeepCopy(mergeState, rec);
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

        public static void HandleDeepCopy(
            MergeState<TMod, TModGetter> mergeState,
            IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec)
        {
            switch (mergeState.Release)
            {
                case GameRelease.Fallout4:
                    break;
                case GameRelease.Oblivion:
                    break;
                default:
                    skyrimSpecifications.HandleCopyFor(
                        new MergeState<ISkyrimMod, ISkyrimModGetter>(
                            mergeState.Release,
                            mergeState.Mods.Cast<ISkyrimModGetter>().ToArray(),
                            mergeState.ModsToMerge,
                            (ISkyrimMod)mergeState.OutgoingMod,
                            mergeState.OutputPath,
                            mergeState.DataPath,
                            null! /* for now */),
                        (IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>)rec);
                    break;
            }
        }

        public static void DuplicateAsNewRecord(
            MergeState<TMod, TModGetter> mergeState,
            IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec)
        {
            //record is not an override so we can just duplicate it
            var duplicated = rec.DuplicateIntoAsNewRecord(mergeState.OutgoingMod, rec.Record.EditorID);


            Console.WriteLine("          Renumbering Record [" + rec.Record.FormKey.ModKey.Name + "] " +
                              rec.Record.FormKey.IDString() + " to [" + mergeState.OutgoingMod.ModKey.Name + "] " +
                              duplicated.FormKey.IDString());
            mergeState.Mapping.Add(rec.Record.FormKey, duplicated.FormKey);
        }

        public static void CopyAsOverride(
            MergeState<TMod, TModGetter> mergeState,
            IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec)
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
}
