using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

using Skyrim = Mutagen.Bethesda.Skyrim;
using Fallout4 = Mutagen.Bethesda.Fallout4;
using Oblivion = Mutagen.Bethesda.Oblivion;
using System;
using MutagenMerger.Lib;

public abstract class GameSpecifications<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
{
    public static IReadOnlyCollection<ObjectKey>? BlacklistedCopyTypes { get; }

    public abstract void HandleCopyFor(MergeState<TMod, TModGetter> mergeState,
        IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec);

    public static bool IsOverride(HashSet<ModKey> modsToMerge, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> rec)
    {
        return !(modsToMerge.Contains(rec.Record.FormKey.ModKey) || rec.ModKey == rec.Record.FormKey.ModKey);
    }

}

// with 
public class SkyrimSpecifications : GameSpecifications<ISkyrimModGetter, ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>
{
    public static new IReadOnlyCollection<ObjectKey> BlacklistedCopyTypes => new List<ObjectKey> {
                                Skyrim.DialogTopic.StaticRegistration.ObjectKey,
                                Skyrim.DialogResponse.StaticRegistration.ObjectKey,
                                Skyrim.DialogResponses.StaticRegistration.ObjectKey,
                                Skyrim.Worldspace.StaticRegistration.ObjectKey,
                                Skyrim.Cell.StaticRegistration.ObjectKey,
                                Skyrim.NavigationMesh.StaticRegistration.ObjectKey,
                                Skyrim.PlacedNpc.StaticRegistration.ObjectKey,
                                Skyrim.PlacedObject.StaticRegistration.ObjectKey,
    };

    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> mergeState,
        IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> rec)
    {

        DialogTopic.TranslationMask dialogTopicMask = new Skyrim.DialogTopic.TranslationMask(defaultOn: true)
        {
            Responses = false
        };

        if (rec.Record.Registration.ObjectKey.Equals(DialogTopic.StaticRegistration.ObjectKey))
        {
            Skyrim.DialogTopic? newRecord;
            if (!IsOverride(mergeState.ModsToMerge, rec))
            {
                newRecord = rec.Record.Duplicate(mergeState.OutgoingMod.GetNextFormKey(), dialogTopicMask) as Skyrim.DialogTopic;


                foreach (var branch in ((IDialogTopicGetter)rec.Record).Responses)
                {
                    var dupeBranch = branch.Duplicate(mergeState.OutgoingMod.GetNextFormKey());
                    mergeState.Mapping.Add(branch.FormKey, dupeBranch.FormKey);
                    newRecord!.Responses.Add(dupeBranch);
                }

                mergeState.OutgoingMod.DialogTopics.Add(newRecord!);
                mergeState.Mapping.Add(rec.Record.FormKey, newRecord!.FormKey);
                Console.WriteLine("          Deep Copying [" + rec.Record.FormKey.ModKey.Name + "] " + rec.Record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
            }
            else
            {
                newRecord = rec.GetOrAddAsOverride(mergeState.OutgoingMod) as Skyrim.DialogTopic;
                foreach (var branch in newRecord!.Responses)
                {
                    newRecord.Remove(branch.FormKey);
                }
                foreach (var branch in ((IDialogTopicGetter)rec.Record).Responses)
                {
                    var dupeBranch = branch.Duplicate(mergeState.OutgoingMod.GetNextFormKey());
                    newRecord!.Responses.Add(dupeBranch);
                    mergeState.Mapping.Add(branch.FormKey, dupeBranch.FormKey);
                }
                Console.WriteLine("          Copying Override Record[" + rec.Record.FormKey.ModKey.Name + "] " + rec.Record.FormKey.IDString());
            }
        }
    }
}
