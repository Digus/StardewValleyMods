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
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.farmer;
using AnimalHusbandryMod.tools;
using AnimalHusbandryMod.meats;

namespace AnimalHusbandryMod
{
    public class AnimalHusbandryModEntery : Mod
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

            if (this.Helper.ModRegistry.IsLoaded("DIGUS.BUTCHER"))
            {
                Monitor.Log("Animal Husbandry Mod can't run along side its older version, ButcherMod. " +
                    "You need to copy the 'data' directory from the ButcherMod directory, into the AnimalHusbandryMod directory, then delete the ButcherMod directory. " +
                    "Animal Husbandry Mod won't load until this is done.", LogLevel.Error);
            }
            else
            {
                DataLoader = new DataLoader(helper);
                _meatCleaverSpawnKey = DataLoader.ModConfig.AddMeatCleaverToInventoryKey;
                _inseminationSyringeSpawnKey = DataLoader.ModConfig.AddInseminationSyringeToInventoryKey;

                SaveEvents.AfterLoad += DataLoader.ToolsLoader.ReplaceOldTools;
                SaveEvents.AfterLoad += (x, y) => FarmerLoader.LoadData();

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

                if (!DataLoader.ModConfig.DisableRancherMeatPriceAjust)
                {
                    var harmony = HarmonyInstance.Create("Digus.AnimalHusbandryMod");
                    var sellToStorePrice = typeof(StardewValley.Object).GetMethod("sellToStorePrice");
                    var prefix = typeof(MeatPriceOverrides).GetMethod("sellToStorePrice");
                    harmony.Patch(sellToStorePrice, new HarmonyMethod(prefix), null);
                }
            }
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
