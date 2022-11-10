using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horseshoe.NET.Text.TextClean
{
    public static class TextClean
    {
        public static string Remove(string text, params char[] chars)
        {
            if (text == null)
                return string.Empty;
            if (chars == null || !chars.Any())
                return text;
            var strb = new StringBuilder(text.Length);
            ReadOnlySpan<char> roSpan = text.AsSpan();
            foreach (var c in roSpan)
            {
                if (!chars.Contains(c))
                {
                    strb.Append(c);
                }
            }
            return strb.ToString();
        }

        public static string RemoveWhitespace(string text, bool preserveNewlines = false)
        {
            return Remove(text, preserveNewlines ? CharLib.AllWhitespacesExceptNewLines : CharLib.AllWhitespaces);
        }

        public static string RemoveNewlines(string text)
        {
            return Remove(text, CharLib.SubsetNewLines);
        }

        public static string CombineWhitespace(string text, bool preserveNewlines = false)
        {
            if (text == null)
                return string.Empty;
            var lastCharWasWhiteSpace = false;
            var wsChars = preserveNewlines ? CharLib.AllWhitespacesExceptNewLines : CharLib.AllWhitespaces;
            var strb = new StringBuilder(text.Length);
            ReadOnlySpan<char> roSpan = text.AsSpan();
            foreach (var c in roSpan)
            {
                if (wsChars.Contains(c))
                {
                    if (!lastCharWasWhiteSpace)
                    {
                        strb.Append(" ");
                    }
                    lastCharWasWhiteSpace = true;
                }
                else
                {
                    strb.Append(c);
                    lastCharWasWhiteSpace = false;
                }
            }
            return strb.ToString();
        }

        /// <summary>
        /// Convert essentially any text to its closest ASCII representation
        /// </summary>
        /// <param name="text">The source text to convert</param>
        /// <param name="replacementLog">Optional.  Collection to which replacement log entried can be written. <c>IDictionary&lt;int,string&gt; contains char index and replacement notes</c></param>
        /// <param name="whitespacePolicy">Whitespace handling instructions</param>
        /// <param name="nonprintablesPolicy">Nonprintables display instructions</param>
        /// <param name="substitute">How to display non-printables (if <c>NonprintablesPolicy.Substitute</c>) and unknown chars</param>
        /// <param name="extendedASCII"><c>true</c> to allow extended ASCII chars in the output</param>
        /// <returns>An ASCII string</returns>
        public static string ToASCIIPrintable(string text, IDictionary<int, string> replacementLog = null, WhitespacePolicy whitespacePolicy = default, NonprintablesPolicy nonprintablesPolicy = default, string substitute = "?", bool extendedASCII = false)
        {
            replacementLog = replacementLog ?? new Dictionary<int, string>();

            if (text == null)
            {
                replacementLog.Add(-1, "[null]");
                return string.Empty;
            }

            var strb = new StringBuilder((int)(text.Length * 1.1));
            ReadOnlySpan<char> roSpan = text.AsSpan();
            var rOptions01 = new RevealOptions { RevealNewLines = true };

            // whitespace policies in order of precedence
            bool combiningSpaces = false;
            bool dropSpaces = (whitespacePolicy & WhitespacePolicy.DropSpaces) == WhitespacePolicy.DropSpaces;
            bool dropTabs = (whitespacePolicy & WhitespacePolicy.DropTabs) == WhitespacePolicy.DropTabs;
            bool dropNewLines = (whitespacePolicy & WhitespacePolicy.DropNewLines) == WhitespacePolicy.DropNewLines;
            bool dropNonBreakingSpaces = (whitespacePolicy & WhitespacePolicy.DropNonBreakingSpaces) == WhitespacePolicy.DropNonBreakingSpaces;
            bool allowSpaces = (whitespacePolicy & WhitespacePolicy.AllowSpaces) == WhitespacePolicy.AllowSpaces;
            bool allowTabs = (whitespacePolicy & WhitespacePolicy.AllowTabs) == WhitespacePolicy.AllowTabs;
            bool allowNewLines = (whitespacePolicy & WhitespacePolicy.AllowNewLines) == WhitespacePolicy.AllowNewLines;
            bool allowNonBreakingSpaces = (whitespacePolicy & WhitespacePolicy.AllowNonBreakingSpaces) == WhitespacePolicy.AllowNonBreakingSpaces;
            bool convertTabs = (whitespacePolicy & WhitespacePolicy.ConvertTabsToSpaces) == WhitespacePolicy.ConvertTabsToSpaces;
            bool convertNewLines = (whitespacePolicy & WhitespacePolicy.ConvertNewLinesToSpaces) == WhitespacePolicy.ConvertNewLinesToSpaces;
            bool convertNonBreakingSpaces = (whitespacePolicy & WhitespacePolicy.ConvertNonBreakingSpacesToSpaces) == WhitespacePolicy.ConvertNonBreakingSpacesToSpaces;
            bool drop = (whitespacePolicy & WhitespacePolicy.Drop) == WhitespacePolicy.Drop;
            bool allow = (whitespacePolicy & WhitespacePolicy.Allow) == WhitespacePolicy.Allow;
            bool combine = (whitespacePolicy & WhitespacePolicy.Combine) == WhitespacePolicy.Combine;
            bool combineExceptNewLines = (whitespacePolicy & WhitespacePolicy.CombineExceptNewLines) == WhitespacePolicy.CombineExceptNewLines;
            bool convert = (whitespacePolicy & WhitespacePolicy.ConvertToSpaces) == WhitespacePolicy.ConvertToSpaces;

            for (int i = 0; i < roSpan.Length; i++)
            {
                char c = roSpan[i];
                if (TextUtil.IsWhitespace(c, extendedASCII: true))
                {
                    var whitespace = _ToASCIIPrintable_Whitespace(c, i, replacementLog, rOptions01, ref combiningSpaces, dropSpaces, dropTabs, dropNewLines, dropNonBreakingSpaces, allowSpaces, allowTabs, allowNewLines, allowNonBreakingSpaces, convertTabs, convertNewLines, convertNonBreakingSpaces: convertNonBreakingSpaces || !extendedASCII, drop, allow, combine, combineExceptNewLines, convert);
                    if (whitespace.HasValue)
                    {
                        strb.Append(whitespace);
                    }
                    if (combiningSpaces)
                        continue;
                }
                else if (TextUtil.IsASCIIPrintable(c, extendedASCII: extendedASCII))  // excluding whitespaces
                {
                    strb.Append(c);
                    replacementLog.Add(i, (extendedASCII ? "extended " : "") + "ascii - detected " + TextUtil._Reveal(c, RevealOptions.All));
                }
                else if (TextUtil.IsASCIIControl(c, extendedASCII: extendedASCII))  // excluding whitespaces
                {
                    replacementLog.Add(i, "controls - dropped " + TextUtil._Reveal(c, new RevealOptions { RevealControlChars = true }));
                }
                else if (CharLib.OtherNonprintables.Contains(c))
                {
                    switch (nonprintablesPolicy)
                    {
                        case NonprintablesPolicy.Drop:
                        default:
                            replacementLog.Add(i, "nonprintables - dropped " + TextUtil._Reveal(c, new RevealOptions { RevealAll = true }));
                            break;
                        case NonprintablesPolicy.Substitute:
                            strb.Append(substitute);
                            replacementLog.Add(i, "nonprintables - " + TextUtil._Reveal(c, new RevealOptions { RevealAll = true }) + " > " + substitute);
                            break;
                    }
                }
                else if (_TryFindASCIIReplacement(c, out string replacement, out string source, extendedASCII: extendedASCII))
                {
                    strb.Append(replacement);
                    replacementLog.Add(i, source + " - replaced " + TextUtil._Reveal(c, RevealOptions.All) + " > " + TextUtil.Reveal(replacement, RevealOptions.All));
                }
                else
                {
                    strb.Append(substitute);
                    replacementLog.Add(i, "[no-replacement-found] " + TextUtil._Reveal(c, RevealOptions.All) + " > " + substitute);
                }
                combiningSpaces = false;
            }
            if (!replacementLog.Any())
            {
                replacementLog.Add(-1, "[clean]");
            }
            return strb.ToString();
        }

        private static char? _ToASCIIPrintable_Whitespace(char c, int i, IDictionary<int, string> replacementLog, RevealOptions rOptions01, 
            ref bool combiningSpaces,
            bool dropSpaces,
            bool dropTabs,
            bool dropNewLines,
            bool dropNonBreakingSpaces,
            bool allowSpaces,
            bool allowTabs,
            bool allowNewLines,
            bool allowNonBreakingSpaces,
            bool convertTabs,
            bool convertNewLines,
            bool convertNonBreakingSpaces,
            bool drop,
            bool allow,
            bool combine,
            bool combineExceptNewLines,
            bool convert
        )
        {
            if (c == ' ')
            {
                if (dropSpaces)
                {
                    replacementLog.Add(i, "whitespace - dropped " + rOptions01.ValueIfSpace);
                }
                else if (allowSpaces && !combine)
                {
                    replacementLog.Add(i, "whitespace - detected " + rOptions01.ValueIfSpace);
                    return c;
                }
                else if (drop)
                {
                    replacementLog.Add(i, "whitespace - dropped " + rOptions01.ValueIfSpace);
                }
                else if (allow && !combine)
                {
                    replacementLog.Add(i, "whitespace - detected " + rOptions01.ValueIfSpace);
                    return c;
                }
                else if (combine && combiningSpaces)
                {
                    replacementLog.Add(i, "whitespace - combined " + rOptions01.ValueIfSpace);
                }
                else if (combine)
                {
                    replacementLog.Add(i, "whitespace - combining " + rOptions01.ValueIfSpace);
                    combiningSpaces = true;
                    return c;
                }
                else
                {
                    replacementLog.Add(i, "whitespace - defaulted " + rOptions01.ValueIfSpace);
                    return c;
                }
            }
            else if (c == '\t')
            {
                if (dropTabs)
                {
                    replacementLog.Add(i, "whitespace - dropped " + rOptions01.ValueIfTab);
                }
                else if (allowTabs)
                {
                    replacementLog.Add(i, "whitespace - detected " + rOptions01.ValueIfTab);
                    return c;
                }
                else if (convertTabs && !combine)
                {
                    replacementLog.Add(i, "whitespace - converted " + rOptions01.ValueIfTab + " to " + rOptions01.ValueIfSpace);
                    return ' ';
                }
                else if (drop)
                {
                    replacementLog.Add(i, "whitespace - dropped " + rOptions01.ValueIfTab);
                }
                else if (allow)
                {
                    replacementLog.Add(i, "whitespace - detected " + rOptions01.ValueIfTab);
                    return c;
                }
                else if (combine && combiningSpaces)
                {
                    replacementLog.Add(i, "whitespace - combined " + rOptions01.ValueIfTab + " to " + rOptions01.ValueIfSpace);
                }
                else if (combine)
                {
                    replacementLog.Add(i, "whitespace - combining " + rOptions01.ValueIfTab + " to " + rOptions01.ValueIfSpace);
                    combiningSpaces = true;
                    return ' ';
                }
                else if (convert)
                {
                    replacementLog.Add(i, "whitespace - converted " + rOptions01.ValueIfTab + " to " + rOptions01.ValueIfSpace);
                    return ' ';
                }
                else
                {
                    replacementLog.Add(i, "whitespace - defaulted " + rOptions01.ValueIfTab);
                    return c;
                }
            }
            else if (c.IsNewLine())
            {
                if (dropNewLines)
                {
                    replacementLog.Add(i, "whitespace - dropped " + TextUtil._Reveal(c, rOptions01));
                }
                else if (allowNewLines)
                {
                    replacementLog.Add(i, "whitespace - detected " + TextUtil._Reveal(c, rOptions01));
                    return c;
                }
                else if (convertNewLines && (!combine || combineExceptNewLines))
                {
                    replacementLog.Add(i, "whitespace - converted " + TextUtil._Reveal(c, rOptions01) + " to " + rOptions01.ValueIfSpace);
                    return ' ';
                }
                else if (drop)
                {
                    replacementLog.Add(i, "whitespace - dropped " + TextUtil._Reveal(c, rOptions01));
                }
                else if (allow)
                {
                    replacementLog.Add(i, "whitespace - detected " + TextUtil._Reveal(c, rOptions01));
                    return c;
                }
                else if (combine && !combineExceptNewLines && combiningSpaces)
                {
                    replacementLog.Add(i, "whitespace - combined " + TextUtil._Reveal(c, rOptions01) + " to " + rOptions01.ValueIfSpace);
                }
                else if (combine && !combineExceptNewLines)
                {
                    replacementLog.Add(i, "whitespace - combining " + TextUtil._Reveal(c, rOptions01) + " to " + rOptions01.ValueIfSpace);
                    combiningSpaces = true;
                    return ' ';
                }
                else if (convert)
                {
                    replacementLog.Add(i, "whitespace - converted " + TextUtil._Reveal(c, rOptions01) + " to " + rOptions01.ValueIfSpace);
                    return ' ';
                }
                else
                {
                    replacementLog.Add(i, "whitespace - defaulted " + TextUtil._Reveal(c, rOptions01));
                    return c;
                }
            }
            else if (c == '\u00A0')  // 160 - nbsp
            {
                if (dropNonBreakingSpaces)
                {
                    replacementLog.Add(i, "whitespace - dropped " + rOptions01.ValueIfNbSpace);
                    combiningSpaces = false;
                }
                else if (allowNonBreakingSpaces)
                {
                    replacementLog.Add(i, "whitespace - detected " + rOptions01.ValueIfNbSpace);
                    return c;
                }
                else if (convertNonBreakingSpaces && !combine)
                {
                    replacementLog.Add(i, "whitespace - converted " + rOptions01.ValueIfNbSpace + " to " + rOptions01.ValueIfSpace);
                    return ' ';
                }
                else if (drop)
                {
                    replacementLog.Add(i, "whitespace - dropped " + rOptions01.ValueIfNbSpace);
                    combiningSpaces = false;
                }
                else if (allow)
                {
                    replacementLog.Add(i, "whitespace - detected " + rOptions01.ValueIfNbSpace);
                    return c;
                }
                else if (combine && combiningSpaces)
                {
                    replacementLog.Add(i, "whitespace - combined " + rOptions01.ValueIfNbSpace + " to " + rOptions01.ValueIfSpace);
                }
                else if (combine)
                {
                    replacementLog.Add(i, "whitespace - combining " + rOptions01.ValueIfNbSpace + " to " + rOptions01.ValueIfSpace);
                    combiningSpaces = true;
                    return ' ';
                }
                else if (convert)
                {
                    replacementLog.Add(i, "whitespace - converted " + rOptions01.ValueIfNbSpace + " to " + rOptions01.ValueIfSpace);
                    return ' ';
                }
                else
                {
                    replacementLog.Add(i, "whitespace - defaulted " + rOptions01.ValueIfNbSpace);
                    return c;
                }
            }
            else
            {
                replacementLog.Add(i, "ERROR: whitespace - missing newline definition: " + TextUtil._Reveal(c, RevealOptions.All) + " - this should never happen");
            }
            return null;
        }

        ///// <summary>
        ///// Returns '?' for known control chars and search <c>CharLib</c> for an ASCII replacement for almost every other character (whitespaces are ignored)
        ///// </summary>
        ///// <param name="c">a char</param>
        ///// <param name="replacement">a char replacement string</param>
        ///// <param name="source">category of the replaced char</param>
        ///// <param name="extendedASCII">whether to prevent replacing extended ASCII chars</param>
        ///// <returns><c>true</c> if a replacement was found in <c>CharLib</c></returns>
        //public static bool TryFindPrintableASCIIReplacement(char c, out string replacement, out string source, bool extendedASCII = false)
        //{
        //    if (TextUtil.IsWhitespace(c, extendedASCII: true))
        //    {
        //        if (!extendedASCII && c == '\u00A0' /* nbsp (160) */)
        //        {
        //            replacement = " ";
        //            source = "whitespace: extended ASCII non-breaking space";
        //            return true;
        //        }
        //        replacement = null;
        //        source = "[no-replace] \"whitespace\"";
        //        return false;
        //    }
        //    if (TextUtil.IsASCIIPrintable(c, extendedASCII: extendedASCII))  // note: not considering whitespaces as printable for this step, for whitespaces see above
        //    {
        //        replacement = null;
        //        source = "[no-replace] \"ASCII printables\"";
        //        return false;
        //    }
        //    if (TextUtil.IsASCIIControl(c, extendedASCII: true))  // note: not considering whitespaces as controls for this step, for whitespaces see above
        //    {
        //        replacement = "?";
        //        source = "ASCII controls";
        //        return true;
        //    }
        //    if (extendedASCII)
        //    {
        //        if (CharLib.AllWhitespaces)
        //            return _TryFindASCIIReplacement(c, out replacement, out source, extendedASCII);
        //    }
        //}

        private static IDictionary<string, IDictionary<char, char[]>> UnicodeToASCIIConversions { get; } = CharLib.AllUnicodeToASCIIConversions;

        private static IDictionary<string, IDictionary<string, char[]>> UnicodeToASCIIComplexConversions { get; } = CharLib.AllUnicodeToASCIIComplexConversions;

        private static bool _TryFindASCIIReplacement(char c, out string replacement, out string source, bool extendedASCII)
        {
            replacement = "none";
            source = "none";

            if (extendedASCII)
            {
                // preclude replacing extended ASCII printable chars, if applicable
                if (CharLib.ExtendedASCIIAlpha.Contains(c) || CharLib.ExtendedASCIISymbols.Contains(c))
                {
                    replacement = null;
                    source = "extended ASCII";
                    return false;
                }
            }

            // iterate the conversion tables (see CharLib)
            foreach (var dictKey in UnicodeToASCIIConversions.Keys)
            {
                foreach (var convKvp in UnicodeToASCIIConversions[dictKey])
                {
                    if (convKvp.Value.Contains(c))
                    {
                        replacement = string.Concat(convKvp.Key);
                        source = dictKey;
                        return true;
                    }
                }
            }

            // iterate the complex conversion tables (see CharLib)
            foreach (var dictKey in UnicodeToASCIIComplexConversions.Keys)
            {
                foreach (var convKvp in UnicodeToASCIIComplexConversions[dictKey])
                {
                    if (convKvp.Value.Contains(c))
                    {
                        replacement = convKvp.Key;
                        source = dictKey;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
