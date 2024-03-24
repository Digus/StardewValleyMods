using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.common
{
    internal interface Channel
    {
        string GetName{ get; }
        string GetDisplayName { get; }        
        string GetScreenTextureName { get; }
        Rectangle GetScreenSourceRectangle { get; }
        bool CheckChannelDay();
        String[] GetEpisodesText();
        void ReloadEpisodes();
    }
}
