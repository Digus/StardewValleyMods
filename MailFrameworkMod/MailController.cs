﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewModdingAPI;

namespace MailFrameworkMod
{
    public class MailController
    {
        public static readonly string CustomMailId = "MailFrameworkPlaceholderId";

        private static String _nextLetterId = "none";
        private static readonly List<Letter> Letters = new List<Letter>();
        private static Letter _shownLetter = null;

        //private static Queue<string> queue = new Queue<string>(Game1.mailbox);

        /// <summary>
        /// Call this method to update the mail box with new letters.
        /// </summary>
        public static void UpdateMailBox()
        {            
            List<Letter> newLetters = MailDao.GetValidatedLetters();
            newLetters.RemoveAll((l)=>Letters.Contains(l));
            // CUSTOM
            // newLetters.ForEach((l) => queue.Enqueue(CustomMailId)); //=>Game1.mailbox.Enqueue(CustomMailId));            
            newLetters.ForEach((l) => Game1.mailbox.Add(CustomMailId));
            Letters.AddRange(newLetters);
            UpdateNextLetterId();
        }


        /// <summary>
        /// Call this method to unload any new letters still on the mailbox.
        /// </summary>
        public static void UnloadMailBox()
        {
            // CUSTOM    
            List<String> tempMailBox = new List<string>();
            while (Game1.mailbox.Count > 0)
            {
                //tempMailBox.Add(queue.Dequeue()); //Game1.mailbox.Dequeue());
                tempMailBox.Add(Game1.mailbox.First());
                Game1.mailbox.Remove(Game1.mailbox.First());
            }
            foreach (Letter letter in Letters)
            {
                tempMailBox.Remove(CustomMailId);
            }
            foreach (String mail in tempMailBox)
            {
                //Game1.mailbox.Enqueue(mail);
                //queue.Enqueue(mail);
                Game1.mailbox.Add(mail);
            }
            Letters.Clear();
        }

        /// <summary>
        /// If exists any custom mail to be delivered.
        /// </summary>
        /// <returns></returns>
        public static bool HasCustomMail()
        {
            return Letters.Count > 0;
        }

        /// <summary>
        /// Shows any custom letter waiting in the mail box.
        /// Don't do anything if there is already a letter being shown.
        /// </summary>
        public static void ShowLetter()
        {            
            if (_shownLetter == null)
            {
                if (Letters.Count > 0 && _nextLetterId == CustomMailId)
                {             
                    _shownLetter = Letters.First();                 
                    var activeClickableMenu = new LetterViewerMenu(_shownLetter.Text.Replace("@", Game1.player.Name),_shownLetter.Id);
                    Game1.activeClickableMenu = activeClickableMenu;
                    _shownLetter.Items?.ForEach(
                        (i) => activeClickableMenu.itemsToGrab.Add
                        (
                            new ClickableComponent
                            (
                                new Rectangle
                                (
                                    activeClickableMenu.xPositionOnScreen + activeClickableMenu.width / 2 - 12 * Game1.pixelZoom
                                    ,activeClickableMenu.yPositionOnScreen + activeClickableMenu.height - Game1.tileSize / 2 -24 * Game1.pixelZoom
                                    , 24 * Game1.pixelZoom
                                    , 24 * Game1.pixelZoom
                                )
                                , i

                            )
                            {
                                myID = 104,
                                leftNeighborID = 101,
                                rightNeighborID = 102
                            }
                        )
                    );
                    if (_shownLetter.Recipe != null)
                    {
                        string recipe = _shownLetter.Recipe;
                        if (!Game1.player.cookingRecipes.ContainsKey(recipe))
                        {
                            Game1.player.cookingRecipes.Add(recipe, 0);
                        }

                        string learnedRecipe = recipe;
                        
                        if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
                        {
                            string recipeString = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes")[recipe];
                            string[] strArray = recipeString.Split('/');
                            if (strArray.Length < 5)
                            {
                                MailFrameworkModEntery.monitor.Log($"The recipe '{recipe}' does not have a internationalized name. The default name will be used.", LogLevel.Warn);
                            }
                            else {
                                learnedRecipe = strArray[strArray.Length - 1];
                            }                            
                        }
                            
                        // CUSTOM
                        MailFrameworkModEntery.ModHelper.Reflection
                            //.GetPrivateField<String>(activeClickableMenu, "cookingOrCrafting").SetValue(Game1.content.LoadString("Strings\\UI:LearnedRecipe_cooking"));
                            .GetField<String>(activeClickableMenu, "cookingOrCrafting").SetValue(Game1.content.LoadString("Strings\\UI:LearnedRecipe_cooking"));
                        MailFrameworkModEntery.ModHelper.Reflection
                            //.GetPrivateField<String>(activeClickableMenu, "learnedRecipe").SetValue(learnedRecipe);
                            .GetField<String>(activeClickableMenu, "learnedRecipe").SetValue(learnedRecipe);
                    }

                    MenuEvents.MenuClosed += MenuEvents_MenuClosed;
                    ;
                }
                else
                {                    
                    UpdateNextLetterId();
                }
            }            
        }

        /// <summary>
        /// Event method to be called when the letter menu is closed.
        /// Remove the showed letter from the list and calls the callback function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            Letters.Remove(_shownLetter);
            _shownLetter.Callback?.Invoke(_shownLetter);
            UpdateNextLetterId();
            MenuEvents.MenuClosed -= MenuEvents_MenuClosed;
            _shownLetter = null;
        }

        /// <summary>
        /// Sees if there is a new letter on the mailbox and saves its id on the class.
        /// </summary>
        private static void UpdateNextLetterId()
        {
            // CUSTOM
            if (Game1.mailbox.Count > 0)
            {
                // _nextLetterId = Game1.mailbox.Peek();
                //_nextLetterId = queue.Peek();                
                _nextLetterId = Game1.mailbox.First();    
                
            }
            else
            {
                _nextLetterId = "none";
            }
        }
    }
}
