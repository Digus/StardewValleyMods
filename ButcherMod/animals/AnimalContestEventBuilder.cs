using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace AnimalHusbandryMod.animals
{
    public class AnimalContestEventBuilder
    {
        public static readonly IList<string> Seasons = new ReadOnlyCollection<string>(new List<string>() {"spring", "summer", "fall", "winter"});

        public static readonly string[] PossibleThirdContenders = new[] { "Shane", "Jodi", "Emily"};
        public static readonly string[] PossibleSecondContenders = new[] { "Jas", "Alex", "Abigail", "Willy", "Maru" };
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
            //string key = "2677835/z spring summer winter/u 17/t 600 900

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
            initialPosition.Append("/specificTemporarySprite animalCompetition/broadcastEvent/skippable");
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

            eventAction.Append(GetVincentAct(vincentAnimal, history.Count % 5 == 4, !history.Exists(h => h.VicentAnimal == vincentAnimal.ToString())));

            eventAction.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.Lewis.ContestExplanation")}\"");
            eventAction.Append("/move Lewis 0 4 2/move Lewis -5 0 3/faceDirection Lewis 0");
            eventAction.Append(GetMarieAct(marnieAnimal, history.Count == 0));
            eventAction.Append("/faceDirection Lewis 1/move Lewis 3 0 1/faceDirection Lewis 0");
            if (!isPlayerJustWatching)
            {
                eventAction.Append(GetPlayerEvalutionAct(animalContestInfo, farmAnimal));
            }
            else
            {
                eventAction.Append(GetJasAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains("Jas"))));
            }
            eventAction.Append("/faceDirection Lewis 1/move Lewis 5 0 1/faceDirection Lewis 0");
            switch (contenders[1])
            {
                case "Jas":
                    eventAction.Append(GetJasAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains("Jas"))));
                    break;
                case "Alex":
                    eventAction.Append(GetAlexAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains("Alex"))));
                    break;
                case "Willy":
                    eventAction.Append(GetWillyAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains("Willy"))));
                    break;
                case "Abigail":
                    eventAction.Append(GetAbigailAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains("Abigail"))));
                    break;
                case "Maru":
                    eventAction.Append(GetMaruAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains("Maru"))));
                    break;
            }

            eventAction.Append("/faceDirection Lewis 1/move Lewis 3 0 1/faceDirection Lewis 0");
            switch (contenders[2])
            {
                case "Jodi":
                    eventAction.Append(GetJodiAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains("Jodi"))));
                    break;
                case "Shane":
                    eventAction.Append(GetShaneAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains("Shane"))));
                    break;
                case "Emily":
                    eventAction.Append(GetEmilyAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains("Emily"))));
                    break;
                case "Linus":
                    eventAction.Append(GetLinusAct(animalContestInfo, linusAlternateAnimal));
                    break;
            }
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
                eventAction.Append("/specificTemporarySprite animalCompetitionMarnieWinning/warp Marnie -2000 -2000/emote marnieAnimal 20 true/pause 500");
                eventAction.Append($"/emote Jas 32 true/pause 1000/textAboveHead Shane \"{i18n.Get("AnimalContest.Dialog.Shane.MarnieCongrats")}\"/pause 1000");
            }
            else if (animalContestInfo.Winner == "Shane")
            {
                eventAction.Append("/emote Shane 16 true/emote shaneAnimal 20 true");
                eventAction.Append($"/pause 500/emote Jas 32 true/pause 500/textAboveHead Marnie \"{i18n.Get("AnimalContest.Dialog.Marnie.ShaneContrats")}\"/pause 1000");
            }
            else if (animalContestInfo.Winner == "Emily")
            {
                eventAction.Append("/specificTemporarySprite animalCompetitionEmilyParrotAction/emote Emily 20 true");
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
            eventAction.Append("/specificTemporarySprite animalCompetitionEnding/end");

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
                sb.Append("/specificTemporarySprite animalCompetitionJoshDog");
            }
            if (contenders.Contains("Jodi"))
            {
                sb.Append("/addTemporaryActor White_Chicken 16 16 34 66 0 false Animal jodiAnimal/showFrame jodiAnimal 0");
            }
            if (contenders.Contains("Jas") || isPlayerJustWatching)
            {
                string jasAnimal = GetJasAnimal(marnieAnimal);
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
                    sb.Append("/specificTemporarySprite animalCompetitionRabbitShow 34 66 true");
                }
                else
                {
                    sb.Append("/specificTemporarySprite animalCompetitionWildBird");
                }
            }
            if (contenders.Contains("Shane"))
            {
                sb.Append("/addTemporaryActor Blue_Chicken 16 16 34 66 0 false Animal shaneAnimal/showFrame shaneAnimal 0");
            }
            if (contenders.Contains("Emily"))
            {
                sb.Append("/specificTemporarySprite animalCompetitionEmilyParrot");
            }
            if (contenders.Contains("Maru"))
            {
                sb.Append("/specificTemporarySprite animalCompetitionMaruRobot");
            }
            if (contenders.Contains("Abigail"))
            {
                sb.Append("/specificTemporarySprite animalCompetitionAbigailSlime");
            }
            if (contenders.Contains("Willy"))
            {
                sb.Append("/specificTemporarySprite animalCompetitionWillyCrab");
            }
            return sb.ToString();
        }

        private static bool IsAnimalSmall(string animal)
        {
            return Array.IndexOf(PossibleAnimals, animal) > 4;
        }

        private static string GetVincentAct(VincentAnimal vincentAnimal, bool isLate, bool isAnimalFirstTime)
        {

            StringBuilder vicentAct = new StringBuilder();
            vicentAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Lewis.Begin1")}\"");
            if (isLate)
            {
                vicentAct.Append($"/pause 2000/emote Lewis 40/pause 1000/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Lewis.Alternate1")}\"/emote Jodi 28 true/pause 1000");
            }
            vicentAct.Append($"/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Vincent.Wait")}\"");
            if (isLate)
            {
                vicentAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Lewis.Alternate2")}\"/emote Jodi 16 true");
            }
            vicentAct.Append($"/speed Vincent 5/move Vincent 0 -16 0/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Vincent.Begin1")}\"");
            vicentAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Lewis.Begin2")}\"");
            vicentAct.Append($"/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Vincent.Begin2")}\"");
            if (isAnimalFirstTime)
            {
                vicentAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Lewis.FirstTime")}\"");
                vicentAct.Append($"/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Vincent.FirstTime")}\"/faceDirection Vincent 1/pause 400");
            }
            else
            {
                vicentAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Lewis.OtherTimes")}\"");
                vicentAct.Append($"/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Vincent.OtherTimes")}\"/faceDirection Vincent 1/pause 400");
            }

            switch (vincentAnimal)
            {
                case VincentAnimal.Frog:
                    vicentAct.Append($"/specificTemporarySprite animalCompetitionFrogShow");
                    if (isAnimalFirstTime)
                    {
                        vicentAct.Append($"/emote Lewis 8/jump Lewis/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Frog.Lewis.FirstTime1")}\"");
                        vicentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Frog.Vincent.FirstTime1")}\"");
                        vicentAct.Append($"/specificTemporarySprite animalCompetitionFrogCroak/playSound croak");
                        vicentAct.Append($"/animate Lewis true false 1000 24/emote Lewis 12/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Frog.Lewis.FirstTime2")}\"");
                        vicentAct.Append($"/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Frog.Vincent.FirstTime2")}\"");
                        vicentAct.Append($"/emote Lewis 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Frog.Lewis.FirstTime3")}\"");
                        vicentAct.Append($"/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Frog.Vincent.FirstTime3")}\"/faceDirection Vincent 1");
                    }
                    else
                    {
                        vicentAct.Append($"/emote Lewis 12/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Frog.Lewis.OtherTime1")}\"");
                        vicentAct.Append($"/speak Sebastian \"{i18n.Get("AnimalContest.Dialog.VincentAct.Frog.Sebastian.OtherTimes")}\"");
                        vicentAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Frog.Lewis.OtherTimes2")}\"");
                        vicentAct.Append($"/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Frog.Vincent.OtherTimes")}\"");
                    }
                    vicentAct.Append($"/pause 500/specificTemporarySprite animalCompetitionFrogCroak/playSound croak");
                    vicentAct.Append($"/pause 2000/specificTemporarySprite animalCompetitionFrogRun");
                break;
                case VincentAnimal.Squirrel:
                    vicentAct.Append($"/specificTemporarySprite animalCompetitionSquirrelShow");
                    if (isAnimalFirstTime)
                    {
                        vicentAct.Append($"/emote Lewis 8/jump Lewis/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Squirrel.Lewis.FirstTime")}\"");
                        vicentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Squirrel.Vincent.FirstTime")}\"");
                        vicentAct.Append($"/animate Sam false false 2000 33/textAboveHead Sam \"{i18n.Get("AnimalContest.Dialog.VincentAct.Squirrel.Sam.FirstTime")}\"/faceDirection Vincent 1/jump Vincent");
                    }
                    else
                    {
                        vicentAct.Append($"/emote Lewis 12/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Squirrel.Lewis.OtherTimes1")}\"");
                        vicentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Squirrel.Vincent.OtherTimes1")}\"");
                        vicentAct.Append($"/speak Jas \"{i18n.Get("AnimalContest.Dialog.VincentAct.Squirrel.Jas.OtherTimes")}\"");
                        vicentAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Squirrel.Lewis.OtherTimes2")}\"");
                        vicentAct.Append($"/speak Jodi \"{i18n.Get("AnimalContest.Dialog.VincentAct.Squirrel.Jodi.OtherTimes")}\"");
                        vicentAct.Append($"/faceDirection Vincent 1/textAboveHead Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Squirrel.Vincent.OtherTimes2")}\"");
                    }
                    vicentAct.Append($"/pause 2000/specificTemporarySprite animalCompetitionSquirrelRun");
                    break;
                case VincentAnimal.Bird:
                    vicentAct.Append($"/specificTemporarySprite animalCompetitionBirdShow");
                    if (isAnimalFirstTime)
                    {
                        vicentAct.Append($"/emote Lewis 8/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Bird.Lewis.FirstTime")}\"");
                        vicentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Bird.Vincent.FirstTime")}\"/faceDirection Vincent 1/pause 500");
                    }
                    else
                    {
                        vicentAct.Append($"/emote Lewis 12/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Bird.Lewis.OtherTimes")}\"");
                        vicentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Bird.Vincent.OtherTimes1")}\"/faceDirection Vincent 1/pause 4000");
                        vicentAct.Append($"/textAboveHead Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Bird.Vincent.OtherTimes2")}\"/pause 500");
                    }
                    vicentAct.Append("/specificTemporarySprite animalCompetitionBirdFly2");
                    break;
                case VincentAnimal.Rabbit:
                    vicentAct.Append("/specificTemporarySprite animalCompetitionRabbitShow 29 64 false true");
                    if (isAnimalFirstTime)
                    {
                        vicentAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Rabbit.Lewis.FirstTime1")}\"");
                        vicentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Rabbit.Vincent.FirstTime")}\"");
                        vicentAct.Append($"/jump Lewis/emote Lewis 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Rabbit.Lewis.FirstTime")}\"");
                        vicentAct.Append($"/speak Linus \"{i18n.Get("AnimalContest.Dialog.VincentAct.Rabbit.Linus.FirstTime2")}\"");
                        vicentAct.Append("/faceDirection Vincent 1");
                    }
                    else
                    {
                        vicentAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Rabbit.Lewis.OtherTimes1")}\"");
                        vicentAct.Append($"/faceDirection Vincent 0/animate Vincent false true 100 16 17/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Rabbit.Vincent.OtherTimes1")}\"");
                        vicentAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.VincentAct.Rabbit.Lewis.OtherTimes2")}\"");
                        vicentAct.Append($"/stopAnimation Vincent/speak Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Rabbit.Vincent.OtherTimes2")}\"/faceDirection Vincent 1/pause 500/playSound eat");
                    }
                    vicentAct.Append($"/pause 1000/specificTemporarySprite animalCompetitionRabbitRun");
                    break;
            }
            vicentAct.Append($"/pause 500/textAboveHead Vincent \"{i18n.Get("AnimalContest.Dialog.VincentAct.Vincent.Wait")}\"/speed Vincent 5/move Vincent 22 0 1 true/pause 2000");
            vicentAct.Append($"/emote Lewis 40");

            return vicentAct.ToString();
        }

        private static string GetMarieAct(string marnieAnimal, bool isFirstTime)
        {
            StringBuilder marnieAct = new StringBuilder();

            string otherTimes = isFirstTime? "" : i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.OtherTimes");
            marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Begin",new {otherTimes})}\"/pause 200");
            
            if (marnieAnimal.Contains("Cow"))
            {
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Cow")}\"");
                marnieAct.Append($"/speak Marnie \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Marnie.Astonished")}\"");
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.ContinueEvalution")}\"");
                marnieAct.Append($"/speak Caroline \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Caroline")}\"");
            }
            else if (marnieAnimal.Contains("Chicken"))
            {
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Chicken")}\"");
                marnieAct.Append($"/speak Marnie \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Marnie.Astonished")}\"");
                marnieAct.Append($"/speak Pierre \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Pierre.Chicken")}\"");
            }
            else if (marnieAnimal.Contains("Duck"))
            {
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Duck")}\"");
                marnieAct.Append($"/speak Marnie \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Marnie.Astonished")}\"");
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.ContinueEvalution")}\"");
                marnieAct.Append($"/speak Jodi \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Jodi.Duck")}\"");
            }
            else if (marnieAnimal.Contains("Pig"))
            {
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Pig")}\"");
                marnieAct.Append($"/speak Marnie \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Marnie.Astonished")}\"");
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Astonished")}\"");
                marnieAct.Append($"/speak George \"{i18n.Get("AnimalContest.Dialog.MarnieAct.George.Pig")}\"");
            }
            else if (marnieAnimal.Contains("Goat"))
            {
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Goat")}\"");
                marnieAct.Append($"/speak Marnie \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Marnie.Astonished")}\"");
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.ContinueEvalution")}\"");
                marnieAct.Append($"/speak Evelyn \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Evelyn.Goat")}\"");
            }
            else if (marnieAnimal.Contains("Rabbit"))
            {
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Rabbit")}\"");
                marnieAct.Append($"/speak Marnie \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Marnie.Astonished")}\"");
                marnieAct.Append("/emote Lewis 40");
                marnieAct.Append($"/speak Jas \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Jas.Rabbit")}\"");
            }
            else if (marnieAnimal.Contains("Sheep"))
            {
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Sheep")}\"");
                marnieAct.Append($"/speak Marnie \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Marnie.Astonished")}\"");
                if (Game1.player.mailReceived.Contains("secretNote21_done"))
                {
                    marnieAct.Append("/emote farmer 16");
                }
                else
                {
                    marnieAct.Append("/emote farmer 8");
                }
                marnieAct.Append($"/speak Jas \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Jas.Sheep")}\"");
            }
            else
            {
                marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Animal")}\"");
                marnieAct.Append($"/speak Marnie \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Marnie.Astonished")}\"");
                marnieAct.Append($"/pause 500/speak Caroline \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Caroline")}\"");
            }
            marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Evalution")}\"");
            string closurePrefix = isFirstTime
                ? i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.ClosurePrefix.FirstTime")
                : i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.ClosurePrefix.OtherTimes");
            marnieAct.Append($"/pause 1500/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Closure", new {closurePrefix})}\"");
            marnieAct.Append($"/speak Marnie \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Marnie.Thanks")}\"");
            marnieAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MarnieAct.Lewis.Ending")}\"");

            return marnieAct.ToString();
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

        private static string GetJasAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder jasAct = new StringBuilder();
            string babyAnimalName = AnimalExtension.GetBabyAnimalNameByType(GetJasAnimal(animalContestInfo.MarnieAnimal));
            jasAct.Append("/emote Lewis 40");
            if (isFirstTime)
            {
                jasAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JasAct.Lewis.FirstTime1")}\"");
                jasAct.Append($"/speak Jas \"{i18n.Get("AnimalContest.Dialog.JasAct.Jas.FirstTime1")}\"");
                jasAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JasAct.Lewis.FirstTime2")}\"");
                jasAct.Append($"/speak Jas \"{i18n.Get("AnimalContest.Dialog.JasAct.Jas.FirstTime2")}\"");
                jasAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JasAct.Lewis.FirstTime3")}\"");
                jasAct.Append($"/speak Marnie \"{i18n.Get("AnimalContest.Dialog.JasAct.Marnie.FirstTime")}\"");
                jasAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JasAct.Lewis.FirstTime4", new {babyAnimalName})}\"");
                jasAct.Append($"/pause 1500/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JasAct.Lewis.FirstTime5")}\"");
                jasAct.Append($"/emote Jas 20/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JasAct.Lewis.FirstTime6")}\"");
            }
            else
            {
                jasAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JasAct.Lewis.OtherTimes1")}\"");
                jasAct.Append($"/speak Jas \"{i18n.Get("AnimalContest.Dialog.JasAct.Jas.OtherTimes1")}\"");
                jasAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JasAct.Lewis.OtherTimes2", new { babyAnimalName })}\"");
                jasAct.Append($"/pause 1500/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JasAct.Lewis.OtherTimes3")}\"");
                jasAct.Append($"/emote Jas 20/speak Jas \"{i18n.Get("AnimalContest.Dialog.JasAct.Jas.OtherTimes2")}\"");
                jasAct.Append($"/jump Lewis/emote Lewis 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JasAct.Lewis.OtherTimes4")}\"");
            }
            return jasAct.ToString();
        }

        private static string GetAlexAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder alexAct = new StringBuilder();
            if (isFirstTime)
            {
                alexAct.Append($"/emote Lewis 8/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AlexAct.Lewis.FirstTime1")}\"");
                alexAct.Append($"/speak Alex \"{i18n.Get("AnimalContest.Dialog.AlexAct.Alex.FirstTime1")}\"");
                alexAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AlexAct.Lewis.FirstTime2")}\"");
                alexAct.Append($"/speak Alex \"{i18n.Get("AnimalContest.Dialog.AlexAct.Alex.FirstTime2")}\"/playSound dogWhining/pause 1000/specificTemporarySprite animalCompetitionJoshDogOut/pause 1000");
                alexAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AlexAct.Lewis.FirstTime3")}\"");
                alexAct.Append($"/speak Alex \"{i18n.Get("AnimalContest.Dialog.AlexAct.Alex.FirstTime3")}\"");
                alexAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AlexAct.Lewis.FirstTime4")}\"");
                alexAct.Append($"/speak Alex \"{i18n.Get("AnimalContest.Dialog.AlexAct.Alex.FirstTime4")}\"");
                alexAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AlexAct.Lewis.FirstTime5")}\"");
                alexAct.Append("/emote Alex 12");
            }
            else
            {
                alexAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AlexAct.Lewis.OtherTimes1")}\"");
                alexAct.Append($"/speak Alex \"{i18n.Get("AnimalContest.Dialog.AlexAct.Alex.OtherTimes1")}\"/pause 500/specificTemporarySprite animalCompetitionJoshDogOut/pause 1000");
                alexAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AlexAct.Lewis.OtherTimes2")}\"");
                alexAct.Append($"/speak Alex \"{i18n.Get("AnimalContest.Dialog.AlexAct.Alex.OtherTimes2")}\"/pause 300/showFrame Alex 26/pause 100/specificTemporarySprite animalCompetitionJoshDogSteak/playSound dwop");
                alexAct.Append($"/pause 1000/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AlexAct.Lewis.OtherTimes3")}\"");
                alexAct.Append($"/speak Alex \"{i18n.Get("AnimalContest.Dialog.AlexAct.Alex.OtherTimes3")}\"/playSound dogWhining");
                alexAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AlexAct.Lewis.OtherTimes4")}\"");
                alexAct.Append("/pause 300/specificTemporarySprite animalCompetitionJoshDogOut/pause 100/showFrame Alex 4/emote Alex 12");
            }
            return alexAct.ToString();
        }

        private static string GetWillyAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder willyAct = new StringBuilder();
            if (isFirstTime)
            {
                willyAct.Append($"/jump Lewis/emote Lewis 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.WillyAct.Lewis.")}\"");
                willyAct.Append($"/speak Willy \"{i18n.Get("AnimalContest.Dialog.WillyAct.Willy.")}\"");
                willyAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.WillyAct.Lewis.")}\"");
                willyAct.Append($"/pause 800/playSound eat/jump Lewis/textAboveHead Lewis \"{i18n.Get("AnimalContest.Dialog.WillyAct.Lewis.")}\"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.WillyAct.Lewis.")}\"");
                willyAct.Append($"/speak Willy \"{i18n.Get("AnimalContest.Dialog.WillyAct.Willy.")}\"");
                willyAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.WillyAct.Lewis.")}\"");
            }
            else
            {
                willyAct.Append($"/emote Lewis 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.WillyAct.Lewis.OtherTimes1")}\"");
                willyAct.Append($"/speak Willy \"{i18n.Get("AnimalContest.Dialog.WillyAct.Willy.OtherTimes1")}\"");
                willyAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.WillyAct.Lewis.OtherTimes2")}\"");
                willyAct.Append($"/pause 800/playSound scissors/jump Lewis/textAboveHead Lewis \"{i18n.Get("AnimalContest.Dialog.WillyAct.Lewis.OtherTimes3")}\"");
                willyAct.Append($"/pause 500/speak Lewis \"{i18n.Get("AnimalContest.Dialog.WillyAct.Lewis.OtherTimes4")}\"");
                willyAct.Append("/emote Willy 28");
                willyAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.WillyAct.Lewis.OtherTimes5")}\"");
            }
            return willyAct.ToString();
        }

        private static string GetAbigailAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder abigailAct = new StringBuilder();
            if (isFirstTime)
            {
                abigailAct.Append($"/jump Lewis/emote Lewis 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Lewis.FirstTime1")}\"");
                abigailAct.Append($"/speak Abigail \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Abigail.FirstTime1")}\"");
                abigailAct.Append($"/emote Lewis 12/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Lewis.FirstTime2")}\"");
                abigailAct.Append($"/speak Abigail \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Abigail.FirstTime2")}\"");
                abigailAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Lewis.FirstTime3")}\"");
                abigailAct.Append($"/speak Abigail \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Abigail.FirstTime3")}\"");
                abigailAct.Append($"/textAboveHead Pierre \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Pierre.FirstTime")}\"/emote Abigail 16");
                abigailAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Lewis.FirstTime4")}\"");
            }
            else
            {
                abigailAct.Append($"/emote Lewis 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Lewis.OtherTimes1")}\"");
                abigailAct.Append($"/speak Abigail \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Abigail.OtherTimes1")}\"");
                abigailAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Lewis.OtherTimes2")}\"");
                abigailAct.Append($"/speak Abigail \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Abigail.OtherTimes2")}\"");
                abigailAct.Append($"/emote farmer 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Lewis.OtherTimes3")}\"");
                abigailAct.Append($"/speak Abigail \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Abigail.OtherTimes3")}\"");
                abigailAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.AbigailAct.Lewis.OtherTimes4")}\"");
                abigailAct.Append("/emote Abigail 28");
            }
            return abigailAct.ToString();
        }

        private static string GetMaruAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder maruAct = new StringBuilder();
            if (isFirstTime)
            {
                maruAct.Append($"/emote Lewis 8/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MaruAct.Lewis.FirstTime1")}\"");
                maruAct.Append($"/speak Maru \"{i18n.Get("AnimalContest.Dialog.MaruAct.Maru.FirstTime1")}\"");
                maruAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MaruAct.Lewis.FirstTime2")}\"");
                maruAct.Append($"/speak Maru \"{i18n.Get("AnimalContest.Dialog.MaruAct.Maru.FirstTime2")}\"");
                maruAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MaruAct.Lewis.FirstTime3")}\"");
                maruAct.Append($"/speak Maru \"{i18n.Get("AnimalContest.Dialog.MaruAct.Maru.FirstTime3")}\"");
                maruAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MaruAct.Lewis.FirstTime4")}\"");
                maruAct.Append("/emote Maru 28");
            }
            else
            {
                maruAct.Append($"/emote Lewis 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MaruAct.Lewis.OtherTimes1")}\"");
                maruAct.Append($"/speak Maru \"{i18n.Get("AnimalContest.Dialog.MaruAct.Maru.OtherTimes1")}\"");
                maruAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MaruAct.Lewis.OtherTimes2")}\"");
                maruAct.Append($"/speak Maru \"{i18n.Get("AnimalContest.Dialog.MaruAct.Maru.OtherTimes2")}\"");
                maruAct.Append($"/pause 500/message \"{i18n.Get("AnimalContest.Dialog.MaruAct.Robot.OtherTimes1")}\"");
                maruAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MaruAct.Lewis.OtherTimes3")}\"");
                maruAct.Append($"/speak Maru \"{i18n.Get("AnimalContest.Dialog.MaruAct.Maru.OtherTimes3")}\"");
                maruAct.Append($"/pause 500/message \"{i18n.Get("AnimalContest.Dialog.MaruAct.Robot.OtherTimes2")}\"");
                maruAct.Append($"/emote Lewis 12/speak Maru \"{i18n.Get("AnimalContest.Dialog.MaruAct.Maru.OtherTimes4")}\"");
                maruAct.Append($"/pause 500/message \"{i18n.Get("AnimalContest.Dialog.MaruAct.Robot.OtherTimes3")}\"");
                maruAct.Append($"/emote Marnie 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.MaruAct.Lewis.OtherTimes4")}\"");
                maruAct.Append("/emote Maru 28");
            }
            return maruAct.ToString();
        }

        private static string GetJodiAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder jodiAct = new StringBuilder();
            if (isFirstTime)
            {
                jodiAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JodiAct.Lewis.FirstTime1")}\"");
                jodiAct.Append($"/speak Jodi \"{i18n.Get("AnimalContest.Dialog.JodiAct.Jodi.FirstTime1")}\"");
                jodiAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JodiAct.Lewis.FirstTime2")}\"");
                jodiAct.Append($"/speak Jodi \"{i18n.Get("AnimalContest.Dialog.JodiAct.Jodi.FirstTime2")}\"");
                jodiAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JodiAct.Lewis.FirstTime3")}\"");
                jodiAct.Append($"/pause 1000/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JodiAct.Lewis.FirstTime4")}\"");
            }
            else
            {
                jodiAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JodiAct.Lewis.OtherTimes1")}\"");
                jodiAct.Append($"/speak Jodi \"{i18n.Get("AnimalContest.Dialog.JodiAct.Jodi.OtherTimes1")}\"");
                if (Game1.player.isMarried() && "Sam".Equals(Game1.player.spouse))
                {
                    jodiAct.Append($"/speak Sam \"{i18n.Get("AnimalContest.Dialog.JodiAct.Sam.Married")}\"");
                    string prefix = "";
                    if (Game1.year > 1)
                    {
                        prefix = i18n.Get("AnimalContest.Dialog.JodiAct.Jodi.OtherTimes2.PrefixYear2");
                    }
                    else
                    {
                        prefix = "AnimalContest.Dialog.JodiAct.Jodi.OtherTimes2.PrefixYear1";
                    }
                    jodiAct.Append($"/speak Jodi \"{i18n.Get("AnimalContest.Dialog.JodiAct.Jodi.OtherTimes2", new { prefix })}\"");
                }
                else
                {
                    jodiAct.Append($"/showFrame Sam 33/textAboveHead Sam \"{i18n.Get("AnimalContest.Dialog.JodiAct.Sam.Single")}\"/pause 1000/showFrame Sam 12");
                }
                jodiAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JodiAct.Lewis.OtherTimes2")}\"");
                jodiAct.Append($"/pause 1500/speak Lewis \"{i18n.Get("AnimalContest.Dialog.JodiAct.Lewis.OtherTimes3")}\"");
                
            }
            jodiAct.Append($"/speak Jodi \"{i18n.Get("AnimalContest.Dialog.JodiAct.Jodi.Thanks")}\"");
            return jodiAct.ToString();
        }

        private static string GetShaneAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder shaneAct = new StringBuilder();
            if (isFirstTime)
            {
                shaneAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime1")}\"");
                shaneAct.Append($"/speak Shane \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Shane.FirstTime1")}\"");
                shaneAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime2")}\"");
                shaneAct.Append($"/speak Shane \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Shane.FirstTime2")}\"");
                shaneAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime3")}\"");
                shaneAct.Append($"/pause 1500/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime4")}\"");
                shaneAct.Append($"/speak Shane \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Shane.FirstTime3")}\"");
                shaneAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime5")}\"");
            }
            else
            {
                shaneAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.OtherTimes1")}\"");
                shaneAct.Append($"/speak Shane \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Shane.OtherTimes1")}\"");
                shaneAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.OtherTimes2")}\"");
                shaneAct.Append($"/speak Shane \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Shane.OtherTimes2")}\"");
                shaneAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.OtherTimes3")}\"");
                shaneAct.Append($"/pause 1500/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.OtherTimes4")}\"");
            }
            return shaneAct.ToString();
        }

        private static string GetEmilyAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder emilyAct = new StringBuilder();
            if (isFirstTime)
            {
                emilyAct.Append($"/jump Lewis/emote Lewis 16/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime1")}\"");
                emilyAct.Append($"/speak Emily \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Emily.FirstTime1")}\"");
                emilyAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime2")}\"");
                emilyAct.Append($"/speak Emily \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Emily.FirstTim2")}\"");
                emilyAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime3")}\"");
                emilyAct.Append($"/speak Emily \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Emily.FirstTime3")}\"");
                emilyAct.Append($"/emote Lewis 8/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime4")}\"");
                emilyAct.Append("/pause 500/specificTemporarySprite animalCompetitionEmilyParrotAction");
                emilyAct.Append($"/pause 1000/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime5")}\"");
                emilyAct.Append("/pause 1500/specificTemporarySprite animalCompetitionEmilyParrotAction");
                emilyAct.Append($"/pause 1500/speak Emily \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Emily.FirstTime4")}\"");
                emilyAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.FirstTime6")}\"");
            }
            else
            {
                emilyAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.OtherTimes1")}\"");
                emilyAct.Append($"/speak Emily \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Emily.OtherTimes1")}\"");
                emilyAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.OtherTimes2")}\"");
                emilyAct.Append("/specificTemporarySprite animalCompetitionEmilyParrotAction");
                emilyAct.Append($"/speak Emily \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Emily.OtherTimes2")}\"");
                emilyAct.Append("/faceDirection Emily 0/specificTemporarySprite animalCompetitionEmilyBoomBox/playMusic EmilyDance/pause 4460/faceDirection Emily 1 true");
                emilyAct.Append("/specificTemporarySprite animalCompetitionEmilyBoomBoxStart/specificTemporarySprite animalCompetitionEmilyParrotDance");
                emilyAct.Append("/pause 10000/faceDirection Emily 0 true/pause 500/specificTemporarySprite animalCompetitionEmilyParrotStopDance");
                emilyAct.Append("/stopMusic/specificTemporarySprite animalCompetitionEmilyBoomBoxStop/pause 500/faceDirection Emily 1/pause 500");
                emilyAct.Append($"/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.OtherTimes3")}\"");
                emilyAct.Append($"/speak Emily \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Emily.OtherTimes3")}\"");
                emilyAct.Append($"/pause 1000/speak Lewis \"{i18n.Get("AnimalContest.Dialog.ShaneAct.Lewis.OtherTimes4")}\"");
            }
            return emilyAct.ToString();
        }

        private static string GetLinusAct(AnimalContestItem animalContestInfo, bool alternate)
        {
            StringBuilder linusAct = new StringBuilder();
            if (!alternate)
            {
                linusAct.Append("/speak Lewis \"\"");
                linusAct.Append("/speak Linus \"\"");
            }
            else
            {
                linusAct.Append("/speak Lewis \"\"");
                linusAct.Append("/speak Linus \"\"");
            }
            return linusAct.ToString();
        }

        private static string GetSecondContender(List<AnimalContestItem> history, bool removeJasFromPool)
        {
            List<string> contendersPool = new List<string>(PossibleSecondContenders);
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
            List<string> contendersPool = new List<string>(PossibleThirdContenders);
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

        private static string GetJasAnimal(string marnieAnimal)
        {
            int i = Array.IndexOf(PossibleAnimals, marnieAnimal);
            return PossibleAnimals[((long) Game1.uniqueIDForThisGame + i) % PossibleAnimals.Length];
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
                List<Tuple<VincentAnimal, int>> animalCount = animalsPool.Select((a) => new Tuple<VincentAnimal, int>(a, history.Count(m => m.VicentAnimal == a.ToString()))).ToList();
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
