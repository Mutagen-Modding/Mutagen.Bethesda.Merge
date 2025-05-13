using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.Merge.Lib.DI.GameSpecifications.Oblivion;

public class WorldspaceOverride : ACopyOverride<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter>
{
    private static readonly Worldspace.TranslationMask WorldspaceMask = new(defaultOn: true)
    {
        SubCells = false,
        TopCell = false,
        OffsetData = false,
        SubCellsTimestamp = false,
    };

    public override void HandleCopyFor(
        MergeState<IOblivionMod, IOblivionModGetter> state,
        IModContext<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter> context)
    {
        Worldspace newRecord;
        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            newRecord = (Worldspace)context.GetOrAddAsOverride(state.OutgoingMod);
            // Readd branches below
            newRecord.SubCells.Clear();
            newRecord.TopCell?.Clear();
            
            Console.WriteLine("          Copying Override Record[" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString());
        }
        else
        {
            // Don't duplicate branches, as they will be added below
            newRecord = (Mutagen.Bethesda.Oblivion.Worldspace)context.Record.Duplicate(state.GetFormKey(context.Record.FormKey), WorldspaceMask);

            state.OutgoingMod.Worldspaces.Add(newRecord);
            
            state.Mapping.Add(context.Record.FormKey, newRecord.FormKey);
            
            Console.WriteLine("          Deep Copying [" + context.Record.FormKey.ModKey.Name + "] " + context.Record.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        }
    }
}
