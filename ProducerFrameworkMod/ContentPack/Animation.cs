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
        public string TextureFile;
        public int FrameInterval = 60;
        public List<int> RelativeFrameIndex = new List<int>();

        //Calculated properties
        internal Texture2D Texture;
        internal int NumberOfFrames;
    }
}
