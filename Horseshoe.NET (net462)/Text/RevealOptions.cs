using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Customizations for revealing <c>char</c>s and <c>string</c>s.
    /// </summary>
    public class RevealOptions
    {
        /* * * * * * * * * * * * * * * * * * *
         *           STRING VALUES
         * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// If the input <c>string</c> is null, display this.  This property is customizable.
        /// </summary>
        public string ValueIfNull { get; set; } = TextConstants.Null;

        /// <summary>
        /// If the input <c>string</c> is empty, display this.  This property is customizable.
        /// </summary>
        public string ValueIfEmpty { get; set; } = TextConstants.Empty;

        /// <summary>
        /// If the input <c>string</c> is whitespace, display this.  This property is customizable.
        /// </summary>
        public string ValueIfWhitespace { get; set; } = TextConstants.Whitespace;

        /// <summary>
        /// 1 for single quotes, 2 for double quotes.  Default is 0.
        /// </summary>
        public int StringQuotationLevel { get; set; }
        
        /* * * * * * * * * * * * * * * * * * *
         *           CHAR VALUES
         * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Value revealing an ASCII space
        /// </summary>
        public string ValueIfSpace { get; set; } = TextConstants.Space;

        /// <summary>
        /// Value revealing a non-breaking space
        /// </summary>
        public string ValueIfNbSpace { get; set; } = TextConstants.NonBreakingSpace;

        /// <summary>
        /// Value revealing a tab
        /// </summary>
        public string ValueIfTab { get; set; } = TextConstants.HorizontalTab;

        /// <summary>
        /// Value revealing a carriage return
        /// </summary>
        public string ValueIfCr { get; set; } = TextConstants.CarriageReturn;

        /// <summary>
        /// Value revealing a line feed
        /// </summary>
        public string ValueIfLf { get; set; } = TextConstants.LineFeed;

        /// <summary>
        /// Value revealing a carriage return / line feed combination
        /// </summary>
        public string ValueIfCrLf { get; set; } = "[cr-lf]";

        /* * * * * * * * * * * * * * * * * * *
         *           CHAR OPTIONS
         * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// A user-supplied list of <c>char</c>s to reveal.
        /// </summary>
        public IEnumerable<char> CharsToReveal { get; set; }

        /// <summary>
        /// Required category(ies) of <c>char</c> to reveal, if omitted no <c>char</c> will be revealed.
        /// </summary>
        public CharCategory CharCategory { get; set; }

        /// <summary>
        /// Use <c>true</c> to reveal ASCII only, <c>false</c> for Unicode only and <c>null</c> for both.
        /// </summary>
        public bool? IsAscii { get; set; }

        /// <summary>
        /// Returns <c>true</c> if <c>CharsToReveal</c> contains any items or if <c>CharCategory</c> is anything other then <c>None</c>.
        /// </summary>
        public bool IsRevealingChars => (CharsToReveal != null && CharsToReveal.Any()) || CharCategory != CharCategory.NotDefined;

        /// <summary>
        /// When combined with <c>CharRevealPolicy.OtherWhitespaces</c> renders actual new lines.
        /// </summary>
        public bool PreserveNewLines { get; set; }

        /// <summary>
        /// A global <c>RevealOptions</c> instance that instructs Horseshoe.NET to reveal only blank and null strings.
        /// </summary>
        public static RevealOptions Default { get; } = new RevealOptions();

        /// <summary>
        /// A global <c>RevealOptions</c> instance that instructs Horseshoe.NET to reveal every <c>char</c> in a string or whether it is blank or null.
        /// </summary>
        public static RevealOptions All { get; } = new RevealOptions { CharCategory = CharCategory.All };
    }
}
