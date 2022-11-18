using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class PlacedConeOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IPlacedCone, IPlacedConeGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedCone, IPlacedConeGetter> context)
    {
        // Do nothing.  Handled in the Worldpsace/Cell override
    }
}
