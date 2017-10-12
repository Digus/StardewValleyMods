using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MailFrameworkMod
{
    public class MailController
    {
        private static readonly string CUSTOM_MAIL_ID = "taxPassed";

        private static String _nextLetterId = "none";
        private static readonly List<Letter> Letters = new List<Letter>();
        private static Letter _shownLetter = null;

        /// <summary>
        /// Call this method to update the mail box with new letters.
        /// </summary>
        public static void UpdateMailBox()
        {
            List<Letter> newLetters = MailDao.GetValidatedLetters();
            newLetters.RemoveAll((l)=>Letters.Contains(l));
            newLetters.ForEach((l) =>Game1.mailbox.Enqueue(CUSTOM_MAIL_ID));
            Letters.AddRange(newLetters);
            UpdateNextLetterId();
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
                if (Letters.Count > 0 && _nextLetterId == CUSTOM_MAIL_ID)
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
            _shownLetter.Callback?.Invoke();
            UpdateNextLetterId();
            MenuEvents.MenuClosed -= MenuEvents_MenuClosed;
            _shownLetter = null;
        }

        /// <summary>
        /// Sees if there is a new letter on the mailbox and saves its id on the class.
        /// </summary>
        private static void UpdateNextLetterId()
        {
            if (Game1.mailbox.Count > 0)
            {
                _nextLetterId = Game1.mailbox.Peek();
            }
            else
            {
                _nextLetterId = "none";
            }
        }
    }
}
