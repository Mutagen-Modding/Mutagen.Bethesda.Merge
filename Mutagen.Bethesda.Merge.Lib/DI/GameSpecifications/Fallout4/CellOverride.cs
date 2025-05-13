using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Merge.Lib.DI.GameSpecifications.Fallout4;

public class CellOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, ICell, ICellGetter>
{
    private static readonly Cell.TranslationMask CellMask = new(defaultOn: true)
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
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter, ICell, ICellGetter> context)
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
