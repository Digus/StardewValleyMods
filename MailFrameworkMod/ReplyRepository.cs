using System;
using System.Collections.Generic;

namespace MailFrameworkMod
{
    public class ReplyRepository
    {
        private static readonly Dictionary<string, Action<string>> QuestionAndAnswerActions = new();

        /// <summary>
        /// Save an action to execute after a questionAndAnswer is chosen by the player.
        /// </summary>
        /// <param name="questionAndAnswer">questionAndAction key</param>
        /// <param name="action">the action to execute. receive the questionAndAnswer as parameter.</param>
        public static void AddQuestionAndAnswerAction(string questionAndAnswer, Action<string> action)
        {
            QuestionAndAnswerActions[questionAndAnswer] = action;
        }

        /// <summary>
        /// Get an action to a questionAndAnswer saved before.
        /// </summary>
        /// <param name="questionAndAnswer">questionAndAction key</param>
        /// <returns>the action that receive the questionAndAnswer as parameter.</returns>
        public static Action<string> GetQuestionAndAnswerAction(string questionAndAnswer)
        {
            QuestionAndAnswerActions.TryGetValue(questionAndAnswer, out var action);
            return action;
        }
    }
}
