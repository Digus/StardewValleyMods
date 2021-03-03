using Harmony;
using Pathoschild.Stardew.Automate;
using PFMAutomate.Automate;
using ProducerFrameworkMod.Controllers;
using Object = StardewValley.Object;

namespace PFMAutomate
{
    public class CCRMAutomateOverrides
    {
        [HarmonyPriority(800)]
        public static void GetFor(ref object __result, Object obj)
        {
            string machineFullName = __result?.GetType().FullName;
            if (machineFullName == "CCRMAutomate.Automate.ClonerMachine" && (ProducerController.HasProducerRule(obj.Name) || ProducerController.GetProducerConfig(obj.Name) != null))
            {
                __result = new CustomClonerMachine((IMachine)__result);
            }
        }
    }
}
