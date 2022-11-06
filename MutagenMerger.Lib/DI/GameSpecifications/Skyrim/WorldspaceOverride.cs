using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class WorldspaceOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter> context)
    {
        throw new System.NotImplementedException();
    }
}
