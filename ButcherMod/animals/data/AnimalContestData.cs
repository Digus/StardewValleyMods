using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class AnimalContestData
    {
        public int MinPointsToPossibleWin;
        public int MinPointsToGaranteeWin;
        public List<string> ContestDays;

        public AnimalContestData()
        {
            MinPointsToPossibleWin = 11;
            MinPointsToGaranteeWin = 14;
            ContestDays = new List<string>() { "26 spring", "26 fall" };
        }
    }
}
