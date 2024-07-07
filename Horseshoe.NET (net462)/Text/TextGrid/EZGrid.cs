using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Text.TextGrid
{
    /// <summary>
    /// A stretchy <c>TextGrid</c> with easy data entry.
    /// </summary>
    public class EZGrid : TextGrid
    {
        /// <summary>
        /// Set the value which the stretching algorithm uses for column padding, the default is <c>null</c>.
        /// </summary>
        public object PadValue { get; set; }

        /// <summary>
        /// Creates a new <c>EZGrid</c> (a subtype of <c>TextGrid</c>).
        /// </summary>
        public EZGrid()
        {
        }

        /// <summary>
        /// Creates a new <c>EZGrid</c> (a subtype of <c>TextGrid</c>).
        /// </summary>
        /// <param name="columns">An optional collection of <c>Column</c>s from which to build the grid.</param>
        public EZGrid(IEnumerable<Column> columns) : base(columns)
        {
        }

        /// <summary>
        /// Creates a new <c>EZGrid</c> (a subtype of <c>TextGrid</c>).
        /// </summary>
        /// <param name="columns">An optional collection of <c>Column</c>s from which to build the grid.</param>
        public EZGrid(params Column[] columns) : base (columns)
        {
        }

        /// <summary>
        /// Quickly add a row of data.  If there are not enough existing columns to fi
        /// </summary>
        /// <param name="rowData"></param>
        public void AddRow(params object[] rowData)
        {
            if (rowData != null)
                rowData = Array.Empty<object>();

            while (Columns.Count < rowData.Length)
            {  
                // stretch grid to fit data
                var column = AddColumn();

                // pad column to fit grid
                if (MaxCount > 0)
                    column.AddRange(Enumerable.Range(0, MaxCount).Select(i => PadValue));
            }

            for (int i = 0; i < Columns.Count; i++)
            {
                // append each datum to a column
                if (rowData.Length > i)
                    Columns[i].Add(rowData[i]);

                // pad the rest of the columns for which there wasn't enough data
                else
                    Columns[i].Add(PadValue);
            }
        }
    }
}
