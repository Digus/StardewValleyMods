using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using StardewModdingAPI.Utilities;
using StardewValley;
using Object = StardewValley.Object;

namespace AnimalHusbandryMod.animals
{
    public class TreatsController
    {
        public static bool IsLikedTreat(int id)
        {
            return 
                   DataLoader.AnimalData.Chicken.LikedTreats.Contains(id)
                || DataLoader.AnimalData.Duck.LikedTreats.Contains(id)
                || DataLoader.AnimalData.Rabbit.LikedTreats.Contains(id)
                || DataLoader.AnimalData.Cow.LikedTreats.Contains(id)
                || DataLoader.AnimalData.Goat.LikedTreats.Contains(id)
                || DataLoader.AnimalData.Sheep.LikedTreats.Contains(id)
                || DataLoader.AnimalData.Pig.LikedTreats.Contains(id)
                ;
        }

        public static bool IsLikedTreat(FarmAnimal farmAnimal, int itemId)
        {
            
            return GetTreatItem(farmAnimal).LikedTreats.Contains(itemId);
        }

        public static bool IsReadyForTreat(FarmAnimal farmAnimal)
        {
            return DaysUntilNextTreat(farmAnimal) <= 0;
        }

        public static int DaysUntilNextTreat(FarmAnimal farmAnimal)
        {
            if (GetAnimalStatus(farmAnimal).LastDayFeedTreat == null)
            {
                return 0;
            }
            return GetAnimalStatus(farmAnimal).LastDayFeedTreat.DaysSinceStart + GetTreatItem(farmAnimal).MinimumDaysBetweenTreats - SDate.Now().DaysSinceStart;
        }

        public static void FeedAnimalTreat(FarmAnimal farmAnimal, Object treat)
        {
            AnimalStatus animalStatus = GetAnimalStatus(farmAnimal);
            animalStatus.LastDayFeedTreat = SDate.Now();
            if (!animalStatus.FeedTreatsQuantity.ContainsKey(treat.parentSheetIndex))
            {
                animalStatus.FeedTreatsQuantity[treat.parentSheetIndex] = 0;
            }
            animalStatus.FeedTreatsQuantity[treat.parentSheetIndex]++;
        }

        private static TreatItem GetTreatItem(FarmAnimal farmAnimal)
        {
            Animal? foundAnimal = AnimalExtension.GetAnimalFromType(farmAnimal.type);
            return DataLoader.AnimalData.getAnimalItem((Animal)foundAnimal) as TreatItem;
        }

        private static AnimalStatus GetAnimalStatus(FarmAnimal farmAnimal)
        {
            AnimalStatus animalStatus = FarmerLoader.FarmerData.AnimalData.Find(s => s.Id == farmAnimal.myID);
            if (animalStatus == null)
            {
                animalStatus = new AnimalStatus(farmAnimal.myID);
                FarmerLoader.FarmerData.AnimalData.Add(animalStatus);
            }

            return animalStatus;
        }

    }
}
