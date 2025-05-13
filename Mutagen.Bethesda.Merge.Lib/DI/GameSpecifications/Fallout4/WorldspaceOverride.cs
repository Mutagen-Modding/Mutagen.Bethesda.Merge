using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.Merge.Lib.DI.GameSpecifications.Fallout4;

public class WorldspaceOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, IWorldspace, IWorldspaceGetter>
{
    private static readonly Worldspace.TranslationMask WorldspaceMask = new(defaultOn: true)
    {
        SubCells = false,
        TopCell = false,
        LargeReferences = false,
        OffsetData = false,
        SubCellsUnknown = false,
        SubCellsTimestamp = false,
    };

    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter, IWorldspace, IWorldspaceGetter> context)
    {
        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            var newRecord = (Mutagen.Bethesda.Fallout4.Worldspace)context.GetOrAddAsOverride(state.OutgoingMod);
            // Readd branches below
            newRecord.LargeReferences.Clear();
            newRecord.SubCells.Clear();
            newRecord.TopCell?.Clear();
            
            Console.WriteLine("          Copying Override Record[" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString());
        }
        else
        {
            // Don't duplicate branches, as they will be added below
            var newRecord = (Mutagen.Bethesda.Fallout4.Worldspace)context.Record.Duplicate(state.GetFormKey(context.Record.FormKey));

            state.OutgoingMod.Worldspaces.Add(newRecord);
            
            state.Mapping.Add(context.Record.FormKey, newRecord.FormKey);
            
            Console.WriteLine("          Deep Copying [" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        }
    }
}
