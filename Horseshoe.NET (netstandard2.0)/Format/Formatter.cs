using System;
using System.Globalization;

using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Format
{
    /// <summary>
    /// Provides a variety of formatting options consolidated in a single class also supporting user-supplied custom formatters
    /// </summary>
    public class Formatter : IEquatable<Formatter>
    {
        /// <summary>
        /// A standard .NET format string, e.g. <c>"C"</c> = currency formatting
        /// </summary>
        public string DisplayFormat { get; }

        /// <summary>
        /// A format provider, e.g. <c>new CultureInfo("en-US")</c>
        /// </summary>
        public IFormatProvider Provider { get; }

        /// <summary>
        /// Gets the locale of the system or optionally supplied provider or the provider created by the optionally supplied locale
        /// </summary>
        public string Locale =>
            (Provider is CultureInfo culture ? culture : CultureInfo.CurrentCulture).Name;

        /// <summary>
        /// How to display <c>null</c> values across all <c>Formatter</c> instances unless otherwise specified
        /// </summary>
        public static string GlobalDisplayNullAs { get; } = TextConstants.Null;

        /// <summary>
        /// How to display <c>null</c> values in this <c>Formatter</c> instance
        /// </summary>
        private string _displayNullAs;

        /// <summary>
        /// How to display <c>null</c> values in this <c>Formatter</c> instance
        /// </summary>
        private string DisplayNullAs 
        { 
            get => _displayNullAs ?? GlobalDisplayNullAs;
            set => _displayNullAs = value;
        }

        /// <summary>
        /// Padding added to the formatted output string, e.g. <c>4 -> " abc"</c>, <c>-4 -> "abc "</c>...
        /// </summary>
        /// <remarks>
        /// The padding is applied internally one of two ways...
        /// <list type="number">
        /// <item>
        /// Applied to the format string.
        /// <list type="bullet">
        /// <item>
        /// <c>6 -&gt; "{0,6:C}" -&gt; " $6.12"</c>
        /// </item>
        /// <item>
        /// <c>-6 -&gt; "{0,-6:C}" -&gt; "$6.12 "</c>
        /// </item>
        /// </list>
        /// </item>
        /// <item>
        /// Applied to the output.
        /// <list type="bullet">
        /// <item>
        /// <c>4 -&gt; "abc".PadLeft(4) -&gt; " abc"</c>
        /// </item>
        /// <item>
        /// <c>-4 -&gt; "abc".PadRight(4) -&gt; "abc "</c>
        /// </item>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        public int Pad { get; set; }

        /// <summary>
        /// A user-supplied custom formatter. Note, the <c>object</c> argument is never <c>null</c>.
        /// </summary>
        public Func<object, string> CustomFormatter { get; }

        /// <summary>
        /// A user-supplied post-renderer for adding custom formatting to the output string prior to padding.
        /// </summary>
        /// <remarks>
        /// Example
        /// <code>
        /// var customPostRenderer = (renderedValue, value) => 
        /// {
        ///     if (value is int intValue &amp;&amp; intValue &gt; 0)
        ///         renderedValue += "+";
        ///     return renderedValue;
        /// }
        /// 
        /// // output for: format = "C", pad = 10
        /// // "  $600.12+"  pay
        /// // "  ($18.50)"  dinner
        /// // " ($128.12)"  electric bill
        /// </code>
        /// </remarks>
        public Func<string, object, string> CustomPostRenderer { get; }

        /// <summary>
        /// Creates a new <c>Formatter</c> instance with a format string and optional provider
        /// </summary>
        /// <param name="displayFormat">A format string, e.g. <c>"C"</c> = currency formatting</param>
        /// <param name="provider">A format provider, e.g. <c>new CultureInfo("en-US")</c></param>
        /// <param name="locale">A locale from which to create a provider, e.g. <c>"en-US" -&gt; <em>new CultureInfo("en-US")</em></c></param>
        /// <param name="pad">Padding added to the format string, e.g. <c>4 -> " abc"</c>, <c>-4 -> "abc "</c>...</param>
        /// <param name="displayNullAs">How to display <c>null</c> values.  One option is <c>Horseshoe.NET.Text.TextConstants.Null</c>.  Default is <c>""</c>.</param>
        /// <param name="customPostRenderer">
        /// A user-supplied custom post-renderer for adding custom formatting to the output string prior to padding.
        /// <para>
        /// Example
        /// <code>
        /// var customPostRenderer = (renderedValue, value) => 
        /// {
        ///     if (value &gt; 0)
        ///         renderedValue += "+";
        /// }
        /// 
        /// // output for: format = "C", pad = 10
        /// // "  $600.12+"  pay
        /// // "  ($18.50)"  dinner
        /// // " ($128.12)"  electric bill
        /// // "    [null]"
        /// </code>
        /// </para>
        /// </param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Formatter
        (
            string displayFormat, 
            IFormatProvider provider = null, 
            string locale = null, 
            int pad = 0,
            string displayNullAs = null,
            Func<string, object, string> customPostRenderer = null
        )
        {
            if (displayFormat == null)
                throw new ArgumentNullException(nameof(displayFormat), "param '" + nameof(displayFormat) + "' cannot be null or blank");
            if (displayFormat.Trim().Length == 0)
                throw new ArgumentException("param '" + nameof(displayFormat) + "' cannot be blank", nameof(displayFormat));
            DisplayFormat = displayFormat.ContainsAllInStrictSequence('{', '}')
                ? displayFormat
                : "{0" + (pad != 0 ? "," + pad : "") + ':' + displayFormat + '}';
            Provider = provider ?? (locale == null ? null : new CultureInfo(locale)) ?? CultureInfo.CurrentCulture;
            Pad = pad;
            DisplayNullAs = displayNullAs;
            CustomPostRenderer = customPostRenderer;
        }

        /// <summary>
        /// Creates a new <c>Formatter</c> instance with a user-supplied custom formatter.
        /// </summary>
        /// <param name="customFormatter">A user-supplied custom formatter.  Note, the <c>object</c> argument is never <c>null</c>.</param>
        /// <param name="displayNullAs">How to display <c>null</c> values.  If not supplied, <c>null</c> values are simply passed to the custom formatter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Formatter
        (
            Func<object, string> customFormatter, 
            string displayNullAs = null
        )
        {
            CustomFormatter = customFormatter ?? throw new ArgumentNullException(nameof(customFormatter), "param '" + nameof(customFormatter) + "' cannot be null");
            DisplayNullAs = displayNullAs;
        }

        private string PreRender(object value)
        {
            if (value == null)
                return DisplayNullAs;
            else if (CustomFormatter != null)
                return CustomFormatter.Invoke(value);
            else
                return string.Format(Provider, DisplayFormat, value);
        }

        /// <summary>
        /// Formats an object according to the supplied format string or user-supplied custom formatter
        /// </summary>
        /// <param name="value">A value</param>
        /// <returns>A formatted <c>string</c> representing the value</returns>
        public string Format(object value)
        {
            string returnValue = PreRender(value);

            if (CustomPostRenderer != null)
                returnValue = CustomPostRenderer.Invoke(returnValue, value);
            
            if (Pad > 0)
                return returnValue.PadLeft(Pad);
            else if (Pad < 0)
                return returnValue.PadRight(Pad);

            return returnValue;
        }

        /// <summary>
        /// Creates a currency formatter for the system (or optionally supplied) format provider.
        /// </summary>
        /// <param name="provider">A format provider, e.g. <c>new CultureInfo("en-US")</c></param>
        /// <param name="locale">A locale from which to create a provider, e.g. <c>"en-US" -&gt; <em>new CultureInfo("en-US")</em></c></param>
        /// <returns>A <c>Formatter</c></returns>
        public static Formatter Currency(IFormatProvider provider = null, string locale = null) =>
            new Formatter("C", provider: provider, locale: locale);

        /// <summary>
        /// Creates a date/time formatter from the supplied locale.
        /// </summary>
        /// <param name="locale">A locale from which to create a provider, e.g. <c>"en-US" -&gt; <em>new CultureInfo("en-US")</em></c></param>
        /// <returns>A <c>Formatter</c></returns>
        public static Formatter LongDate(string locale) =>
            new Formatter(new CultureInfo(locale).DateTimeFormat.LongDatePattern);

        /// <summary>
        /// Creates a date/time formatter from the supplied locale.
        /// </summary>
        /// <param name="locale">A locale from which to create a provider, e.g. <c>"en-US" -&gt; <em>new CultureInfo("en-US")</em></c></param>
        /// <param name="separator">A date and time separator, default is <c>" "</c>.</param>
        /// <returns>A <c>Formatter</c></returns>
        public static Formatter LongDateAndTime(string locale, string separator = " ")
        {
            var culture = new CultureInfo(locale);
            return culture.DateTimeFormat.LongDatePattern + separator + culture.DateTimeFormat.LongTimePattern;
        }

        /// <summary>
        /// Creates a date/time formatter from the supplied locale.
        /// </summary>
        /// <param name="locale">A locale from which to create a provider, e.g. <c>"en-US" -&gt; <em>new CultureInfo("en-US")</em></c></param>
        /// <returns>A <c>Formatter</c></returns>
        public static Formatter ShortDate(string locale) =>
            new Formatter(new CultureInfo(locale).DateTimeFormat.ShortDatePattern);

        /// <summary>
        /// Creates a date/time formatter from the supplied locale.
        /// </summary>
        /// <param name="locale">A locale from which to create a provider, e.g. <c>"en-US" -&gt; <em>new CultureInfo("en-US")</em></c></param>
        /// <param name="separator">A date and time separator, default is <c>" "</c>.</param>
        /// <returns>A <c>Formatter</c></returns>
        public static Formatter ShortDateAndTime(string locale, string separator = " ")
        {
            var culture = new CultureInfo(locale);
            return culture.DateTimeFormat.ShortDatePattern + separator + culture.DateTimeFormat.ShortTimePattern;
        }

        /// <summary>
        /// Creates a date/time formatter from the supplied locale.
        /// </summary>
        /// <param name="locale">A locale from which to create a provider, e.g. <c>"en-US" -&gt; <em>new CultureInfo("en-US")</em></c></param>
        /// <returns>A <c>Formatter</c></returns>
        public static Formatter FullDateTime(string locale) =>
            new Formatter(new CultureInfo(locale).DateTimeFormat.FullDateTimePattern);

        /// <summary>
        /// Checks to see if two <c>Formatter</c> instances are equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Formatter other)
        {
            if (other == null)
                return false;
            if (this == other)
                return true;
            return string.Equals(DisplayFormat, other.DisplayFormat)
                && string.Equals(Locale, other.Locale)
                && string.Equals(DisplayNullAs, other.DisplayNullAs)
                && Pad == other.Pad
                && ValueUtil.AllOrNone(CustomFormatter, other.CustomFormatter)
                && ValueUtil.AllOrNone(CustomPostRenderer, other.CustomPostRenderer);
        }

        /// <summary>
        /// Implicitly creates a <c>Formatter</c> from a format string
        /// </summary>
        /// <param name="format">A format string</param>
        public static implicit operator Formatter(string format) => new Formatter(format);

        /// <summary>
        /// Implicitly creates a <c>Formatter</c> from a user-supplied custom formatter
        /// </summary>
        /// <param name="custom">A user-supplied custom formatter</param>
        public static implicit operator Formatter(Func<object, string> custom) => new Formatter(custom);
    }
}
