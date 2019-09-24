using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.animals.events;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.animals
{
    public class AnimalContestEventBuilder
    {
        public static readonly IList<string> Seasons = new ReadOnlyCollection<string>(new List<string>() {"spring", "summer", "fall", "winter"});

        public static readonly IAnimalContestAct[] PossibleThirdContenders = new IAnimalContestAct[] { new ShaneAct(), new JodiAct(), new EmilyAct()};
        public static readonly IAnimalContestAct[] PossibleSecondContenders = new IAnimalContestAct[] { new JasAct(),new AlexAct(), new AbigailAct(), new WillyAct(), new MaruAct() };
        public static readonly string[] PossibleAnimals = { "White_Cow", "Brown_Cow", "Goat", "Sheep", "Pig", "Brown_Chicken", "White_Chicken", "Duck", "Rabbit" };
        public static readonly int MinPointsToPossibleWin = 11;
        public static readonly int MinPointsToGaranteeWin = 14;

        private const string SmallSize = "16 16";
        private const string BigSize = "32 32";

        private static readonly ITranslationHelper i18n = DataLoader.i18n;

        public static CustomEvent CreateEvent()
        {
            SDate contestDate = AnimalContestController.GetNextContestDate();
            int eventId = GetEventId(contestDate);
            string key = GenerateKey(eventId, contestDate);

            Random random = new Random((int)((long)Game1.uniqueIDForThisGame * 100000 + contestDate.Year* 1000 + Utility.getSeasonNumber(contestDate.Season) *100 + contestDate.Day));

            //Player and Participant init
            long? contestParticipantId = AnimalContestController.GetNextContestParticipantId();
            AnimalStatus participantAnimalStatus = contestParticipantId != null ?AnimalStatusController.GetAnimalStatus((long)contestParticipantId): null;
            bool isPlayerJustWatching = participantAnimalStatus == null;
            bool isParticipantPet = !isPlayerJustWatching && participantAnimalStatus.Id == AnimalData.PetId;
            FarmAnimal farmAnimal = null;
            if (isParticipantPet)
            {
                AnimalContestController.TemporalyRemovePet();
            }
            else if (!isPlayerJustWatching)
            {
                farmAnimal = AnimalContestController.GetAnimal(participantAnimalStatus.Id);
                AnimalContestController.TemporalyRemoveFarmAnimal(farmAnimal);
            }

            List<AnimalContestItem> history = FarmerLoader.FarmerData.AnimalContestData;

            string[] contenders = new string[3] ;
            contenders[0] = "Marnie";
            contenders[1] = GetSecondContender(history, isPlayerJustWatching);
            contenders[2] = GetThirdContender(history);

            string marnieAnimal = ChooseMarnieAnimal(random, history);
            VincentAnimal vincentAnimal = ChooseVincentAnimal(random, history);

            AnimalContestItem animalContestInfo = new AnimalContestItem(eventId, contestDate, contenders.ToList(), vincentAnimal.ToString(), marnieAnimal)
            {
                ParticipantId = isParticipantPet ? AnimalData.PetId : farmAnimal?.myID.Value
            };
            animalContestInfo = PickTheWinner(animalContestInfo, history, participantAnimalStatus, farmAnimal, contenders[2]);

            // Deciding who will be present
            bool isHaleyPresent = Game1.player.eventsSeen.Contains(14);
            bool isKentPresent = Game1.year > 1 && contenders.Contains("Jodi");
            bool isSebastianPresent = vincentAnimal == VincentAnimal.Frog || contenders.Contains("Abigail");
            bool isDemetriusPresent = contenders.Contains("Maru");
            bool isClintPresent = contenders.Contains("Emily");
            bool isLeahPresent = Game1.player.eventsSeen.Contains(53); 

            StringBuilder initialPosition = new StringBuilder();
            initialPosition.Append("none/-100 -100");
            if (!isPlayerJustWatching)
            {
                initialPosition.Append("/farmer 27 62 2");
            }
            else
            {
                initialPosition.Append("/farmer 37 62 2");
            }

            initialPosition.Append(" Lewis 28 63 2");
            initialPosition.Append($" {contenders[0]} 24 66 3");
            initialPosition.Append($" {contenders[1]} 30 66 1");
            initialPosition.Append($" {contenders[2]} 33 66 1");
            if (isKentPresent)
            {
                initialPosition.Append($" Kent 36 66 3");
            }

            if (!contenders.Contains("Jodi"))
            {
                initialPosition.Append($" Jodi 36 65 3");
            }
            initialPosition.Append($" Sam 37 66 3");
            initialPosition.Append($" Gus 36 68 3");
            initialPosition.Append($" Evelyn 30 69 0");
            initialPosition.Append($" George 31 69 0");
            if (!contenders.Contains("Alex"))
            {
                initialPosition.Append($" Alex 31 70 0");
            }
            initialPosition.Append($" Pierre 26 69 1");
            initialPosition.Append($" Caroline 27 69 3");
            initialPosition.Append($" Elliott 33 69 0");
            if (!contenders.Contains("Willy"))
            {
                initialPosition.Append($" Willy 35 69 0");
            }
            if (isHaleyPresent)
            {
                initialPosition.Append($" Haley 22 68 1");
            }
            if (isLeahPresent)
            {
                initialPosition.Append($" Leah 22 70 0");
            }
            if (isSebastianPresent)
            {
                initialPosition.Append($" Sebastian 37 67 3");
            }
            if (isDemetriusPresent)
            {
                initialPosition.Append($" Demetrius 32 70 0");
            }
            if (isClintPresent)
            {
                initialPosition.Append($" Clint 34 70 0");
            }
            if (isPlayerJustWatching)
            {
                initialPosition.Append($" Jas 27 66 0");
            }
            else if(!contenders.Contains("Jas"))
            {
                initialPosition.Append($" Jas 23 70 0");
            }
            if (!contenders.Contains("Shane") && (contenders.Contains("Jas") || isPlayerJustWatching))
            {
                initialPosition.Append($" Shane 24 70 0");
            }

            bool linusAlternateAnimal = false;
            if (!contenders.Contains("Linus"))
            {
                initialPosition.Append($" Linus 37 70 3");
            }
            else
            {
                linusAlternateAnimal = history.Count(h => h.Contenders.Contains("Linus")) % 2 == new Random((int)Game1.uniqueIDForThisGame).Next(2);
            }

            initialPosition.Append($" Vincent 28 80 0");

            if (isParticipantPet)
            {
                if (!Game1.player.catPerson)
                {
                    initialPosition.Append(" dog 26 66 2/showFrame Dog 23");
                }
                else
                {
                    initialPosition.Append(" cat 26 66 2/showFrame Cat 18");
                }
            }
            else
            {
                if (!isPlayerJustWatching)
                {
                    string playerAnimalTextureName = farmAnimal.Sprite.textureName.Value.Split('\\')[1].Replace(' ', '_');
                    bool isPlayerAnimalSmall = IsAnimalSmall(playerAnimalTextureName);
                    initialPosition.Append($"/addTemporaryActor {playerAnimalTextureName} {(isPlayerAnimalSmall? SmallSize : BigSize)} {(isPlayerAnimalSmall?26:25)} 66 0 false Animal participant/showFrame participant 0");
                }
            }
            initialPosition.Append("/specificTemporarySprite animalContest/broadcastEvent/skippable");
            initialPosition.Append(GetContendersAnimalPosition(contenders, marnieAnimal, isPlayerJustWatching, linusAlternateAnimal));

            initialPosition.Append("/viewport 28 65 true");

            StringBuilder eventAction = new StringBuilder();
            if (isHaleyPresent)
            {
                eventAction.Append("/showFrame Haley 25");
            }
            if (contenders.Contains("Maru"))
            {
                eventAction.Append("/animate Maru false false 130 16 16 16 16 16 17 18 19 20 21 22 23 23 23 23");
            }
            if (!isPlayerJustWatching)
            {
                eventAction.Append("/move farmer 0 4 2/faceDirection farmer 3");
            }
            else
            {
                eventAction.Append("/move farmer 0 3 2/faceDirection farmer 3");
            }
                
            if (isHaleyPresent)
            {
                eventAction.Append("/pause 200/playSound cameraNoise/shake Haley 50/screenFlash .5/pause 1000/showFrame Haley 5/pause 1000");
            }
            if (!isPlayerJustWatching)
            {
                eventAction.Append("/faceDirection farmer 0");
            }

            if (!isPlayerJustWatching)
            {
                eventAction.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.Lewis.PlayerParticipant")}\"");
            }
            else
            {
                eventAction.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.Lewis.PlayerWatching")}\"");
            }

            eventAction.Append($"/pause 500/emote Lewis 40/speak Lewis \"{i18n.Get("AnimalContest.Dialog.Lewis.Attention")}\"");
            eventAction.Append($"/faceDirection {contenders[0]} 0 true");
            eventAction.Append($"/faceDirection {contenders[1]} 0");
            eventAction.Append($"/faceDirection {contenders[2]} 0 true");
            eventAction.Append($"/faceDirection Pierre 0 true");
            if (!contenders.Contains("Willy"))
            {
                initialPosition.Append($"/faceDirection Willy 3");
            }
            eventAction.Append($"/faceDirection Caroline 0");

            if (history.Count == 0)
            {
                eventAction.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.Lewis.IntroductionFirstTime")}\"");
            }
            else
            {
                eventAction.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.Lewis.IntroductionOtherTimes")}\"");
            }

            eventAction.Append(new VincentAct().GetAct(animalContestInfo, history));

            eventAction.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.Lewis.ContestExplanation")}\"");
            eventAction.Append("/move Lewis 0 4 2/move Lewis -5 0 3/faceDirection Lewis 0");
            eventAction.Append(new MarnieAct().GetAct(animalContestInfo, history));
            eventAction.Append("/faceDirection Lewis 1/move Lewis 3 0 1/faceDirection Lewis 0");
            if (!isPlayerJustWatching)
            {
                eventAction.Append(GetPlayerEvalutionAct(animalContestInfo, farmAnimal));
            }
            else
            {
                eventAction.Append(new JasAct().GetAct(animalContestInfo, history));
            }
            eventAction.Append("/faceDirection Lewis 1/move Lewis 5 0 1/faceDirection Lewis 0");
            eventAction.Append(PossibleSecondContenders.First(c => c.NpcName.Equals(contenders[1])).GetAct(animalContestInfo, history));
            eventAction.Append("/faceDirection Lewis 1/move Lewis 3 0 1/faceDirection Lewis 0");
            eventAction.Append(PossibleThirdContenders.First(c=>c.NpcName.Equals(contenders[2])).GetAct(animalContestInfo, history));
            eventAction.Append($"/faceDirection Lewis 3/faceDirection Lewis 2/speak Lewis \"{i18n.Get("AnimalContest.Dialog.Lewis.Closure")}\"");
            eventAction.Append("/faceDirection Lewis 3/move Lewis -6 0 3/faceDirection Lewis 0/move Lewis 0 -4 0/faceDirection Lewis 3/faceDirection Lewis 2");

            eventAction.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.Lewis.ClosureThanks")}#$b#");
            if (history.Count == 0)
            {
                eventAction.Append($"{i18n.Get("AnimalContest.Dialog.Lewis.ClosureSuccessFirstTime")}\"");
            }
            else
            {
                eventAction.Append($"{i18n.Get("AnimalContest.Dialog.Lewis.ClosureSuccessOtherTimes")}\"");
            }
            String winnerAnimalName = animalContestInfo.Winner == "Farmer" ? farmAnimal.displayName : animalContestInfo.Winner == "Emily" ? "the unnamed parrot" : "%name";
            String winnerName = animalContestInfo.Winner=="Farmer"?"@":animalContestInfo.Winner;
            eventAction.Append($"/pause 200/speak Lewis \"{ i18n.Get("AnimalContest.Dialog.Lewis.WinnerAnnouncement", new {winnerName, winnerAnimalName})}\"");
            eventAction.Append("/playMusic event1/emote Alex 56 true/pause 60");
            if (animalContestInfo.Winner == "Farmer")
            {
                eventAction.Append("/emote farmer 32 true/pause 500/emote participant 20 true/pause 1000");
                if (Game1.player.getFriendshipHeartLevelForNPC("Pierre")>=4)
                {
                    eventAction.Append($"/textAboveHead Pierre \"{i18n.Get("AnimalContest.Dialog.Pierre.PlayerCongrats", new {playerName = Game1.player.Name})}\"/pause 1000");
                }
                if (Game1.player.getFriendshipHeartLevelForNPC("Gus") >= 4)
                {
                    eventAction.Append($"/textAboveHead Gus \"{i18n.Get("AnimalContest.Dialog.Gus.", new { playerName = Game1.player.Name })}\"/pause 1000");
                }
            }
            else if (animalContestInfo.Winner == "Marnie")
            {
                eventAction.Append("/specificTemporarySprite animalContestMarnieWinning/warp Marnie -2000 -2000/emote marnieAnimal 20 true/pause 500");
                eventAction.Append($"/emote Jas 32 true/pause 1000/textAboveHead Shane \"{i18n.Get("AnimalContest.Dialog.Shane.MarnieCongrats")}\"/pause 1000");
            }
            else if (animalContestInfo.Winner == "Shane")
            {
                eventAction.Append("/emote Shane 16 true/emote shaneAnimal 20 true");
                eventAction.Append($"/pause 500/emote Jas 32 true/pause 500/textAboveHead Marnie \"{i18n.Get("AnimalContest.Dialog.Marnie.ShaneContrats")}\"/pause 1000");
            }
            else if (animalContestInfo.Winner == "Emily")
            {
                eventAction.Append("/specificTemporarySprite animalContestEmilyParrotAction/emote Emily 20 true");
                eventAction.Append($"/textAboveHead Clint \"{i18n.Get("AnimalContest.Dialog.Clint.EmilyContrats")}\"/pause 500");
                if (isHaleyPresent)
                {
                    eventAction.Append($"/textAboveHead Haley \"{i18n.Get("AnimalContest.Dialog.Haley.EmilyContrats")}\"/pause 500");
                }
                eventAction.Append($"/pause 1000/textAboveHead Gus \"{i18n.Get("AnimalContest.Dialog.Gus.EmilyContrats")}\"/pause 1000");
            }
            else if (animalContestInfo.Winner == "Jodi")
            {
                eventAction.Append("/emote jodiAnimal 20 true/emote Jodi 32 true");
                eventAction.Append($"/pause 1000/textAboveHead Sam \"{i18n.Get("AnimalContest.Dialog.Sam.JodiContrats")}\"/pause 1500");
                eventAction.Append($"/textAboveHead Caroline \"{i18n.Get("AnimalContest.Dialog.Caroline.JodiContrats")}\"/pause 1500");
                if (isKentPresent)
                {
                    eventAction.Append($"/textAboveHead Kent \"{i18n.Get("AnimalContest.Dialog.Kent.JodiContrats")}\"/pause 1000");
                }
            }
            eventAction.Append($"/textAboveHead Evelyn \"{i18n.Get("AnimalContest.Dialog.Evelyn.Contrats")}\"");

            eventAction.Append($"/pause 3000/speak Lewis \"{i18n.Get("AnimalContest.Dialog.Lewis.Ending")}\"/faceDirection Lewis 3/move Lewis -2 0 3 true");
            if (animalContestInfo.Winner != "Marnie")
            {
                eventAction.Append($"/faceDirection Marnie 0/move Marnie 0 -2 0 true");
            }
            eventAction.Append($"/pause 1500/showFrame Lewis 16/globalFade/viewport -1000 -1000");
            if (animalContestInfo.Winner == "Farmer")
            {
                string bonusType = contestDate.Season == "spring" || contestDate.Season == "summer" ? "fertility" : "production";
                eventAction.Append($"/playSound reward/message \"{i18n.Get("AnimalContest.Message.Reward", new { animalName = farmAnimal.displayName, bonusType })}\"");
            }
            eventAction.Append("/specificTemporarySprite animalContestEnding/end");

            string script = initialPosition.ToString() + eventAction.ToString();
            
            FarmerLoader.FarmerData.AnimalContestData.Add(animalContestInfo);

            return new CustomEvent(key,script);
        }

        private static AnimalContestItem PickTheWinner(AnimalContestItem animalContestInfo, List<AnimalContestItem> history, AnimalStatus participantAnimalStatus, FarmAnimal farmAnimal, string thirdContender)
        {
            if (farmAnimal != null)
            {
                int friendshipPoints = farmAnimal.friendshipTowardFarmer.Value / 195;
                int monthsOld = (farmAnimal.age.Value + 1) / 28 + 1;
                int agePoints = monthsOld < 4 ? monthsOld : Math.Max(0, 5 - (monthsOld + 1) / 2);
                int treatVariatyPoints = Math.Min(3, participantAnimalStatus.FeedTreatsQuantity.Keys.Count);
                int weeksOld = (farmAnimal.age.Value + 1) / 7 + 1;
                int treatAvaregePoints = Math.Min(3, (participantAnimalStatus.FeedTreatsQuantity.Values.Sum() * 3) / weeksOld);
                int parentWinnerPoints = history.Exists(h => h.Winner == "Farmer" && h.ParticipantId == farmAnimal.parentId.Value) ? 1 : 0;

                animalContestInfo.FarmAnimalScore = new AnimalContestScore(friendshipPoints, monthsOld, agePoints, treatAvaregePoints, treatAvaregePoints, parentWinnerPoints);
                int totalPoints = animalContestInfo.FarmAnimalScore.TotalPoints;
                if (
                    (totalPoints >= MinPointsToGaranteeWin)
                    || (totalPoints >= MinPointsToPossibleWin
                        && agePoints > 0 && treatVariatyPoints > 0 && treatAvaregePoints > 0 && friendshipPoints > 0
                        && history.Count(h => h.Winner != "Farmer") >= history.Count(h => h.Winner == "Farmer"))
                )
                {
                    animalContestInfo.Winner = "Farmer";
                    return animalContestInfo;
                }
            }
            if (history.Exists(h => h.Contenders.Contains(thirdContender)) && history.Count(h => h.Winner == "Marnie") > history.Count(h => h.Winner == thirdContender))
            {
                animalContestInfo.Winner = thirdContender;
            }
            else
            {
                animalContestInfo.Winner = "Marnie";
            }
            return animalContestInfo;
        }

        private static string GetContendersAnimalPosition(string[] contenders, string marnieAnimal, bool isPlayerJustWatching, bool linusAlternateAnimal)
        {
            StringBuilder sb = new StringBuilder();
            bool isMarnieAnimalSmall = IsAnimalSmall(marnieAnimal);
            sb.Append($"/addTemporaryActor {marnieAnimal} {(isMarnieAnimalSmall ? SmallSize : BigSize)} {(isMarnieAnimalSmall ? 23 : 22)} 66 0 false Animal marnieAnimal/showFrame marnieAnimal 0");
            if (contenders.Contains("Alex"))
            {
                sb.Append("/specificTemporarySprite animalContestJoshDog");
            }
            if (contenders.Contains("Jodi"))
            {
                sb.Append("/addTemporaryActor White_Chicken 16 16 34 66 0 false Animal jodiAnimal/showFrame jodiAnimal 0");
            }
            if (contenders.Contains("Jas") || isPlayerJustWatching)
            {
                string jasAnimal = JasAct.GetJasAnimal(marnieAnimal);
                bool isJasAnimalSmall = IsAnimalSmall(jasAnimal);
                if (isPlayerJustWatching)
                {
                    sb.Append($"/addTemporaryActor Baby{(jasAnimal.Equals("Duck") ? "White_Chicken" : jasAnimal)} {(isJasAnimalSmall ? SmallSize : BigSize)} {(isJasAnimalSmall ? 26 : 25)} 66 0 false Animal jasAnimal/showFrame jasAnimal 0");
                }
                else
                {
                    sb.Append($"/addTemporaryActor Baby{(jasAnimal.Equals("Duck") ? "White_Chicken" : jasAnimal)} {(isJasAnimalSmall ? SmallSize : BigSize)} 31 66 0 false Animal jasAnimal/showFrame jasAnimal 0");
                }
            }
            if (contenders.Contains("Linus"))
            {
                if (!linusAlternateAnimal)
                {
                    sb.Append("/specificTemporarySprite animalContestRabbitShow 34 66 true");
                }
                else
                {
                    sb.Append("/specificTemporarySprite animalContestWildBird");
                }
            }
            if (contenders.Contains("Shane"))
            {
                sb.Append("/addTemporaryActor Blue_Chicken 16 16 34 66 0 false Animal shaneAnimal/showFrame shaneAnimal 0");
            }
            if (contenders.Contains("Emily"))
            {
                sb.Append("/specificTemporarySprite animalContestEmilyParrot");
            }
            if (contenders.Contains("Maru"))
            {
                sb.Append("/specificTemporarySprite animalContestMaruRobot");
            }
            if (contenders.Contains("Abigail"))
            {
                sb.Append("/specificTemporarySprite animalContestAbigailSlime");
            }
            if (contenders.Contains("Willy"))
            {
                sb.Append("/specificTemporarySprite animalContestWillyCrab");
            }
            return sb.ToString();
        }

        private static bool IsAnimalSmall(string animal)
        {
            return Array.IndexOf(PossibleAnimals, animal) > 4;
        }
        
        private static string GetPlayerEvalutionAct(AnimalContestItem animalContestInfo, FarmAnimal farmAnimal)
        {
            StringBuilder playerAct = new StringBuilder();
            string animalName = farmAnimal.displayName;

            if (animalContestInfo.FarmAnimalScore is AnimalContestScore score)
            {
                playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Begin", new { animalName})}\"");

                if (score.AgePoints == 0)
                {
                    playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.DisqualifyAge", new { animalName})}\"");
                }
                else if (score.TreatVariatyPoints == 0)
                {
                    playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.DisqualifyTreatVariaty", new { animalName})}\"");
                }
                else if (score.TreatAvaregePoints == 0)
                {
                    playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.DisqualifyTreatAvarege", new { animalName})}\"");
                }
                else if (score.FriendshipPoints == 0)
                {
                    playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.DisqualifyFriendship")}\"");
                }
                else
                {
                    if (score.ParentWinnerPoints > 0)
                    {
                        playerAct.Append($"/pause 500/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.ParentWinner", new { animalName })}\"");
                    }
                    playerAct.Append($"/pause 800");
                    if (score.MonthsOld <= 1)
                    {
                        playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Age1Young")}\"");
                    }
                    else if (score.MonthsOld <= 2)
                    {
                        playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Age2Young")}\"");
                    }
                    else if (score.MonthsOld <= 4)
                    {
                        playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Age3")}\"");
                    }
                    else if (score.MonthsOld <= 6)
                    {
                        playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Age2Old")}\"");
                    }
                    else if (score.MonthsOld <= 8)
                    {
                        playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Age1Old")}\"");
                    }
                    playerAct.Append($"/pause 800");
                    switch (score.FriendshipPoints)
                    {
                        case 1:
                        case 2:
                            playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Friendship1&2", new { animalName })}\"");
                            break;
                        case 3:
                            playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Friendship3", new { animalName })}\"");
                            break;
                        case 4:
                            playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Friendship4")}\"");
                            break;
                        case 5:
                        default:
                            playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Friendship5")}\"");
                            break;
                    }
                    playerAct.Append($"/pause 800");
                    switch (score.TreatVariatyPoints)
                    {
                        case 1:
                            string shortDisplayType = farmAnimal.shortDisplayType().ToLower();
                            playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.TreatVariaty1", new { shortDisplayType })}");
                            break;
                        case 2:
                            playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.TreatVariaty2")}");
                            break;
                        case 3:
                        default:
                            playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.TreatVariaty3")}");
                            break;
                    }
                    playerAct.Append("#$b#");
                    string conjunction;
                    switch (score.TreatAvaregePoints)
                    {
                        case 1:
                            conjunction = score.TreatVariatyPoints < 3
                                ? i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.TreatAvarege1.Conjunction1")
                                : i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.TreatAvarege1.Conjunction2");
                            playerAct.Append($"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.TreatAvarege1", new { conjunction })}\"");
                            break;
                        case 2:
                            playerAct.Append($"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.TreatAvarege2", new { animalName })}\"");
                            break;
                        case 3:
                        default:
                            conjunction = score.TreatVariatyPoints < 3
                                ? i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.TreatAvarege3.Conjunction1")
                                : i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.TreatAvarege3.Conjunction2");
                            playerAct.Append($"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.TreatAvarege3", new { conjunction })}\"");
                            break;
                    }
                    playerAct.Append($"/pause 200");
                    if (score.TotalPoints >= MinPointsToPossibleWin)
                    {
                        playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.ChanceWinning")}\"");
                        playerAct.Append("/emote farmer 32");
                    }
                    else
                    {
                        playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.BetterNextTime")}\"");
                        playerAct.Append("/emote farmer 28");
                    }
                }
            }
            else
            {
                playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Pet")}\"");
                string petName = Game1.player.getPetDisplayName();
                string petSound = Game1.player.catPerson
                    ? i18n.Get("AnimalContest.Dialog.PlayerAct.PetSound.Cat")
                    : i18n.Get("AnimalContest.Dialog.PlayerAct.PetSound.Dog");
                playerAct.Append($"/question null \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Player.PetAwnsers", new { petName, petSound })}\"");
                playerAct.Append($"/splitSpeak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.PetAwnsers")}\"");
                playerAct.Append("/emote farmer 28");
            }
            playerAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.PlayerAct.Lewis.Ending")}\"");
            return playerAct.ToString();
        }

        private static string GetSecondContender(List<AnimalContestItem> history, bool removeJasFromPool)
        {
            List<string> contendersPool = PossibleSecondContenders.Select(c => c.NpcName).ToList();
            if (removeJasFromPool)
            {
                contendersPool.Remove("Jas");
            }
            if (!Game1.player.eventsSeen.Contains(2481135))
            {
                contendersPool.Remove("Alex");
            }
            if (!Game1.player.eventsSeen.Contains(4) || (history.Exists(i=> i.Contenders.Contains("Abigail")) && !Game1.player.mailReceived.Contains("slimeHutchBuilt")))
            {
                contendersPool.Remove("Abigail");
            }
            if (!Game1.player.eventsSeen.Contains(711130))
            {
                contendersPool.Remove("Willy");
            }
            if (history.Count > 0)
            {
                contendersPool.RemoveAll(history.Last().Contenders.Contains);
                contendersPool.Sort((c1, c2) => history.Sum(d => d.Contenders.Count(c3 => c1 == c3)) - history.Sum(d => d.Contenders.Count(c3 => c2 == c3)));
            }
            return contendersPool.DefaultIfEmpty(removeJasFromPool? "Maru" : "Jas").FirstOrDefault();
        }

        private static string GetThirdContender(List<AnimalContestItem> history)
        {
            List<string> contendersPool = PossibleThirdContenders.Select(c => c.NpcName).ToList();
            if (!Game1.player.eventsSeen.Contains(3900074))
            {
                contendersPool.Remove("Shane");
            }
            if (!Game1.player.eventsSeen.Contains(463391))
            {
                contendersPool.Remove("Emily");
            }
            if (history.Count > 0)
            {
                contendersPool.RemoveAll(history.Last().Contenders.Contains);
                contendersPool.Sort((c1, c2) => history.Sum(d => d.Contenders.Count(c3 => c1 == c3)) - history.Sum(d => d.Contenders.Count(c3 => c2 == c3)));
            }
            return contendersPool.DefaultIfEmpty("Jodi").FirstOrDefault();
        }

        private static string ChooseMarnieAnimal(Random random, List<AnimalContestItem> history)
        {
            List<string> animalsPool = new List<string>(PossibleAnimals);
            if (history.Count > 0)
            {
                List<Tuple<string, int>> animalCount = animalsPool.Select((a) => new Tuple<String,int>(a, history.Count(m => m.MarnieAnimal == a))).ToList();
                int minCount = animalCount.Min(t2 => t2.Item2);
                animalsPool = animalCount.Where(t1 => t1.Item2 == minCount).Select(t=> t.Item1).ToList();
            }
            return animalsPool[random.Next(animalsPool.Count-1)];
        }

        private static VincentAnimal ChooseVincentAnimal(Random random, List<AnimalContestItem> history)
        {
            List<VincentAnimal> animalsPool = Enum.GetValues(typeof(VincentAnimal)).Cast<VincentAnimal>().ToList();
            if (history.Count < 2)
            {
                animalsPool.Remove(VincentAnimal.Rabbit);
            }
            if (history.Count > 0)
            {
                List<Tuple<VincentAnimal, int>> animalCount = animalsPool.Select((a) => new Tuple<VincentAnimal, int>(a, history.Count(m => m.VincentAnimal == a.ToString()))).ToList();
                int minCount = animalCount.Min(t2 => t2.Item2);
                animalsPool = animalCount.Where(t1 => t1.Item2 == minCount).Select(t => t.Item1).ToList();
            }
            return animalsPool[random.Next(animalsPool.Count - 1)];
        }

        private static string GenerateKey(int id, SDate date)
        {
            string key = "{0}/z {1}/u {2}/t {3}";

            string seasons = String.Join(" ",Seasons.Where(s => s != date.Season).ToArray());
            string day = date.Day.ToString();
            string time = "600 1000";
            return String.Format(key, id, seasons, day, time);
        }

        private static int GetEventId(SDate date)
        {
            return Convert.ToInt32($"6572{date.Year:00}{Utility.getSeasonNumber(date.Season)}{date.Day:00}");
        }
    }

    enum VincentAnimal
    {
        Frog
        ,Squirrel
        ,Bird
        ,Rabbit
    }
}
