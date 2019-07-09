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
        public static readonly int MinPointsToPossibleWin = 11;
        public static readonly int MinPointsToGaranteeWin = 14;

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

            if (!contenders.Contains("Linus"))
            {
                initialPosition.Append($" Linus 37 70 3");
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

            if (!isPlayerJustWatching)
            {
                eventAction.Append("/speak Lewis \"@! You're here!$h#$b#Well folks, now that all competitors are here, let's get started.\"");
            }
            else
            {
                eventAction.Append("/speak Lewis \"@! You came!$h#$b#Well folks, I think we are ready, let's get started.\"");
            }

            //eventAction.Append("/pause 500/emote Lewis 40/speak Lewis \"Ahem... Attention everyone!$u!\"");
            //eventAction.Append($"/faceDirection {contenders[0]} 0 true");
            //eventAction.Append($"/faceDirection {contenders[1]} 0");
            //eventAction.Append($"/faceDirection {contenders[2]} 0 true");
            //eventAction.Append($"/faceDirection Pierre 0 true");
            //if (!contenders.Contains("Willy"))
            //{
            //    initialPosition.Append($"/faceDirection Willy 3");
            //}
            //eventAction.Append($"/faceDirection Caroline 0");

            //if (history.Count == 0)
            //{
            //    eventAction.Append("/speak Lewis \"We're gethered here today to bring back an old tradition, the Animal Contest!#$b#We stopped doing it because almost no one was interested in participating.$s#$b#But now that we have again a thriving farm in the valley I hope we can do the contest every spring and fall.$h\"");
            //}
            //else
            //{
            //    eventAction.Append("/speak Lewis \"We're gethered here again for our traditional Animal Contest!#$b#It makes me really happy to see all of you're still interested in participating.$h#$b#Let's hope we can do this a lot more times.\"");
            //}

            //eventAction.Append(GetVincentAct(vincentAnimal, history.Count%5 == 4, !history.Exists(h => h.VicentAnimal == vincentAnimal.ToString())));

            //eventAction.Append("/speak Lewis \"Shall we continue then.#$b#As I was saying before, I'll start by evaluting every participant for their own merrits.#$b#After that I will annonce who owns the greatest animal of Stardew Valley.#$b#So, let's get started.\"");
            eventAction.Append("/move Lewis 0 4 2/move Lewis -5 0 3/faceDirection Lewis 0");
            //eventAction.Append(GetMarieAct(marnieAnimal, history.Count == 0));
            eventAction.Append("/faceDirection Lewis 1/move Lewis 3 0 1/faceDirection Lewis 0");
            if (!isPlayerJustWatching)
            {
                //eventAction.Append(GetPlayerEvalutionAct(animalContestInfo, farmAnimal));
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
            }


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

        private static string GetVincentAct(VincentAnimal vincentAnimal, bool isLate, bool isAnimalFirstTime)
        {

            StringBuilder vicentAct = new StringBuilder();
            vicentAct.Append("/speak Lewis \"I'll start by evaluating every participant...\"");
            if (isLate)
            {
                vicentAct.Append("/pause 2000/emote Lewis 40/pause 1000/speak Lewis \"Now I'm worried, where is Vincent?$s\"/emote Jodi 28 true/pause 1000");
            }
            vicentAct.Append("/speak Vincent \"WAIT!\"");
            if (isLate)
            {
                vicentAct.Append("/speak Lewis \"There he is...\"/emote Jodi 16 true");
            }
            vicentAct.Append($"/speed Vincent 5/move Vincent 0 -16 0/speak Vincent \"I want to participate too.\"");
            vicentAct.Append($"/speak Lewis \"You're late Vincent and we already got four contenders.\"");
            vicentAct.Append($"/speak Vincent \"But you said I could.$s\"");
            if (isAnimalFirstTime)
            {
                vicentAct.Append($"/speak Lewis \"I said you could, but you should have placed the participant ribbon on your animal yesterday so I could bring it to the contest earlier.\"");
                vicentAct.Append($"/speak Vincent \"That's ok, I brought it myself.$h\"/faceDirection Vincent 1/pause 400");
            }
            else
            {
                vicentAct.Append($"/speak Lewis \"But I also said a hundred times you should place the participant ribbon on your animal the day before or bring it to the contest earlier.\"");
                vicentAct.Append($"/speak Vincent \"That's ok, I brought it now.$h\"/faceDirection Vincent 1/pause 400");
            }

            switch (vincentAnimal)
            {
                case VincentAnimal.Frog:
                    vicentAct.Append($"/specificTemporarySprite animalCompetitionFrogShow");
                    if (isAnimalFirstTime)
                    {
                        vicentAct.Append($"/emote Lewis 8/jump Lewis/speak Lewis \"It's a frog! You can't compete with a frog, the contest is for farm animals and pets only.\"");
                        vicentAct.Append($"/faceDirection Vincent 0/speak Vincent \"But it's my pet, its name is Lewis.\"");
                        vicentAct.Append($"/specificTemporarySprite animalCompetitionFrogCroak/playSound croak");
                        vicentAct.Append($"/animate Lewis true false 1000 24/emote Lewis 12/speak Lewis \"You named the frog Lewis? That's outrageous!$4\"");
                        vicentAct.Append($"/speak Vincent \"But I only did it because of how green and smart it is...$s\"");
                        vicentAct.Append($"/emote Lewis 16/speak Lewis \"That is fine then, but you shouldn't name your animals after people.#$b#So, does it know how to do any tricks?\"");
                        vicentAct.Append($"/speak Vincent \"Tricks?$u\"/faceDirection Vincent 1");
                    }
                    else
                    {
                        vicentAct.Append($"/emote Lewis 12/speak Lewis \"It's a frog again! You can't keep bringing a random animal, the contest is for farm animals and pets only.\"");
                        vicentAct.Append($"/speak Sebastian \"There're some farms that grow frogs. They're amazing animals, let it participate.\"");
                        vicentAct.Append($"/speak Lewis \"I wouldn't even know how to evaluate a frog. What do you feed it?\"");
                        vicentAct.Append($"/speak Vincent \"Feed it?$u\"");
                    }
                    vicentAct.Append($"/pause 500/specificTemporarySprite animalCompetitionFrogCroak/playSound croak");
                    vicentAct.Append($"/pause 2000/specificTemporarySprite animalCompetitionFrogRun");
                    vicentAct.Append($"/pause 500/textAboveHead Vincent \"WAIT!\"/speed Vincent 5/move Vincent 22 0 1 true/pause 2000");
                break;
                case VincentAnimal.Squirrel:
                    vicentAct.Append($"/specificTemporarySprite animalCompetitionSquirrelShow");
                    if (isAnimalFirstTime)
                    {
                        vicentAct.Append($"/emote Lewis 8/jump Lewis/speak Lewis \"It's a squirrel! You can't compete with a squirrel, the contest is for farm animals and pets only.\"");
                        vicentAct.Append($"/faceDirection Vincent 0/speak Vincent \"But it's my pet, it likes to climb trees.#$b#I named it Sam because my brother likes to climb trees with...\"");
                        vicentAct.Append($"/animate Sam false false 2000 33/textAboveHead Sam \"VINCENT!\"/faceDirection Vincent 1/jump Vincent");
                    }
                    else
                    {
                        vicentAct.Append($"/emote Lewis 12/speak Lewis \"Vincent, squirrels can't be pets, they're wild animals.\"");
                        vicentAct.Append($"/faceDirection Vincent 0/speak Vincent \"But this one is realy nice.$s\"");
                        vicentAct.Append($"/speak Jas \"It's also cute, much better than bugs.$h\"");
                        vicentAct.Append($"/speak Lewis \"Kids, no metter how nice and cute squirrels are, when they become stressed they might bite and scratch you.\"");
                        vicentAct.Append($"/speak Jodi \"Vincent, listen to Lewis, you can't keep the squirrel.$4\"");
                        vicentAct.Append($"/faceDirection Vincent 1/textAboveHead Vincent \"BUT MOM!\"");
                    }
                    vicentAct.Append($"/pause 2000/specificTemporarySprite animalCompetitionSquirrelRun");
                    vicentAct.Append($"/pause 500/textAboveHead Vincent \"WAIT!\"/speed Vincent 5/move Vincent 22 0 1 true/pause 2000");
                    break;
                case VincentAnimal.Bird:
                    vicentAct.Append($"/specificTemporarySprite animalCompetitionBirdShow");
                    if (isAnimalFirstTime)
                    {
                        vicentAct.Append($"/emote Lewis 8/speak Lewis \"Oh, It's a bird! That's fine. But should it be in a cage?\"");
                        vicentAct.Append($"/faceDirection Vincent 0/speak Vincent \"A cage?$u\"/faceDirection Vincent 1/pause 500");
                    }
                    else
                    {
                        vicentAct.Append("/emote Lewis 12/speak Lewis \"Vincent, you can't catch a random bird to participate. It'll just fly away like last time.\"");
                        vicentAct.Append("/faceDirection Vincent 0/speak Vincent \"This one won't, he is my friend.$h\"/faceDirection Vincent 1/pause 4000/textAboveHead Vincent \"See...\"/pause 500");
                    }
                    vicentAct.Append("/specificTemporarySprite animalCompetitionBirdFly2");
                    vicentAct.Append("/pause 500/textAboveHead Vincent \"WAIT!\"/speed Vincent 5/move Vincent 22 0 1 true/pause 2000");
                    break;
                case VincentAnimal.Rabbit:
                    vicentAct.Append("/specificTemporarySprite animalCompetitionRabbitShow 29 64 false true");
                    if (isAnimalFirstTime)
                    {
                        vicentAct.Append("/speak Lewis \"Oh, it's a rabbit. Did you mother bought you one from Marnie?\"");
                        vicentAct.Append("/faceDirection Vincent 0/speak Vincent \"No, Linus tought me how to catch it.\"");
                        vicentAct.Append("/jump Lewis/emote Lewis 16/speak Lewis \"You cautch a wild rabbit? I'm impressed.\"");
                        vicentAct.Append("/speak Linus \"He is quite good at it, but never learned how to befriend one so it would stay put.$h\"");
                        vicentAct.Append("/faceDirection Vincent 1");
                    }
                    else
                    {
                        vicentAct.Append("/speak Lewis \"You catch another wild rabbit, I'll always be impressed.$h#$b#Did you learn how to befriend it yet?\"");
                        vicentAct.Append("/faceDirection Vincent 0/animate Vincent false true 100 16 17/speak Vincent \"Yes, I got a cave carrot by my hand just like Linus tought me.\"");
                        vicentAct.Append("/speak Lewis \"Vincent, your hand is empty.$h\"");
                        vicentAct.Append("/stopAnimation Vincent/speak Vincent \"Oh, I must have droped it.$u\"/faceDirection Vincent 1/pause 500/playSound eat");
                    }
                    vicentAct.Append($"/pause 1000/specificTemporarySprite animalCompetitionRabbitRun");
                    vicentAct.Append($"/pause 500/textAboveHead Vincent \"WAIT!\"/speed Vincent 5/move Vincent 22 0 1 true/pause 2000");

                    break;
            }
            vicentAct.Append($"/emote Lewis 40");

            return vicentAct.ToString();
        }

        private static string GetMarieAct(string marnieAnimal, bool isFirstTime)
        {
            StringBuilder marnieAct = new StringBuilder();

            marnieAct.Append($"/speak Lewis \"Hi Marnie. Let's see what fine animal you brought{(isFirstTime?"":" this time")}.$h\"/pause 200");
            
            if (marnieAnimal.Contains("Cow"))
            {
                marnieAct.Append("/speak Lewis \"What an astonishing cow we have here. It remind me of that time we were at the barn and...\"");
                marnieAct.Append("/speak Marnie \"...$4\"");
                marnieAct.Append("/speak Lewis \"Er... shall we continue with the evalution...\"");
                marnieAct.Append("/speak Caroline \"I wouldn't mind wearing the story.\"");
            }
            else if (marnieAnimal.Contains("Chicken"))
            {
                marnieAct.Append("/speak Lewis \"What a majestic chicken we have here. It remind me of that time inside the coop we...\"");
                marnieAct.Append("/speak Marnie \"...$4\"");
                marnieAct.Append("/speak Pierre \"Inside the coop what?\"");
            }
            else if (marnieAnimal.Contains("Duck"))
            {
                marnieAct.Append("/speak Lewis \"What a marvelous duck we have here. It remind me of that time by the lake you...\"");
                marnieAct.Append("/speak Marnie \"...$4\"");
                marnieAct.Append("/speak Lewis \"Er... shall we continue with the evalution...\"");
                marnieAct.Append("/speak Jodi \"What happen by the lake?\"");
            }
            else if (marnieAnimal.Contains("Pig"))
            {
                marnieAct.Append("/speak Lewis \"What a magnificent pig we have here. It remind me of that time I brought a truffle oil...\"");
                marnieAct.Append("/speak Marnie \"...$4\"");
                marnieAct.Append("/speak Lewis \"...$4\"");
                marnieAct.Append("/speak George \"Truffle oil?\"");
            }
            else if (marnieAnimal.Contains("Goat"))
            {
                marnieAct.Append("/speak Lewis \"What a sublime goat we have here. It remind me of that time you got a cave carrot to...\"");
                marnieAct.Append("/speak Marnie \"...$4\"");
                marnieAct.Append("/speak Lewis \"Er... shall we continue with the evalution...\"");
                marnieAct.Append("/speak Evelyn \"Cave carrot?\"");
            }
            else if (marnieAnimal.Contains("Rabbit"))
            {
                marnieAct.Append("/speak Lewis \"What a outstanding rabbit we have here. It remind me of that time Jas knocked on the door with a rabbit...\"");
                marnieAct.Append("/speak Marnie \"...$4\"");
                marnieAct.Append("/emote Lewis 40");
                marnieAct.Append("/speak Jas \"I saw shoe under the b...$u\"");
            }
            else if (marnieAnimal.Contains("Sheep"))
            {
                marnieAct.Append("/speak Lewis \"What a woolly sheep we have here. It remind me of that night by the bush...\"");
                marnieAct.Append("/speak Marnie \"...$4\"");
                if (Game1.player.mailReceived.Contains("secretNote21_done"))
                {
                    marnieAct.Append("/emote farmer 16");
                }
                else
                {
                    marnieAct.Append("/emote farmer 8");
                }
                marnieAct.Append("/speak Jas \"Did you see something scary?\"");
            }
            else
            {
                marnieAct.Append("/speak Lewis \"What a wonderful animal we have here. It remind me of that time we...\"");
                marnieAct.Append("/speak Marnie \"...$4\"");                
                marnieAct.Append("/emote farmer 8");
            }
            marnieAct.Append("/speak Lewis \"Well... this isn't a story contests. Let me take a look at the participant.$u\"");
            marnieAct.Append($"/pause 1500/speak Lewis \"{(isFirstTime?"As no surprise, ":"As always, ")}a realy well treated animal.$h\"");
            marnieAct.Append($"/speak Marnie \"Thank you Lewis.$h\"");
            marnieAct.Append($"/speak Lewis \"I'll see the rest of them now.\"");

            return marnieAct.ToString();
        }

        private static string GetPlayerEvalutionAct(AnimalContestItem animalContestInfo, FarmAnimal farmAnimal)
        {
            StringBuilder playerAct = new StringBuilder();

            if (animalContestInfo.FarmAnimalScore is AnimalContestScore score)
            {
                playerAct.Append($"/speak Lewis \"Now, let's take a look at {farmAnimal.displayName}.\"");

                if (score.AgePoints == 0)
                {
                    playerAct.Append($"/speak Lewis \"I'm sorry @ but {farmAnimal.displayName} is too old, I have to disqualify it.$s#$b#That's not a matter of prejudice against old animals, it's just that these contests can be quite stressful for them.#$b#After serving us so well for years, the animals also deserve their rest.\"");
                }
                else if (score.TreatVariatyPoints == 0)
                {
                    playerAct.Append($"/speak Lewis \"Haven't you ever given any treat to {farmAnimal.displayName}? I have to disqualify it, poor thing.$s\"");
                }
                else if (score.TreatAvaregePoints == 0)
                {
                    playerAct.Append($"/speak Lewis \"You've been giving so few treats to {farmAnimal.displayName} that I can't do the appraisal. I have to disqualify it.\"");
                }
                else if (score.FriendshipPoints == 0)
                {
                    playerAct.Append($"/speak Lewis \"@, you should be ashamed. Why would you even bring to the contest an animal that has no affection for you?$u#$b#It's so basic that I never thought I would disqualify someone for that.\"");
                }
                else
                {
                    if (score.ParentWinnerPoints > 0)
                    {
                        playerAct.Append($"/pause 500/speak Lewis \"Oh, I see it's mother is a former winner. That certanly gives {farmAnimal.displayName} an advantage.\"");
                    }
                    playerAct.Append($"/pause 800");
                    if (score.MonthsOld <= 1)
                    {
                        playerAct.Append($"/speak Lewis \"It's a bit too young to compete. You should have waited for another contest.\"");
                    }
                    else if (score.MonthsOld <= 2)
                    {
                        playerAct.Append($"/speak Lewis \"It's almost old enough to compete, but you could have waited for another contest.\"");
                    }
                    else if (score.MonthsOld <= 4)
                    {
                        playerAct.Append($"/speak Lewis \"It's at the age we are looking for. Let's see if you're treating it well.$h\"");
                    }
                    else if (score.MonthsOld <= 6)
                    {
                        playerAct.Append($"/speak Lewis \"It's a little older than we are looking for, but you sure had time to make it outstand on other criterias.\"");
                    }
                    else if (score.MonthsOld <= 8)
                    {
                        playerAct.Append($"/speak Lewis \"It's almost at retirement age but it might still have a chance.\"");
                    }
                    playerAct.Append($"/pause 800");
                    switch (score.FriendshipPoints)
                    {
                        case 1:
                        case 2:
                            playerAct.Append($"/speak Lewis \"{farmAnimal.displayName} could use some more attention so it can grow into a fine animal.$s\"");
                            break;
                        case 3:
                            playerAct.Append($"/speak Lewis \"{farmAnimal.displayName} has an avarage affection towards you. That's good, but you should work on it a little more.\"");
                            break;
                        case 4:
                            playerAct.Append($"/speak Lewis \"It's nice to see how close you two are. A little more care and it'll be fully grown.\"");
                            break;
                        case 5:
                        default:
                            playerAct.Append("/speak Lewis \"I'm always happy to see a fully grown animal that love it's owner so much.$h\"");
                            break;
                    }
                    playerAct.Append($"/pause 800");
                    switch (score.TreatVariatyPoints)
                    {
                        case 1:
                            playerAct.Append($"/speak Lewis \"Hmm, to eat only the same treat every time can become quite boring. You should learn more treats that {farmAnimal.shortDisplayType().ToLower()}s like.");
                            break;
                        case 2:
                            playerAct.Append("/speak Lewis \"Hmm, to give two kinds of treats is better than one, but I know you can do better than this.");
                            break;
                        case 3:
                        default:
                            playerAct.Append("/speak Lewis \"Just by the look of its eyes I can see it has tasted a lot of flavors.$h");
                            break;
                    }
                    playerAct.Append("#$b#");
                    string conjunction;
                    switch (score.TreatAvaregePoints)
                    {
                        case 1:
                            conjunction = score.TreatVariatyPoints < 3 ? "And" : "But";
                            playerAct.Append($"{conjunction} it haven't been given much treats, what a shame.$s\"");
                            break;
                        case 2:
                            playerAct.Append($"And though it's been given some treats, I'm sure {farmAnimal.displayName} would like them more often.\"");
                            break;
                        case 3:
                        default:
                            conjunction = score.TreatVariatyPoints < 3? "At least" : "And";
                            playerAct.Append($"{conjunction} by the bright of its color I can see it's been given a lot of treats.$h\"");
                            break;
                    }
                    playerAct.Append($"/pause 200");
                    if (score.TotalPoints >= MinPointsToPossibleWin)
                    {
                        playerAct.Append("/speak Lewis \"Well @, it seems you have a great animal here, with real chances of winning.$h\"");
                        playerAct.Append("/emote farmer 32");
                    }
                    else
                    {
                        playerAct.Append("/speak Lewis \"Well @, I'm sure you can do better next time.\"");
                        playerAct.Append("/emote farmer 28");
                    }
                }
            }
            else
            {
                playerAct.Append("/speak Lewis \"What a nice pet you have here. Does it know any tricks?\"");
                playerAct.Append($"/question null \"#No.#Not yet.#{Game1.player.getPetDisplayName()} {(Game1.player.catPerson? "mews":"barks")} when it wants.\"");
                playerAct.Append("/splitSpeak Lewis \"You need to teach it some tricks for the competition.~Well, bring it again when it has learned something, okay?~That's not realy a trick, is it?$u\"");
                playerAct.Append("/emote farmer 28");
            }
            playerAct.Append("/speak Lewis \"Let's see the next participant.\"");
            return playerAct.ToString();
        }

        private static string GetJasAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder jasAct = new StringBuilder();
            jasAct.Append("/emote Lewis 40");
            if (isFirstTime)
            {
                jasAct.Append("/speak Lewis \"Jas, you can't bring a baby animal to the contest.\"");
                jasAct.Append("/speak Jas \"But you said you're choosing the greatest animal of Stardew Valley, and everybody knows that babies are the greatest.$h\"");
                jasAct.Append("/speak Lewis \"But that's not how the contest work.\"");
                jasAct.Append("/speak Jas \"Why not? I think you should change the rules then.$4\"");
                jasAct.Append("/speak Lewis \"I can't...\"");
                jasAct.Append("/speak Marnie \"Lewis, she is just a kid. Why can't she participate? The contest was one person short anyway.$u\"");
                jasAct.Append($"/speak Lewis \"Hmm... yeah... okay. Come here little {AnimalExtension.GetBabyAnimalNameByType(GetJasAnimal(animalContestInfo.MarnieAnimal))}. Let's take a look at you.\"");
                jasAct.Append("/pause 1500/speak Lewis \"Your baby is really cute Jas.$h\"");
                jasAct.Append("/emote Jas 20/speak Lewis \"Now let's see the next participant.\"");
            }
            else
            {
                jasAct.Append("/speak Lewis \"Jas, I've already told you can't bring a baby animal to the contest.\"");
                jasAct.Append("/speak Jas \"But you let me participate the other time.$s\"");
                jasAct.Append($"/speak Lewis \"Okay, fine, it's just that if you want your animal to win and be congratulated it should be a little older.#$b#Come here little {AnimalExtension.GetBabyAnimalNameByType(GetJasAnimal(animalContestInfo.MarnieAnimal))}. Let's take a look at you.\"");
                jasAct.Append("/pause 1500/speak Lewis \"What a fine baby you have here Jas. I'm impressed, congratulations.$h\"");
                jasAct.Append("/emote Jas 20/speak Jas \"Thank you, did it win?$h\"");
                jasAct.Append("/jump Lewis/emote Lewis 16/speak Lewis \"That's not what I meant. I have to look at all participants. Let me see the next one.\"");
            }
            return jasAct.ToString();
        }

        private static string GetAlexAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder alexAct = new StringBuilder();
            alexAct.Append("/emote Lewis 8");
            if (isFirstTime)
            {
                alexAct.Append("/speak Lewis \"Did you brought Dusty's box? But where is it?\"");
                alexAct.Append("/speak Alex \"Inside the box, there's to much people here and it is a little shy.\"");
                alexAct.Append("/speak Lewis \"Alex, I can't evaluete it inside the box.$u\"");
                alexAct.Append("/speak Alex \"Come here Dusty, it's okay boy.$h\"/playSound dogWhining/pause 1000/specificTemporarySprite animalCompetitionJoshDogOut/pause 1000");
                alexAct.Append("/speak Lewis \"It's not totaly out but that will do. What are you feeding it?\"");
                alexAct.Append("/speak Alex \"It eats only the best kind of food, steak!\"");
                alexAct.Append("/speak Lewis \"Only steak? That's why it hibernates so much inside that box. That's not good Alex, a dog needs other kinds of food too.\"");
                alexAct.Append("/speak Alex \"What can I do? It won't eat other things.$9\"");
                alexAct.Append("/speak Lewis \"In that case I won't even ask if it knows any tricks, you need to teach Dusty how to properly eat first.#$b#I'll see the next participant.\"");
                alexAct.Append("/emote Alex 12");
            }
            else
            {
                alexAct.Append("/speak Lewis \"So, Dusty is still\"");
                alexAct.Append("/speak Alex \"Come here Dusty, it's okay boy.$h\"/playSound dogWhining/pause 1000/specificTemporarySprite animalCompetitionJoshDogOut/pause 1000");
                alexAct.Append("/speak Lewis \"\"");
                alexAct.Append("/speak Alex \"\"");
                alexAct.Append("/speak Lewis \"\"");
                alexAct.Append("/speak Alex \"\"");
                alexAct.Append("/speak Lewis \"\"");
                alexAct.Append("/emote Alex 12");
            }
            return alexAct.ToString();
        }

        private static string GetWillyAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder willyAct = new StringBuilder();
            if (isFirstTime)
            {
                willyAct.Append("/jump Lewis/emote Lewis 16/speak Lewis \"You brought a crab Willy! I know about crab farming, but never thought I'd see one here in the valley animal competition.\"");
                willyAct.Append("/speak Willy \"Aye... It's an experiment I've been makin'. They're the sweetest things.\"");
                willyAct.Append("/speak Lewis \"Hmm... okay, I'll try to evalute it.\"");
                willyAct.Append("/pause 800/playSound eat/jump Lewis/textAboveHead Lewis \"Youch!\"/speak Lewis \"It pinched me.$4\"");
                willyAct.Append("/speak Willy \"The old girl must be stressed. It's always so docile.$s\"");
                willyAct.Append("/speak Lewis \"I'm sorry Willy, but I have to disquilify it. The participant can't attack the judge.$u#$b#I'll see the next participant.\"");
            }
            else
            {
                willyAct.Append("/emote Lewis 16/speak Lewis \"You brought a crab again, Willy.$u\"");
                willyAct.Append("/speak Willy \"Aye... I've been trainin' it to behave in the competition. You'll see.$h\"");
                willyAct.Append("/speak Lewis \"Well... this one hasn't attacked me yet, so I can't disquilify it.$s#$b#Let me try to evalute it.\"");
                willyAct.Append("/pause 800/playSound scissors/jump Lewis/textAboveHead Lewis \"Not again!\"/pause 500/speak Lewis \"It also tried to pinche me. As you already know, I have to disquilify it.$u\"");
                willyAct.Append("/emote Willy 28");
                willyAct.Append("/speak Lewis \"I'll see the next participant.\"");
            }
            return willyAct.ToString();
        }

        private static string GetAbigailAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder abigailAct = new StringBuilder();
            if (isFirstTime)
            {
                abigailAct.Append("/jump Lewis/emote Lewis 16/speak Lewis \"\"");
                abigailAct.Append("/speak Abigail \"\"");
                abigailAct.Append("/speak Lewis \"\"");
                abigailAct.Append("/pause 800/playSound eat/jump Lewis/textAboveHead Lewis \"Youch!\"/speak Lewis \"\"");
                abigailAct.Append("/speak Abigail \"\"");
                abigailAct.Append("/speak Lewis \"$u#$b#I'll see the next participant.\"");
            }
            else
            {
                abigailAct.Append("/jump Lewis/emote Lewis 16/speak Lewis \"\"");
                abigailAct.Append("/speak Abigail \"\"");
                abigailAct.Append("/speak Lewis \"\"");
                abigailAct.Append("/pause 800/playSound eat/jump Lewis/textAboveHead Lewis \"Youch!\"/speak Lewis \"\"");
                abigailAct.Append("/speak Abigail \"\"");
                abigailAct.Append("/speak Lewis \"$u#$b#I'll see the next participant.\"");
            }
            return abigailAct.ToString();
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
                //contendersPool.Remove("Alex");
            }
            if (!Game1.player.eventsSeen.Contains(4))
            {
                //contendersPool.Remove("Abigail");
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
            //TODO - remove, only for test.
            return "Abigail"; //contendersPool.DefaultIfEmpty(removeJasFromPool? "Maru" : "Jas").FirstOrDefault();
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
