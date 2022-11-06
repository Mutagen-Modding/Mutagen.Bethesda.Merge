using System;
using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class SkyrimSpecifications : IGameSpecifications<ISkyrimModGetter, ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>
{
    private static readonly DialogTopic.TranslationMask DialogTopicMask = new Mutagen.Bethesda.Skyrim.DialogTopic.TranslationMask(defaultOn: true)
    {
        Responses = false
    };

    public IReadOnlyCollection<ObjectKey> BlacklistedCopyTypes { get; } = new HashSet<ObjectKey> {
        Mutagen.Bethesda.Skyrim.DialogTopic.StaticRegistration.ObjectKey,
        Mutagen.Bethesda.Skyrim.DialogResponse.StaticRegistration.ObjectKey,
        Mutagen.Bethesda.Skyrim.DialogResponses.StaticRegistration.ObjectKey,
        Mutagen.Bethesda.Skyrim.Worldspace.StaticRegistration.ObjectKey,
        Mutagen.Bethesda.Skyrim.Cell.StaticRegistration.ObjectKey,
        Mutagen.Bethesda.Skyrim.NavigationMesh.StaticRegistration.ObjectKey,
        Mutagen.Bethesda.Skyrim.PlacedNpc.StaticRegistration.ObjectKey,
        Mutagen.Bethesda.Skyrim.PlacedObject.StaticRegistration.ObjectKey,
    };

    public void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context)
    {
        switch (context.Record)
        {
            case IDialogTopicGetter dialogTopic:
                CopyDialogTopic(state, context, dialogTopic);
                break;
            case IDialogResponsesGetter:
                // Do nothing. Handled within topics
                break;
        }
    }

    private void CopyDialogTopic(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> context,
        IDialogTopicGetter dialogTopic)
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
            
            state.Mapping.Add(dialogTopic.FormKey, newRecord.FormKey);
            
            Console.WriteLine("          Deep Copying [" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        }
        
        // Do the branches
        foreach (var branch in dialogTopic.Responses)
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
