using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailFrameworkMod
{
    public class MailDao
    {
        private static readonly List<Letter> Letters =  new List<Letter>();

        /// <summary>
        /// Saves a letter on the repository.
        /// </summary>
        /// <param name="letter"> The letter to be saved.</param>
        public static void SaveLetter(Letter letter)
        {
            Letters.Add(letter);
        }

        /// <summary>
        /// Removes the letter from the repository.
        /// Comparison done by id.
        /// </summary>
        /// <param name="letter">The letter to be removed.</param>
        public static void RemoveLetter(Letter letter)
        {
            Letters.Remove(Letters.Find((l) => l.Id == letter.Id));
        }

        /// <summary>
        /// Validates the condition to show the letters and returns a list with all that matches.
        /// </summary>
        /// <returns>The list with all letter that matched their conditions</returns>
        public static List<Letter> GetValidatedLetters()
        {
            return Letters.FindAll((l) => l.Condition());
        }
    }
}
