using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.DataImport
{
    public static class DataImportExtensions
    {
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

        public static int GetErrorCount(this IList<Column> cols)
        {
            return cols.Sum(c => c.DataErrors.Count());
        }

        public static IList<DataError> GetErrors(this IList<Column> cols)
        {
            return cols.SelectMany(c => c.DataErrors).ToList();
        }

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
