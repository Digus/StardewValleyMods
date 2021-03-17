using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ProducerFrameworkMod.ContentPack
{
    public class Animation
    {
        public int FrameInterval = 60;
        public List<int> RelativeFrameIndex = new List<int>();
        public Dictionary<string, List<int>> AdditionalAnimations = new Dictionary<string, List<int>>();
        public Dictionary<int, List<int>> AdditionalAnimationsId = new Dictionary<int, List<int>>();
    }
}
