using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class PlacedObjectOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter> context)
    {
        throw new System.NotImplementedException();
    }
}
