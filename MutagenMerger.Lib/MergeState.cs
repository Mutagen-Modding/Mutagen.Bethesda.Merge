using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace MutagenMerger.Lib;

public record MergeState<TMod, TModGetter>(
    GameRelease Release,
    IReadOnlyList<TModGetter> Mods,
    HashSet<ModKey> ModsToMerge,
    TMod OutgoingMod, 
    FilePath OutputPath,
    DirectoryPath DataPath,
    ImmutableLoadOrderLinkCache<TMod, TModGetter> LinkCache,
    IGameEnvironment<TMod,TModGetter> env) 
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
{
    public Dictionary<FormKey, FormKey> Mapping { get; } = new();
    
    public bool IsOverride(FormKey rec, ModKey currentMod)
    {
        return !(ModsToMerge.Contains(rec.ModKey) || currentMod == rec.ModKey);
    }

    public FormKey GetFormKey(FormKey key) {
        if (OutgoingMod.EnumerateMajorRecords().Where(x => x.FormKey.ID == key.ID).Any()) {
            return GetFormKey(OutgoingMod.GetNextFormKey());
        }
        else {
            return FormKey.Factory(key.IDString() + ":" + OutgoingMod.ModKey.FileName);
        }
    }
    
}
