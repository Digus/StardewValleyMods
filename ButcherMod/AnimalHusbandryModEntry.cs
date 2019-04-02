using System;
using AnimalHusbandryMod.animals;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Harmony;
using AnimalHusbandryMod.farmer;
using AnimalHusbandryMod.tools;
using AnimalHusbandryMod.meats;
using DataLoader = AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class AnimalHusbandryModEntry : Mod
    {
        internal static IModHelper ModHelper;
        internal static IMonitor monitor;
        internal static DataLoader DataLoader;
        private SButton? _meatCleaverSpawnKey;
        private SButton? _inseminationSyringeSpawnKey;
        private SButton? _feedingBasketSpawnKey;
        private bool IsEnabled = true;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            monitor = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            if (this.Helper.ModRegistry.IsLoaded("DIGUS.BUTCHER"))
            {
                Monitor.Log("Animal Husbandry Mod can't run along side its older version, ButcherMod. " +
                    "You need to copy the 'data' directory from the ButcherMod directory, into the AnimalHusbandryMod directory, then delete the ButcherMod directory. " +
                    "Animal Husbandry Mod won't load until this is done.", LogLevel.Error);
                IsEnabled = false;
            }
            else
            {
                DataLoader = new DataLoader(Helper);
                _meatCleaverSpawnKey = DataLoader.ModConfig.AddMeatCleaverToInventoryKey;
                _inseminationSyringeSpawnKey = DataLoader.ModConfig.AddInseminationSyringeToInventoryKey;
                _feedingBasketSpawnKey = DataLoader.ModConfig.AddFeedingBasketToInventoryKey;

                //TimeEvents.AfterDayStarted += (x, y) => EventsLoader.CheckEventDay();

                if (!DataLoader.ModConfig.DisableMeat)
                    ModHelper.ConsoleCommands.Add("player_addallmeatrecipes", "Add all meat recipes to the player.", DataLoader.RecipeLoader.AddAllMeatRecipes);

                if (_meatCleaverSpawnKey != null || _inseminationSyringeSpawnKey != null || _feedingBasketSpawnKey != null)
                    Helper.Events.Input.ButtonPressed += this.OnButtonPressed;

                var harmony = HarmonyInstance.Create("Digus.AnimalHusbandryMod");

                try
                {
                    var farmAnimalPet = typeof(FarmAnimal).GetMethod("pet");
                    var animalQueryMenuExtendedPet = typeof(AnimalQueryMenuExtended).GetMethod("Pet");
                    harmony.Patch(farmAnimalPet, new HarmonyMethod(animalQueryMenuExtendedPet), null);
                }
                catch (Exception)
                {
                    Monitor.Log("Error patching the FarmAnimal 'pet' Method. Applying old method of opening the extended animal query menu.", LogLevel.Warn);
                    Helper.Events.Display.MenuChanged += (s, e) =>
                    {
                        if (e.NewMenu is AnimalQueryMenu && !(e.NewMenu is AnimalQueryMenuExtended))
                        {
                            Game1.activeClickableMenu = new AnimalQueryMenuExtended(this.Helper.Reflection.GetField<FarmAnimal>(e.NewMenu, "animal").GetValue());
                        }
                    };
                }

                if (!DataLoader.ModConfig.DisableRancherMeatPriceAjust)
                {
                    var sellToStorePrice = typeof(StardewValley.Object).GetMethod("sellToStorePrice");
                    var sellToStorePricePrefix = typeof(MeatOverrides).GetMethod("sellToStorePrice");
                    harmony.Patch(sellToStorePrice, new HarmonyMethod(sellToStorePricePrefix), null);
                }

                if (!DataLoader.ModConfig.DisableMeat)
                {
                    var objectIsPotentialBasicShippedCategory = typeof(StardewValley.Object).GetMethod("isPotentialBasicShippedCategory");
                    var meatOverridesIsPotentialBasicShippedCategory = typeof(MeatOverrides).GetMethod("isPotentialBasicShippedCategory");
                    harmony.Patch(objectIsPotentialBasicShippedCategory, new HarmonyMethod(meatOverridesIsPotentialBasicShippedCategory), null);

                    var objectCountsForShippedCollection = typeof(StardewValley.Object).GetMethod("countsForShippedCollection");
                    var meatOverridesCountsForShippedCollection = typeof(MeatOverrides).GetMethod("countsForShippedCollection");
                    harmony.Patch(objectCountsForShippedCollection, new HarmonyMethod(meatOverridesCountsForShippedCollection), null);
                }

                //var addSpecificTemporarySprite = typeof(Event).GetMethod("addSpecificTemporarySprite");
                //var addSpecificTemporarySprite = this.Helper.Reflection.GetMethod(new Event(), "addSpecificTemporarySprite").MethodInfo;
                //var addSpecificTemporarySpritePostfix = typeof(EventsOverrides).GetMethod("addSpecificTemporarySprite");
                //harmony.Patch(addSpecificTemporarySprite, null, new HarmonyMethod(addSpecificTemporarySpritePostfix));

                //var petCheckAction = typeof(Pet).GetMethod("checkAction");
                //var petCheckActionPrefix = typeof(PetOverrides).GetMethod("checkAction");
                //harmony.Patch(petCheckAction, new HarmonyMethod(petCheckActionPrefix), null);
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!IsEnabled)
                return;

            DataLoader.ToolsLoader.ReplaceOldTools();
            FarmerLoader.LoadData();
            DataLoader.ToolsLoader.LoadMail();
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!IsEnabled)
                return;

            DataLoader.LivingWithTheAnimalsChannel.CheckChannelDay();
            if (!DataLoader.ModConfig.DisableMeat)
                DataLoader.RecipeLoader.MeatFridayChannel.CheckChannelDay();
            if (!DataLoader.ModConfig.DisablePregnancy)
            {
                PregnancyController.CheckForBirth();
                PregnancyController.UpdatePregnancy();
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!IsEnabled || !Context.IsWorldReady)
                return;

            if (e.Button == _meatCleaverSpawnKey)
                Game1.player.addItemToInventory(new MeatCleaver());

            if (e.Button == _inseminationSyringeSpawnKey)
                Game1.player.addItemToInventory(new InseminationSyringe());
                
            if (e.Button == _feedingBasketSpawnKey)
                Game1.player.addItemToInventory(new FeedingBasket());
        }
    }
}
