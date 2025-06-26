using System;

namespace Horseshoe.NET.Mathematics.Geometry
{
    /// <summary>
    /// Geometric calculation utility methods
    /// </summary>
    internal static class CalcUtil
    {
        internal static double R(double valueOrExpressionResult, int? precision)
        {
            switch (precision)
            {
                case -1: 
                    return valueOrExpressionResult;
                default:
                    return Math.Round(valueOrExpressionResult, precision ?? CalcConstants.Precision);
            }
        }
    }
}
