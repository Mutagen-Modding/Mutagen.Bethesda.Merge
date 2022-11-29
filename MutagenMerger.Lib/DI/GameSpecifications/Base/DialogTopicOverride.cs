using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using SkyrimRecord = Mutagen.Bethesda.Skyrim;
using Fallout4Record = Mutagen.Bethesda.Fallout4;
using OblivionRecord = Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Noggog;
using System.Collections.Generic;

namespace MutagenMerger.Lib.DI.GameSpecifications.Base;

public class DialogTopicOverride
{


    public static IMajorRecord CopyDialogTopicAsOverride<TMod, TModGetter>(MergeState<TMod, TModGetter> state,
        IMajorRecordGetter record)
        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
    {
        IMajorRecord newRecord = record.DeepCopy();

        switch (state.Release)
        {
            case GameRelease.Oblivion:
                ((OblivionRecord.DialogTopic)newRecord).Items.Clear();
                break;
            case GameRelease.Fallout4:
                ((Fallout4Record.DialogTopic)newRecord).Responses.Clear();
                break;
            default:
                ((SkyrimRecord.DialogTopic)newRecord).Responses.Clear();
                break;
        }

        Console.WriteLine("          Copying Override Record[" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        return newRecord;
    }

    public static IMajorRecord DuplicateDialogTopic<TMod, TModGetter>(MergeState<TMod, TModGetter> state,
        IMajorRecordGetter record, MajorRecord.TranslationMask mask)

        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
    {
        // Don't duplicate branches, as they will be added below
        IMajorRecord newRecord = record.Duplicate(state.GetFormKey(record.FormKey),mask);


        switch (state.Release)
        {
            case GameRelease.Oblivion:
                ((OblivionRecord.OblivionMod)(state.OutgoingMod as IMod)).DialogTopics.Add((OblivionRecord.DialogTopic)newRecord);
                break;
            case GameRelease.Fallout4:
                // ((Fallout4Record.Fallout4Mod)(state.OutgoingMod as IMod)).DialogViews.Add((Fallout4Record.DialogView)newRecord);
                break;
            default:
                ((SkyrimRecord.SkyrimMod)(state.OutgoingMod as IMod)).DialogTopics.Add((SkyrimRecord.DialogTopic)newRecord);
                break;
        }
        state.Mapping.Add(record.FormKey, newRecord.FormKey);

        Console.WriteLine("          Deep Copying [" + record.FormKey.ModKey.Name + "] " + record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        return newRecord;
    }


    public static void CopyDialogResponses<TMod, TModGetter>(MergeState<TMod, TModGetter> state,
        ModKey currentMod,
        IMajorRecord topic,
        IMajorRecordGetter dialogResponses)
        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
    {
        IMajorRecord newRecord;
        if (state.IsOverride(dialogResponses.FormKey, currentMod))
        {
            newRecord = dialogResponses.DeepCopy();
            Console.WriteLine("          Copying Override Record[" + currentMod.Name + "] " + dialogResponses.FormKey.IDString());
        }
        else
        {
            newRecord = dialogResponses.Duplicate(state.GetFormKey(dialogResponses.FormKey));
            state.Mapping.Add(dialogResponses.FormKey, newRecord.FormKey);
            Console.WriteLine("          Deep Copying [" + currentMod.Name + "] " + dialogResponses.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        }
        switch (state.Release)
        {
            case GameRelease.Fallout4:
                ((Fallout4Record.DialogTopic)topic).Responses.Add((Fallout4Record.DialogResponses)newRecord);
                break;
            default:
                ((SkyrimRecord.DialogTopic)topic).Responses.Add((SkyrimRecord.DialogResponses)newRecord);
                break;
        }

    }


}
