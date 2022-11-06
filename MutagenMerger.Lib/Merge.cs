using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Records;
using Skyrim = Mutagen.Bethesda.Skyrim;
using Fallout4 = Mutagen.Bethesda.Fallout4;
using Oblivion = Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Cache;
using Loqui;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace MutagenMerger.Lib
{
    public static class MergeExtensions
    {
        static SkyrimSpecifications skyrimSpecifications = new SkyrimSpecifications();
        // static Fallout4Specifications fallout4Specifications = new Fallout4Specifications();
        // static OblivionSpecifications oblivionSpecifications = new OblivionSpecifications();
        static IReadOnlyCollection<ObjectKey> blacklist = SkyrimSpecifications.BlacklistedCopyTypes;//.Join();

        public static void MergeMods<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
            this IReadOnlyList<TModGetter> mods,
            TMod outputMod,
            GameRelease game,
            DirectoryPath dataPath,
            FilePath outputPath,
            out Dictionary<FormKey, FormKey> mapping)
            where TMod : class, TModGetter, IMod, IContextMod<TMod, TModGetter>
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMajorRecord : class, TMajorRecordGetter, IMajorRecord
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            var modsToMerge = mods.Select(x => x.ModKey).ToHashSet();

            var linkCache = mods.ToImmutableLinkCache<TMod, TModGetter>();

            mapping = new Dictionary<FormKey, FormKey>();

            var state = new MergeState<TMod, TModGetter>(
                game,
                mods,
                modsToMerge,
                outputMod,
                OutputPath: outputPath,
                DataPath: dataPath,
                LinkCache: linkCache);

            CopyRecords<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(state);

            outputMod.RemapLinks(mapping);

            Directory.CreateDirectory(outputPath.Directory ?? "");
            outputMod.WriteToBinary(outputPath, Utils.SafeBinaryWriteParameters);

            HandleAssets<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(state);

            HandleScripts<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(state);

        }

        private static void HandleScripts<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
            MergeState<TMod, TModGetter> mergeState)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            // Console.WriteLine("HandleScript");
        }

        private static void HandleAssets<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>( MergeState<TMod, TModGetter> mergeState)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            AssetMerge<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>.Handle(mergeState);
        }

        private static void CopyRecords<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
            MergeState<TMod, TModGetter> mergeState)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            foreach (var rec in mergeState.Mods
                            .WinningOverrideContexts<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(mergeState.LinkCache))
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

        private static void HandleDeepCopy<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
            MergeState<TMod, TModGetter> mergeState,
            IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
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

        private static void DuplicateAsNewRecord<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
            MergeState<TMod, TModGetter> mergeState,
            IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            //record is not an override so we can just duplicate it
            var duplicated = rec.DuplicateIntoAsNewRecord(mergeState.OutgoingMod, rec.Record.EditorID);


            Console.WriteLine("          Renumbering Record [" + rec.Record.FormKey.ModKey.Name + "] " + rec.Record.FormKey.IDString() + " to [" + mergeState.OutgoingMod.ModKey.Name + "] " + duplicated.FormKey.IDString());
            mergeState.Mapping.Add(rec.Record.FormKey, duplicated.FormKey);
        }

        private static void CopyAsOverride<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
            MergeState<TMod, TModGetter> mergeState, 
            IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
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
                Console.WriteLine("          Renumbering Record[" + rec.Record.FormKey.ModKey.Name + "] " + rec.Record.FormKey.IDString() + " to [" + mergeState.OutgoingMod.ModKey.Name + "] " + duplicate.FormKey.IDString());
                mergeState.Mapping.Add(rec.Record.FormKey, duplicate.FormKey);
            }
            else
            {
                Console.WriteLine("          Copying Override Record[" + rec.Record.FormKey.ModKey.Name + "] " + rec.Record.FormKey.IDString());
                rec.GetOrAddAsOverride(mergeState.OutgoingMod);
            }
        }

    }
}
