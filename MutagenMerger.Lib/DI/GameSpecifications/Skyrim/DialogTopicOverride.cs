using System;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class DialogTopicOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>
{
    private static readonly DialogTopic.TranslationMask DialogTopicMask = new Mutagen.Bethesda.Skyrim.DialogTopic.TranslationMask(defaultOn: true)
    {
        Responses = false
    };

    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter> context)
    {
        Mutagen.Bethesda.Skyrim.DialogTopic newRecord;
        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            newRecord = (DialogTopic)Base.DialogTopicOverride.CopyDialogTopicAsOverride(state, (IMajorRecordGetter)context.Record);
        }
        else
        {
            newRecord = (DialogTopic)Base.DialogTopicOverride.DuplicateDialogTopic(state, (IMajorRecordGetter)context.Record, DialogTopicMask);
        }

        // Do the branches
        foreach (var response in context.Record.Responses)
        {
            Base.DialogTopicOverride.CopyDialogResponses(state, context.ModKey, newRecord, response);
        }
    }

}
