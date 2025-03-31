using System.Text;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Options for importing data
    /// </summary>
    public class DataImportOptions
    {
        /// <summary>
        /// If <c>true</c>, the first imported row is assumed to be column names and is 
        /// ignored.  Default is <c>false</c>.
        /// </summary>
        public bool HasHeaderRow { get; set; }

        ///// <summary>
        ///// If <c>true</c> an exception will be thrown if any row's item counts differ from a) the column count
        ///// or b) the item count of the other rows.  Default is <c>false</c>.
        ///// </summary>
        //public bool EnforceItemCount { get; set; }
        
        /// <summary>
        /// If <c>true</c> quotation marks (") are treated as any other character, otherwise if <c>false</c> (default) 
        /// parses double quoted values as a unit which may or may not also include delimiters.
        /// </summary>
        public bool SuppressQuotationIsolation { get; set; }

        /// <summary>
        /// Defines how to handle blank rows encountered by the parser.  
        /// Default is <c>PruneOptions.LeadingAndTrailing</c>.
        /// </summary>
        public PruneOptions PruneOptions { get; set; } = PruneOptions.LeadingAndTrailing;

        /// <summary>
        /// The encoding to use when reading a filesystem file
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// How to diplay <c>null</c> values, trumped by <c>Column.DisplayNullAs</c>
        /// </summary>
        public string DisplayNullAs { get; set; } = TextConstants.Null;

        /// <summary>
        /// How to handle parse/convert errors.  Default is <c>Throw</c> but you can also <c>Embed</c> or <c>Ignore</c>.
        /// </summary>
        public DataErrorHandlingPolicy DataErrorHandling { get; set; }
    }
}
