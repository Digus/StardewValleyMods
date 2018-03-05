using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using Harmony;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.animals
{
    public class AnimalContestController : AnimalStatusController
    {
        public static readonly IList<string> ContestDays = new ReadOnlyCollection<string>(new List<string>() { "26 spring", "26 fall" }) ;

        public static bool IsParticipant(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID).DayParticipatedContest != null;
        }

        public static bool HasParticipated(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID).HasWon != null;
        }

        public static bool HasWon(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID).HasWon??false;
        }

        public static bool CanChangeParticipant(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID).DayParticipatedContest > SDate.Now();
        }

        public static SDate GetParticipantDate(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID).DayParticipatedContest;
        }


        public static void MakeAnimalParticipant(FarmAnimal farmAnimal)
        {
            MakeAnimalParticipant(farmAnimal.myID);
        }

        public static void MakePetParticipant()
        {
            MakeAnimalParticipant(AnimalData.PetId);
        }

        private static void MakeAnimalParticipant(long id)
        {
            AnimalStatus animalStatus = GetAnimalStatus(id);
            animalStatus.DayParticipatedContest = GetNextContestDate();
        }

        public static SDate GetNextContestDate()
        {
            return ContestDays
                .Select(d => new SDate(Convert.ToInt32(d.Split(' ')[0]), d.Split(' ')[1]))
                .Where(d => d > SDate.Now()).OrderBy(d => d)
                .DefaultIfEmpty(new SDate(Convert.ToInt32(ContestDays[0].Split(' ')[0]), ContestDays[0].Split(' ')[1]))
                .FirstOrDefault();
        }
    }
}
