namespace Horseshoe.NET.Text
{
    public class RevealOptions
    {
        public static RevealOptions Default = new RevealOptions();
        public static RevealOptions All = new RevealOptions { RevealAll = true };

        // string values
        public string ValueIfNullArg { get; set; } = "[null-arg]";
        public string ValueIfEmptyArg { get; set; } = "[empty-arg]";

        // string
        public string ValueIfNull { get; set; } = "[null]";
        public string ValueIfEmpty { get; set; } = "[empty]";
        public string ValueIfWhitespace { get; set; } = "[whitespace]";

        // char
        public string ValueIfSpace { get; set; } = "[space]";
        public string ValueIfTab { get; set; } = "[tab]";
        public string ValueIfCr { get; set; } = "[cr]";
        public string ValueIfLf { get; set; } = "[lf]";
        public string ValueIfCrLf { get; set; } = "[cr-lf]";
        public string ValueIfNbSpace { get; set; } = "[nb-space]";

        // options
        public bool RevealASCIIChars { get; set; }
        /// <summary>
        /// Applies to ASCII constrol chars except tab, line feed and carriage return (see <c>RevealWhitespaces</c>)
        /// </summary>
        public bool RevealControlChars { get; set; }
        public bool RevealNonprintableChars { get; set; }
        public bool RevealWhitespaces { get; set; }
        public bool RevealSpaces { get; set; }
        public bool RevealTabs { get; set; }
        public bool RevealNewLines { get; set; }
        public bool PreserveNewLines { get; set; }
        public bool RevealAll { get; set; }
    }
}
