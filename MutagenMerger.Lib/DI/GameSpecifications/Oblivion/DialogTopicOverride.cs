using System;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Records;

namespace MutagenMerger.Lib.DI.GameSpecifications.Oblivion;

public class DialogTopicOverride : ACopyOverride<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter>
{
    private static readonly DialogTopic.TranslationMask DialogTopicMask = new Mutagen.Bethesda.Oblivion.DialogTopic.TranslationMask(defaultOn: true)
    {
        Items = false
    };
    public override void HandleCopyFor(
        MergeState<IOblivionMod, IOblivionModGetter> state,
        IModContext<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter> context)
    {
        Mutagen.Bethesda.Oblivion.DialogTopic newRecord;
        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            newRecord = (DialogTopic)Base.DialogTopicOverride.CopyDialogTopicAsOverride(state, (IMajorRecordGetter)context.Record);
        }
        else
        {
            newRecord = (DialogTopic)Base.DialogTopicOverride.DuplicateDialogTopic(state, (IMajorRecordGetter)context.Record, DialogTopicMask);
        }

        // Do the branches
        foreach (var branch in context.Record.Items)
        {
            CopyDialogItem(state, context.ModKey, newRecord, branch);
        }
    }


    private void CopyDialogItem(
        MergeState<IOblivionMod, IOblivionModGetter> state,
        ModKey currentMod,
        Mutagen.Bethesda.Oblivion.DialogTopic topic,
        IDialogItemGetter item)
    {
        Mutagen.Bethesda.Oblivion.DialogItem newRecord;
        if (state.IsOverride(item.FormKey, currentMod))
        {
            newRecord = item.DeepCopy();
            Console.WriteLine("          Copying Override Record[" + currentMod.Name + "] " + item.FormKey.IDString());
        }
        else
        {
            newRecord = item.Duplicate(state.GetFormKey(item.FormKey));
            state.Mapping.Add(item.FormKey, newRecord.FormKey);
            Console.WriteLine("          Deep Copying [" + currentMod.Name + "] " + item.FormKey.IDString() + " to [" + newRecord.FormKey.ModKey.Name + "] " + newRecord.FormKey.IDString());
        }

        topic.Items.Add(newRecord);
    }
}
