using Microsoft.Xna.Framework.Input;

namespace CropTransplantMod
{
    public class ModConfig
    {

        public bool GetGradenPotEarlier;
        public float TransplantEnergyCost;

        public ModConfig()
        {
            GetGradenPotEarlier = false;
            TransplantEnergyCost = 4f;
        }
    }
}
