using System.Collections.Generic;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace WaterRetainingFieldMod
{
    public class WaterRetainingFieldModEntry : Mod
    {
        internal static Config config;

        internal static Dictionary<Vector2, int> TileLocationState =  new Dictionary<Vector2, int>();


        public override void Entry(IModHelper helper)
        {
            //Monitor.Log($"Started {helper.ModRegistry.ModID} from folder: {helper.DirectoryPath}");

            config = helper.ReadConfig<Config>();

            TimeEvents.AfterDayStarted += (x, y) => TileLocationState.Clear();

            var harmony = HarmonyInstance.Create("Digus.WaterRetainingFieldMod");

            var hoeDirtDayUpdate = typeof(HoeDirt).GetMethod("dayUpdate");
            var waterRetainingFieldModEntryDayUpdatePrefix = typeof(WaterRetainingFieldModEntry).GetMethod("DayUpdatePrefix");
            var waterRetainingFieldModEntryDayUpdatePostfix = typeof(WaterRetainingFieldModEntry).GetMethod("DayUpdatePostfix");
            harmony.Patch(hoeDirtDayUpdate, new HarmonyMethod(waterRetainingFieldModEntryDayUpdatePrefix), new HarmonyMethod(waterRetainingFieldModEntryDayUpdatePostfix));

        }

        public static bool DayUpdatePrefix(HoeDirt __instance, ref int __state)
        {
            __state = __instance.state.Value;
            return true;
        }

        public static void DayUpdatePostfix(HoeDirt __instance, ref GameLocation environment, ref Vector2 tileLocation, ref int __state)
        {
            if (environment is Farm farm)
            {
                if (__state == 1 && __instance.fertilizer.Value == 370 || __instance.fertilizer.Value == 371)
                {
                    if (TileLocationState.ContainsKey(tileLocation))
                    {
                        __instance.state.Value = TileLocationState[tileLocation];
                        return;
                    }
                    else
                    {
                        TileLocationState[tileLocation] = __instance.state.Value;
                        AddStateAdjacentFertilizedTiles(farm, tileLocation, __instance.state.Value, __instance.fertilizer.Value);
                    }
                }
            }
        }

        private static void AddStateAdjacentFertilizedTiles(Farm farm, Vector2 tileLocation, int stateValue, int fertilizer)
        {
            Vector2[] adjasent = new Vector2[]
            {
                new Vector2(tileLocation.X, tileLocation.Y + 1)
                , new Vector2(tileLocation.X + 1, tileLocation.Y) 
                , new Vector2(tileLocation.X - 1, tileLocation.Y) 
                , new Vector2(tileLocation.X, tileLocation.Y - 1) 
            };
            foreach (var adjacentTileLocation in adjasent)
            {
                if (!TileLocationState.ContainsKey(adjacentTileLocation) && farm.terrainFeatures.ContainsKey(adjacentTileLocation) && farm.terrainFeatures[adjacentTileLocation] is HoeDirt hoeDirt)
                {
                    if (hoeDirt.state.Value == 1 && hoeDirt.fertilizer.Value == fertilizer)
                    {
                        TileLocationState[adjacentTileLocation] = stateValue;
                        AddStateAdjacentFertilizedTiles(farm, adjacentTileLocation, stateValue, fertilizer);
                    }
                }
            }
            
        }
    }
}
