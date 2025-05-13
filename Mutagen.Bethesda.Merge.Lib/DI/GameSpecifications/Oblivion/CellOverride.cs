using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Merge.Lib.DI.GameSpecifications.Oblivion;

public class CellOverride : ACopyOverride<IOblivionMod, IOblivionModGetter, ICell, ICellGetter>
{
    public static readonly Cell.TranslationMask CellMask = new(defaultOn: true)
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
        IMajorRecord? newRecord;

        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            newRecord = Base.CellOverride.CopyCellAsOverride(state, context);
        }
        else
        {
            // Don't duplicate branches, as they will be added below
            newRecord = Base.CellOverride.DuplicateCell(state, context, CellMask);
        }

        Base.CellOverride.CopySubRecords(state, context, newRecord);
    }
}
