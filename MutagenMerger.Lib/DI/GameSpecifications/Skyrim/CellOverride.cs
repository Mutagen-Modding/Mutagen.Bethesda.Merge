﻿using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class CellOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>
{
    public static readonly Cell.TranslationMask CellMask = new Mutagen.Bethesda.Skyrim.Cell.TranslationMask(defaultOn: true)
    {
        Persistent = false,
        Temporary = false,
        Landscape = false,
        NavigationMeshes = false,
        Timestamp = false,
        PersistentTimestamp = false,
        TemporaryTimestamp = false,
        UnknownGroupData = false,
        PersistentUnknownGroupData = false,
        TemporaryUnknownGroupData = false,
    };
    public override void HandleCopyFor(
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> context)
    {

        Mutagen.Bethesda.Skyrim.Cell? newRecord;

        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            newRecord = (Mutagen.Bethesda.Skyrim.Cell)context.GetOrAddAsOverride(state.OutgoingMod);

            // Readd branches below
            newRecord.NavigationMeshes.Clear();
            newRecord.Persistent.Clear();
            newRecord.Temporary.Clear();
            newRecord.Landscape?.Clear();
            Console.WriteLine("          Copying Override Record[" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString());
        }
        else
        {
            // Don't duplicate branches, as they will be added below
            newRecord = (Mutagen.Bethesda.Skyrim.Cell)context.DuplicateIntoAsNewRecord(state.OutgoingMod, context.Record.EditorID);

            state.Mapping.Add(context.Record.FormKey, newRecord.FormKey);

            Console.WriteLine("          Deep Copying [" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        }

        CopySubRecords(state, context, newRecord);
    }

    public static void CopySubRecords(MergeState<ISkyrimMod, ISkyrimModGetter> state, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> context, Cell newRecord)
    {
        foreach (var nav in context.Record.NavigationMeshes)
        {
            Mutagen.Bethesda.Skyrim.NavigationMesh newNav;
            if (state.IsOverride(nav.FormKey, context.ModKey))
            {
                newNav = nav.DeepCopy();
            }
            else
            {
                newNav = nav.Duplicate(state.OutgoingMod.GetNextFormKey());

                state.Mapping.Add(nav.FormKey, newNav.FormKey);
            }

            newRecord.NavigationMeshes.Add(newNav);
            Console.WriteLine("            Copying Child [" + nav.FormKey.ModKey.Name + "] " + nav.FormKey.IDString() + " to [" + newNav.FormKey.ModKey.Name + "] " + newNav.FormKey.IDString());

        }

        foreach (var pers in context.Record.Persistent)
        {
            Mutagen.Bethesda.Skyrim.IPlaced newPersistent;
            if (state.IsOverride(pers.FormKey, context.ModKey))
            {
                newPersistent = (Mutagen.Bethesda.Skyrim.IPlaced)pers.DeepCopy();
            }
            else
            {
                newPersistent = (Mutagen.Bethesda.Skyrim.IPlaced)pers.Duplicate(state.OutgoingMod.GetNextFormKey());

                state.Mapping.Add(pers.FormKey, newPersistent.FormKey);
            }
            newRecord.Persistent.Add(newPersistent);

            Console.WriteLine("            Copying Child [" + pers.FormKey.ModKey.Name + "] " + pers.FormKey.IDString() + " to [" + newPersistent.FormKey.ModKey.Name + "] " + newPersistent.FormKey.IDString());

        }

        foreach (var temp in context.Record.Temporary)
        {
            Mutagen.Bethesda.Skyrim.IPlaced newTemp;
            if (state.IsOverride(temp.FormKey, context.ModKey))
            {
                newTemp = (Mutagen.Bethesda.Skyrim.IPlaced)temp.DeepCopy();

            }
            else
            {
                newTemp = (Mutagen.Bethesda.Skyrim.IPlaced)temp.Duplicate(state.OutgoingMod.GetNextFormKey());

                state.Mapping.Add(temp.FormKey, newTemp.FormKey);

            }
            newRecord.Persistent.Add(newTemp);
            Console.WriteLine("            Copying Child [" + temp.FormKey.ModKey.Name + "] " + temp.FormKey.IDString() + " to [" + newTemp.FormKey.ModKey.Name + "] " + newTemp.FormKey.IDString());

        }
    }
}
