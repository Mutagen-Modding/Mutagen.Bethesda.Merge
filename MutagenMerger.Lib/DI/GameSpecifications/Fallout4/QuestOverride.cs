using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Fallout4;

namespace MutagenMerger.Lib.DI.GameSpecifications.Fallout4;

public class QuestOverride : ACopyOverride<IFallout4Mod, IFallout4ModGetter, IQuest, IQuestGetter>
{
    public static readonly Quest.TranslationMask QuestMask = new Quest.TranslationMask(defaultOn: true)
    {
        DialogTopics = false
    };
    public static readonly DialogTopic.TranslationMask DialogTopicMask = new DialogTopic.TranslationMask(defaultOn: true)
    {
        Responses = false
    };
    public override void HandleCopyFor(
        MergeState<IFallout4Mod, IFallout4ModGetter> state,
        IModContext<IFallout4Mod, IFallout4ModGetter, IQuest, IQuestGetter> context)
    {

        IQuest? newRecord;


        if (state.IsOverride(context.Record.FormKey, context.ModKey))
        {
            newRecord = context.GetOrAddAsOverride(state.OutgoingMod);

        }
        else
        {
            newRecord = context.Record.Duplicate(state.OutgoingMod.GetNextFormKey(), QuestMask);
        }

        foreach (var topic in context.Record.DialogTopics)
        {
            IDialogTopic newTopic;
            if (state.IsOverride(topic.FormKey, context.ModKey))
            {
                newTopic = (IDialogTopic)Base.DialogTopicOverride.CopyDialogTopicAsOverride(state, (IMajorRecordGetter)topic);
            }
            else
            {
                newTopic = (IDialogTopic)Base.DialogTopicOverride.DuplicateDialogTopic(state, (IMajorRecordGetter)topic, DialogTopicMask);
            }

            foreach (var response in newTopic.Responses)
            {
                Base.DialogTopicOverride.CopyDialogResponses(state, context.ModKey, newTopic, response);
            }

        }

        foreach (var branch in context.Record.DialogBranches)
        {

            DialogBranch newBranch;
            if (state.IsOverride(branch.FormKey, context.ModKey))
            {
                newBranch = (DialogBranch)branch.DeepCopy();
            }
            else
            {
                newBranch = (DialogBranch)branch.Duplicate(state.OutgoingMod.GetNextFormKey());
            }
            state.Mapping.Add(branch.FormKey, newBranch.FormKey);

            newRecord.DialogBranches.Add(newBranch);

            Console.WriteLine("          Deep Copying [" + branch.FormKey.ModKey.Name + "] " + branch.FormKey.IDString() + " to [" + newBranch.FormKey.ModKey.Name + "] " + newBranch.FormKey.IDString());
        }
    }

}
