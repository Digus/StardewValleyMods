﻿using Harmony;
using Pathoschild.Stardew.Automate;
using PFMAutomate.Automate;
using ProducerFrameworkMod.Controllers;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace PFMAutomate
{
    public class AutomateOverrides
    {
        public static readonly HashSet<string> SupportedVanillaMachines = new HashSet<string>()
        {
            "Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CharcoalKilnMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.FurnaceMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.KegMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.LoomMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MayonnaiseMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.OilMakerMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.PreservesJarMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.RecyclingMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.SeedMakerMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.SlimeEggPressMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.SodaMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.StatueOfPerfectionMachine"
            , "Pathoschild.Stardew.Automate.Framework.Machines.Objects.WormBinMachine"
        };

        [HarmonyPriority(800)]
        public static void GetFor(ref object __result, SObject obj)
        {
            string machineFullName = __result?.GetType().FullName;
            if (SupportedVanillaMachines.Contains(machineFullName) && (ProducerController.HasProducerRule(obj.Name) || ProducerController.GetProducerConfig(obj.Name) != null))
            {
                __result = new VanillaProducerMachine((IMachine)__result);
            }
        }
    }
}