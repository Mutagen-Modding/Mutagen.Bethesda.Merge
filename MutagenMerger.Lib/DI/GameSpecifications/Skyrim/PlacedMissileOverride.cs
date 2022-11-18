using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class PlacedMissileOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IPlacedMissile, IPlacedMissileGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedMissile, IPlacedMissileGetter> context)
    {
        // Do nothing.  Handled in the Worldpsace/Cell override
    }
}
