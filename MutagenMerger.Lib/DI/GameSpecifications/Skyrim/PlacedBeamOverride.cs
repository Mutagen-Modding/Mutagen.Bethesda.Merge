using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class PlacedBeamOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IPlacedBeam, IPlacedBeamGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedBeam, IPlacedBeamGetter> context)
    {
        // Do nothing.  Handled in the Worldpsace/Cell override
    }
}
