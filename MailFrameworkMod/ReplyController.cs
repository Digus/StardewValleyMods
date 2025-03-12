using System;
using System.Linq;
using MailFrameworkMod.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;

namespace MailFrameworkMod
{
    public class ReplyController
    {
        private const string MfmDialogKeyPrefix = "MFM_Dialog::";

        public static void OpenReplyDialog(ReplyConfig replyConfig, ITranslationHelper i18n = null)
        {
            var replyConfigQuestionKey = MfmDialogKeyPrefix + replyConfig.QuestionKey;
            var options = replyConfig.Replies
                .Where(a => a.RequireMailReceived == null || (a.RequireAllMailReceived
                    ? !a.RequireMailReceived.Except(Game1.player.mailReceived).Any()
                    : a.RequireMailReceived.Intersect(Game1.player.mailReceived).Any()))
                .Select(i => CreateResponse(replyConfigQuestionKey, i, replyConfig, i18n))
                .ToArray();
            if (Game1.activeClickableMenu != null)
            {
                Game1.currentLocation.lastQuestionKey = replyConfigQuestionKey;
                Game1.nextClickableMenu.Add(new DialogueBox(TranslateOrDefault(i18n, replyConfig.QuestionDialog), options));
            }
            else
            {
                Game1.player.currentLocation.createQuestionDialogue(
                    TranslateOrDefault(i18n, replyConfig.QuestionDialog)
                    , options
                    , replyConfigQuestionKey);
            }
        }

        public static Response CreateResponse(string questionKey, ReplyOption replyOption, ReplyConfig replyConfig, ITranslationHelper i18n = null)
        {
            var answerConfigQuestionKey = questionKey + "_" + replyOption.ReplyKey;
            Action<string> replyOptionAction = (k) =>
            {
                if (replyOption.Cost > 0 && Game1.player.Money < replyOption.Cost)
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1780"));
                    OpenReplyDialog(replyConfig, i18n);
                }
                else
                {
                    if (replyOption.Cost > 0) ShopMenu.chargePlayer(Game1.player, 0, replyOption.Cost);
                    Game1.player.mailReceived.AddRange(replyOption.MailReceivedToAdd);
                    Game1.player.mailReceived.RemoveWhere(s => replyOption.MailReceivedToRemove?.Contains(s) ?? false);
                    Game1.drawObjectDialogue((string)TranslateOrDefault(i18n, replyOption.ReplyResponseDialog));
                }
            };
            ReplyRepository.AddQuestionAndAnswerAction(answerConfigQuestionKey, replyOptionAction);
            return new Response(
                replyOption.ReplyKey
                , TranslateOrDefault(i18n, replyOption.ReplyOptionDialog) + GetCostString(replyOption.Cost));
        }

        private static string GetCostString(int cost)
        {
            return cost > 0 
                ? $" {Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", cost)}" 
                : "";
        }

        public static void answerDialogueAction(string questionAndAnswer)
        {
            try
            {
                Action<string> action = ReplyRepository.GetQuestionAndAnswerAction(questionAndAnswer);
                action?.Invoke(questionAndAnswer);
            }
            catch (Exception e)
            {
                MailFrameworkModEntry.ModMonitor.Log("Error trying to answer an MFM letter.", LogLevel.Error);
                MailFrameworkModEntry.ModMonitor.Log($"The error message above: {e.Message}", LogLevel.Trace);
                MailFrameworkModEntry.ModMonitor.Log(e.StackTrace, LogLevel.Trace);
            }
        }

        private static string TranslateOrDefault(ITranslationHelper i18n, string key)
        {
            return i18n?.Get(key).Default(key) ?? key;
        }
    }
}