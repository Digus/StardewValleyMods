using CustomCrystalariumMod;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace CCRMAutomate.Automate
{
    internal class CustomCrystalariumAutomationFactory : IAutomationFactory
    {
        public IAutomatable GetFor(SObject obj, GameLocation location, in Vector2 tile)
        {
            if (ClonerController.HasCloner(obj.QualifiedItemId))
            {
                return new ClonerMachine(obj,location,tile);
            }
            return null;
        }

        public IAutomatable GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile)
        {
            return null;
        }

        public IAutomatable GetFor(Building building, GameLocation location, in Vector2 tile)
        {
            return null;
        }

        public IAutomatable GetForTile(GameLocation location, in Vector2 tile)
        {
            return null;
        }
    }
}
