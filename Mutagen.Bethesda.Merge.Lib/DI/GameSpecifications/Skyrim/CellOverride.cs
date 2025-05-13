using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.Merge.Lib.DI.GameSpecifications.Skyrim;

public class CellOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>
{
    public static readonly Cell.TranslationMask CellMask = new(defaultOn: true)
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
