using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.farmer;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;

namespace AnimalHusbandryMod.animals
{
    public class AnimalContestController : AnimalStatusController
    {
        public static readonly IList<string> ContestDays = new ReadOnlyCollection<string>(new List<string>() { "26 spring", "26 fall" }) ;
        private static FarmAnimal _temporaryFarmAnimal = null;

        public static bool IsParticipant(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID.Value).DayParticipatedContest != null;
        }

        public static bool HasParticipated(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID.Value).HasWon != null;
        }

        public static bool HasWon(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID.Value).HasWon??false;
        }

        public static bool CanChangeParticipant(FarmAnimal farmAnimal)
        {
            return CanChangeParticipant(farmAnimal.myID.Value);
        }

        public static bool CanChangeParticipantPet()
        {
            return CanChangeParticipant(AnimalData.PetId);
        }

        private static bool CanChangeParticipant(long id)
        {
            return GetAnimalStatus(id).DayParticipatedContest > SDate.Now();
        }

        public static SDate GetParticipantDate(FarmAnimal farmAnimal)
        {
            return GetAnimalStatus(farmAnimal.myID.Value).DayParticipatedContest;
        }

        public static void MakeAnimalParticipant(FarmAnimal farmAnimal)
        {
            MakeAnimalParticipant(farmAnimal.myID.Value);
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

        public static void RemoveAnimalParticipant(FarmAnimal farmAnimal)
        {
            RemoveAnimalParticipant(farmAnimal.myID.Value);
        }

        public static void RemovePetParticipant()
        {
            RemoveAnimalParticipant(AnimalData.PetId);
        }

        private static void RemoveAnimalParticipant(long id)
        {
            AnimalStatus animalStatus = GetAnimalStatus(id);
            animalStatus.DayParticipatedContest = null;
        }

        public static SDate GetNextContestDate()
        {
            return SDate.Now();
            //return ContestDays
            //    .Select(d => new SDate(Convert.ToInt32(d.Split(' ')[0]), d.Split(' ')[1]))
            //    .Where(d => d >= SDate.Now()).OrderBy(d => d)
            //    .DefaultIfEmpty(new SDate(Convert.ToInt32(ContestDays[0].Split(' ')[0]), ContestDays[0].Split(' ')[1]))
            //    .FirstOrDefault();
        }

        public static String GetNextContestDateKey()
        {
            SDate date = GetNextContestDate();
            return $"{date.Year:00}{Utility.getSeasonNumber(date.Season)}{date.Day:00}";
        }

        public static long? GetNextContestParticipantId()
        {
            AnimalStatus animalStatus = FarmerLoader.FarmerData.AnimalData.Find(s => s.DayParticipatedContest == GetNextContestDate()) ?? FarmerLoader.FarmerData.AnimalData.Find(s=> s.Id == -9223372036854775399);
            return animalStatus?.Id;
        }

        public static void EndEvent(AnimalContestItem animalContestItem, bool participated = true)
        {
            if (!participated)
            {
                animalContestItem.Winner = "Marnie";
            }
            if (animalContestItem.ParticipantId.HasValue)
            {
                long participantIdValue = animalContestItem.ParticipantId.Value;
                AnimalStatus animalStatus = GetAnimalStatus(participantIdValue);
                animalStatus.HasWon = (animalStatus.HasWon??false) || animalContestItem.Winner == "Farmer";
                if (participantIdValue != AnimalData.PetId)
                {
                    ReAddFarmAnimal(participantIdValue);
                }
                else
                {
                    AnimalContestController.ReAddPet();
                }
            }
        }

        public static FarmAnimal GetAnimal(long id)
        {
            if (_temporaryFarmAnimal?.myID.Value == id)
            {
                return _temporaryFarmAnimal;
            }
            FarmAnimal animal = Utility.getAnimal(id);
            if (animal != null)
            {
                return animal;
            }
            else
            {
                AnimalHusbandryModEntry.monitor.Log($"The animal id '{id}' was not found in the game and its animal status data is being discarted.", LogLevel.Warn);
                RemoveAnimalStatus(id);
                return null;
            }
        }

        public static void TemporalyRemoveFarmAnimal(FarmAnimal farmAnimal)
        {
            if (farmAnimal.currentLocation is Farm farm)
            {
                farm.animals.Remove(farmAnimal.myID.Value);
                _temporaryFarmAnimal = farmAnimal;
            }
            else if (farmAnimal.currentLocation is AnimalHouse animalHouse)
            {
                animalHouse.animals.Remove(farmAnimal.myID.Value);
                _temporaryFarmAnimal = farmAnimal;
            }
        }

        public static void TemporalyRemovePet()
        {
            Game1.getCharacterFromName(Game1.player.getPetName()).IsInvisible = true;
        }

        public static void ReAddFarmAnimal(long participantIdValue)
        {
            if (_temporaryFarmAnimal != null && _temporaryFarmAnimal.myID.Value == participantIdValue)
            {
                (_temporaryFarmAnimal.home.indoors.Value as AnimalHouse)?.animals.Add(_temporaryFarmAnimal.myID.Value, _temporaryFarmAnimal);
            }
            _temporaryFarmAnimal = null;
        }

        public static void ReAddPet()
        {
            Game1.getCharacterFromName(Game1.player.getPetName()).IsInvisible = false;
        }

        public static void CleanTemporaryParticipant()
        {
            _temporaryFarmAnimal = null;
        }
    }
}
