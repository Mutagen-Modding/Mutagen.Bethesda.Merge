using System;
using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Skyrim = Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications;

public class SkyrimSpecifications : IGameSpecifications<ISkyrimModGetter, ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>
{
    private static readonly DialogTopic.TranslationMask DialogTopicMask = new Skyrim.DialogTopic.TranslationMask(defaultOn: true)
    {
        Responses = false
    };

    public IReadOnlyCollection<ObjectKey> BlacklistedCopyTypes { get; } = new HashSet<ObjectKey> {
        Skyrim.DialogTopic.StaticRegistration.ObjectKey,
        Skyrim.DialogResponse.StaticRegistration.ObjectKey,
        Skyrim.DialogResponses.StaticRegistration.ObjectKey,
        Skyrim.Worldspace.StaticRegistration.ObjectKey,
        Skyrim.Cell.StaticRegistration.ObjectKey,
        Skyrim.NavigationMesh.StaticRegistration.ObjectKey,
        Skyrim.PlacedNpc.StaticRegistration.ObjectKey,
        Skyrim.PlacedObject.StaticRegistration.ObjectKey,
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
        Skyrim.DialogTopic newRecord;
        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            newRecord = (Skyrim.DialogTopic)context.GetOrAddAsOverride(state.OutgoingMod);
            
            // Readd branches below
            newRecord.Responses.Clear();
            
            Console.WriteLine("          Copying Override Record[" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString());
        }
        else
        {
            // Don't duplicate branches, as they will be added below
            newRecord = (Skyrim.DialogTopic)context.Record.Duplicate(state.OutgoingMod.GetNextFormKey(), DialogTopicMask);

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
        Skyrim.DialogTopic topic,
        IDialogResponsesGetter dialogResponses)
    {
        Skyrim.DialogResponses newRecord;
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
