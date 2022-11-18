using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class PlacedBarrierOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IPlacedBarrier, IPlacedBarrierGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedBarrier, IPlacedBarrierGetter> context)
    {
        // Do nothing.  Handled in the Worldpsace/Cell override
    }
}
