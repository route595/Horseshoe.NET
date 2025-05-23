﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Text;

namespace Horseshoe.NET
{
    /// <summary>
    /// A set of utility methods for some of the base Horseshoe.NET classes.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Inspired by SQL, determines if an item is one of a supplied array of values
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="obj">The item to locate</param>
        /// <param name="collection">The collection to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In<T>(this T obj, params T[] collection)
        {
            return In(obj, collection as IEnumerable<T>);
        }

        /// <summary>
        /// Inspired by SQL, determines if an item is one of a supplied array of values
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="obj">The item to locate</param>
        /// <param name="collection">The collection to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In<T>(this T obj, IEnumerable<T> collection)
        {
            if (collection == null)
                return false;
            return collection.Contains(obj);
        }

        /// <summary>
        /// Inspired by SQL, determines if a <c>string</c> is one of a supplied array (not case-senstitive)
        /// </summary>
        /// <param name="text">The <c>string</c> to locate</param>
        /// <param name="collection">The collection of <c>string</c>s to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(this string text, params string[] collection)
        {
            return InIgnoreCase(text, collection as IEnumerable<string>);
        }

        /// <summary>
        /// Inspired by SQL, determines if a <c>string</c> is one of a supplied array (not case-senstitive)
        /// </summary>
        /// <param name="text">The <c>string</c> to locate</param>
        /// <param name="collection">The collection of <c>string</c>s to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(this string text, IEnumerable<string> collection)
        {
            return collection.Any(s => string.Equals(s, text, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Inspired by SQL, determines if a <c>char</c> is one of a supplied array (not case-senstitive)
        /// </summary>
        /// <param name="char">The <c>char</c> to locate</param>
        /// <param name="collection">The collection of <c>char</c>s to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool InIgnoreCase(this char @char, IEnumerable<char> collection)
        {
            return collection.Any(c => string.Equals(new string(c, 1), new string(@char, 1), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Inspired by SQL, determines if a <c>char</c> is one of a supplied array of <c>int</c> char codes
        /// </summary>
        /// <param name="chr">A <c>char</c> to locate</param>
        /// <param name="charCodes">A collection or char codes to search</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool In(this char chr, params int[] charCodes)
        {
            return In(chr, charCodes?.Select(i => (char)i));
        }

        /// <summary>
        /// Display an assembly name and version
        /// </summary>
        /// <param name="assembly">An assembly</param>
        /// <param name="minDepth">A value between 1 and 4 determining how many 0's to display</param>
        /// <returns>Display name</returns>
        public static string GetDisplayName(this Assembly assembly, int minDepth = 2)
        {
            return GetDisplayName(assembly.GetName(), minDepth: minDepth);
        }

        /// <summary>
        /// Display an assembly name and version
        /// </summary>
        /// <param name="assemblyName">An assembly name</param>
        /// <param name="minDepth">A value between 1 and 4 determining how many 0's to display</param>
        /// <returns>Display name</returns>
        public static string GetDisplayName(this AssemblyName assemblyName, int minDepth = 2)
        {
            return assemblyName.Name + " " + GetDisplayVersion(assemblyName, minDepth: minDepth);
        }

        /// <summary>
        /// Display an assembly version
        /// </summary>
        /// <param name="assembly">An assembly</param>
        /// <param name="minDepth">A value between 1 and 4 determining how many 0's to display</param>
        /// <returns>Display version</returns>
        public static string GetDisplayVersion(this Assembly assembly, int minDepth = 2)
        {
            return GetDisplayVersion(assembly.GetName(), minDepth: minDepth);
        }

        /// <summary>
        /// Display an assembly version
        /// </summary>
        /// <param name="assemblyName">An assembly name</param>
        /// <param name="minDepth">A value between 1 and 4 determining how many 0's to display</param>
        /// <returns>Display version</returns>
        public static string GetDisplayVersion(this AssemblyName assemblyName, int minDepth = 2)
        {
            return Display(assemblyName.Version, minDepth: minDepth);
        }

        /// <summary>
        /// Display an assembly version
        /// </summary>
        /// <param name="version">An assembly version</param>
        /// <param name="minDepth">A value between 1 and 4 determining how many 0's to display</param>
        /// <returns>Display version</returns>
        public static string Display(this Version version, int minDepth = 2)
        {
            var sb = new StringBuilder();

            // input validation
            if (minDepth < 1)
            {
                minDepth = 2;
            }

            // going in reverse starting at revision
            if (version.Revision > 0 || minDepth >= 4)
            {
                sb.Insert(0, "." + version.Revision);                // e.g. 1.0.501[.10971] or 1.0.0[.0]
            }

            if (sb.Length > 0 || version.Build > 0 || minDepth >= 3)
            {
                sb.Insert(0, "." + version.Build);                   // e.g.  1.0[.501].10971 or 1.0[.0].0 or 1.0[.0]
            }

            if (sb.Length > 0 || version.Minor > 0 || minDepth >= 2)
            {
                sb.Insert(0, "." + version.Minor);                   // e.g.  1[.0].501.10971 or 1[.0].0.0 or 1[.0].0 or 1[.0]
            }

            sb.Insert(0, version.Major);                             // e.g.  [1].0.501.10971 or [1].0.0.0 or [1].0.0 or [1].0 or [1]

            return sb.ToString();
        }

        /// <summary>
        /// Prepares an exception for viewing in a console or other text field (e.g. &lt;textarea&gt;, &lt;pre&gt;, etc.)
        /// </summary>
        /// <param name="exception">An exception.</param>
        /// <param name="typeRendering">If <c>true</c> use the fully qualified type name (default is <c>false</c>).</param>
        /// <param name="includeDateTime">If <c>true</c> renders the approximate date and time the source exception was raised (default is <c>false</c>).</param>
        /// <param name="includeMachineName">If <c>true</c> renders the machine name where the exception probably occurred (default is <c>false</c>).</param>
        /// <param name="includeStackTrace">If <c>true</c> renders the stack trace (default is <c>false</c>).</param>
        /// <param name="indent">Number of spaces to indent detail lines in the output.</param>
        /// <param name="recursive">If <c>true</c> renders inner exceptions (default is <c>false</c>).</param>
        /// <returns>Console-formatted exception dump</returns>
        public static string Render(this Exception exception, ExceptionTypeRenderingPolicy typeRendering = default, bool includeDateTime = false, bool includeMachineName = false, bool includeStackTrace = false, int indent = 2, bool recursive = false)
        {
            var strb = new StringBuilder();
            _Render(exception, strb, typeRendering, includeDateTime, includeMachineName, includeStackTrace, indent, recursive);
            return strb.ToString();
        }

        /// <summary>
        /// Prepares an exception for viewing in a console or other text field (e.g. &lt;textarea&gt;, &lt;pre&gt;, etc.).
        /// </summary>
        /// <param name="exceptionInfo">An exception info.</param>
        /// <param name="typeRendering">If <c>Fqn</c> (default) display the fully qualified type name.</param>
        /// <param name="includeDateTime">If <c>true</c> renders the approximate date and time the source exception was raised (default is <c>false</c>).</param>
        /// <param name="includeMachineName">If <c>true</c> renders the machine name where the exception probably occurred (default is <c>false</c>).</param>
        /// <param name="includeStackTrace">If <c>true</c> renders the stack trace (default is <c>false</c>).</param>
        /// <param name="indent">Number of spaces to indent detail lines in the output.</param>
        /// <param name="recursive">If <c>true</c> renders inner exceptions (default is <c>false</c>).</param>
        /// <returns>Console-formatted exception dump.</returns>
        public static string Render(this ExceptionInfo exceptionInfo, ExceptionTypeRenderingPolicy typeRendering = default, bool includeDateTime = false, bool includeMachineName = false, bool includeStackTrace = false, int indent = 2, bool recursive = false)
        {
            var strb = new StringBuilder();
            _Render(exceptionInfo, strb, typeRendering, includeDateTime, includeMachineName, includeStackTrace, indent, recursive);
            return strb.ToString();
        }

        /// <summary>
        /// Prepares an exception for viewing in a console or other text field (e.g. &lt;textarea&gt;, &lt;pre&gt;, etc.)
        /// </summary>
        /// <param name="exception">An exception.</param>
        /// <param name="typeRendering">If <c>Fqn</c> (default) display the fully qualified type name.</param>
        /// <returns>Console-formatted exception dump.</returns>
        public static string RenderMessage(this Exception exception, ExceptionTypeRenderingPolicy typeRendering = default)
        {
            var strb = new StringBuilder();
            _RenderMessage(exception, strb, typeRendering);
            return strb.ToString();
        }

        /// <summary>
        /// Prepares an exception for viewing in a console or other text field (e.g. &lt;textarea&gt;, &lt;pre&gt;, etc.)
        /// </summary>
        /// <param name="exceptionInfo">An exception</param>
        /// <param name="typeRendering">If <c>Fqn</c> (default) display the fully qualified type name</param>
        /// <returns>Console-formatted exception dump</returns>
        public static string RenderMessage(this ExceptionInfo exceptionInfo, ExceptionTypeRenderingPolicy typeRendering = default)
        {
            var strb = new StringBuilder();
            _RenderMessage(exceptionInfo, strb, typeRendering);
            return strb.ToString();
        }

        /// <summary>
        /// Prepares an exception for viewing in a web browser
        /// </summary>
        /// <param name="exception">An exception</param>
        /// <param name="typeRendering">If <c>Fqn</c> (default) display the fully qualified type name</param>
        /// <param name="includeDateTime">If <c>true</c> renders the approximate date and time the source exception was raised (default is <c>false</c>).</param>
        /// <param name="includeMachineName">If <c>true</c> renders the machine name where the exception probably occurred (default is <c>false</c>).</param>
        /// <param name="includeStackTrace">If <c>true</c> renders the stack trace (default is <c>false</c>)</param>
        /// <param name="indent">Number of spaces to indent detail lines in the output</param>
        /// <param name="recursive">If <c>true</c> renders inner exceptions (default is <c>false</c>)</param>
        /// <returns>HTML-formatted exception dump</returns>
        public static string RenderHtml(this Exception exception, ExceptionTypeRenderingPolicy typeRendering = default, bool includeDateTime = false, bool includeMachineName = false, bool includeStackTrace = false, int indent = 2, bool recursive = false)
        {
            var text = Render(exception, typeRendering: typeRendering, includeDateTime: includeDateTime, includeMachineName: includeMachineName, includeStackTrace: includeStackTrace, indent: indent, recursive: recursive);
            var html = TextUtil.ConvertToHtml(text);
            return html;
        }

        /// <summary>
        /// Prepares an exception for viewing in a web browser
        /// </summary>
        /// <param name="exceptionInfo">An exception</param>
        /// <param name="typeRendering">If <c>Fqn</c> (default) display the fully qualified type name</param>
        /// <param name="includeDateTime">If <c>true</c> renders the approximate date and time the source exception was raised (default is <c>false</c>).</param>
        /// <param name="includeMachineName">If <c>true</c> renders the machine name where the exception probably occurred (default is <c>false</c>).</param>
        /// <param name="includeStackTrace">If <c>true</c> renders the stack trace (default is <c>false</c>)</param>
        /// <param name="indent">Number of spaces to indent detail lines in the output</param>
        /// <param name="recursive">If <c>true</c> renders inner exceptions (default is <c>false</c>)</param>
        /// <returns>HTML-formatted exception dump</returns>
        public static string RenderHtml(this ExceptionInfo exceptionInfo, ExceptionTypeRenderingPolicy typeRendering = default, bool includeDateTime = false, bool includeMachineName = false, bool includeStackTrace = false, int indent = 2, bool recursive = false)
        {
            var text = Render(exceptionInfo, typeRendering: typeRendering, includeDateTime: includeDateTime, includeMachineName: includeMachineName, includeStackTrace: includeStackTrace, indent: indent, recursive: recursive);
            var html = TextUtil.ConvertToHtml(text);
            return html;
        }

        private static void _RenderMessage(ExceptionInfo exceptionInfo, StringBuilder strb, ExceptionTypeRenderingPolicy typeRendering)
        {
            bool fqn = true;
            switch (typeRendering)
            {
                case ExceptionTypeRenderingPolicy.FqnExceptSystem:
                    fqn = !exceptionInfo.FullType.StartsWith("System.");
                    break;
                case ExceptionTypeRenderingPolicy.NameOnly:
                    fqn = false;
                    break;
            }
            strb.Append(fqn ? exceptionInfo.FullType : exceptionInfo.Type)
                .AppendLine(": " + TextUtil.Reveal(exceptionInfo.Message));
        }

        private static void _Render(ExceptionInfo exceptionInfo, StringBuilder strb, ExceptionTypeRenderingPolicy typeRendering, bool includeDateTime, bool includeMachineName, bool includeStackTrace, int indent, bool recursive)
        {
            _RenderMessage(exceptionInfo, strb, typeRendering);
            strb.AppendLineIf(includeDateTime, () => "Date/Time: " + TextUtil.Reveal(exceptionInfo.DateTime?.ToString("yyyy-MM-dd HH:mm:ss")))
                .AppendLineIf(includeMachineName, () => "Source Location: " + TextUtil.Reveal(exceptionInfo.SourceLocation));
            if (includeStackTrace)
            {
                strb.AppendLine("Stack Trace:")
                    .AppendLine(IndentStackTrace(exceptionInfo.StackTrace, indent));
            }
            if (recursive && exceptionInfo.InnerException != null)
            {
                strb.AppendLine();
                _Render(exceptionInfo.InnerException, strb, typeRendering, includeDateTime, includeMachineName, includeStackTrace, indent, recursive);
            }
        }

        private static string IndentStackTrace(string stackTrace, int indent)
        {
            if (stackTrace == null)
                return string.Empty;
            var lines = stackTrace.Replace("\r\n", "\n")
                .Split('\n')
                .Select(ln => new string(' ', indent) + ln);
            return string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// Converts the <c>Password</c> to an encrypted <c>string</c>.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="cryptoOptions"></param>
        /// <returns>An encrypted password</returns>
        public static string ToEncryptedPassword(this Password password, CryptoOptions cryptoOptions) => password.HasSecurePassword
            ? Encrypt.String(TextUtil.ConvertToUnsecureString(password), cryptoOptions)
            : null;
    }
}
