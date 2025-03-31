namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Represents a data parsing error if the policy is to embed them rather than throw an exception
    /// </summary>
    public class ImportError
    {
        /// <summary>
        /// Imported row
        /// </summary>
        public ImportedDataRow Row { get; set; }

        /// <summary>
        /// Row number
        /// </summary>
        public int RowNumber => Row?.RowNumber ?? 0;

        /// <summary>
        /// Column number
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// The exception message prefixed by convention with the exception type
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Formats the error for display using R1:C1 position notation
        /// </summary>
        /// <returns>The displayed error</returns>
        public override string ToString()
        {
            return ToString(default);
        }

        /// <summary>
        /// Formats the error for display using either R1:C1 or A1 position notation
        /// </summary>
        /// <returns>The displayed error</returns>
        public string ToString(PositionNotation positionNotation)
        {
            switch (positionNotation)
            {
                case PositionNotation.R1C1:
                default:
                    return "R" + RowNumber + ":C" + ColumnNumber + " - " + Message;
                case PositionNotation.A1:
                    return TabularDataUtil.GetA1StyleColumnLabel(ColumnNumber) + RowNumber + " - " + Message;
            }
        }
    }
}
