using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using Microsoft.Xna.Framework;
using PyTK.CustomTV;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace AnimalHusbandryMod.animals
{
    public class LivingWithTheAnimalsChannel
    {
        private TemporaryAnimatedSprite showSprite;

        private readonly Dictionary<int, Episode> _episodes;
        public static readonly string LivingWithTheAnimals = "LivingWithTheAnimals";

        public LivingWithTheAnimalsChannel()
        {
            this._episodes = new Dictionary<int, Episode>();

            _episodes.Add(1, new Episode("TV.LivingWithTheAnimals.Episode.Buildings", false, false, false));
        }

        public void CheckChannelDay()
        {
            CustomTVMod.removeKey(LivingWithTheAnimals);

            if (SDate.Now().DayOfWeek == DayOfWeek.Tuesday || SDate.Now().DayOfWeek == DayOfWeek.Saturday)
            {
                if (this._episodes.ContainsKey(GetShowNumber()))
                {
                    string name = DataLoader.i18n.Get("TV.LivingWithTheAnimals.ChannelDisplayName");
                    CustomTVMod.addChannel(LivingWithTheAnimals, name, ShowAnnouncement);
                }
            }
        }

        private static int GetShowNumber()
        {
            return (int)(Game1.stats.DaysPlayed % 224U / 3.5) + 1;
        }

        private void ShowAnnouncement(TV tv, TemporaryAnimatedSprite sprite, StardewValley.Farmer farmer, string answer)
        {
            showSprite = new TemporaryAnimatedSprite(DataLoader.LooseSprites, new Rectangle(0, 0, 42, 28), 150f, 2, 999999, tv.getScreenPosition(), false, false, (float)((double)(tv.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, tv.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f, false);
            CustomTVMod.showProgram(showSprite, DataLoader.i18n.Get("TV.LivingWithTheAnimals.Announcement"), ShowPresentation);
        }

        private void ShowPresentation()
        {
            string text = DataLoader.i18n.Get(_episodes[GetShowNumber()].Text);
            CustomTVMod.showProgram(showSprite, text, CustomTVMod.endProgram);
        }
    }

    public class Episode
    {
        public String Text { get; }
        public bool AboutMeat { get; }
        public bool AboutPregnancy { get; }
        public bool AboutTreats { get; }

        public Episode(string text, bool aboutMeat, bool aboutPregnancy, bool aboutTreats)
        {
            Text = text;
            AboutMeat = aboutMeat;
            AboutPregnancy = aboutPregnancy;
            AboutTreats = aboutTreats;
        }
    }
}
