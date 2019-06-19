using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

namespace AnimalHusbandryMod.common
{
    public class EventGreenSlime : GreenSlime
    {
        public EventGreenSlime()
        {
        }

        public EventGreenSlime(Vector2 position) : base(position)
        {
        }

        public EventGreenSlime(Vector2 position, int mineLevel) : base(position, mineLevel)
        {
        }

        public EventGreenSlime(Vector2 position, Color color) : base(position, color)
        {
        }

        public override void behaviorAtGameTick(GameTime time)
        {
        }

        public override void updateMovement(GameLocation location, GameTime time)
        {
        }
    }
}
