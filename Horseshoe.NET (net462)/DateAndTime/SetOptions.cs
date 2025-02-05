using System.Reflection;
using System.Security.Authentication.ExtendedProtection;

namespace Horseshoe.NET.DateAndTime
{
    /// <summary>
    /// A <c>DisplayOptions</c> subclass focused on which specific date time parts to include in the output
    /// </summary>
    public class SetOptions : DisplayOptions
    {
        /// <summary>
        /// Whether to swith the display of the 'year' date time part off, on or auto
        /// </summary>
        public TimePartDisplay DisplayYears { get; set; }

        /// <summary>
        /// Whether to swith the display of the 'month' date time part off, on or auto
        /// </summary>
        public TimePartDisplay DisplayMonths { get; set; }

        /// <summary>
        /// Whether to swith the display of the 'day' date time part off, on or auto
        /// </summary>
        public TimePartDisplay DisplayDays { get; set; }

        /// <summary>
        /// Whether to swith the display of the 'hour' date time part off, on or auto
        /// </summary>
        public TimePartDisplay DisplayHours { get; set; }

        /// <summary>
        /// Whether to swith the display of the 'minute' date time part off, on or auto
        /// </summary>
        public TimePartDisplay DisplayMinutes { get; set; }

        /// <summary>
        /// Whether to swith the display of the 'second' date time part off, on or auto
        /// </summary>
        public TimePartDisplay DisplaySeconds { get; set; }

        /// <summary>
        /// Whether to swith the display of the 'millisecond' date time part off, on or auto
        /// </summary>
        public TimePartDisplay DisplayMilliseconds { get; set; }

        /// <summary>
        /// Creates a new <c>SetOptions</c>
        /// </summary>
        public SetOptions() { }

        /// <summary>
        /// Creates a new <c>SetOptions</c> fron another instance
        /// </summary>
        /// <param name="options"></param>
        public SetOptions(SetOptions options)
        {
            var props = typeof(SetOptions).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in props)
            {
                prop.SetValue(this, prop.GetValue(options));
            }
        }

        /// <summary>
        /// Pre-built <c>SetOptions</c> with the display of all date time parts switched to 'Auto'
        /// </summary>
        public static SetOptions Auto { get; } = new SetOptions
        {
            DisplayYears = TimePartDisplay.Auto,
            DisplayMonths = TimePartDisplay.Auto,
            DisplayDays = TimePartDisplay.Auto,
            DisplayHours = TimePartDisplay.Auto,
            DisplayMinutes = TimePartDisplay.Auto,
            DisplaySeconds = TimePartDisplay.Auto,
            DisplayMilliseconds = TimePartDisplay.Auto
        };

        /// <summary>
        /// Pre-built <c>SetOptions</c> with the display of all date time parts switched 'On'
        /// </summary>
        public static SetOptions On { get; } = new SetOptions
        {
            DisplayYears = TimePartDisplay.On,
            DisplayMonths = TimePartDisplay.On,
            DisplayDays = TimePartDisplay.On,
            DisplayHours = TimePartDisplay.On,
            DisplayMinutes = TimePartDisplay.On,
            DisplaySeconds = TimePartDisplay.On,
            DisplayMilliseconds = TimePartDisplay.On
        };
    }
}
