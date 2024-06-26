﻿using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using ProducerFrameworkMod.Controllers;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace PFMAutomate.Automate
{
    internal class ProducerFrameworkAutomationFactory : IAutomationFactory
    {
        public IAutomatable GetFor(SObject obj, GameLocation location, in Vector2 tile)
        {
            if (ProducerController.HasProducerRule(obj.QualifiedItemId))
            {
                return new CustomProducerMachine(obj,location,tile);
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
