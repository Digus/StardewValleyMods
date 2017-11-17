using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.cooking
{
    public class CookingData
    {
        public CookingItem Meatloaf;
        public CookingItem OrangeChicken;
        public CookingItem MonteCristo;
        public CookingItem BaconCheeseburger;
        public CookingItem RoastDuck;
        public CookingItem RabbitAuVin;
        public CookingItem SteakFajitas;
        public CookingItem GlazedHam;
        public CookingItem SummerSausage;
        public CookingItem SweetAndSourPork;
        public CookingItem RabbitStew;
        public CookingItem WinterDuck;
        public CookingItem SteakWithMushrooms;
        public CookingItem CowboyDinner;
        public CookingItem Bacon;

        public CookingData()
        {
            Meatloaf = new CookingItem(370,90,0,0,0,1,0,0,0,1,1,0,1440);
            OrangeChicken = new CookingItem(250,65,0,0,2,2,0,0,0,0,0,0,600);
            MonteCristo = new CookingItem(620,120,0,0,0,3,6,0,64,2,0,0,780);
            BaconCheeseburger = new CookingItem(660,130,0,0,0,0,0,100,0,0,0,0,960);
            RoastDuck = new CookingItem(410,100,0,0,0,0,0,0,0,1,6,0,780);
            RabbitAuVin = new CookingItem(570,110,0,0,0,4,0,0,64,2,4,4,1440);
            SteakFajitas = new CookingItem(415,100,0,0,0,0,0,0,0,2,0,0,300);
            GlazedHam = new CookingItem(550,105,0,0,6,3,0,0,64,1,0,0,960);
            SummerSausage = new CookingItem(360,90,0,0,0,0,0,0,0,0,2,2,780);
            SweetAndSourPork = new CookingItem(450,105,6,0,0,3,0,0,32,2,0,0,780);
            RabbitStew = new CookingItem(360,90,0,0,4,4,0,0,64,2,4,0,1440);
            WinterDuck = new CookingItem(360,90,0,0,0,6,0,0,0,0,0,0,1440);
            SteakWithMushrooms = new CookingItem(510,105,0,0,0,0,0,0,0,2,3,6,1440);
            CowboyDinner = new CookingItem(305,80,4,0,4,3,4,50,0,1,0,0,960);
            Bacon = new CookingItem(300,75,0,0,0,0,0,50,0,0,0,0,780);
        }

        public CookingItem getCookingItem(Cooking cooking)
        {
            switch (cooking)
            {
                case Cooking.Meatloaf:
                    return Meatloaf;
                case Cooking.OrangeChicken:
                    return OrangeChicken;
                case Cooking.MonteCristo:
                    return MonteCristo;
                case Cooking.BaconCheeseburger:
                    return BaconCheeseburger;
                case Cooking.RoastDuck:
                    return RoastDuck;
                case Cooking.RabbitAuVin:
                    return RabbitAuVin;
                case Cooking.SteakFajitas:
                    return SteakFajitas;
                case Cooking.GlazedHam:
                    return GlazedHam;
                case Cooking.SummerSausage:
                    return SummerSausage;
                case Cooking.SweetAndSourPork:
                    return SweetAndSourPork;
                case Cooking.RabbitStew:
                    return RabbitStew;
                case Cooking.WinterDuck:
                    return WinterDuck;
                case Cooking.SteakWithMushrooms:
                    return SteakWithMushrooms;
                case Cooking.CowboyDinner:
                    return CowboyDinner;
                case Cooking.Bacon:
                    return Bacon;
                default:
                    throw new ArgumentException("Invalid Cooking");
            }
        }
    }
}
