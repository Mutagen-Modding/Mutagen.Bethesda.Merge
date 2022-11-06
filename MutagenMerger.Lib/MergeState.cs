using System.Collections.Generic;
using Mutagen.Bethesda;
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
    ImmutableLoadOrderLinkCache<TMod, TModGetter> LinkCache) 
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
{
    public Dictionary<FormKey, FormKey> Mapping { get; } = new();
}
