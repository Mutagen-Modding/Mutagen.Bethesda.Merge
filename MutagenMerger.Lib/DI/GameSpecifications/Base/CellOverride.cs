using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using SkyrimRecord = Mutagen.Bethesda.Skyrim;
using Fallout4Record = Mutagen.Bethesda.Fallout4;
using OblivionRecord = Mutagen.Bethesda.Oblivion;

namespace MutagenMerger.Lib.DI.GameSpecifications.Base;

public class CellOverride
{
    public static IMajorRecord CopyCellAsOverride<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state,
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
                ((OblivionRecord.Cell)newRecord).Persistent.Clear();
                ((OblivionRecord.Cell)newRecord).Temporary.Clear();
                ((OblivionRecord.Cell)newRecord).Landscape?.Clear();
                ((OblivionRecord.Cell)newRecord).PathGrid?.Clear();
                break;
            case GameRelease.Fallout4:
                ((Fallout4Record.Cell)newRecord).NavigationMeshes.Clear();
                ((Fallout4Record.Cell)newRecord).Persistent.Clear();
                ((Fallout4Record.Cell)newRecord).Temporary.Clear();
                ((Fallout4Record.Cell)newRecord).Landscape?.Clear();
                break;
            default:
                ((SkyrimRecord.Cell)newRecord).NavigationMeshes.Clear();
                ((SkyrimRecord.Cell)newRecord).Persistent.Clear();
                ((SkyrimRecord.Cell)newRecord).Temporary.Clear();
                ((SkyrimRecord.Cell)newRecord).Landscape?.Clear();
                break;
        }

        Console.WriteLine("          Copying Override Record[" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString());
        return newRecord;
    }


    public static IMajorRecord DuplicateCell<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state,
        IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context, MajorRecord.TranslationMask mask)

        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter
    {
        // Don't duplicate branches, as they will be added below
        IMajorRecord newRecord = context.DuplicateIntoAsNewRecord(state.OutgoingMod, context.Record.EditorID);

        state.Mapping.Add(context.Record.FormKey, newRecord.FormKey);

        Console.WriteLine("          Deep Copying [" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        return newRecord;
    }



}
