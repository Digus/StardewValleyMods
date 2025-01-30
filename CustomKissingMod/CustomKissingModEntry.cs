using System;
using System.Linq;
using CustomKissingMod.Api;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace CustomKissingMod
{
    public class CustomKissingModEntry : Mod
    {
        public const string MessageType = "Kissing";
        internal IModHelper ModHelper;
        internal static IMonitor ModMonitor;
        internal DataLoader DataLoader;
        public static IManifest Manifest;
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            Manifest = ModManifest;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            DataLoader = new DataLoader(ModHelper);
            DataLoader.LoadContentPacks();

            try
            {
                var harmony = new Harmony("Digus.CustomKissingMod");
                harmony.Patch(
                    original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                    prefix: new HarmonyMethod(typeof(NPCOverrides), nameof(NPCOverrides.checkAction_prefix)),
                    postfix: new HarmonyMethod(typeof(NPCOverrides), nameof(NPCOverrides.checkAction_postfix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), nameof(Utility.checkForCharacterInteractionAtTile)),
                    prefix: new HarmonyMethod(typeof(NPCOverrides), nameof(NPCOverrides.checkForCharacterInteractionAtTile_prefix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(NPC), nameof(NPC.DrawBreathing)),
                    prefix: new HarmonyMethod(typeof(NPCOverrides), nameof(NPCOverrides.DrawBreathing_prefix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log("Error while trying to apply harmony patch. This mod won't work.",LogLevel.Error);
                Monitor.Log($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public override object GetApi()
        {
            return new CustomKissingModApi();
        }
    }
}
