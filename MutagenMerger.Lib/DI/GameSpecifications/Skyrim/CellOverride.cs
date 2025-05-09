using Mutagen.Bethesda.Plugins.Cache;
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

        Base.CellOverride.CopySubRecords(state, context, newRecord);
    }

}
