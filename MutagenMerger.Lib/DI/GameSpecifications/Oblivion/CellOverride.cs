using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Oblivion;

namespace MutagenMerger.Lib.DI.GameSpecifications.Oblivion;

public class CellOverride : ACopyOverride<IOblivionMod, IOblivionModGetter, ICell, ICellGetter>
{
    public static readonly Cell.TranslationMask CellMask = new Mutagen.Bethesda.Oblivion.Cell.TranslationMask(defaultOn: true)
    {
        Persistent = false,
        Temporary = false,
        VisibleWhenDistant = false,
        PathGrid = false,
        Landscape = false,
        Timestamp = false,
        PersistentTimestamp = false,
        TemporaryTimestamp = false,
    };
    public override void HandleCopyFor(
        MergeState<IOblivionMod, IOblivionModGetter> state,
        IModContext<IOblivionMod, IOblivionModGetter, ICell, ICellGetter> context)
    {

        Mutagen.Bethesda.Oblivion.Cell? newRecord;


        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            newRecord = (Mutagen.Bethesda.Oblivion.Cell)Base.CellOverride.CopyCellAsOverride(state, context);

        }
        else
        {
            // Don't duplicate branches, as they will be added below
            newRecord = (Mutagen.Bethesda.Oblivion.Cell)Base.CellOverride.DuplicateCell(state, context, CellMask);
        }

        Base.CellOverride.CopySubRecords(state, context, newRecord);
    }

}
