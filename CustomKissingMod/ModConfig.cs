﻿using System.Collections.Generic;
using StardewModdingAPI;

namespace CustomKissingMod
{
    public class ModConfig
    {
        public bool DisableDatingRequirement { get; set; }
        public bool DisableEventRequirement { get; set; }
        public bool DisableJealousy { get; set; }
        public bool DisableExhaustionReset { get; set; }
        public int RequiredFriendshipLevel { get; set; }
        public int KissingFriendshipPoints { get; set; }
        public int JealousyFriendshipPoints { get; set; }
        public bool EnableContentPacksOverrides { get; set; }
        public bool DisableCursorKissingIndication { get; set; }
        public List<NpcConfig> NpcConfigs { get; set; }

        public ModConfig()
        {
            DisableDatingRequirement = false;
            DisableEventRequirement = false;
            DisableJealousy = false;
            DisableExhaustionReset = false;
            RequiredFriendshipLevel = 8;
            KissingFriendshipPoints = 10;
            JealousyFriendshipPoints = -250;
            NpcConfigs = new List<NpcConfig>()
            {
                new NpcConfig
                {
                    Name = "Abigail",
                    Frame = 33,
                    FrameDirectionRight = false,
                    BeachFrame = 12,
                    BeachFrameDirectionRight = false,
                    RequiredEvent = "901756"
                },
                new NpcConfig
                {
                    Name = "Alex",
                    Frame = 42,
                    FrameDirectionRight = true,
                    BeachFrame = 4,
                    BeachFrameDirectionRight = true,
                    RequiredEvent = "911526"
                },
                new NpcConfig
                {
                    Name = "Elliott",
                    Frame = 35,
                    FrameDirectionRight = false,
                    BeachFrame = 12,
                    BeachFrameDirectionRight = false,
                    RequiredEvent = "43"
                },
                new NpcConfig
                {
                    Name = "Emily",
                    Frame = 33,
                    FrameDirectionRight = false,
                    BeachFrame = 12,
                    BeachFrameDirectionRight = false,
                    RequiredEvent = "2123343"
                },
                new NpcConfig
                {
                    Name = "Haley",
                    Frame = 28,
                    FrameDirectionRight = true,
                    BeachFrame = 4,
                    BeachFrameDirectionRight = true,
                    RequiredEvent = "15"
                },
                new NpcConfig
                {
                    Name = "Harvey",
                    Frame = 31,
                    FrameDirectionRight = false,
                    BeachFrame = 12,
                    BeachFrameDirectionRight = false,
                    RequiredEvent = "528052"
                },
                new NpcConfig
                {
                    Name = "Leah",
                    Frame = 25,
                    FrameDirectionRight = true,
                    BeachFrame = 4,
                    BeachFrameDirectionRight = true,
                    RequiredEvent = "54"
                },
                new NpcConfig
                {
                    Name = "Maru",
                    Frame = 28,
                    FrameDirectionRight = false,
                    BeachFrame = 12,
                    BeachFrameDirectionRight = false,
                    RequiredEvent = "10"
                },
                new NpcConfig
                {
                    Name = "Penny",
                    Frame = 35,
                    FrameDirectionRight = true,
                    BeachFrame = 4,
                    BeachFrameDirectionRight = true,
                    RequiredEvent = "38"
                },
                new NpcConfig
                {
                    Name = "Sam",
                    Frame = 36,
                    FrameDirectionRight = true,
                    BeachFrame = 4,
                    BeachFrameDirectionRight = true,
                    RequiredEvent = "233104"
                },
                new NpcConfig
                {
                    Name = "Sebastian",
                    Frame = 40,
                    FrameDirectionRight = false,
                    BeachFrame = 12,
                    BeachFrameDirectionRight = false,
                    RequiredEvent = "384882"
                },
                new NpcConfig
                {
                    Name = "Shane",
                    Frame = 34,
                    FrameDirectionRight = false,
                    BeachFrame = 12,
                    BeachFrameDirectionRight = false,
                    RequiredEvent = "9581348"
                }
            };
        }
    }
}