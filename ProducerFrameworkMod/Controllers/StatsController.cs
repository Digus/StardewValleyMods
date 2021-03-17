using ProducerFrameworkMod.ContentPack;
using StardewValley;

namespace ProducerFrameworkMod.Controllers
{
    public class StatsController
    {
        /// <summary>
        /// Increment the given stats for the given amout
        /// </summary>
        /// <param name="stats">The stat to increment</param>
        /// <param name="amount">The amount to increment</param>
        public static void IncrementStardewStats(StardewStats stats, int amount = 1)
        {
            var statusValue = ProducerFrameworkModEntry.Helper.Reflection.GetProperty<uint>(Game1.stats, stats.ToString());
            statusValue.SetValue(statusValue.GetValue() + (uint) amount);
        }
        
        /// <summary>
        /// Increment the given stats label for the given amout
        /// </summary>
        /// <param name="label">The stat label to increment</param>
        /// <param name="amount">The amount to increment</param>
        public static void IncrementStardewStats(string label, int amount = 1)
        {
            Game1.stats.incrementStat(label, amount);
        }
    }
}
