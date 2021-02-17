using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;

namespace AnimalHusbandryMod.common
{
    internal class TvOverrides
    {
        [HarmonyPriority(Priority.First)]
        public static bool createQuestionDialogue(string question, ref Response[] answerChoices)
        {
            if (question == Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13120"))
            {
                List<Response> answerChoicesList = new List<Response>(answerChoices);
                TvController.GetChannelsWithEpisodeToday().ForEach(c =>
                {
                    answerChoicesList.Insert(answerChoicesList.Count - 1, new Response(c.GetName, c.GetDisplayName));
                });
                answerChoices = answerChoicesList.ToArray();
            }
            return true;
        }

        public static void selectChannel(TV __instance, Farmer who, string answer)
        {
            Channel channel = TvController.GetChannel(answer);
            if (channel != null)
            {
                DataLoader.Helper.Reflection.GetField<int>(__instance, "currentChannel").SetValue(-1);
                DataLoader.Helper.Reflection.GetField<TemporaryAnimatedSprite>(__instance, "screen").SetValue(
                    new TemporaryAnimatedSprite(channel.GetScreenTextureName, channel.GetScreenSourceRectangle, 150f, 2, 999999, __instance.getScreenPosition(), flicker: false, flipped: false, (float)(__instance.boundingBox.Value.Bottom - 1) / 10000f + 1E-05f, 0f, Color.White, __instance.getScreenSizeModifier(), 0f, 0f, 0f)
                );
                Game1.multipleDialogues(channel.GetEpisodesText());
                Game1.afterDialogues = __instance.turnOffTV;
            }
        }

        /// <summary>Patch needed for compatibility with PyTK</summary>
        /// <returns>always true</returns>
        public static bool checkForAction()
        {
            return true;
        }
    }
}
