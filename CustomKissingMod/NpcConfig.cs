using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomKissingMod
{
    public class NpcConfig
    {
        public string Name { get; set; }
        public int Frame { get; set; }
        public bool FrameDirectionRight { get; set; }
        public int BeachFrame { get; set; }
        public bool BeachFrameDirectionRight { get; set; }
        public string RequiredEvent { get; set; }

        public void Copy(NpcConfig npcConfig)
        {
            this.Name = npcConfig.Name;
            this.Frame = npcConfig.Frame;
            this.FrameDirectionRight = npcConfig.FrameDirectionRight;
            this.BeachFrame = npcConfig.BeachFrame;
            this.BeachFrameDirectionRight = npcConfig.BeachFrameDirectionRight;
            this.RequiredEvent = npcConfig.RequiredEvent;
        }

        protected bool Equals(NpcConfig other)
        {
            return Name == other.Name && Frame == other.Frame && FrameDirectionRight == other.FrameDirectionRight && BeachFrame == other.BeachFrame && BeachFrameDirectionRight == other.BeachFrameDirectionRight && RequiredEvent == other.RequiredEvent;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NpcConfig)obj);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Frame, FrameDirectionRight, BeachFrame, BeachFrameDirectionRight, RequiredEvent);
        }
    }
}
