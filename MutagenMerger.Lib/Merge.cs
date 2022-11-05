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

namespace MutagenMerger.Lib
{
    public static class MergeExtensions
    {
        static SkyrimSpecifications skyrimSpecifications = new SkyrimSpecifications();
        // static Fallout4Specifications fallout4Specifications = new Fallout4Specifications();
        // static OblivionSpecifications oblivionSpecifications = new OblivionSpecifications();
        static IReadOnlyCollection<ObjectKey> blacklist = SkyrimSpecifications.BlacklistedCopyTypes;//.Join();

        public static void MergeMods<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
            this IEnumerable<TModGetter> mods,
            TMod outputMod,
            GameRelease game,
            string dataPath,
            string outputFolder,
            out Dictionary<FormKey, FormKey> mapping)
            where TMod : class, TModGetter, IMod, IContextMod<TMod, TModGetter>
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMajorRecord : class, TMajorRecordGetter, IMajorRecord
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            // Just in case user gave us a lazy IEnumerable
            var modsCached = mods.ToArray();

            var modsToMerge = modsCached.Select(x => x.ModKey).ToHashSet();

            var linkCache = modsCached.ToImmutableLinkCache<TMod, TModGetter>();

            mapping = new Dictionary<FormKey, FormKey>();

            CopyRecords<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(outputMod, mapping, modsCached, modsToMerge, linkCache, game);

            outputMod.RemapLinks(mapping);

            Directory.CreateDirectory(Path.GetDirectoryName(outputFolder) ?? "");
            outputMod.WriteToBinary(Path.Combine(outputFolder), Utils.SafeBinaryWriteParameters);

            HandleAssets<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(outputMod, mapping, modsCached, modsToMerge, linkCache, game, dataPath, outputFolder);

            HandleScripts<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(outputMod, mapping, modsCached, modsToMerge, linkCache, game, dataPath, outputFolder);

        }

        private static void HandleScripts<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(TMod outputMod, Dictionary<FormKey, FormKey> mapping, TModGetter[] modsCached, HashSet<ModKey> modsToMerge, ImmutableLoadOrderLinkCache<TMod, TModGetter> linkCache, GameRelease game, string dataPath, string outputFolder)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            // Console.WriteLine("HandleScript");
        }

        private static void HandleAssets<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(TMod outputMod, Dictionary<FormKey, FormKey> mapping, TModGetter[] modsCached, HashSet<ModKey> modsToMerge, ImmutableLoadOrderLinkCache<TMod, TModGetter> linkCache, GameRelease game, string dataPath, string outputFolder)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            AssetMerge<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>.Handle(outputMod, mapping, modsCached, modsToMerge, linkCache, game, dataPath, outputFolder);
        }

        private static void CopyRecords<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(TMod outputMod, Dictionary<FormKey, FormKey> mapping, TModGetter[] modsCached, HashSet<ModKey> modsToMerge, ImmutableLoadOrderLinkCache<TMod, TModGetter> linkCache, GameRelease game)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            foreach (var rec in modsCached
                            .WinningOverrideContexts<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(linkCache))
            {
                if (blacklist.Contains(rec.Record.Registration.ObjectKey))
                {
                    HandleDeepCopy(outputMod, mapping, modsToMerge, rec, game);
                }
                else if (rec.ModKey == rec.Record.FormKey.ModKey)
                {
                    DuplicateAsNewRecord(outputMod, mapping, rec);
                }
                else
                {
                    CopyAsOverride(outputMod, mapping, modsToMerge, rec);
                }
            }
            Console.WriteLine();
        }

        private static void HandleDeepCopy<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(TMod outputMod, Dictionary<FormKey, FormKey> mapping, HashSet<ModKey> modsToMerge, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec, GameRelease game)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            switch (game)
            {
                case GameRelease.Fallout4:
                    break;
                case GameRelease.Oblivion:
                    break;
                default:
                    skyrimSpecifications.HandleCopyFor((ISkyrimMod)outputMod, mapping, modsToMerge, (IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>)rec);
                    break;
            }

        }

        private static void DuplicateAsNewRecord<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(TMod outputMod, Dictionary<FormKey, FormKey> mapping, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec)
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            //record is not an override so we can just duplicate it
            var duplicated = rec.DuplicateIntoAsNewRecord(outputMod, rec.Record.EditorID);


            Console.WriteLine("          Renumbering Record [" + rec.Record.FormKey.ModKey.Name + "] " + rec.Record.FormKey.IDString() + " to [" + outputMod.ModKey.Name + "] " + duplicated.FormKey.IDString());
            mapping.Add(rec.Record.FormKey, duplicated.FormKey);
        }

        private static void CopyAsOverride<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(TMod outputMod, Dictionary<FormKey, FormKey> mapping, HashSet<ModKey> modsToMerge, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec)
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
            if (modsToMerge.Contains(rec.Record.FormKey.ModKey))
            {
                if (mapping.ContainsKey(rec.Record.FormKey))
                    throw new NotImplementedException();

                var duplicate = rec.DuplicateIntoAsNewRecord(outputMod, rec.Record.EditorID);
                Console.WriteLine("          Renumbering Record[" + rec.Record.FormKey.ModKey.Name + "] " + rec.Record.FormKey.IDString() + " to [" + outputMod.ModKey.Name + "] " + duplicate.FormKey.IDString());
                mapping.Add(rec.Record.FormKey, duplicate.FormKey);
            }
            else
            {
                Console.WriteLine("          Copying Override Record[" + rec.Record.FormKey.ModKey.Name + "] " + rec.Record.FormKey.IDString());
                rec.GetOrAddAsOverride(outputMod);
            }
        }

    }
}
