﻿using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using ProducerFrameworkMod.Api;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace ProducerFrameworkMod
{
    public class ProducerFrameworkModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; set; }
        internal new static IModHelper Helper { get; set; }

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Helper = helper;
            
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.GameLaunched += DataLoader.LoadContentPacks;
            helper.Events.GameLoop.SaveLoaded += DataLoader.LoadContentPacks;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, EventArgs e)
        {
            new DataLoader(Helper);

            var harmony = new Harmony("Digus.ProducerFrameworkMod");

            Assembly dgaAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.StartsWith("DynamicGameAssets,"));
            if (dgaAssembly != null)
            {
                MethodInfo dgaLoadDisplayNameMethodInfo = AccessTools
                    .GetDeclaredMethods(
                        dgaAssembly.GetType("DynamicGameAssets.Game.CustomObject")).Find(m =>
                        m.Name == "loadDisplayName");
                harmony.Patch(
                    original: dgaLoadDisplayNameMethodInfo,
                    prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.LoadDisplayName))
                    {
                        priority = Priority.First
                    }
                );
            }

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.PerformObjectDropInAction))
                {
                    priority = Priority.First
                }
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "loadDisplayName"),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.LoadDisplayName))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.checkForActionPrefix)),
                postfix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.checkForActionPostfix))
            ); 
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.minutesElapsed)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.minutesElapsedPrefix)),
                postfix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.minutesElapsedPostfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performDropDownAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.performDropDownAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.DayUpdate)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.DayUpdate))
            ); 
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.draw), new Type[]{typeof(SpriteBatch),typeof(int), typeof(int), typeof(float)}),
                transpiler: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.draw_Transpiler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.initializeLightSource)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.initializeLightSource))
            );
        }

        public override object GetApi()
        {
            return new ProducerFrameworkModApi();
        }
    }
}
