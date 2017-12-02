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
using StardewValley.Buildings;
using StardewValley.Menus;
using Harmony;
using ButcherMod.animals;
using ButcherMod.farmer;
using ButcherMod.tools;

namespace ButcherMod
{
    public class ButcherModEntery : Mod
    {

        internal static IModHelper ModHelper;
        internal static IMonitor monitor;
        internal static DataLoader DataLoader;
        private string _meatCleaverSpawnKey;
        private string _inseminationSyringeSpawnKey;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            monitor = Monitor;

            DataLoader = new DataLoader(helper);
            _meatCleaverSpawnKey = DataLoader.ModConfig.AddMeatCleaverToInventoryKey;
            _inseminationSyringeSpawnKey = DataLoader.ModConfig.AddInseminationSyringeToInventoryKey;

            if (!DataLoader.ModConfig.DisableMeat)
            {
                TimeEvents.AfterDayStarted += (x, y) => DataLoader.RecipeLoader.MeatFridayChannel.CheckChannelDay();
            }

            if (_meatCleaverSpawnKey != null || _inseminationSyringeSpawnKey != null)
            {
                ControlEvents.KeyPressed += this.ControlEvents_KeyPress;
            }

            if (!DataLoader.ModConfig.DisablePregnancy)
            {
                TimeEvents.AfterDayStarted += (x, y) => PregnancyController.CheckForBirth();
                SaveEvents.BeforeSave += (x, y) => PregnancyController.UpdatePregnancy();
                MenuEvents.MenuChanged += (s, e) =>
                {
                    if (e.NewMenu is AnimalQueryMenu && !(e.NewMenu is AnimalQueryMenuExtended))
                    {
                        Game1.activeClickableMenu = new AnimalQueryMenuExtended(this.Helper.Reflection.GetPrivateValue<FarmAnimal>(e.NewMenu, "animal"));
                    }
                };
            }            

            var harmony = HarmonyInstance.Create("Digus.ButcherMod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
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
                if (_inseminationSyringeSpawnKey != null && e.KeyPressed == (Keys)Enum.Parse(typeof(Keys), _inseminationSyringeSpawnKey.ToUpper()))
                {
                    Game1.player.addItemToInventory(new InseminationSyringe());
                }
            }
        }
    }
}
