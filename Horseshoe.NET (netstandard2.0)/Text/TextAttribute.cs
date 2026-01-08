using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// A [de]serializable shorthand method of tagging strings with metadata
    /// </summary>
    /// <remarks>
    /// Example:
    /// <code>
    /// raw: [name:startDate,type:date,value:20251001][name:endDate,value:20251031]more text bla bla bla...
    /// parsed:
    /// ┌───────────┬─────────────────┬────────────┬──────────────────────┐
    /// │ Name      │ Type (optional) │ Value      │ GetValue&lt;DateTime&gt;() │
    /// ├───────────┼─────────────────┼────────────┼──────────────────────┤
    /// │ startDate │ System.DateTime │ 10/1/2025  │ 10/1/2025            │
    /// ├───────────┼─────────────────┼────────────┼──────────────────────┤
    /// │ endDate   │                 │ "20251031" │ 10/31/2025           │
    /// └───────────┴─────────────────┴────────────┴──────────────────────┘
    /// </code>
    /// </remarks>
    public class TextAttribute
    {
        /// <summary>
        /// Attribute name (case-senstive)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Attribute value type
        /// </summary>
        public Type Type { get; set; } = typeof(string);

        /// <summary>
        /// Gets or sets the text attribute value.
        /// </summary>
        public object Value { get; set; } = "";

        /// <summary>
        /// Returns a string representation of the current <see cref="TextAttribute"/> instance,  including its name,
        /// type, and value.
        /// </summary>
        public override string ToString() =>
            nameof(TextAttribute) + $": {{ Name = \"{Name}\", Type = \"{Type.FullName}\", Value = {ValueUtil.Display(Value)} }}";

        /// <summary>
        /// A regex pattern used to validate text attributes. Matches partial input, for example <c>"pre-text[name:mydate,value:20251031]post-text"</c>.
        /// </summary>
        public static Regex OpenPattern { get; } = new Regex("(\\[name(?<sep>[=:])[a-z0-9_.-]+,(type\\k<sep>[a-z]+,)?value\\k<sep>([^\\[\\],:]*|\"[^\"]*\")\\])+", RegexOptions.IgnoreCase);

        private static Regex _closedPattern;
        private static Regex _startsWithPattern;
        private static Regex _endsWithPattern;

        /// <summary>
        /// Generates a regex pattern based on <see cref="OpenPattern"/> used to validate text attributes. Matches the entire input, for example <c>"[name:mydate,value:20251031]"</c>.
        /// </summary>
        public static Regex ClosedPattern 
        { 
            get 
            { 
                if (_closedPattern == null)
                    _closedPattern = new Regex("^" + OpenPattern + "$", RegexOptions.IgnoreCase);
                return _closedPattern;
            } 
        }

        /// <summary>
        /// Generates a regex pattern based on <see cref="OpenPattern"/> used to validate text attributes. Matches the start of the input, for example <c>"[name:mydate,value:20251031]post-text"</c>.
        /// </summary>
        public static Regex StartsWithPattern
        { 
            get 
            { 
                if (_startsWithPattern == null)
                    _startsWithPattern = new Regex("^" + OpenPattern, RegexOptions.IgnoreCase);
                return _startsWithPattern;
            }
        }

        /// <summary>
        /// Generates a regex pattern based on <see cref="OpenPattern"/> used to validate text attributes. Matches the end of the input, for example <c>"pre-text[name:mydate,value:20251031]"</c>.
        /// </summary>
        public static Regex EndsWithPattern
        {
            get
            {
                if (_endsWithPattern == null)
                    _endsWithPattern = new Regex(OpenPattern + "$", RegexOptions.IgnoreCase);
                return _endsWithPattern;
            }
        }

        /// <summary>
        /// Represents a collection of key-value pairs where the keys are strings and the values are <see
        /// cref="TextAttribute"/> objects.
        /// </summary>
        public class List : Dictionary<string, TextAttribute>
        {
            /// <summary>
            /// Retrieves the <see cref="TextAttribute"/> associated with the specified name.
            /// </summary>
            /// <param name="name">The name of the attribute to retrieve. This value cannot be <see langword="null"/>.</param>
            public TextAttribute Get(string name)
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                if (TryGetValue(name, out TextAttribute value))
                    return value;

                return null;
            }

            /// <summary>
            /// Retrieves the value associated with the specified name, converted to the type specified in the <see cref="TextAttribute"/> if applicable.
            /// </summary>
            /// <remarks>
            /// If the attribute does not specify a type, the raw value is returned as is. 
            /// </remarks>
            /// <param name="name">The name of the attribute to retrieve.</param>
            /// <param name="dateFormat">The date format needed to parse inputted dates.</param>
            public object GetValue(string name, string dateFormat = "yyyyMMdd")
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                var textAttribute = Get(name);
                if (textAttribute == null || textAttribute.Value == null)
                    return null;
                if (textAttribute.Type == null)
                    return textAttribute.Value;
                return Zap.To(textAttribute.Type, textAttribute.Value, dateFormat: dateFormat);
            }

            /// <summary>
            /// Retrieves the value associated with the specified name, converted to the type specified by type parameter <typeparamref name="T"/>.
            /// </summary>
            /// <param name="name">The name of the attribute to retrieve.</param>
            /// <param name="dateFormat">The date format needed to parse inputted dates.</param>
            public T GetValue<T>(string name, string dateFormat = "yyyyMMdd")
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                var textAttribute = Get(name);
                if (textAttribute == null || textAttribute.Value == null)
                    return default;
                if (textAttribute.Value is string stringValue)
                {
                    if (string.IsNullOrEmpty(stringValue))
                        return default;
                    if (typeof(T) == typeof(string))
                        return (T)(object)stringValue;
                }
                if (textAttribute.Value is T t)
                    return t;
                return Zap.To<T>(textAttribute.Value, dateFormat: dateFormat);
            }
        }

        /// <summary>
        /// Attempts to parse the beginning of the specified input string into a <see cref="List"/> of <see cref="TextAttribute"/>s.
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <param name="list">When this method returns, contains the parsed <see cref="List"/> of <see cref="TextAttribute"/>s if the operation succeeds;
        /// otherwise, <see langword="null"/>.</param>
        /// <param name="remainingInput">When this method returns, contains the portion of the input string that remains unparsed if the operation
        /// succeeds; otherwise, the original input string.</param>
        /// <param name="dateFormat">The date format needed to parse inputted dates.</param>
        /// <param name="peekEvent">An optional callback invoked for each character and its associated context during parsing. This parameter
        /// can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the input string starts with a valid pattern and is successfully parsed;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool TryParseStartsWith(string input, out List list, out string remainingInput, string dateFormat = "yyyyMMdd", Action<char, string> peekEvent = null)
        {
            var match = StartsWithPattern.Match(input);
            if (match.Success)
            {
                try
                {
                    list = Parse(match.Value, dateFormat: dateFormat, peekEvent: peekEvent);
                    remainingInput = input.Substring(match.Value.Length);
                    return true;
                }
                catch
                {
                    list = null;
                    remainingInput = input;
                    return false;
                }
            }
            else
            {
                list = null;
                remainingInput = input;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse the end of the specified input string into a <see cref="List"/> of <see cref="TextAttribute"/>s.
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <param name="list">When this method returns, contains the parsed <see cref="List"/> of <see cref="TextAttribute"/>s if the operation succeeds;
        /// otherwise, <see langword="null"/>.</param>
        /// <param name="remainingInput">When this method returns, contains the portion of the input string that remains unparsed if the operation
        /// succeeds; otherwise, the original input string.</param>
        /// <param name="dateFormat">The date format needed to parse inputted dates.</param>
        /// <param name="peekEvent">An optional callback invoked for each character and its associated context during parsing. This parameter
        /// can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the input string starts with a valid pattern and is successfully parsed;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool TryParseEndsWith(string input, out List list, out string remainingInput, string dateFormat = "yyyyMMdd", Action<char, string> peekEvent = null)
        {
            var match = EndsWithPattern.Match(input);
            if (match.Success)
            {
                try
                {
                    list = Parse(match.Value, dateFormat: dateFormat, peekEvent: peekEvent);
                    remainingInput = input.Substring(0, input.Length - match.Value.Length);
                    return true;
                }
                catch
                {
                    list = null;
                    remainingInput = input;
                    return false;
                }
            }
            else
            {
                list = null;
                remainingInput = input;
                return false;
            }
        }

        /// <summary>
        /// Parses the specified input string into a <see cref="List"/> of <see cref="TextAttribute"/>s.
        /// </summary>
        /// <param name="input">The input string to parse. Cannot be <see langword="null"/> or empty.</param>
        /// <param name="dateFormat">The date format needed to parse inputted dates.</param>
        /// <param name="peekEvent">An optional callback invoked for each character and its associated context during parsing. The first
        /// parameter is the character being processed, and the second parameter provides additional context.</param>
        /// <returns>A <see cref="List"/> object representing the parsed structure of the input string.</returns>
        public static List Parse(string input, string dateFormat = "yyyyMMdd", Action<char, string> peekEvent = null)
        {
            var list = new List();
            Parse(list, input, dateFormat: dateFormat, peekEvent: peekEvent);
            return list;
        }

        /// <summary>
        /// Parses the specified input string and populates the provided <see cref="List"/> with extracted <see cref="TextAttribute"/>s.
        /// </summary>
        /// <param name="list">The list to populate with parsed text attributes. Cannot be null.</param>
        /// <param name="input">The input string to parse. Must match the expected regex pattern.</param>
        /// <param name="dateFormat">The date format needed to parse inputted dates.</param>
        /// <param name="peekEvent">An optional callback invoked during parsing to provide insights into the parsing process.  The callback
        /// receives the current character being processed and a descriptive message.</param>
        /// <exception cref="Exception"></exception>
        public static void Parse(List list, string input, string dateFormat = "yyyyMMdd", Action<char, string> peekEvent = null)
        {
            // Validation
            if (!ClosedPattern.IsMatch(input))
                throw new PatternMatchException($"input does not match \"{nameof(ClosedPattern)}\": \"{ClosedPattern}\"");

            string token = null;
            TextAttribute textAttribute = null;
            bool readingStringLiteral = false;
            var strb = new StringBuilder();

            foreach (char c in input.AsSpan())
            {
                switch (c)
                {
                    case '[':
                        if (readingStringLiteral)
                            strb.Append(c);
                        else
                        {
                            peekEvent?.Invoke(c, "initting text attribute, resetting token");
                            textAttribute = new TextAttribute();
                            token = null;
                            strb.Clear();
                        }
                        break;

                    case ']':
                        if (readingStringLiteral)
                            strb.Append(c);
                        else
                        {
                            _Parse(c, textAttribute, ref token, strb, dateFormat, peekEvent);

                            peekEvent?.Invoke(c, "(part 2) adding text attribute to list and resetting text attribute");

                            if (textAttribute?.Name == null)
                                throw new Exception("no text attribute or text attribute name");

                            list.Add(textAttribute.Name, textAttribute);
                            textAttribute = null;
                            strb.Clear();
                        }
                        break;

                    case '=':
                    case ':':
                        if (readingStringLiteral)
                            strb.Append(c);
                        else
                        {
                            token = strb.ToString();
                            peekEvent?.Invoke(c, $"setting token from read in chars: \"{token}\"");
                            switch (token)
                            {
                                case "name":
                                case "type":
                                case "value":
                                    break;
                                default:
                                    throw new Exception($"invalid token: \"{token}\"");
                            }
                            strb.Clear();
                        }
                        break;

                    case ',':
                        if (readingStringLiteral)
                            strb.Append(c);
                        else
                        {
                            _Parse(c, textAttribute, ref token, strb, dateFormat, peekEvent);
                        }
                        break;

                    case '"':
                        readingStringLiteral = !readingStringLiteral;
                        peekEvent?.Invoke(c, $"toggling readingStringLiteral: {readingStringLiteral}");
                        break;

                    default:
                        strb.Append(c);
                        break;
                }
            }
        }

        private static void _Parse(char c, TextAttribute textAttribute, ref string token, StringBuilder strb, string dateFormat, Action<char, string> peekEvent)
        {
            peekEvent?.Invoke(c, $"setting attribute (based on token) from read in chars: \"{token}\" - \"{strb}\"");

            if (textAttribute == null)
                throw new Exception("text attribute cannot be null");

            switch (token)
            {
                case "name":
                    textAttribute.Name = strb.ToString();
                    break;
                case "type":
                    switch (strb.ToString().ToLower())
                    {
                        case "byt":
                        case "byte":
                            textAttribute.Type = typeof(byte);
                            break;
                        case "short":
                            textAttribute.Type = typeof(short);
                            break;
                        case "int":
                        case "integer":
                            textAttribute.Type = typeof(int);
                            break;
                        case "lng":
                        case "long":
                            textAttribute.Type = typeof(long);
                            break;
                        case "dbl":
                        case "double":
                            textAttribute.Type = typeof(double);
                            break;
                        case "dec":
                        case "decimal":
                        case "cur":
                        case "currency":
                            textAttribute.Type = typeof(decimal);
                            break;
                        case "dtm":
                        case "date":
                        case "datetime":
                            textAttribute.Type = typeof(DateTime);
                            break;
                        case "bit":
                        case "bool":
                        case "boolean":
                            textAttribute.Type = typeof(bool);
                            break;
                        default:
                            try
                            {
                                if (Type.GetType(strb.ToString()) is Type type)
                                    textAttribute.Type = type;
                            }
                            finally { }
                            break;
                    }
                    peekEvent?.Invoke(c, $"textAttribute.Type = \"{textAttribute.Type}\"");
                    break;
                case "value":
                    if (textAttribute.Type == typeof(DateTime))
                    {
                        textAttribute.Value = Zap.To(textAttribute.Type, strb.ToString(), dateFormat: dateFormat);
                        break;
                    }
                    textAttribute.Value = Zap.To(textAttribute.Type, strb.ToString());
                    break;
            }
            token = null;
            strb.Clear();
        }
    }
}