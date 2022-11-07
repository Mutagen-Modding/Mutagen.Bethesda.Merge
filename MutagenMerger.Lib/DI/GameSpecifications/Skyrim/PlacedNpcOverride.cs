using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class PlacedNpcOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter> context)
    {
        throw new System.NotImplementedException();
    }
}
