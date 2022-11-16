using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class NavigationMeshOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter> context)
    {
        // Do nothing.  Handled in the Worldpsace/Cell override
    }
}
