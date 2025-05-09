using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib.DI.GameSpecifications.Skyrim;

public class WorldspaceOverride : ACopyOverride<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>
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
        MergeState<ISkyrimMod, ISkyrimModGetter> state,
        IModContext<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter> context)
    {
        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            var newRecord = context.GetOrAddAsOverride(state.OutgoingMod);
            // Readd branches below
            newRecord.LargeReferences.Clear();
            newRecord.SubCells.Clear();
            newRecord.TopCell?.Clear();
            
            Console.WriteLine("          Copying Override Record[" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString());
        }
        else
        {
            // Don't duplicate branches, as they will be added below
            var newRecord = context.Record.Duplicate(state.GetFormKey(context.Record.FormKey), WorldspaceMask);

            state.OutgoingMod.Worldspaces.Add(newRecord);
            
            state.Mapping.Add(context.Record.FormKey, newRecord.FormKey);
            
            Console.WriteLine("          Deep Copying [" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        }
    }
}
