using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace AnimalHusbandryMod.animals
{
    public class AnimalContestEventBuilder
    {
        public static readonly IList<string> Seasons = new ReadOnlyCollection<string>(new List<string>() {"spring", "summer", "fall", "winter"});

        public static readonly string[] PossibleThirdContenders = new[] {"Jodi", "Linus", "Shane", "Emily"};
        public static readonly string[] PossibleSecondContenders = new[] { "Jas", "Alex", "Abigail", "Willy", "Maru" };
        public static readonly string[] PossibleAnimals = { "White_Cow", "Brown_Cow", "Goat", "Sheep", "Pig", "Brown_Chicken", "White_Chicken", "Duck", "Rabbit" };

        private const string SmallSize = "16 16";
        private const string BigSize = "32 32";

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

            AnimalContestItem animalContestInfo = new AnimalContestItem(eventId, contestDate, contenders.ToList(), vincentAnimal.ToString(), marnieAnimal);
            animalContestInfo.ParticipantId = isParticipantPet? AnimalData.PetId : farmAnimal?.myID.Value;
            animalContestInfo = PickTheWinner(animalContestInfo, history, participantAnimalStatus, farmAnimal, contenders[2]);

            // Deciding who will be present
            bool isHaleyPresent = Game1.player.eventsSeen.Contains(14);
            bool isKentPresent = Game1.year > 1;
            bool isSebastianPresent = vincentAnimal == VincentAnimal.Frog || contenders.Contains("Abigail");
            bool isDemetriusPresent = contenders.Contains("Maru");

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
            if (isSebastianPresent)
            {
                initialPosition.Append($" Sebastian 37 67 3");
            }
            if (isDemetriusPresent)
            {
                initialPosition.Append($" Demetrius 32 70 0");
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
            initialPosition.Append(GetContendersAnimalPosition(contenders, marnieAnimal, isPlayerJustWatching));

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
            eventAction.Append("/speak Lewis \"@! You're here!#$b#Okay... Now that everybody is here, let's get started!\"");
            eventAction.Append("/pause 300/speak Lewis \"Umm... Attention everyone!\"");
            eventAction.Append($"/faceDirection {contenders[0]} 0 true");
            eventAction.Append($"/faceDirection {contenders[1]} 0");
            eventAction.Append($"/faceDirection {contenders[2]} 0 true");
            eventAction.Append($"/faceDirection Pierre 0 true");
            if (!contenders.Contains("Willy"))
            {
                initialPosition.Append($"/faceDirection Willy 3");
            }
            eventAction.Append($"/faceDirection Caroline 0");

            eventAction.Append(GetVincentAct(vincentAnimal));

            eventAction.Append($"/speak Lewis \"And the winner is...#$b#{(animalContestInfo.Winner=="Farmer"?"@":animalContestInfo.Winner)}!\"");

            eventAction.Append($"/pause 4000/globalFade/viewport -1000 -1000");
            if (animalContestInfo.Winner == "Farmer")
            {
                eventAction.Append($"/playSound reward/message \"{farmAnimal.displayName} got a {(contestDate.Season=="spring"? "fertility": "production")} bonus.\"");
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

                int totalPoints = friendshipPoints + agePoints + treatVariatyPoints + treatAvaregePoints + parentWinnerPoints;
                if (
                    (totalPoints >= 14)
                    || (totalPoints >= 11
                        && agePoints > 0 && treatVariatyPoints > 0 && treatAvaregePoints > 0
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

        private static string GetContendersAnimalPosition(string[] contenders, string marnieAnimal, bool isPlayerJustWatching)
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
                string jasAnimal = ChooseJasAnimal(marnieAnimal);
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
                sb.Append("/specificTemporarySprite animalCompetitionRabbitShow 34 66 true");
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

        private static string GetVincentAct(VincentAnimal vincentAnimal)
        {
            StringBuilder vicentAct = new StringBuilder();
            vicentAct.Append($"/pause 1000/speak Vincent \"WAIT!\"/speed Vincent 5/move Vincent 0 -16 0/speak Vincent \"You said I could bring an animal.\"");
            switch (vincentAnimal)
            {
                case VincentAnimal.Frog:
                    vicentAct.Append($"/faceDirection Vincent 1/pause 200/specificTemporarySprite animalCompetitionFrogShow");
                    vicentAct.Append($"/pause 500/specificTemporarySprite animalCompetitionFrogCroak/playSound croak");
                    vicentAct.Append($"/pause 1000/specificTemporarySprite animalCompetitionFrogRun");
                    vicentAct.Append($"/pause 500/textAboveHead Vincent \"WAIT!\"/speed Vincent 5/move Vincent 22 0 1 true/pause 2000");
                break;
                case VincentAnimal.Squirrel:
                    vicentAct.Append($"/faceDirection Vincent 1/pause 200/specificTemporarySprite animalCompetitionSquirrelShow");
                    vicentAct.Append($"/pause 5000/specificTemporarySprite animalCompetitionSquirrelRun");
                    vicentAct.Append($"/pause 500/textAboveHead Vincent \"WAIT!\"/speed Vincent 5/move Vincent 22 0 1 true/pause 2000");
                    break;
                case VincentAnimal.Bird:
                    vicentAct.Append($"/faceDirection Vincent 1/pause 200/specificTemporarySprite animalCompetitionBirdShow");
                    vicentAct.Append($"/pause 5000/specificTemporarySprite animalCompetitionBirdFly2");
                    vicentAct.Append($"/pause 500/textAboveHead Vincent \"WAIT!\"/speed Vincent 5/move Vincent 22 0 1 true/pause 2000");
                    break;
                case VincentAnimal.Rabbit:
                    vicentAct.Append($"/faceDirection Vincent 1/pause 200/specificTemporarySprite animalCompetitionRabbitShow 29 64 false true");
                    vicentAct.Append($"/pause 5000/specificTemporarySprite animalCompetitionRabbitRun");
                    vicentAct.Append($"/pause 500/textAboveHead Vincent \"WAIT!\"/speed Vincent 5/move Vincent 22 0 1 true/pause 2000");
                    break;
            }
            
            return vicentAct.ToString();
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
            if (!Game1.player.eventsSeen.Contains(4))
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

        private static string ChooseJasAnimal(string marnieAnimal)
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
