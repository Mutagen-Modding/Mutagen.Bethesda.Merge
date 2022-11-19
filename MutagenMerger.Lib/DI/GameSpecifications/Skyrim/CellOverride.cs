using System;
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
            newRecord = (Mutagen.Bethesda.Skyrim.Cell)Base.CellOverride.CopyCellAsOverride(state, context);

        }
        else
        {
            // Don't duplicate branches, as they will be added below
            newRecord = (Mutagen.Bethesda.Skyrim.Cell)Base.CellOverride.DuplicateCell(state, context, CellMask);
        }

        CopySubRecords(state, context, newRecord);
    }

    public static void CopySubRecords(MergeState<ISkyrimMod, ISkyrimModGetter> state, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> context, Cell newRecord)
    {
        foreach (var iter in context.Record.ToLink()
                                           .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(state.LinkCache))
        {

            CopyNavmesh(state, newRecord, iter);
            CopyPersistent(state, newRecord, iter);
            CopyTemporary(state, newRecord, iter);

        }

        CopyLandscape(state, context, newRecord);
    }

    private static void CopyLandscape(MergeState<ISkyrimMod, ISkyrimModGetter> state, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> context, Cell newRecord)
    {
        if (context.Record.Landscape is not null)
        {
            Mutagen.Bethesda.Skyrim.Landscape? landscape;
            if (state.IsOverride(context.Record.Landscape.FormKey, context.ModKey))
            {
                landscape = context.Record.Landscape.DeepCopy();

            }
            else
            {
                landscape = context.Record.Landscape.Duplicate(state.OutgoingMod.GetNextFormKey());
                state.Mapping.Add(context.Record.Landscape.FormKey, landscape.FormKey);

            }
            if (landscape is not null)
            {
                newRecord.Landscape = landscape;
                Console.WriteLine("            Copying Child [" + context.Record.Landscape.FormKey.ModKey.Name + "] " + context.Record.Landscape.FormKey.IDString() + " to [" + landscape.FormKey.ModKey.Name + "] " + landscape.FormKey.IDString());

            }

        }
    }

    private static void CopyTemporary(MergeState<ISkyrimMod, ISkyrimModGetter> state, Cell newRecord, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> iter)
    {
        foreach (var temp in iter.Record.Temporary)
        {
            if (RecordExists(state, newRecord, temp)) { continue; }
            Mutagen.Bethesda.Skyrim.IPlaced newTemp;
            if (state.IsOverride(temp.FormKey, iter.ModKey))
            {
                newTemp = (Mutagen.Bethesda.Skyrim.IPlaced)temp.DeepCopy();
                state.Mapping.Add(temp.FormKey, newTemp.FormKey);

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

    private static bool RecordExists(MergeState<ISkyrimMod, ISkyrimModGetter> state, Cell newRecord, IMajorRecordGetter temp)
    {
        return newRecord.Temporary.Exists(x => x.FormKey == temp.FormKey)
               || newRecord.Persistent.Exists(x => x.FormKey == temp.FormKey)
               || newRecord.NavigationMeshes.Exists(x => x.FormKey == temp.FormKey)
               || state.Mapping.ContainsKey(temp.FormKey);
    }

    private static void CopyPersistent(MergeState<ISkyrimMod, ISkyrimModGetter> state, Cell newRecord, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> iter)
    {
        foreach (var pers in iter.Record.Persistent)
        {
            if (RecordExists(state, newRecord, pers)) { continue; }

            Mutagen.Bethesda.Skyrim.IPlaced newPersistent;
            if (state.IsOverride(pers.FormKey, iter.ModKey))
            {
                newPersistent = (Mutagen.Bethesda.Skyrim.IPlaced)pers.DeepCopy();
                state.Mapping.Add(pers.FormKey, newPersistent.FormKey);
            }
            else
            {
                newPersistent = (Mutagen.Bethesda.Skyrim.IPlaced)pers.Duplicate(state.OutgoingMod.GetNextFormKey());

                state.Mapping.Add(pers.FormKey, newPersistent.FormKey);
            }


            newRecord.Persistent.Add(newPersistent);

            Console.WriteLine("            Copying Child [" + pers.FormKey.ModKey.Name + "] " + pers.FormKey.IDString() + " to [" + newPersistent.FormKey.ModKey.Name + "] " + newPersistent.FormKey.IDString());

        }
    }

    private static void CopyNavmesh(MergeState<ISkyrimMod, ISkyrimModGetter> state, Cell newRecord, IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> iter)
    {
        foreach (var nav in iter.Record.NavigationMeshes)
        {
            if (RecordExists(state, newRecord, nav)) { continue; }
            Mutagen.Bethesda.Skyrim.NavigationMesh newNav;
            if (state.IsOverride(nav.FormKey, iter.ModKey))
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
    }
}
