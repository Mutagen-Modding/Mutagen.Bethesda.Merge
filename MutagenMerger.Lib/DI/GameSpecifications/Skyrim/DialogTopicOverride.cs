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
            newRecord = (Mutagen.Bethesda.Skyrim.DialogTopic)context.GetOrAddAsOverride(state.OutgoingMod);
            
            // Readd branches below
            newRecord.Responses.Clear();
            
            Console.WriteLine("          Copying Override Record[" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString());
        }
        else
        {
            // Don't duplicate branches, as they will be added below
            newRecord = (Mutagen.Bethesda.Skyrim.DialogTopic)context.Record.Duplicate(state.OutgoingMod.GetNextFormKey(), DialogTopicMask);

            state.OutgoingMod.DialogTopics.Add(newRecord);
            
            state.Mapping.Add(context.Record.FormKey, newRecord.FormKey);
            
            Console.WriteLine("          Deep Copying [" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
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
