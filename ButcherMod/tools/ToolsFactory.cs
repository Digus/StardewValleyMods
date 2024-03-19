using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using Netcode;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Tools;

namespace AnimalHusbandryMod.tools
{
    public class ToolsFactory
    {
        public static Tool GetMeatCleaver()
        {
            Tool meatCleaver = ItemRegistry.Create<Tool>(MeatCleaverOverrides.MeatCleaverItemId);
            meatCleaver.modData.Add(MeatCleaverOverrides.MeatCleaverKey, Game1.random.Next().ToString());
            return meatCleaver;
        }

        public static Item GetInseminationSyringe()
        {
            Tool inseminationSyringe = ItemRegistry.Create<Tool>(InseminationSyringeOverrides.InseminationSyringeItemId);
            inseminationSyringe.modData.Add(InseminationSyringeOverrides.InseminationSyringeKey, Game1.random.Next().ToString());
            return inseminationSyringe;
        }

        public static Item GetFeedingBasket()
        {
            Tool feedingBasket = ItemRegistry.Create<Tool>(FeedingBasketOverrides.FeedingBasketItemId);
            feedingBasket.modData.Add(FeedingBasketOverrides.FeedingBasketKey, Game1.random.Next().ToString());
            return feedingBasket;
        }
        
        public static Item GetParticipantRibbon()
        {
            Tool participantRibbon = ItemRegistry.Create<Tool>(ParticipantRibbonOverrides.ParticipantRibbonItemId);
            participantRibbon.modData.Add(ParticipantRibbonOverrides.ParticipantRibbonKey, Game1.random.Next().ToString());
            return participantRibbon;
        }
    }
}
