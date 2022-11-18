using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class PlacedArrowOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IPlacedArrow, IPlacedArrowGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedArrow, IPlacedArrowGetter> context)
    {
        // Do nothing.  Handled in the Worldpsace/Cell override
    }
}
