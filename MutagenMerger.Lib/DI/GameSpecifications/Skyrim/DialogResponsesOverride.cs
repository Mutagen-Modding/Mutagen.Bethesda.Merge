using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class DialogResponsesOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter>
{
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter> context)
    {
        // Do nothing.  Handled in the DialogTopic override
    }
}
