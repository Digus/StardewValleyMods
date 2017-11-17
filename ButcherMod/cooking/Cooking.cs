using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.cooking
{
    public enum Cooking
    {
        [Description("Meatloaf")]
        [Recipe("639 2 256 1 248 1 -5 1 399 1/1 10/652/default")]
        Meatloaf = 652,
        [Description("Orange Chicken")]
        [Recipe("641 1 635 1/1 10/653/default")]
        OrangeChicken = 653,
        [Description("Monte Cristo")]
        [Recipe("216 1 640 1 424 1 -5 1/1 10/654/default")]
        MonteCristo = 654,
        [Description("Bacon Cheeseburger")]
        [Recipe("216 1 639 1 424 1 666 1/1 10/655/default")]
        BaconCheeseburger = 655,
        [Description("Roast Duck")]
        [Recipe("642 1/1 10/656/default")]
        RoastDuck = 656,
        [Description("Rabbit au Vin")]
        [Recipe("643 1 348 1 404 1 399 1/1 10/657/default")]
        RabbitAuVin = 657,
        [Description("Steak Fajitas")]
        [Recipe("639 2 247 1 260 2 399 2 229 1/1 10/658/default")]
        SteakFajitas = 658,
        [Description("Glazed Ham")]
        [Recipe("640 1 340 1 724 1 245 1/1 10/659/default")]
        GlazedHam = 659,
        [Description("Summer Sausage")]
        [Recipe("639 2 666 1 248 1/1 10/660/default")]
        SummerSausage = 660,
        [Description("Sweet and Sour Pork")]
        [Recipe("640 1 419 1 245 1 247 1/1 10/661/default")]
        SweetAndSourPork = 661,
        [Description("Rabbit Stew")]
        [Recipe("643 1 192 1 256 1 20 1 78 1/1 10/662/default")]
        RabbitStew = 662,
        [Description("Winter Duck")]
        [Recipe("642 1 637 1 250 1 416 1/1 10/663/default")]
        WinterDuck = 663,
        [Description("Steak with Mushrooms")]
        [Recipe("644 1 404 1 257 1 281 1 432 1/1 10/664/default")]
        SteakWithMushrooms = 664,
        [Description("Cowboy Dinner")]
        [Recipe("644 1 207 1 194 1 270 1 426 1/1 10/665/default")]
        CowboyDinner = 665,
        [Description("Bacon")]
        [Recipe("640 1/1 10/666 4/default")]
        Bacon = 666
    }
}
