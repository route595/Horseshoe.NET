using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A collection of extension methods for <c>string</c> and <c>char</c> interpretation and <c>string</c> manipulation.
    /// </summary>
    public static class Extensions
    {
        /// <inheritdoc cref="TextUtil.Fill" />
        public static string Fill(this string text, int targetLength, bool allowOverflow = false)
        {
            return TextUtil.Fill(text, targetLength, allowOverflow: allowOverflow);
        }

        /// <inheritdoc cref="TextUtil.Fit" />
        public static string Fit(this string text, int targetLength, HorizontalPosition direction = HorizontalPosition.Left, string padding = null, string leftPadding = null, HorizontalPosition? truncateDirection = null, string truncateMarker = null)
        {
            return TextUtil.Fit(text, targetLength, direction: direction, padding: padding, leftPadding: leftPadding, truncateDirection: truncateDirection, truncateMarker: truncateMarker);
        }

        /// <inheritdoc cref="TextUtil.Fit" />
        public static string FitCenter(this string text, int targetLength, string padding = null, string leftPadding = null, HorizontalPosition? truncateDirection = null, string truncateMarker = null)
        {
            return TextUtil.Fit(text, targetLength, direction: HorizontalPosition.Center, padding: padding, leftPadding: leftPadding, truncateDirection: truncateDirection, truncateMarker: truncateMarker);
        }

        /// <inheritdoc cref="TextUtil.Fit" />
        public static string FitLeft(this string text, int targetLength, string padding = null, string leftPadding = null, HorizontalPosition? truncateDirection = null, string truncateMarker = null)
        {
            return TextUtil.Fit(text, targetLength, direction: HorizontalPosition.Left, padding: padding, leftPadding: leftPadding, truncateDirection: truncateDirection, truncateMarker: truncateMarker);
        }

        /// <inheritdoc cref="TextUtil.Fit" />
        public static string FitRight(this string text, int targetLength, string padding = null, string leftPadding = null, HorizontalPosition? truncateDirection = null, string truncateMarker = null)
        {
            return TextUtil.Fit(text, targetLength, direction: HorizontalPosition.Right, padding: padding, leftPadding: leftPadding, truncateDirection: truncateDirection, truncateMarker: truncateMarker);
        }

        /// <inheritdoc cref="TextUtil.HasWhitespace" />
        public static bool HasWhitespace(this string text)
        {
            return TextUtil.HasWhitespace(text);
        }

        /// <inheritdoc cref="TextUtil.MultilineTrim" />
        public static string MultilineTrim(this string text)
        {
            return TextUtil.MultilineTrim(text);
        }

        /// <inheritdoc cref="TextUtil.Pad" />
        public static string Pad(this string text, int targetLength, HorizontalPosition direction = HorizontalPosition.Right, string padding = null, string leftPadding = null, bool cannotExceedTargetLength = false)
        {
            return TextUtil.Pad(text, targetLength, direction: direction, padding: padding, leftPadding: leftPadding, cannotExceedTargetLength: cannotExceedTargetLength);
        }

        /// <inheritdoc cref="TextUtil.Pad" />
        public static string PadCenter(this string text, int targetLength, string padding = null, string leftPadding = null, bool cannotExceedTargetLength = false)
        {
            return TextUtil.Pad(text, targetLength, direction: HorizontalPosition.Center, padding: padding, leftPadding: leftPadding, cannotExceedTargetLength: cannotExceedTargetLength);
        }

        /// <inheritdoc cref="TextUtil.Pad" />
        public static string PadLeft(this string text, int targetLength, string padding = null, bool cannotExceedTargetLength = false)
        {
            return TextUtil.Pad(text, targetLength, direction: HorizontalPosition.Left, padding: padding, cannotExceedTargetLength: cannotExceedTargetLength);
        }

        /// <inheritdoc cref="TextUtil.Pad" />
        public static string PadRight(this string text, int targetLength, string padding = null, bool cannotExceedTargetLength = false)
        {
            return TextUtil.Pad(text, targetLength, direction: HorizontalPosition.Right, padding: padding, cannotExceedTargetLength: cannotExceedTargetLength);
        }

        /// <inheritdoc cref="TextUtil.Repeat" />
        public static string Repeat(this string text, int numberOfTimes)
        {
            return TextUtil.Repeat(text, numberOfTimes);
        }

        /// <inheritdoc cref="TextUtil.Crop" />
        public static string Crop(this string text, int targetLength, HorizontalPosition direction = HorizontalPosition.Right, string truncateMarker = null)
        {
            return TextUtil.Crop(text, targetLength, direction: direction, truncateMarker: truncateMarker);
        }

        /// <inheritdoc cref="TextUtil.Crop" />
        public static string CropCenter(this string text, int targetLength, string truncateMarker = null)
        {
            return TextUtil.Crop(text, targetLength, direction: HorizontalPosition.Center, truncateMarker: truncateMarker);
        }

        /// <inheritdoc cref="TextUtil.Crop" />
        public static string CropLeft(this string text, int targetLength, string truncateMarker = null)
        {
            return TextUtil.Crop(text, targetLength, direction: HorizontalPosition.Left, truncateMarker: truncateMarker);
        }

        /// <inheritdoc cref="TextUtil.Crop" />
        public static string CropRight(this string text, int targetLength, string truncateMarker = null)
        {
            return TextUtil.Crop(text, targetLength, direction: HorizontalPosition.Right, truncateMarker: truncateMarker);
        }

        /// <summary>
        /// Contitionally appends a reference type (class) value to a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is appended.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="sb">A <c>StringBuilder</c></param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="valueIfTrue">The value to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append only if supplied and if <c>condition == false</c>.</param>
        /// <param name="functionIfFalse">An optional function whose result to append only if supplied and if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendIf<T>(this StringBuilder sb, bool condition, T valueIfTrue, T valueIfFalse = null, Func<T> functionIfFalse = null) where T : class
        {
            if (condition)
                sb.Append(valueIfTrue);
            else if (valueIfFalse != null)
                sb.Append(valueIfFalse);
            else if (functionIfFalse != null)
                sb.Append(functionIfFalse.Invoke());
            return sb;
        }

        /// <summary>
        /// Contitionally appends a reference type (class) value to a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is appended.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="sb">A <c>StringBuilder</c></param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="functionIfTrue">The function whose result to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append only if supplied and if <c>condition == false</c>.</param>
        /// <param name="functionIfFalse">An optional function whose result to append only if supplied and if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendIf<T>(this StringBuilder sb, bool condition, Func<T> functionIfTrue, T valueIfFalse = null, Func<T> functionIfFalse = null) where T : class
        {
            if (condition)
                sb.Append(functionIfTrue.Invoke());
            else if (valueIfFalse != null)
                sb.Append(valueIfFalse);
            else if (functionIfFalse != null)
                sb.Append(functionIfFalse.Invoke());
            return sb;
        }

        /// <summary>
        /// Contitionally appends a value type (struct) value to a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is appended.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="sb">A <c>StringBuilder</c></param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="valueIfTrue">The value to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append only if supplied and if <c>condition == false</c>.</param>
        /// <param name="functionIfFalse">An optional function whose result to append only if supplied and if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendIf<T>(this StringBuilder sb, bool condition, T valueIfTrue, T? valueIfFalse = null, Func<T> functionIfFalse = null) where T : struct
        {
            if (condition)
                sb.Append(valueIfTrue);
            else if (valueIfFalse != null)
                sb.Append(valueIfFalse);
            else if (functionIfFalse != null)
                sb.Append(functionIfFalse.Invoke());
            return sb;
        }

        /// <summary>
        /// Contitionally appends a value type (struct) value to a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is appended.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="sb">A <c>StringBuilder</c></param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="functionIfTrue">The function whose result to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append only if supplied and if <c>condition == false</c>.</param>
        /// <param name="functionIfFalse">An optional function whose result to append only if supplied and if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendIf<T>(this StringBuilder sb, bool condition, Func<T> functionIfTrue, T? valueIfFalse = null, Func<T> functionIfFalse = null) where T : struct
        {
            if (condition)
                sb.Append(functionIfTrue.Invoke());
            else if (valueIfFalse != null)
                sb.Append(valueIfFalse);
            else if (functionIfFalse != null)
                sb.Append(functionIfFalse.Invoke());
            return sb;
        }
        /// <summary>
        /// Contitionally appends a reference type (class) value to a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is appended.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="sb">A <c>StringBuilder</c></param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="valueIfTrue">The value to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append only if supplied and if <c>condition == false</c>.</param>
        /// <param name="functionIfFalse">An optional function whose result to append only if supplied and if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf<T>(this StringBuilder sb, bool condition, T valueIfTrue, T valueIfFalse = null, Func<T> functionIfFalse = null) where T : class
        {
            if (condition)
                sb.AppendLine(valueIfTrue?.ToString() ?? "");
            else if (valueIfFalse != null)
                sb.AppendLine(valueIfFalse?.ToString() ?? "");
            else if (functionIfFalse != null)
                sb.AppendLine(functionIfFalse.Invoke()?.ToString() ?? "");
            return sb;
        }

        /// <summary>
        /// Contitionally appends a reference type (class) value to a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is appended.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="sb">A <c>StringBuilder</c></param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="functionIfTrue">The function whose result to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append only if supplied and if <c>condition == false</c>.</param>
        /// <param name="functionIfFalse">An optional function whose result to append only if supplied and if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf<T>(this StringBuilder sb, bool condition, Func<T> functionIfTrue, T valueIfFalse = null, Func<T> functionIfFalse = null) where T : class
        {
            if (condition)
                sb.AppendLine(functionIfTrue.Invoke()?.ToString() ?? "");
            else if (valueIfFalse != null)
                sb.AppendLine(valueIfFalse?.ToString() ?? "");
            else if (functionIfFalse != null)
                sb.AppendLine(functionIfFalse.Invoke()?.ToString() ?? "");
            return sb;
        }

        /// <summary>
        /// Contitionally appends a value type (struct) value to a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is appended.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="sb">A <c>StringBuilder</c></param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="valueIfTrue">The value to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append only if supplied and if <c>condition == false</c>.</param>
        /// <param name="functionIfFalse">An optional function whose result to append only if supplied and if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf<T>(this StringBuilder sb, bool condition, T valueIfTrue, T? valueIfFalse = null, Func<T> functionIfFalse = null) where T : struct
        {
            if (condition)
                sb.AppendLine(valueIfTrue.ToString());
            else if (valueIfFalse != null)
                sb.AppendLine(valueIfFalse.ToString());
            else if (functionIfFalse != null)
                sb.AppendLine(functionIfFalse.Invoke().ToString());
            return sb;
        }

        /// <summary>
        /// Contitionally appends a value type (struct) value to a <c>StringBuilder</c>.  If <c>condition == false</c> then nothing is appended 
        /// unless <c>valueIfFalse</c> has a value, then that is appended.
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="sb">A <c>StringBuilder</c></param>
        /// <param name="condition"><c>true</c> or <c>false</c></param>
        /// <param name="functionIfTrue">The function whose result to append if <c>condition == true</c>.</param>
        /// <param name="valueIfFalse">An optional value to append only if supplied and if <c>condition == false</c>.</param>
        /// <param name="functionIfFalse">An optional function whose result to append only if supplied and if <c>condition == false</c>.</param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf<T>(this StringBuilder sb, bool condition, Func<T> functionIfTrue, T? valueIfFalse = null, Func<T> functionIfFalse = null) where T : struct
        {
            if (condition)
                sb.AppendLine(functionIfTrue.Invoke().ToString());
            else if (valueIfFalse != null)
                sb.AppendLine(valueIfFalse.ToString());
            else if (functionIfFalse != null)
                sb.AppendLine(functionIfFalse.Invoke().ToString());
            return sb;
        }

        ///// <summary>
        ///// Appends a <c>string</c> to a <c>StringBuilder</c> if <c>criteria == true</c>.  If <c>false</c> then nothing is appended 
        ///// unless <c>valueIfFalse</c> has a value, then that is appended.
        ///// </summary>
        ///// <param name="sb">.</param>
        ///// <param name="criteria"><c>true</c> or <c>false</c></param>
        ///// <param name="valueIfTrue"></param>
        ///// <param name="valueIfFalse"></param>
        ///// <returns>The <c>StringBuilder</c>.</returns>
        //public static StringBuilder AppendIf(this StringBuilder sb, bool criteria, string valueIfTrue, string valueIfFalse = null)
        //{
        //    if (criteria)
        //    {
        //        return sb.Append(valueIfTrue);
        //    }
        //    else if(valueIfFalse != null)
        //    {
        //        return sb.Append(valueIfFalse);
        //    }
        //    return sb;
        //}

        ///// <summary>
        ///// Appends an <c>object</c> to a <c>StringBuilder</c> if <c>criteria == true</c>.  If <c>false</c> then nothing is appended 
        ///// unless <c>valueIfFalse</c> has a value, then that is appended.
        ///// </summary>
        ///// <param name="sb">A <c>StringBuilder</c>.</param>
        ///// <param name="criteria"><c>true</c> or <c>false</c></param>
        ///// <param name="valueIfTrue">The value to append if <c>criteria == true</c>.</param>
        ///// <param name="valueIfFalse">An optional value to append if <c>criteria == false</c>. If <c>null</c> then nothing is appended.</param>
        ///// <returns>The <c>StringBuilder</c>.</returns>
        //public static StringBuilder AppendIf(this StringBuilder sb, bool criteria, object valueIfTrue, object valueIfFalse = null)
        //{
        //    if (criteria)
        //    {
        //        return sb.Append(valueIfTrue);
        //    }
        //    else if (valueIfFalse != null)
        //    {
        //        return sb.Append(valueIfFalse);
        //    }
        //    return sb;
        //}

        ///// <summary>
        ///// Appends a new line to a <c>StringBuilder</c> if <c>criteria == true</c>.
        ///// </summary>
        ///// <param name="sb">A <c>StringBuilder</c>.</param>
        ///// <param name="criteria"><c>true</c> or <c>false</c></param>
        ///// <returns>The <c>StringBuilder</c>.</returns>
        //public static StringBuilder AppendLineIf(this StringBuilder sb, bool criteria)
        //{
        //    if (criteria)
        //    {
        //        return sb.AppendLine();
        //    }
        //    return sb;
        //}

        ///// <summary>
        ///// Appends a new line of content to a <c>StringBuilder</c> if <c>criteria == true</c>.  If <c>false</c> then nothing is appended 
        ///// unless <c>valueIfFalse</c> has a value, then that is appended to a new line.
        ///// </summary>
        ///// <param name="sb">A <c>StringBuilder</c>.</param>
        ///// <param name="criteria"><c>true</c> or <c>false</c></param>
        ///// <param name="valueIfTrue">The value to append if <c>criteria == true</c>.</param>
        ///// <param name="valueIfFalse">An optional value to append if <c>criteria == false</c>. If <c>null</c> then nothing is appended.</param>
        ///// <returns></returns>
        //public static StringBuilder AppendLineIf(this StringBuilder sb, bool criteria, string valueIfTrue, string valueIfFalse = null)
        //{
        //    if (criteria)
        //    {
        //        return sb.AppendLine(valueIfTrue);
        //    }
        //    else if (valueIfFalse != null)
        //    {
        //        return sb.AppendLine(valueIfFalse);
        //    }
        //    return sb;
        //}


        /// <inheritdoc cref="TextUtil.ReplaceLast" />
        public static string ReplaceLast(this string textToSearch, string textToReplace, string replacementText)
        {
            return TextUtil.ReplaceLast(textToSearch, textToReplace, replacementText);
        }

        /// <inheritdoc cref="TextUtil.ConvertToSecureString" />
        public static SecureString ToSecureString(this string unsecureString)
        {
            return TextUtil.ConvertToSecureString(unsecureString);
        }

        /// <inheritdoc cref="TextUtil.ConvertToUnsecureString" />
        public static string ToUnsecureString(this SecureString secureString)
        {
            return TextUtil.ConvertToUnsecureString(secureString);
        }

        /// <summary>
        /// Tests to see if <c>text</c> contains at least one of a group of <c>string</c>s.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny(this string text, params string[] contentsToSearchFor)
        {
            return ContainsAny(text, contentsToSearchFor, ignoreCase: false);
        }

        /// <summary>
        /// Tests to see if <c>text</c> contains at least one of a group of <c>string</c>s, not case-sensitive if <c>ignoreCase == true</c>.
        /// </summary>
        /// <param name="text">A <c>string</c> to search.</param>
        /// <param name="contentsToSearchFor">A group of <c>string</c> to search for.</param>
        /// <param name="ignoreCase">If <c>true</c> the search is not case-senstive, default is <c>false</c>.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool ContainsAny(this string text, IEnumerable<string> contentsToSearchFor, bool ignoreCase = false)
        {
            if (contentsToSearchFor != null)
            {
                if (ignoreCase)
                {
                    var upper = text.ToUpper();
                    foreach (var content in contentsToSearchFor)
                    {
                        if (upper.Contains(content.ToUpper()))
                            return true;
                    }
                }
                else
                {
                    foreach (var content in contentsToSearchFor)
                    {
                        if (text.Contains(content))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Inspired by SQL, determines if an item is one of a supplied array of values
        /// </summary>
        /// <param name="chr">The item to locate</param>
        /// <param name="charCodes">The collection to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In(this char chr, params int[] charCodes)
        {
            return ExtensionAbstractions.In(chr, charCodes?.Select(i => (char)i));
        }

        /// <summary>
        /// Determines if character is carriage return (\r) or line feed (\n)
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool IsNewLine(this char chr)
        {
            return In(chr, 10, 13);
        }

        internal static void IncrementCleanedWhitespaces(this TraceJournal journal)
        {
            if (journal.Data.ContainsKey("text.clean.whitespaces.count"))
            {
                journal.Data["text.clean.whitespaces.count"] = (int)journal.Data["text.clean.whitespaces.count"] + 1;
            }
            else
            {
                journal.Data.Add("text.clean.whitespaces.count", 1);
            }
        }

        internal static void IncrementCleanedNonprintables(this TraceJournal journal)
        {
            if (journal.Data.ContainsKey("text.clean.nonprintables.count"))
            {
                journal.Data["text.clean.nonprintables.count"] = (int)journal.Data["text.clean.nonprintables.count"] + 1;
            }
            else
            {
                journal.Data.Add("text.clean.nonprintables.count", 1);
            }
        }

        internal static void IncrementCleanedUnicode(this TraceJournal journal)
        {
            if (journal.Data.ContainsKey("text.clean.unicode.count"))
            {
                journal.Data["text.clean.unicode.count"] = (int)journal.Data["text.clean.unicode.count"] + 1;
            }
            else
            {
                journal.Data.Add("text.clean.unicode.count", 1);
            }
        }

        internal static void IncrementCleanedOther(this TraceJournal journal)
        {
            if (journal.Data.ContainsKey("text.clean.other.count"))
            {
                journal.Data["text.clean.other.count"] = (int)journal.Data["text.clean.other.count"] + 1;
            }
            else
            {
                journal.Data.Add("text.clean.other.count", 1);
            }
        }

        /// <summary>
        /// Gets the count of whitespace <c>char</c>s that were eliminated or converted during a 'clean' operation.
        /// </summary>
        /// <param name="journal"></param>
        /// <returns>Affected whitespace <c>char</c> count</returns>
        public static int GetCleanedWhitespaceCharacters(this TraceJournal journal)
        {
            if (journal.Data.TryGetValue("text.clean.whitespaces.count", out object count))
                return (int)count;
            return 0;
        }

        /// <summary>
        /// Gets the count of nonprintable <c>char</c>s that were eliminated or converted during a 'clean' operation.
        /// </summary>
        /// <param name="journal"></param>
        /// <returns>Affected nonprintable <c>char</c> count</returns>
        public static int GetCleanedNonprintableCharacters(this TraceJournal journal)
        {
            if (journal.Data.TryGetValue("text.clean.nonprintables.count", out object count))
                return (int)count;
            return 0;
        }

        /// <summary>
        /// Gets the count of Unicode <c>char</c>s that were eliminated or converted during a 'clean' operation.
        /// </summary>
        /// <param name="journal"></param>
        /// <returns>Affected Unicode <c>char</c> count</returns>
        public static int GetCleanedUnicodeCharacters(this TraceJournal journal)
        {
            if (journal.Data.TryGetValue("text.clean.unicode.count", out object count))
                return (int)count;
            return 0;
        }

        /// <summary>
        /// Gets the count of other <c>char</c>s that were eliminated or converted during a 'clean' operation.
        /// </summary>
        /// <param name="journal"></param>
        /// <returns>Other affected <c>char</c> count</returns>
        public static int GetCleanedOtherCharacters(this TraceJournal journal)
        {
            if (journal.Data.TryGetValue("text.clean.other.count", out object count))
                return (int)count;
            return 0;
        }

        /// <summary>
        /// Gets the total count of <c>char</c>s that were eliminated or converted during a 'clean' operation.
        /// </summary>
        /// <param name="journal"></param>
        /// <returns></returns>
        public static int GetTotalCleanedCharacters(this TraceJournal journal)
        {
            var total = 0;
            if (journal.Data.TryGetValue("text.clean.whitespaces.count", out object whitespacesCount))
                total += (int)whitespacesCount;
            if (journal.Data.TryGetValue("text.clean.nonprintables.count", out object nonprintablesCount))
                total += (int)nonprintablesCount;
            if (journal.Data.TryGetValue("text.clean.unicode.count", out object unicodeCount))
                total += (int)unicodeCount;
            if (journal.Data.TryGetValue("text.clean.other.count", out object otherCount))
                total += (int)otherCount;
            return total;
        }
    }
}
