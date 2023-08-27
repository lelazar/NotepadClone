using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotepadClone
{
    public static class TextAnalyzer
    {
        public static int CountWords(string text)
        {
            string[] words = text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

        public static int CountCharacters(string text, bool includeSpaces)
        {
            return includeSpaces ? text.Length : text.Replace(" ", "").Length;
        }

        public static int CountSentences(string text)
        {
            // Split on period, question mark, and exclamation mark.
            // More complex split would be needed for real-world use.
            return text.Split(new char[] { '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}
