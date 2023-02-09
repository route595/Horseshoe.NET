using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A collection of factory methods for <c>string</c> and <c>char</c> interpretation and <c>string</c> manipulation.
    /// </summary>
    public static class TextUtilAbstractions
    {
        /// <summary>
        /// Creates a <c>SecureString</c> instance from text.
        /// </summary>
        /// <param name="unsecureString">A text <c>string</c>.</param>
        /// <returns>A <c>SecureString</c>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SecureString ConvertToSecureString(string unsecureString)
        {
            if (unsecureString == null) throw new ArgumentNullException(nameof(unsecureString));

            var secureString = new SecureString();
            foreach (char c in unsecureString)
            {
                secureString.AppendChar(c);
            }
            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Restores a <c>string</c> from a <c>SecureString</c>.
        /// </summary>
        /// <param name="secureString">A <c>SecureString</c>.</param>
        /// <returns>A <c>string</c>.</returns>
        /// <remarks>
        /// ref: https://blogs.msdn.microsoft.com/fpintos/2009/06/12/how-to-properly-convert-securestring-to-string/
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ConvertToUnsecureString(SecureString secureString)
        {
            if (secureString == null) throw new ArgumentNullException(nameof(secureString));

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if <c>c</c> represents a printable ASCII <c>char</c>.
        /// </summary>
        /// <param name="c">A char.</param>
        /// <param name="excludeSpaces">If <c>true</c>, spaces do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeTabs">If <c>true</c>, tabs do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeNewlines">If <c>true</c>, newlines do not count as printable. Default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsAsciiPrintable(char c, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            if (c == 32)
                return !excludeSpaces;
            if (c == 9)
                return !excludeTabs;
            if (c == 10 || c == 13)
                return !excludeNewlines;
            return c <= 127 && !char.IsControl(c);
        }

        /// <summary>
        /// Returns <c>true</c> if <c>text</c> contains only printable ASCII <c>char</c>s.
        /// </summary>
        /// <param name="text">A text string.</param>
        /// <param name="excludeSpaces">If <c>true</c>, spaces do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeTabs">If <c>true</c>, tabs do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeNewlines">If <c>true</c>, newlines do not count as printable. Default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsAsciiPrintable(string text, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            return text.All(c => IsAsciiPrintable(c, excludeSpaces: excludeSpaces, excludeTabs: excludeTabs, excludeNewlines: excludeNewlines));
        }

        /// <summary>
        /// Returns <c>true</c> if <c>c</c> represents a printable <c>char</c>.
        /// </summary>
        /// <param name="c">a char</param>
        /// <param name="excludeSpaces">If <c>true</c>, spaces do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeTabs">If <c>true</c>, tabs do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeNewlines">If <c>true</c>, newlines do not count as printable. Default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsPrintable(char c, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            if (c <= 127)
                return IsAsciiPrintable(c, excludeSpaces: excludeSpaces, excludeTabs: excludeTabs, excludeNewlines: excludeNewlines);
            if (c <= 159)
                return false;
            if (c == 160)
                return !excludeSpaces;
            /* 
             * ByteOrderMark          '\uFEFF'   (65279)    
             * UnicodeReplacementChar '\uFFFD' � (65533) 
             * ------------------------------------
             * Source: CharLib.cs > UnicodeNonprintables
             */
            if (c == '\uFEFF' || c == '\uFFFD')
                return false;
            return true;
        }

        /// <summary>
        /// Returns <c>true</c> if <c>text</c> contains only printable <c>char</c>s.
        /// </summary>
        /// <param name="text">A text string.</param>
        /// <param name="excludeSpaces">If <c>true</c>, spaces do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeTabs">If <c>true</c>, tabs do not count as printable. Default is <c>false</c>.</param>
        /// <param name="excludeNewlines">If <c>true</c>, newlines do not count as printable. Default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsPrintable(string text, bool excludeSpaces = false, bool excludeTabs = false, bool excludeNewlines = false)
        {
            return text.All(c => IsPrintable(c, excludeSpaces: excludeSpaces, excludeTabs: excludeTabs, excludeNewlines: excludeNewlines));
        }

        /// <summary>
        /// Inspired by SQL, determines if a string is found in a collection of strings (case-sensitive).
        /// </summary>
        /// <param name="text">The <c>string</c> to search match.</param>
        /// <param name="strings">A <c>string</c> collection to search.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In(string text, params string[] strings)
        {
            return In(text, false, strings);
        }

        /// <summary>
        /// Inspired by SQL, determines if a string is found in a collection of strings. (Case-sensitivity dictated by <c>ingoreCase</c>.)
        /// </summary>
        /// <param name="text">The <c>string</c> to search match.</param>
        /// <param name="ignoreCase">If <c>true</c>, causes the comparison/search to be case-insensitive, default is <c>false</c>.</param>
        /// <param name="strings">A <c>string</c> collection to search.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In(string text, bool ignoreCase, params string[] strings)
        {
            if (strings == null) 
                return false;
            foreach (var s in strings)
            {
                if (ignoreCase) {
                    if (string.Equals(s, text, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                else if (string.Equals(s, text))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
