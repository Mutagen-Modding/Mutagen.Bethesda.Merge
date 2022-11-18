using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class PlacedFlameOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IPlacedFlame, IPlacedFlameGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedFlame, IPlacedFlameGetter> context)
    {
        // Do nothing.  Handled in the Worldpsace/Cell override
    }
}
