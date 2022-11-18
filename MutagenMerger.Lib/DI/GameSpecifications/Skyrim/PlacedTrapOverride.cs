using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class PlacedTrapOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IPlacedTrap, IPlacedTrapGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedTrap, IPlacedTrapGetter> context)
    {
        // Do nothing.  Handled in the Worldpsace/Cell override
    }
}
