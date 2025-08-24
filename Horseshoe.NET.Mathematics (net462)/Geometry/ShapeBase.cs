using System;

namespace Horseshoe.NET.Mathematics.Geometry
{
    public abstract class ShapeBase
    {
        /// <summary>
        /// Number of sides in this shape
        /// </summary>
        public abstract double NumberOfSides { get; }

        /// <summary>
        /// Number of angles in this shape
        /// </summary>
        public abstract double NumberOfAngles { get; }

        /// <summary>
        /// Units of measurement, for display purposes only
        /// </summary>
        public string UnitsOfMeasurement { get; set; }

        /// <summary>
        /// The default number of rounding digits to use for either comparison or display
        /// </summary>
        public static int DefaultRoundingDigits { get; set; } = 6;

        protected static string MessageRelayGroup { get; } = typeof(ShapeBase).Namespace;

        /// <summary>
        /// Rounds the value to the number of digits if specified, otherwise to the default number of rounding digits
        /// </summary>
        /// <param name="value"></param>
        /// <param name="roundingDigits"></param>
        public static double R(double value, int? roundingDigits = null)
        {
            return Math.Round(value, roundingDigits ?? DefaultRoundingDigits);
        }

        /// <summary>
        /// Calculates the area of the shape
        /// </summary>
        /// <returns></returns>
        public abstract double CalculateArea();

        /// <summary>
        /// Calculates the perimiter or circumference of the shape
        /// </summary>
        /// <returns></returns>
        public abstract double CalculatePerimeter();
    }
}
