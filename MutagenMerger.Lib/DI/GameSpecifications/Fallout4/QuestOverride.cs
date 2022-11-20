using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Fallout4;

namespace MutagenMerger.Lib.DI.GameSpecifications.Fallout4;

public class QuestOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, IQuest, IQuestGetter>
{
    public static readonly Quest.TranslationMask CellMask = new Mutagen.Bethesda.Fallout4.Quest.TranslationMask(defaultOn: true)
    {
    };
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter, IQuest, IQuestGetter> context)
    {

        Mutagen.Bethesda.Fallout4.IQuest? newRecord;


        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            newRecord = context.DuplicateIntoAsNewRecord(state.OutgoingMod,context.Record.EditorID);

        }
        else
        {
            // Don't duplicate branches, as they will be added below
            // newRecord = (Mutagen.Bethesda.Fallout4.Cell)Base.CellOverride.DuplicateCell(state, context, CellMask);
        }

        // Base.CellOverride.CopySubRecords(state, context, newRecord);
    }

}
