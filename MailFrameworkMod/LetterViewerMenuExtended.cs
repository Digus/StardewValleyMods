using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;

namespace MailFrameworkMod
{
    public class LetterViewerMenuExtended : LetterViewerMenu
    {
        public int? TextColor { get; set; }

        public LetterViewerMenuExtended(string text) : base(text)
        {
        }

        public LetterViewerMenuExtended(int secretNoteIndex) : base(secretNoteIndex)
        {
        }

        public LetterViewerMenuExtended(string mail, string mailTitle) : base(mail, mailTitle)
        {
        }

        public static bool GetTextColor(LetterViewerMenu __instance, ref int __result)
        {
            if (__instance is LetterViewerMenuExtended letterViewerMenuExtended && letterViewerMenuExtended.TextColor.HasValue)
            {
                __result = (int)letterViewerMenuExtended.TextColor;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
