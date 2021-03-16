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
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
            where TMajorRecord : class, TMajorRecordGetter, IMajorRecord
            where TMajorRecordGetter : class, IMajorRecordGetter
        {
            foreach (var listing in loadOrder.PriorityOrder)
            {
                if (listing.Mod == null) continue;
                if (!listing.Enabled) continue;

                if (!modsToMerge.Contains(listing.ModKey)) continue;

                foreach (var rec in listing.Mod.EnumerateMajorRecordContexts<TMajorRecord, TMajorRecordGetter>(
                    linkCache))
                {
                    rec.DuplicateIntoAsNewRecord(outputMod, rec.Record.EditorID);
                }
            }
        }
    }
}
