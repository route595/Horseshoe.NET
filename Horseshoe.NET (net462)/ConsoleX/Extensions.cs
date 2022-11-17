using System;
using System.Collections.Generic;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A handful of extension methods used by <c>ConsoleX</c> and may be used by client code as well
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Whether a key stroke indicates and ascii letter
        /// </summary>
        /// <param name="info">a key stroke</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsLetter(this ConsoleKeyInfo info)
        {
            return char.IsLetter(info.KeyChar);
        }

        /// <summary>
        /// Whether a key stroke indicates a number
        /// </summary>
        /// <param name="info">a key stroke</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsNumber(this ConsoleKeyInfo info)
        {
            return char.IsNumber(info.KeyChar);
        }

        /// <summary>
        /// Whether a key stroke indicates a letter or number
        /// </summary>
        /// <param name="info">a key stroke</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsLetterOrDigit(this ConsoleKeyInfo info)
        {
            return char.IsLetterOrDigit(info.KeyChar);
        }

        /// <summary>
        /// Whether a key stroke indicates a space
        /// </summary>
        /// <param name="info">a key stroke</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsSpace(this ConsoleKeyInfo info)
        {
            return info.KeyChar == ' ';
        }

        /// <summary>
        /// Whether a key stroke indicates a special character or punctuation
        /// </summary>
        /// <param name="info">a key stroke</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsSpecialCharacter(this ConsoleKeyInfo info)
        {
            if (IsPunctuation(info))
                return true;
            switch (info.KeyChar)
            {
                case '`':
                case '~':
                case '@':
                case '#':
                case '$':
                case '%':
                case '^':
                case '&':
                case '*':
                case '_':
                case '=':
                case '+':
                case '\\':
                case '|':
                case '<':
                case '>':
                case '/':
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Whether a key stroke indicates punctuation
        /// </summary>
        /// <param name="info">a key stroke</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsPunctuation(this ConsoleKeyInfo info)
        {
            switch (info.KeyChar)
            {
                case '.':
                case '?':
                case '!':
                case ',':
                case ';':
                case ':':
                case '-':
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                case '\'':
                case '"':
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Whether a key stroke indicates keyboard navigation
        /// </summary>
        /// <param name="info">a key stroke</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsCursorNavigation(this ConsoleKeyInfo info)
        {
            switch (info.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.RightArrow:
                case ConsoleKey.DownArrow:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.End:
                case ConsoleKey.Home:
                case ConsoleKey.PageUp:
                case ConsoleKey.PageDown:
                case ConsoleKey.Tab:
                case ConsoleKey.Enter:
                case ConsoleKey.Backspace:
                case ConsoleKey.Delete:
                case ConsoleKey.Insert:
                    return true;
            }
            return false;
        }

        internal static bool ContainsKeyIgnoreCase(this Dictionary<string, string> dictionary, string searchKey, out KeyValuePair<string, string> matchingKvp)
        {
            var _matchingKvp = GetByKeyIgnoreCase(dictionary, searchKey);
            if (_matchingKvp.HasValue)
            {
                matchingKvp = _matchingKvp.Value;
                return true;
            }
            matchingKvp = default;
            return false;
        }

        internal static KeyValuePair<string, string>? GetByKeyIgnoreCase(this Dictionary<string, string> dictionary, string searchKey)
        {
            foreach (var kvp in dictionary)
            {
                if (string.Equals(kvp.Key, searchKey, StringComparison.CurrentCultureIgnoreCase))
                {
                    return kvp;
                }
            }
            return null;
        }
    }
}
