using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Merge.Lib.DI.GameSpecifications.Skyrim;

public class DialogTopicOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>
{
    private static readonly DialogTopic.TranslationMask DialogTopicMask = new(defaultOn: true)
    {
        Responses = false
    };

    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter> context)
    {
        IMajorRecord newRecord;
        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            newRecord = Base.DialogTopicOverride.CopyDialogTopicAsOverride(state, (IMajorRecordGetter)context.Record);
        }
        else
        {
            newRecord = Base.DialogTopicOverride.DuplicateDialogTopic(state, (IMajorRecordGetter)context.Record, DialogTopicMask);
        }

        // Do the branches
        foreach (var response in context.Record.Responses)
        {
            Base.DialogTopicOverride.CopyDialogResponses(state, context.ModKey, newRecord, response);
        }
    }
}
