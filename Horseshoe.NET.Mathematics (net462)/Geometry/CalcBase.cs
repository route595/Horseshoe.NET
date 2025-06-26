using System;

namespace Horseshoe.NET.Mathematics.Geometry
{
    public abstract class CalcBase
    {
        public static int DefaultRoundingDigits { get; set; } = 6;

        public static double R(double value, int? roundingDigits = null)
        {
            return Math.Round(value, roundingDigits ?? DefaultRoundingDigits);
        }
    }
}
