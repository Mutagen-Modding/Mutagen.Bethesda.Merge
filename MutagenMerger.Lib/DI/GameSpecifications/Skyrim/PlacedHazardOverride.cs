using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class PlacedHazardOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IPlacedHazard, IPlacedHazardGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedHazard, IPlacedHazardGetter> context)
    {
        // Do nothing.  Handled in the Worldpsace/Cell override
    }
}
