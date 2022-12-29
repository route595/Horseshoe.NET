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
        public string ValueIfWhitespace { get; set; } = "[whitespace]";
        
        /* * * * * * * * * * * * * * * * * * *
         *           CHAR VALUES
         * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Value revealing an ASCII space
        /// </summary>
        public string ValueIfSpace { get; set; } = "[space]";

        /// <summary>
        /// Value revealing a non-breaking space
        /// </summary>
        public string ValueIfNbSpace { get; set; } = "[nb-space]";

        /// <summary>
        /// Value revealing a tab
        /// </summary>
        public string ValueIfTab { get; set; } = "[tab]";

        /// <summary>
        /// Value revealing a carriage return
        /// </summary>
        public string ValueIfCr { get; set; } = "[cr]";

        /// <summary>
        /// Value revealing a line feed
        /// </summary>
        public string ValueIfLf { get; set; } = "[lf]";

        /// <summary>
        /// Value revealing a carriage return / line feed combination
        /// </summary>
        public string ValueIfCrLf { get; set; } = "[cr-lf]";

        /* * * * * * * * * * * * * * * * * * *
         *           CHAR OPTIONS
         * * * * * * * * * * * * * * * * * * */

        /// <summary>
        /// Reveals ASCII character codes (not including extended ASCII).
        /// </summary>
        public RevealCharCategory CharsToReveal { get; set; }

        /// <summary>
        /// When combined with <c>RevealCharCategory.RevealWhitespaces</c> indicates which whitespaces to reveal.
        /// </summary>
        public WhitespacePolicy WhitespacesToReveal { get; set; } = WhitespacePolicy.IncludeAllWhitespaces;

        /// <summary>
        /// When combined with <c>RevealNewLines</c> renders actual new lines to the output in addition to revealed new lines.
        /// </summary>
        public bool PreserveNewLines { get; set; }

        /// <summary>
        /// A global <c>RevealOptions</c> instance that instructs Horseshoe.NET to reveal only blank and null strings.
        /// </summary>
        public static RevealOptions Default { get; } = new RevealOptions();

        /// <summary>
        /// A global <c>RevealOptions</c> instance that instructs Horseshoe.NET to reveal every <c>char</c> in a string or whether it is blank or null.
        /// </summary>
        public static RevealOptions All { get; } = new RevealOptions { CharsToReveal = RevealCharCategory.All };
    }
}
