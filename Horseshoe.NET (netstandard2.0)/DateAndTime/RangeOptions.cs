using System.Reflection;

namespace Horseshoe.NET.DateAndTime
{
    /// <summary>
    /// <c>DisplayOptions</c> subclass focused on the level of detail of the output.  
    /// For example, in some situations a range of 2 (1 day 4 hours) might be preferable 
    /// to a range of 4 (1 day 4 hours 11 minutes 32 seconds)
    /// </summary>
    public class RangeOptions : DisplayOptions
    {
        /// <summary>
        /// Set to a number between 1 and 7
        /// </summary>
        public int MaxRange { get; set; }

        /// <summary>
        /// Creates a new <c>RangeOptions</c>
        /// </summary>
        public RangeOptions() { }

        /// <summary>
        /// Creates a new <c>RangeOptions</c> from another instance
        /// </summary>
        /// <param name="options"></param>
        public RangeOptions(RangeOptions options)
        {
            var props = typeof(SetOptions).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in props)
            {
                prop.SetValue(this, prop.GetValue(options));
            }
        }
    }
}
