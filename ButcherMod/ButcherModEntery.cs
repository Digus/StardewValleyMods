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

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            //ControlEvents.KeyPressed += this.ControlEvents_KeyPress;
            DataLoader = new DataLoader(helper);

            TimeEvents.AfterDayStarted += (x, y) => DataLoader.RecipeLoader.MeatFridayChannel.CheckChannelDay();

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
            //if (Context.IsWorldReady) // save is loaded
            //{
            //    this.Monitor.Log($"{Game1.player.name} pressed {e.KeyPressed}.");
            //    if (e.KeyPressed == Keys.G)
            //    {
            //        Game1.createRadialDebris(Game1.currentLocation, 639, Game1.player.getTileX() - 1,
            //            Game1.player.getTileY() - 1, 3, false, -1, true);
            //        Game1.createRadialDebris(Game1.currentLocation, 640, Game1.player.getTileX() - 1,
            //            Game1.player.getTileY() - 1, 3, false, -1, true);
            //        Game1.createRadialDebris(Game1.currentLocation, 641, Game1.player.getTileX() - 1,
            //            Game1.player.getTileY() - 1, 3, false, -1, true);
            //        Game1.createRadialDebris(Game1.currentLocation, 642, Game1.player.getTileX() - 1,
            //            Game1.player.getTileY() - 1, 3, false, -1, true);
            //        Game1.createRadialDebris(Game1.currentLocation, 643, Game1.player.getTileX() - 1,
            //            Game1.player.getTileY() - 1, 3, false, -1, true);
            //        Game1.createRadialDebris(Game1.currentLocation, 644, Game1.player.getTileX() - 1,
            //            Game1.player.getTileY() - 1, 3, false, -1, true);
            //    }
            //    else if (e.KeyPressed == Keys.U)
            //    {
            //        Game1.player.addItemToInventory(new MeatCleaver());
            //    }
            //}
        }
    }
}
