using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Skyrim = Mutagen.Bethesda.Skyrim;
using System;
using MutagenMerger.Lib;
using MutagenMerger.Lib.GameSpecifications;

public class SkyrimSpecifications : IGameSpecifications<ISkyrimModGetter, ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>
{
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
        IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> rec)
    {
        DialogTopic.TranslationMask dialogTopicMask = new Skyrim.DialogTopic.TranslationMask(defaultOn: true)
        {
            Responses = false
        };

        if (rec.Record.Registration.ObjectKey.Equals(DialogTopic.StaticRegistration.ObjectKey))
        {
            Skyrim.DialogTopic? newRecord;
            if (!state.IsOverride(rec))
            {
                newRecord = rec.Record.Duplicate(state.OutgoingMod.GetNextFormKey(), dialogTopicMask) as Skyrim.DialogTopic;


                foreach (var branch in ((IDialogTopicGetter)rec.Record).Responses)
                {
                    var dupeBranch = branch.Duplicate(state.OutgoingMod.GetNextFormKey());
                    state.Mapping.Add(branch.FormKey, dupeBranch.FormKey);
                    newRecord!.Responses.Add(dupeBranch);
                }

                state.OutgoingMod.DialogTopics.Add(newRecord!);
                state.Mapping.Add(rec.Record.FormKey, newRecord!.FormKey);
                Console.WriteLine("          Deep Copying [" + rec.Record.FormKey.ModKey.Name + "] " + rec.Record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());

            }
            else
            {
                newRecord = rec.GetOrAddAsOverride(state.OutgoingMod) as Skyrim.DialogTopic;
                foreach (var branch in newRecord!.Responses)
                {
                    newRecord.Remove(branch.FormKey);
                }
                foreach (var branch in ((IDialogTopicGetter)rec.Record).Responses)
                {
                    var dupeBranch = branch.Duplicate(state.OutgoingMod.GetNextFormKey());
                    newRecord!.Responses.Add(dupeBranch);
                    state.Mapping.Add(branch.FormKey, dupeBranch.FormKey);
                }
                Console.WriteLine("          Copying Override Record[" + rec.Record.FormKey.ModKey.Name + "] " + rec.Record.FormKey.IDString());
            }
        }
    }
}
