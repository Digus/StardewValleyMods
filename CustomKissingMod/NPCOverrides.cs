using System;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Audio;
using StardewValley.Extensions;
using StardewValley.Menus;

namespace CustomKissingMod
{
    internal class NPCOverrides
    {
        private static string _lastDialog = null;
        
        private static void FrameIdentification(Farmer who) {}

        public static bool checkAction_prefix(NPC __instance, ref Farmer who, ref bool __result, GameLocation l, out bool __state)
        {
            __state = __instance.isMoving();
            return true;
        }
        public static void checkAction_postfix(NPC __instance, ref Farmer who, ref bool __result, GameLocation l, bool __state)
        {
            NpcConfig npcConfig = DataLoader.ModConfig.NpcConfigs.Find(n => n.Name == __instance.Name);
            var isMoving = __state;

            if (npcConfig != null && !__instance.isMarried() && CanKissNpc(who, __instance) && who.IsLocalPlayer)
            {
                int timeOfDay = Game1.timeOfDay;
                
                if (__instance.Sprite.CurrentAnimation == null 
                    && (Game1.timeOfDay < 2200) 
                    && (!isMoving && who.ActiveObject == null)
                    && (HasNoDialog(__instance) 
                        || (
                            HasRequiredFriendshipToKiss(who, __instance)
                            && __instance.CurrentDialogue.Count > 0
                            && __instance.CurrentDialogue.Peek().TranslationKey.Equals(_lastDialog))))
                {
                    if (Game1.activeClickableMenu is DialogueBox dialogueBox)
                    {
                        dialogueBox.closeDialogue();
                    }

                    __instance.faceDirection(-3);
                    if (__instance.faceTowardFarmerTimer <= 0)
                    {
                        DataLoader.Helper.Reflection.GetField<NetInt>(__instance, "facingDirectionBeforeSpeakingToPlayer").GetValue().Value = __instance.FacingDirection;
                    }
                    __instance.faceGeneralDirection(who.getStandingPosition(), 0, opposite: false, useTileCalculations: false);
                    who.faceGeneralDirection(__instance.getStandingPosition(), 0, opposite: false, useTileCalculations: false);
                    if (__instance.FacingDirection == 3 || __instance.FacingDirection == 1)
                    {
                        var isWearingIslandAttire = DataLoader.Helper.Reflection.GetField<bool>(__instance,"isWearingIslandAttire").GetValue();
                        int frame = isWearingIslandAttire ? npcConfig.BeachFrame : npcConfig.Frame;
                        bool flag = isWearingIslandAttire ? npcConfig.BeachFrameDirectionRight : npcConfig.FrameDirectionRight;
                        
                        bool flip = flag && __instance.FacingDirection == 3 || !flag && __instance.FacingDirection == 1;
                        if (HasRequiredFriendshipToKiss(who, __instance))
                        {
                            int delay = (__instance.movementPause = (Game1.IsMultiplayer ? 1000 : 10));
                            __instance.faceTowardFarmerForPeriod(3000, 3, faceAway: false, who);
                            __instance.Sprite.ClearAnimation();
                            __instance.Sprite.AddFrame(new FarmerSprite.AnimationFrame(frame, delay, secondaryArm: false, flip, __instance.haltMe, behaviorAtEndOfFrame: true){frameStartBehavior = FrameIdentification});

                            if (!__instance.hasBeenKissedToday.Value)
                            {
                                who.changeFriendship(DataLoader.ModConfig.KissingFriendshipPoints, __instance);
                                DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue()
                                    .broadcastSprites
                                    (who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors"
                                        , new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0
                                        , __instance.Tile * 64f + new Vector2(16f, -64f), flicker: false
                                        , flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
                                {
                                    motion = new Vector2(0f, -0.5f),
                                    alphaFade = 0.01f
                                });
                                l.playSound("dwop", null, null, SoundContext.NPC);
                                if (!DataLoader.ModConfig.DisableExhaustionReset)
                                {
                                    who.exhausted.Value = false;
                                }
                                if (!DataLoader.ModConfig.DisableJealousy)
                                {
                                    if (who.spouse != null && !who.spouse.Contains(__instance.Name) && !who.hasCurrentOrPendingRoommate())
                                    {
                                        NPC characterFromName = Game1.getCharacterFromName(who.spouse, false);
                                        if (l.characters.Contains(characterFromName) || !(Game1.random.NextDouble() >= 0.3 - (double) who.LuckLevel / 100.0 - who.DailyLuck))
                                        {
                                            who.changeFriendship(Math.Abs(DataLoader.ModConfig.JealousyFriendshipPoints)*-1, characterFromName);
                                            characterFromName.CurrentDialogue.Clear();
                                            characterFromName.CurrentDialogue.Push(new StardewValley.Dialogue(characterFromName, null, DataLoader.Helper.Translation.Get("Jealousy.Dialog", new { npcName = (object)__instance.displayName })));
                                            characterFromName.doEmote(12);
                                        }
                                    }
                                }
                            }
                            else if (Game1.random.NextDouble() < 0.1)
                            {
                                __instance.doEmote(20);
                            }
                            __instance.hasBeenKissedToday.Value = true;
                            __instance.Sprite.UpdateSourceRect();
                        }
                        else
                        {
                            __instance.faceDirection(Game1.random.Choose(2, 0));
                            __instance.doEmote(12);
                        }
                        int playerFaceDirection = 1;
                        if ((flag && !flip) || (!flag && flip))
                        {
                            playerFaceDirection = 3;
                        }
                        who.PerformKiss(playerFaceDirection);
                        _lastDialog = null;
                        __result = true;
                    }
                } else if (__instance.hasTemporaryMessageAvailable())
                {
                    _lastDialog = __instance.CurrentDialogue.Peek().TranslationKey;
                }
            }
            else if (__instance.isMarried())
            {
                NpcConfig defaultNpcConfig = DataLoader.DefaultConfig.NpcConfigs.Find(n => n.Name == __instance.Name);
                if (defaultNpcConfig != null && npcConfig != null && defaultNpcConfig.Frame != npcConfig.Frame)
                {
                    if (__instance.Sprite.CurrentFrame == defaultNpcConfig.Frame)
                    {
                        int frame = npcConfig.Frame;
                        bool flag = npcConfig.FrameDirectionRight;

                        bool flip = flag && __instance.FacingDirection == 3 || !flag && __instance.FacingDirection == 1;
                        __instance.Sprite.currentFrame = frame;
                        var oldAnimationFrame = __instance.Sprite.CurrentAnimation[__instance.Sprite.currentAnimationIndex];
                        __instance.Sprite.CurrentAnimation[__instance.Sprite.currentAnimationIndex] =
                            new FarmerSprite.AnimationFrame(frame, 0, oldAnimationFrame.milliseconds, oldAnimationFrame.armOffset, flip, oldAnimationFrame.frameStartBehavior, oldAnimationFrame.frameEndBehavior, 0);
                        __instance.Sprite.UpdateSourceRect();
                    }
                }
            }
        }

