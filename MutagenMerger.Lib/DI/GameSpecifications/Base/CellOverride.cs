using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using SkyrimRecord = Mutagen.Bethesda.Skyrim;
using Fallout4Record = Mutagen.Bethesda.Fallout4;
using OblivionRecord = Mutagen.Bethesda.Oblivion;
using System.Collections.Generic;

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
                ((OblivionRecord.Cell)newRecord).VisibleWhenDistant.Clear();
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
    public static bool RecordExists<TMod, TModGetter>(
        MergeState<TMod, TModGetter> state,
        IMajorRecord newRecord,
        IMajorRecordGetter temp)

        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
    {
        switch (state.Release)
        {
            case GameRelease.Oblivion:
                return ((OblivionRecord.Cell)newRecord).Temporary.Exists(x => x.FormKey == temp.FormKey)
               || ((OblivionRecord.Cell)newRecord).Persistent.Exists(x => x.FormKey == temp.FormKey)
               || ((OblivionRecord.Cell)newRecord).VisibleWhenDistant.Exists(x => x.FormKey == temp.FormKey)
               || state.Mapping.ContainsKey(temp.FormKey);
            case GameRelease.Fallout4:
                return ((Fallout4Record.Cell)newRecord).Temporary.Exists(x => x.FormKey == temp.FormKey)
               || ((Fallout4Record.Cell)newRecord).Persistent.Exists(x => x.FormKey == temp.FormKey)
               || ((Fallout4Record.Cell)newRecord).NavigationMeshes.Exists(x => x.FormKey == temp.FormKey)
               || state.Mapping.ContainsKey(temp.FormKey);
            default:
                return ((SkyrimRecord.Cell)newRecord).Temporary.Exists(x => x.FormKey == temp.FormKey)
               || ((SkyrimRecord.Cell)newRecord).Persistent.Exists(x => x.FormKey == temp.FormKey)
               || ((SkyrimRecord.Cell)newRecord).NavigationMeshes.Exists(x => x.FormKey == temp.FormKey)
               || state.Mapping.ContainsKey(temp.FormKey);
        }

    }

    public static void CopySubRecords<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context, IMajorRecord newRecord)


        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter
    {
        foreach (var iter in context.Record.ToLink()
                                           .ResolveAllContexts<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(state.LinkCache))
        {
            if (state.Release != GameRelease.Oblivion)
            {
                CopyNavmesh(state, iter, newRecord);
            }

            CopyPersistent(state, iter, newRecord);
            CopyTemporary(state, iter, newRecord);

            if (state.Release == GameRelease.Oblivion)
            {
                CopyDistance(state, context, newRecord);
            }

        }

        if (state.Release == GameRelease.Oblivion)
        {
            CopyPathing(state, context, newRecord);
        }
        CopyLandscape(state, context, newRecord);
    }


    private static void CopyNavmesh<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context, IMajorRecord newRecord)


        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter
    {
        IReadOnlyList<IMajorRecordGetter> list = state.Release == GameRelease.Fallout4 ? ((Fallout4Record.ICellGetter)context.Record).NavigationMeshes :
         ((SkyrimRecord.ICellGetter)context.Record).NavigationMeshes;

        foreach (var nav in list)
        {
            if (Base.CellOverride.RecordExists(state, newRecord, nav)) { continue; }
            IMajorRecord newNav;
            if (state.IsOverride(nav.FormKey, context.ModKey))
            {
                newNav = nav.DeepCopy();
            }
            else
            {
                newNav = nav.Duplicate(state.OutgoingMod.GetNextFormKey());

                state.Mapping.Add(nav.FormKey, newNav.FormKey);
            }

            switch (state.Release)
            {
                case GameRelease.Fallout4:
                    ((Fallout4Record.Cell)newRecord).NavigationMeshes.Add((Fallout4Record.NavigationMesh)newNav);
                    break;
                default:
                    ((SkyrimRecord.Cell)newRecord).NavigationMeshes.Add((SkyrimRecord.NavigationMesh)newNav);
                    break;
            }
            Console.WriteLine("            Copying Child [" + nav.FormKey.ModKey.Name + "] " + nav.FormKey.IDString() + " to [" + newNav.FormKey.ModKey.Name + "] " + newNav.FormKey.IDString());
        }
    }


    private static void CopyLandscape<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context, IMajorRecord newRecord)


        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter
    {
        IMajorRecordGetter? originalLandscape = state.Release == GameRelease.Oblivion ? ((OblivionRecord.ICellGetter)context.Record).Landscape :
            state.Release == GameRelease.Fallout4 ? ((Fallout4Record.ICellGetter)context.Record).Landscape :
             ((SkyrimRecord.ICellGetter)context.Record).Landscape;

        if (originalLandscape is null) { return; }


        IMajorRecord? landscape;

        if (state.IsOverride(originalLandscape.FormKey, context.ModKey))
        {
            landscape = originalLandscape.DeepCopy();

        }
        else
        {
            landscape = originalLandscape.Duplicate(state.OutgoingMod.GetNextFormKey());
            if (landscape is not null)
            {
                state.Mapping.Add(originalLandscape.FormKey, landscape.FormKey);
            }

        }
        if (landscape is not null)
        {

            switch (state.Release)
            {
                case GameRelease.Oblivion:
                    ((OblivionRecord.Cell)newRecord).Landscape = (OblivionRecord.Landscape?)landscape;
                    break;
                case GameRelease.Fallout4:
                    ((Fallout4Record.Cell)newRecord).Landscape = (Fallout4Record.Landscape?)landscape;
                    break;
                default:
                    ((SkyrimRecord.Cell)newRecord).Landscape = (SkyrimRecord.Landscape?)landscape;
                    break;
            }
            Console.WriteLine("            Copying Child [" + originalLandscape.FormKey.ModKey.Name + "] " + originalLandscape.FormKey.IDString() + " to [" + landscape.FormKey.ModKey.Name + "] " + landscape.FormKey.IDString());

        }

    }


    private static void CopyPathing<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context, IMajorRecord newRecord)


        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter
    {
        OblivionRecord.IPathGridGetter? originalPathing = ((OblivionRecord.ICellGetter)context.Record).PathGrid;

        if (originalPathing is null) { return; }


        OblivionRecord.PathGrid? pathing;

        if (state.IsOverride(originalPathing.FormKey, context.ModKey))
        {
            pathing = (OblivionRecord.PathGrid?)originalPathing.DeepCopy();

        }
        else
        {
            pathing = (OblivionRecord.PathGrid?)originalPathing.Duplicate(state.OutgoingMod.GetNextFormKey());

            if (pathing is not null)
            {
                state.Mapping.Add(originalPathing.FormKey, pathing.FormKey);
            }

        }
        if (pathing is not null)
        {

            ((OblivionRecord.Cell)newRecord).PathGrid = pathing;
            Console.WriteLine("            Copying Child [" + originalPathing.FormKey.ModKey.Name + "] " + originalPathing.FormKey.IDString() + " to [" + pathing.FormKey.ModKey.Name + "] " + pathing.FormKey.IDString());

        }

    }



    private static void CopyPersistent<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context, IMajorRecord newRecord)


        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter
    {

        IReadOnlyList<IMajorRecordGetter> list =
            state.Release == GameRelease.Oblivion ? ((OblivionRecord.ICellGetter)context.Record).Persistent :
            state.Release == GameRelease.Fallout4 ? ((Fallout4Record.ICellGetter)context.Record).Persistent :
            ((SkyrimRecord.ICellGetter)context.Record).Persistent;
        foreach (var pers in list)
        {
            if (Base.CellOverride.RecordExists(state, newRecord, pers)) { continue; }

            IMajorRecord newPersistent;
            if (state.IsOverride(pers.FormKey, context.ModKey))
            {
                newPersistent = pers.DeepCopy();
                state.Mapping.Add(pers.FormKey, newPersistent.FormKey);
            }
            else
            {
                newPersistent = pers.Duplicate(state.OutgoingMod.GetNextFormKey());

                state.Mapping.Add(pers.FormKey, newPersistent.FormKey);
            }


            switch (state.Release)
            {
                case GameRelease.Oblivion:
                    ((OblivionRecord.Cell)newRecord).Persistent.Add((OblivionRecord.IPlaced)newPersistent);
                    break;
                case GameRelease.Fallout4:
                    ((Fallout4Record.Cell)newRecord).Persistent.Add((Fallout4Record.IPlaced)newPersistent);
                    break;
                default:
                    ((SkyrimRecord.Cell)newRecord).Persistent.Add((SkyrimRecord.IPlaced)newPersistent);
                    break;
            }

            Console.WriteLine("            Copying Child [" + pers.FormKey.ModKey.Name + "] " + pers.FormKey.IDString() + " to [" + newPersistent.FormKey.ModKey.Name + "] " + newPersistent.FormKey.IDString());

        }
    }


    private static void CopyTemporary<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context, IMajorRecord newRecord)


        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter
    {

        IReadOnlyList<IMajorRecordGetter> list =
            state.Release == GameRelease.Oblivion ? ((OblivionRecord.ICellGetter)context.Record).Temporary :
            state.Release == GameRelease.Fallout4 ? ((Fallout4Record.ICellGetter)context.Record).Temporary :
            ((SkyrimRecord.ICellGetter)context.Record).Temporary;
        foreach (var temp in list)
        {
            if (Base.CellOverride.RecordExists(state, newRecord, temp)) { continue; }
            IMajorRecord newTemp;
            if (state.IsOverride(temp.FormKey, context.ModKey))
            {
                newTemp = temp.DeepCopy();
                state.Mapping.Add(temp.FormKey, newTemp.FormKey);

            }
            else
            {

                newTemp = temp.Duplicate(state.OutgoingMod.GetNextFormKey());

                state.Mapping.Add(temp.FormKey, newTemp.FormKey);

            }
            switch (state.Release)
            {
                case GameRelease.Oblivion:
                    ((OblivionRecord.Cell)newRecord).Temporary.Add((OblivionRecord.IPlaced)newTemp);
                    break;
                case GameRelease.Fallout4:
                    ((Fallout4Record.Cell)newRecord).Temporary.Add((Fallout4Record.IPlaced)newTemp);
                    break;
                default:
                    ((SkyrimRecord.Cell)newRecord).Temporary.Add((SkyrimRecord.IPlaced)newTemp);
                    break;
            }
            Console.WriteLine("            Copying Child [" + temp.FormKey.ModKey.Name + "] " + temp.FormKey.IDString() + " to [" + newTemp.FormKey.ModKey.Name + "] " + newTemp.FormKey.IDString());

        }
    }
    private static void CopyDistance<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>(MergeState<TMod, TModGetter> state, IModContext<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> context, IMajorRecord newRecord)


            where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
    {

        IReadOnlyList<OblivionRecord.IPlacedGetter> list = ((OblivionRecord.ICellGetter)context.Record).VisibleWhenDistant;
        foreach (var vis in list)
        {
            if (Base.CellOverride.RecordExists(state, newRecord, vis)) { continue; }
            OblivionRecord.IPlaced newVis;
            if (state.IsOverride(vis.FormKey, context.ModKey))
            {
                newVis = (OblivionRecord.IPlaced)vis.DeepCopy();
                state.Mapping.Add(vis.FormKey, newVis.FormKey);

            }
            else
            {

                newVis = (OblivionRecord.IPlaced)vis.Duplicate(state.OutgoingMod.GetNextFormKey());

                state.Mapping.Add(vis.FormKey, newVis.FormKey);

            }

            ((OblivionRecord.Cell)newRecord).Temporary.Add(newVis);

            Console.WriteLine("            Copying Child [" + vis.FormKey.ModKey.Name + "] " + vis.FormKey.IDString() + " to [" + newVis.FormKey.ModKey.Name + "] " + newVis.FormKey.IDString());

        }
    }

}
