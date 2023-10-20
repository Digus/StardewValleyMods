using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnimalHusbandryMod.animals.data;

namespace AnimalHusbandryMod.animals.events
{
    public class VincentAct : AbstractAct
    {
        public override string NpcName => "Vincent";

        public override string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history)
        {
            Enum.TryParse(animalContestInfo.VincentAnimal, out VincentAnimal vincentAnimal);
            return GetVincentAct(vincentAnimal, history.Count % 5 == 4, !history.Exists(h => h.VincentAnimal == vincentAnimal.ToString()), history.Count == 0);
        }

        private string GetVincentAct(VincentAnimal vincentAnimal, bool isLate, bool isAnimalFirstTime, bool isFirstTime)
        {

            StringBuilder vincentAct = new StringBuilder();
            vincentAct.Append($"/speak Lewis \"{GetDialog("Lewis.Begin1")}\"");
            if (isLate)
            {
                vincentAct.Append($"/pause 2000/emote Lewis 40/pause 1000/speak Lewis \"{GetDialog("Lewis.Alternate1")}\"/emote Jodi 28 true/pause 1000");
            }
            vincentAct.Append($"/speak Vincent \"{GetDialog("Vincent.Wait")}\"");
            if (isLate)
            {
                vincentAct.Append($"/speak Lewis \"{GetDialog("Lewis.Alternate2")}\"/emote Jodi 16 true");
            }
            vincentAct.Append($"/speed Vincent 5/move Vincent 0 -16 0/speak Vincent \"{GetDialog("Vincent.Begin1")}\"");
            vincentAct.Append($"/speak Lewis \"{GetDialog("Lewis.Begin2")}\"");
            vincentAct.Append($"/speak Vincent \"{GetDialog("Vincent.Begin2")}\"");
            if (isFirstTime)
            {
                vincentAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime")}\"");
                vincentAct.Append($"/speak Vincent \"{GetDialog("Vincent.FirstTime")}\"/faceDirection Vincent 1/pause 400");
            }
            else
            {
                vincentAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes")}\"");
                vincentAct.Append($"/speak Vincent \"{GetDialog("Vincent.OtherTimes")}\"/faceDirection Vincent 1/pause 400");
            }

            switch (vincentAnimal)
            {
                case VincentAnimal.Frog:
                    vincentAct.Append($"/specificTemporarySprite animalContestFrogShow");
                    if (isAnimalFirstTime)
                    {
                        vincentAct.Append($"/emote Lewis 8/jump Lewis/speak Lewis \"{GetDialog("Frog.Lewis.FirstTime1")}\"");
                        vincentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{GetDialog("Frog.Vincent.FirstTime1")}\"");
                        vincentAct.Append($"/specificTemporarySprite animalContestFrogCroak/playSound croak");
                        vincentAct.Append($"/animate Lewis true false 1000 24/emote Lewis 12/speak Lewis \"{GetDialog("Frog.Lewis.FirstTime2")}\"");
                        vincentAct.Append($"/speak Vincent \"{GetDialog("Frog.Vincent.FirstTime2")}\"");
                        vincentAct.Append($"/emote Lewis 16/speak Lewis \"{GetDialog("Frog.Lewis.FirstTime3")}\"");
                        vincentAct.Append($"/speak Vincent \"{GetDialog("Frog.Vincent.FirstTime3")}\"/faceDirection Vincent 1");
                    }
                    else
                    {
                        vincentAct.Append($"/emote Lewis 12/speak Lewis \"{GetDialog("Frog.Lewis.OtherTime1")}\"");
                        vincentAct.Append($"/speak Sebastian \"{GetDialog("Frog.Sebastian.OtherTimes")}\"");
                        vincentAct.Append($"/speak Lewis \"{GetDialog("Frog.Lewis.OtherTimes2")}\"");
                        vincentAct.Append($"/speak Vincent \"{GetDialog("Frog.Vincent.OtherTimes")}\"");
                    }
                    vincentAct.Append($"/pause 500/specificTemporarySprite animalContestFrogCroak/playSound croak");
                    vincentAct.Append($"/pause 2000/specificTemporarySprite animalContestFrogRun");
                    break;
                case VincentAnimal.Squirrel:
                    vincentAct.Append($"/specificTemporarySprite animalContestSquirrelShow");
                    if (isAnimalFirstTime)
                    {
                        vincentAct.Append($"/emote Lewis 8/jump Lewis/speak Lewis \"{GetDialog("Squirrel.Lewis.FirstTime")}\"");
                        vincentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{GetDialog("Squirrel.Vincent.FirstTime")}\"");
                        vincentAct.Append($"/animate Sam false false 2000 33/textAboveHead Sam \"{GetDialog("Squirrel.Sam.FirstTime")}\"/pause 500/faceDirection Vincent 1/jump Vincent");
                    }
                    else
                    {
                        vincentAct.Append($"/emote Lewis 12/speak Lewis \"{GetDialog("Squirrel.Lewis.OtherTimes1")}\"");
                        vincentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{GetDialog("Squirrel.Vincent.OtherTimes1")}\"");
                        vincentAct.Append($"/speak Jas \"{GetDialog("Squirrel.Jas.OtherTimes")}\"");
                        vincentAct.Append($"/speak Lewis \"{GetDialog("Squirrel.Lewis.OtherTimes2")}\"");
                        vincentAct.Append($"/speak Jodi \"{GetDialog("Squirrel.Jodi.OtherTimes")}\"");
                        vincentAct.Append($"/faceDirection Vincent 1/textAboveHead Vincent \"{GetDialog("Squirrel.Vincent.OtherTimes2")}\"");
                    }
                    vincentAct.Append($"/pause 1000/specificTemporarySprite animalContestSquirrelRun");
                    break;
                case VincentAnimal.Bird:
                    vincentAct.Append($"/specificTemporarySprite animalContestBirdShow");
                    if (isAnimalFirstTime)
                    {
                        vincentAct.Append($"/emote Lewis 8/speak Lewis \"{GetDialog("Bird.Lewis.FirstTime")}\"");
                        vincentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{GetDialog("Bird.Vincent.FirstTime")}\"/faceDirection Vincent 1/pause 500");
                    }
                    else
                    {
                        vincentAct.Append($"/emote Lewis 12/speak Lewis \"{GetDialog("Bird.Lewis.OtherTimes")}\"");
                        vincentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{GetDialog("Bird.Vincent.OtherTimes1")}\"/faceDirection Vincent 1/pause 4000");
                        vincentAct.Append($"/textAboveHead Vincent \"{GetDialog("Bird.Vincent.OtherTimes2")}\"/pause 500");
                    }
                    vincentAct.Append("/specificTemporarySprite animalContestBirdFly");
                    break;
                case VincentAnimal.Rabbit:
                    vincentAct.Append("/specificTemporarySprite animalContestRabbitShow 29 64 false true");
                    if (isAnimalFirstTime)
                    {
                        vincentAct.Append($"/speak Lewis \"{GetDialog("Rabbit.Lewis.FirstTime1")}\"");
                        vincentAct.Append($"/faceDirection Vincent 0/speak Vincent \"{GetDialog("Rabbit.Vincent.FirstTime")}\"");
                        vincentAct.Append($"/jump Lewis/emote Lewis 16/speak Lewis \"{GetDialog("Rabbit.Lewis.FirstTime2")}\"");
                        vincentAct.Append($"/speak Linus \"{GetDialog("Rabbit.Linus.FirstTime")}\"");
                        vincentAct.Append("/faceDirection Vincent 1");
                    }
                    else
                    {
                        vincentAct.Append($"/speak Lewis \"{GetDialog("Rabbit.Lewis.OtherTimes1")}\"");
                        vincentAct.Append($"/faceDirection Vincent 0/animate Vincent false true 100 16 17/speak Vincent \"{GetDialog("Rabbit.Vincent.OtherTimes1")}\"");
                        vincentAct.Append($"/speak Lewis \"{GetDialog("Rabbit.Lewis.OtherTimes2")}\"");
                        vincentAct.Append($"/stopAnimation Vincent/speak Vincent \"{GetDialog("Rabbit.Vincent.OtherTimes2")}\"/faceDirection Vincent 1/pause 500/playSound eat");
                    }
                    vincentAct.Append($"/pause 1000/specificTemporarySprite animalContestRabbitRun");
                    break;
            }
            vincentAct.Append($"/pause 500/textAboveHead Vincent \"{GetDialog("Vincent.Wait")}\"/speed Vincent 5/move Vincent 22 0 1 true/pause 2000");
            vincentAct.Append($"/emote Lewis 40");

            return vincentAct.ToString();
        }

        public VincentAnimal ChooseVincentAnimal(Random random, List<AnimalContestItem> history)
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
            else
            {
                animalsPool.Remove(VincentAnimal.Bird);
            }

            return animalsPool[random.Next(animalsPool.Count - 1)];
        }
    }
}
