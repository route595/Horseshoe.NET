using System;

using static Horseshoe.NET.Mathematics.Geometry.CalcUtil;

namespace Horseshoe.NET.Mathematics.Geometry
{
    /// <summary>
    /// Geometric calculations extension methods
    /// </summary>
    public static class GeomExtensions
    {
        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        public static double DegreesToRadians(this double degrees, int? precision = null) =>
            precision.HasValue
                ? R(degrees * Math.PI / 180.0, precision)
                : degrees * Math.PI / 180.0;

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        public static double RadiansToDegrees(this double radians, int? precision = null) =>
            precision.HasValue
                ? R(radians * 180.0 / Math.PI, precision)
                : radians * 180.0 / Math.PI;
    }
}
