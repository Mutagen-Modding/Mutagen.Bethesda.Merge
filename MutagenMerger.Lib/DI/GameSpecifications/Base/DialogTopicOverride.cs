using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using SkyrimRecord = Mutagen.Bethesda.Skyrim;
using OblivionRecord = Mutagen.Bethesda.Oblivion;

namespace MutagenMerger.Lib.DI.GameSpecifications.Base;

public class DialogTopicOverride
{


    public static IMajorRecord CopyDialogTopicAsOverride<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state,
        IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context)
        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter
    {
        IMajorRecord newRecord = context.GetOrAddAsOverride(state.OutgoingMod);

        switch (state.Release)
        {
            case GameRelease.Oblivion:
                ((OblivionRecord.DialogTopic)newRecord).Items.Clear();
                break;
            // case GameRelease.Fallout4:
            //     ((Fallout4Record.DialogTopic)newRecord).Responses.Clear();
            //     break;
            default:
                ((SkyrimRecord.DialogTopic)newRecord).Responses.Clear();
                break;
        }

        Console.WriteLine("          Copying Override Record[" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString());
        return newRecord;
    }


    public static IMajorRecord DuplicateDialogTopic<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state,
        IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context, MajorRecord.TranslationMask mask)

        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter
    {
        // Don't duplicate branches, as they will be added below
        IMajorRecord newRecord = context.Record.Duplicate(state.OutgoingMod.GetNextFormKey(), mask);


        switch (state.Release)
        {
            case GameRelease.Oblivion:
                ((OblivionRecord.OblivionMod)(state.OutgoingMod as IMod)).DialogTopics.Add((OblivionRecord.DialogTopic)newRecord);
                break;
            // case GameRelease.Fallout4:
            //     ((Fallout4Record.Fallout4Mod)(state.OutgoingMod as IMod)).DialogTopics.Add((Fallout4Record.DialogView)newRecord);
            //     break;
            default:
                ((SkyrimRecord.SkyrimMod)(state.OutgoingMod as IMod)).DialogTopics.Add((SkyrimRecord.DialogTopic)newRecord);
                break;
        }
        state.Mapping.Add(context.Record.FormKey, newRecord.FormKey);

        Console.WriteLine("          Deep Copying [" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        return newRecord;
    }
}