        internal static bool checkForCharacterInteractionAtTile_prefix(Vector2 tileLocation, Farmer who, ref bool __result)
        {
            NPC character = Game1.currentLocation.isCharacterAtTile(tileLocation);
            if (character is { IsMonster: false, IsInvisible: false })
            {
                NpcConfig npcConfig = DataLoader.ModConfig.NpcConfigs.Find(n => n.Name == character.Name);

                if (npcConfig != null && !character.isMarried() && CanKissNpc(who, character) && who.IsLocalPlayer)
                {
                    if (character.Sprite.CurrentAnimation == null 
                        && (Game1.timeOfDay < 2200) 
                        && (!character.isMoving() && who.ActiveObject == null)
                        && (HasNoDialog(character)
                            || (HasRequiredFriendshipToKiss(who, character)
                                && character.hasTemporaryMessageAvailable() 
                                && GetTemporaryMessages(who, character).Equals(_lastDialog))))
                    {
                        Game1.mouseCursorTransparency = Utility.tileWithinRadiusOfPlayer((int)tileLocation.X, (int)tileLocation.Y, 1, who) ? 1f : 0.5f;
                        Game1.mouseCursor = 7;
                        __result = true;
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool DrawBreathing_prefix(NPC __instance)
        {
            return __instance.Sprite.CurrentAnimation == null 
                   || __instance.Sprite.CurrentAnimation[__instance.Sprite.currentAnimationIndex].frameStartBehavior != FrameIdentification;
        }

        internal static bool CanKissNpc(Farmer who, NPC npc)
        {
            NpcConfig npcConfig = DataLoader.ModConfig.NpcConfigs.Find(n => n.Name == npc.Name);
            var isDating = Game1.player.friendshipData.TryGetValue(npc.Name, out var friendship) && friendship.IsDating();
            return npcConfig != null
                   && (isDating
                       || DataLoader.ModConfig.DisableDatingRequirement)
                   && (npcConfig.RequiredEvent == null 
                       || who.eventsSeen.Contains(npcConfig.RequiredEvent) 
                       || DataLoader.ModConfig.DisableEventRequirement);
        }

        internal static bool HasRequiredFriendshipToKiss(Farmer who, NPC npc)
        {
            return who.getFriendshipHeartLevelForNPC(npc.Name) >= DataLoader.ModConfig.RequiredFriendshipLevel;
        }

        private static string GetTemporaryMessages(Farmer who, NPC npc)
        {
            if (npc.isOnSilentTemporaryMessage())
            {
                return "";
            }
            if (npc.endOfRouteMessage.Value != null && (npc.doingEndOfRouteAnimation.Value || !npc.goingToDoEndOfRouteAnimation.Value))
            {
                if (!npc.isDivorcedFrom(Game1.player) && (!npc.endOfRouteMessage.Value.Contains("marriage") || npc.getSpouse() == Game1.player))
                {
                    return npc.endOfRouteMessage.Value;
                }
            }
            else if (npc.currentLocation != null && npc.currentLocation.HasLocationOverrideDialogue(npc))
            {
                return npc.currentLocation.GetLocationOverrideDialogue(npc);
            }
            return "";
        }

        private static bool HasNoDialog(NPC npc)
        {
            return !npc.hasTemporaryMessageAvailable() 
                   && npc.currentMarriageDialogue.Count == 0 
                   && npc.CurrentDialogue.Count == 0;
        }
    }
}
