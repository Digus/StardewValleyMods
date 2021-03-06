﻿using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Menus;

namespace MailServicesMod
{
    internal class GiftShipmentController
    {
        internal const string GiftResponseKeyNext = "Next";
        internal const string GiftResponseKeyPrevious = "Previous";
        internal const string GiftResponseKeyNone = "None";
        internal const string GiftDialogKey = "MailServiceMod_GiftShipment";

        internal static bool CreateResponsePage(int page)
        {
            List<Response> options = new List<Response>();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (Game1.player.tryGetFriendshipLevelForNPC(npc.Name) != null
                    && Game1.player.friendshipData[npc.Name].GiftsToday < 1
                    && (Game1.player.friendshipData[npc.Name].GiftsThisWeek < 2 || npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
                    && !Game1.player.friendshipData[npc.Name].IsDivorced()
                    && (Game1.player.friendshipData[npc.Name].Points < 2500 || DataLoader.ModConfig.EnableGiftToNpcWithMaxFriendship)
                    && !(Game1.player.spouse != null && Game1.player.spouse.Equals(Game1.player.Name))
                    && !(npc is Child))
                {
                    options.Add(new Response(npc.Name, npc.displayName + (npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth) ? " (" + Game1.content.LoadString("Strings\\UI:Profile_Birthday") +  ")" : "")));
                }
            }
            if (options.Count > 0)
            {
                options.Sort((r1, r2) => r1.responseText.CompareTo(r2.responseText));
                List<Response> optionsPage = options.Skip(page * DataLoader.ModConfig.GiftChoicePageSize).Take(DataLoader.ModConfig.GiftChoicePageSize).ToList();
                if (page > 0)
                {
                    optionsPage.Add(new Response(GiftResponseKeyPrevious + (page - 1), DataLoader.I18N.Get("Shipment.Gift.Previous")));
                }
                if (page < (options.Count - 1) / DataLoader.ModConfig.GiftChoicePageSize)
                {
                    optionsPage.Add(new Response(GiftResponseKeyNext + (page + 1), DataLoader.I18N.Get("Shipment.Gift.Next")));
                }
                optionsPage.Add(new Response("None", DataLoader.I18N.Get("Shipment.Gift.None")));
                Game1.player.currentLocation.createQuestionDialogue(DataLoader.I18N.Get("Shipment.Gift.Question", new { Gift = Game1.player.ActiveObject.DisplayName }), optionsPage.ToArray(), GiftDialogKey);
                return false;
            }
            return true;
        }

        internal static void GiftToNpc(string npcName)
        {
            NPC npc = Game1.getCharacterFromName(npcName);
            Farmer who = Game1.player;
            string giftName = who.ActiveObject.DisplayName;
            npc.receiveGift(who.ActiveObject, who, true, 1, DataLoader.ModConfig.ShowDialogOnItemDelivery);
            ShopMenu.chargePlayer(who, 0, DataLoader.ModConfig.GiftServiceFee);
            who.reduceActiveItemByOne();
            who.completeQuest(25);
            if (!DataLoader.ModConfig.ShowDialogOnItemDelivery)
            {
                Game1.drawObjectDialogue(DataLoader.I18N.Get("Shipment.Gift.GiftSent", new { Gift = giftName, Npc = npc.displayName }));
            }
            if (DataLoader.ModConfig.EnableJealousyFromMailedGifts)
            {
                if (npc.datable.Value 
                    && who.spouse != null 
                    && !who.spouse.Contains(npc.Name) 
                    && !who.spouse.Contains("Krobus") 
                    && Utility.isMale(who.spouse) == Utility.isMale(npc.Name) 
                    && Game1.random.NextDouble() < 0.3 - (double)((float)who.LuckLevel / 100f) - who.DailyLuck 
                    && !npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth) 
                    && who.friendshipData[npc.Name].IsDating())
                {
                    NPC spouse = Game1.getCharacterFromName(who.spouse);
                    who.changeFriendship(-30, spouse);
                    spouse.CurrentDialogue.Clear();
                    spouse.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3985", npc.displayName), spouse));
                }
            }
        }
    }
}
