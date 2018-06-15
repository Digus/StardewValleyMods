using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    public class EverlastingBaitsAndUnbreakableTacklesModEntery : Mod
    {
        public static IMonitor ModMonitor;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            new DataLoader(helper);

            var harmony = HarmonyInstance.Create("Digus.InfiniteBaitAndLureMod");

            var fishingRodDoDoneFishing =
                typeof(FishingRod).GetMethod("doDoneFishing", BindingFlags.NonPublic | BindingFlags.Instance);
            var fishingRodOverridesDoDoneFishing = typeof(GameOverrides).GetMethod("DoDoneFishing");
            harmony.Patch(fishingRodDoDoneFishing, new HarmonyMethod(fishingRodOverridesDoDoneFishing), null);

            var craftingRecipeCreateItem = typeof(CraftingRecipe).GetMethod("createItem");
            var fishingRodOverridesCreateItem = typeof(GameOverrides).GetMethod("CreateItem");
            harmony.Patch(craftingRecipeCreateItem, new HarmonyMethod(fishingRodOverridesCreateItem), null);

            var craftingPageClickCraftingRecipe = typeof(CraftingPage).GetMethod("clickCraftingRecipe",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var gameOverridesClickCraftingRecipe = typeof(GameOverrides).GetMethod("ClickCraftingRecipe");
            harmony.Patch(craftingPageClickCraftingRecipe, new HarmonyMethod(gameOverridesClickCraftingRecipe), null);

            var npcTryToReceiveActiveObject = typeof(NPC).GetMethod("tryToReceiveActiveObject");
            var gameOverridesTryToReceiveActiveObject = typeof(GameOverrides).GetMethod("TryToReceiveActiveObject");
            harmony.Patch(npcTryToReceiveActiveObject, new HarmonyMethod(gameOverridesTryToReceiveActiveObject), null);

            if (!DataLoader.ModConfig.DisableIridiumQualityFish)
            {
                var bobberBarConstructor = typeof(BobberBar).GetConstructor(new []{ typeof(int), typeof(float), typeof(bool), typeof(int)});
                var gameOverridesBobberBar = typeof(GameOverrides).GetMethod("BobberBar");
                harmony.Patch(bobberBarConstructor, null, new HarmonyMethod(gameOverridesBobberBar));
            }

            helper.ConsoleCommands.Add("player_addallbaitstacklesrecipes",
                "Add all everlasting baits and unbreakable tackles recipes to the player.", AddAllBaitTackleRecipes);
            helper.ConsoleCommands.Add("player_getallbaitstackles", "Get all everlasting baits and unbreakable tackles.",GetAllBaitTackle);
        }

        private static void AddAllBaitTackleRecipes(string command, string[] args)
        {
            if (Context.IsWorldReady)
            {
                foreach (BaitTackle baitTackle in Enum.GetValues(typeof(BaitTackle)))
                {
                    if (!Game1.player.craftingRecipes.ContainsKey(baitTackle.GetDescription()))
                    {
                        Game1.player.craftingRecipes.Add(baitTackle.GetDescription(), 0);
                        ModMonitor.Log($"Added {baitTackle.GetDescription()} recipe to the player.", LogLevel.Info);
                    }
                }
            }
            else
            {
                ModMonitor.Log("No player loaded to add the recipes.", LogLevel.Info);
            }
        }

        private static void GetAllBaitTackle(string command, string[] args)
        {
            if (Context.IsWorldReady)
            {
                if (Game1.activeClickableMenu == null)
                {
                    IList<Item> baitsTackles = new List<Item>();
                    foreach (BaitTackle baitTackle in Enum.GetValues(typeof(BaitTackle)))
                    {
                        baitsTackles.Add(new Object((int)baitTackle, 1, false, -1, 4));
                    }
                    Game1.activeClickableMenu = new ItemGrabMenu(baitsTackles);
                }
                else
                {
                    ModMonitor.Log("Close all menus to use this command.", LogLevel.Info);
                }
            }
            else
            {
                ModMonitor.Log("No player loaded to get the baits and tackles.", LogLevel.Info);
            }
        }
    }
}
