﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.common
{
    internal class TvController
    {
        private static Dictionary<string, Channel> channels = new Dictionary<string, Channel>();

        internal static void AddChannel(Channel channel)
        {
            channels.Add(channel.GetName, channel);
        }

        internal static Channel GetChannel(string name)
        {
            channels.TryGetValue(name, out Channel channel);
            return channel;
        }

        internal static List<Channel> GetChannelsWithEpisodeToday()
        {
            return channels.Select(p => p.Value).Where(c => c.CheckChannelDay()).ToList();
        }

        internal static void ReloadEpisodes()
        {
            foreach (var channel in channels)
            {
                channel.Value.ReloadEpisodes();
            }
        }
    }
}
