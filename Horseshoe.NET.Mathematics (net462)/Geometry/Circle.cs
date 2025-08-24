using System;

using Horseshoe.NET.Mathematics.Geometry.Trigonometry;
using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Mathematics.Geometry
{
    /// <summary>
    /// Describes a circle.
    /// </summary>
    /// <remarks>
    /// <code>
    ///        ----
    ///     -        -
    ///   /            \
    ///  |       _______|
    ///  |          R   |
    ///   \            /
    ///     -        -
    ///        ----
    /// </code>
    /// </remarks>
    public class Circle : ShapeBase
    {
        /// <summary>
        /// Radius 'R'
        /// </summary>
        public double? R { get; internal set; }

        /// <inheritdoc cref="ShapeBase.NumberOfAngles"/>
        public override double NumberOfAngles => 0;

        /// <inheritdoc cref="ShapeBase.NumberOfSides"/>
        public override double NumberOfSides => double.PositiveInfinity;

        /// <summary>
        /// Triangle constructor.  All parameters are optional, supply the ones you know and the rest will attempt to be calculated.
        /// </summary>
        /// <param name="r">Radius 'R"</param>
        /// <param name="unitsOfMeasurement">Units of length used for display, e.g. 'cm' -&gt; '34 cm'.  Default is <c>null</c> -&gt; 'len=34'.</param>
        /// <exception cref="GeomException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Circle(double? r = null, string unitsOfMeasurement = null)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            // geom measurements
            R = r ?? throw new ArgumentNullException(nameof(r));

            // units of lengths
            UnitsOfMeasurement = unitsOfMeasurement;

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
        }

        /// <inheritdoc cref="ShapeBase.CalculateArea"/>
        public override double CalculateArea()
        {
            if (R.HasValue)
                return Math.PI * Math.Pow(R.Value, 2.0);
            throw new GeomException("Insufficient information to calculate area");
        }

        /// <inheritdoc cref="ShapeBase.CalculatePerimeter"/>
        public override double CalculatePerimeter()
        {
            if (R.HasValue)
                return Math.PI * R.Value * 2.0;
            throw new GeomException("Insufficient information to calculate perimeter");
        }

        /// <summary>
        /// See <c>CalculatePerimeter()</c>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GeomException"></exception>
        public double CalculateCircumference()
        {
            try
            {
                return CalculatePerimeter();
            }
            catch (GeomException)
            {
                throw new GeomException("Insufficient information to calculate circumference");
            }
        }
    }
}
