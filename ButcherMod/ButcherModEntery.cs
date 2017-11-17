using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ButcherMod
{
    public class ButcherModEntery : Mod
    {

        internal static IModHelper ModHelper;
        internal static DataLoader DataLoader;
        private string _meatCleaverSpawnKey;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            
            DataLoader = new DataLoader(helper);
            _meatCleaverSpawnKey = DataLoader.ModConfig.MeatCleaverSpawnKey;

            TimeEvents.AfterDayStarted += (x, y) => DataLoader.RecipeLoader.MeatFridayChannel.CheckChannelDay();

            if (_meatCleaverSpawnKey != null)
            {
                ControlEvents.KeyPressed += this.ControlEvents_KeyPress;
            }
            
            //SaveEvents.AfterLoad += SaveEvents_AfterLoad;

        }

        /// <summary>
        /// To be invoked after returning loading a game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {

        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ControlEvents_KeyPress(object sender, EventArgsKeyPressed e)
        {
            if (Context.IsWorldReady) // save is loaded
            {
                
                if (_meatCleaverSpawnKey != null && e.KeyPressed == (Keys)Enum.Parse(typeof(Keys), _meatCleaverSpawnKey.ToUpper()))
                {
                    Game1.player.addItemToInventory(new MeatCleaver());
                }
            }
        }
    }
}
