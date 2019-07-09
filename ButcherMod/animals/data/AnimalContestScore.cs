namespace AnimalHusbandryMod.animals.data
{
    public class AnimalContestScore
    {
        public int FriendshipPoints;
        public int MonthsOld;
        public int AgePoints;
        public int TreatVariatyPoints;
        public int TreatAvaregePoints;
        public int ParentWinnerPoints;
        public int TotalPoints => FriendshipPoints + AgePoints + TreatVariatyPoints + TreatAvaregePoints + ParentWinnerPoints;

        public AnimalContestScore(int friendshipPoints, int monthsOld, int agePoints, int treatVariatyPoints, int treatAvaregePoints, int parentWinnerPoints)
        {
            this.FriendshipPoints = friendshipPoints;
            this.MonthsOld = monthsOld;
            this.AgePoints = agePoints;
            this.TreatVariatyPoints = treatVariatyPoints;
            this.TreatAvaregePoints = treatAvaregePoints;
            this.ParentWinnerPoints = parentWinnerPoints;
        }
    }
}