using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// A collection of extension methods for <c>Horseshoe.NET.DataImport</c>
    /// </summary>
    public static class DataImportExtensions
    {
        /// <summary>
        /// Renders of list of <c>Column</c> to text
        /// </summary>
        /// <param name="cols">a collection of <c>Column</c>s</param>
        /// <param name="nullReplacement">how to display <c>null</c> values</param>
        /// <returns>a <c>string</c> rendition of the collection</returns>
        public static string Render(this IList<Column> cols, string nullReplacement = "[null]")
        {
            // render the column values
            var renderedLists = cols
                .Select(c => c.Render(nullReplacement: nullReplacement))
                .ToList();

            // calculate the rendered column widths
            var widths = renderedLists
                .Select(l => l.Max(s => s.Length))
                .ToList();

            // adjust for column name length and add 1 char padding
            for (int i = 0; i < widths.Count; i++)
            {
                widths[i] = Math.Max(widths[i], cols[i].Name?.Length ?? 0) + 1;
            }

            // build the monospace grid
            var strb = new StringBuilder();
            for (int i = 0; i < widths.Count; i++)
            {
                strb.Append((cols[i].Name ?? "C0l " + (i + 1)).PadRight(widths[i]));
            }
            strb.AppendLine();
            for (int i = 0; i < widths.Count; i++)
            {
                strb.Append(new string('-', (cols[i].Name ?? "C0l " + (i + 1)).Length).PadRight(widths[i]));
            }
            strb.AppendLine();
            for (int r = 0, maxCount = renderedLists.Max(s => s.Count); r < maxCount; r++)
            {
                for (int i = 0; i < widths.Count; i++)
                {
                    try
                    {
                        strb.Append(renderedLists[i][r].PadRight(widths[i]));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        strb.Append("[void]".PadRight(widths[i]));
                    }
                }
                strb.AppendLine();
            }
            return strb.ToString();
        }

        /// <summary>
        /// Renders a <c>Column</c> to text
        /// </summary>
        /// <param name="col">a <c>Column</c></param>
        /// <param name="nullReplacement">how to display <c>null</c> values</param>
        /// <returns>a <c>string</c> rendition of the <c>Column</c></returns>
        public static List<string> Render(this Column col, string nullReplacement = "[null]")
        {
            //var list = new List<string>(col.Select(o => col.Formatter == null || o is DataError ? o?.ToString() ?? nullReplacement : col.Formatter.Invoke(o) ?? nullReplacement));
            var list = new List<string>();
            foreach (var value in col)
            {
                if (value == null)
                {
                    list.Add(nullReplacement);
                }
                else if (col.Formatter == null || value is DataError)
                {
                    list.Add(value.ToString());
                }
                else
                {
                    list.Add(col.Formatter.Invoke(value));
                }
            }
            return list;
        }
    }
}
