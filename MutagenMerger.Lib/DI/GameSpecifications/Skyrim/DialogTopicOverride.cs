using System;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
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
            newRecord = (DialogTopic)Base.DialogTopicOverride.CopyDialogTopicAsOverride(state, context);
        }
        else
        {
            newRecord = (DialogTopic)Base.DialogTopicOverride.DuplicateDialogTopic(state, context, DialogTopicMask);
        }

        // Do the branches
        foreach (var branch in context.Record.Responses)
        {
            CopyDialogResponse(state, context.ModKey, newRecord, branch);
        }
    }


    private void CopyDialogResponse(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        ModKey currentMod,
        Mutagen.Bethesda.Skyrim.DialogTopic topic,
        IDialogResponsesGetter dialogResponses)
    {
        Mutagen.Bethesda.Skyrim.DialogResponses newRecord;
        if (state.IsOverride(dialogResponses.FormKey, currentMod))
        {
            newRecord = dialogResponses.DeepCopy();
            Console.WriteLine("          Copying Override Record[" + currentMod.Name + "] " + dialogResponses.FormKey.IDString());
        }
        else
        {
            newRecord = dialogResponses.Duplicate(state.OutgoingMod.GetNextFormKey());
            state.Mapping.Add(dialogResponses.FormKey, newRecord.FormKey);
            Console.WriteLine("          Deep Copying [" + currentMod.Name + "] " + dialogResponses.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        }
        
        topic.Responses.Add(newRecord);
    }
}
