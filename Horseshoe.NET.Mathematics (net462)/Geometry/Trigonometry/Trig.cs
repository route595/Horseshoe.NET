using System;

using static Horseshoe.NET.Mathematics.Geometry.CalcUtil;

namespace Horseshoe.NET.Mathematics.Geometry.Trigonometry
{
    /// <summary>
    /// A suite of common trigonometric calculations
    /// </summary>
    public static class Trig
    {
        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="opposite"></param>
        /// <param name="hypotenuse"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>sin(A) = opposite / hypotenuse</c></item>
        /// <item><c>A = sin^-1(opposite / hypotenuse)</c></item>
        /// </list>
        /// </remarks>
        public static double RadiansFromOppositeAndHypotenuse(double opposite, double hypotenuse, int? precision = null) =>
            R(RadiansFromOppositeAndHypotenuseInternal(opposite, hypotenuse), precision);

        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="adjacent"></param>
        /// <param name="hypotenuse"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>cos(A) = adjacent / hypotenuse</c></item>
        /// <item><c>A = cos^-1(adjacent / hypotenuse)</c></item>
        /// </list>
        /// </remarks>
        public static double RadiansFromAdjacentAndHypotenuse(double adjacent, double hypotenuse, int? precision = null) =>
            R(RadiansFromAdjacentAndHypotenuseInternal(adjacent, hypotenuse), precision);

        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="adjacent"></param>
        /// <param name="opposite"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>tan(A) = opposite / adjacent</c></item>
        /// <item><c>A = tan^-1(opposite / adjacent)</c></item>
        /// </list>
        /// </remarks>
        public static double RadiansFromAdjacentAndOpposite(double adjacent, double opposite, int? precision = null) =>
            R(RadiansFromAdjacentAndOppositeInternal(adjacent, opposite), precision);

        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="side"></param>
        /// <param name="otherSide"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>side^2 + otherSide^2 = hypotenuse^2</c></item>
        /// <item><c>hypotenuse = sqrt(side^2 + otherSide^2)</c></item>
        /// </list>
        /// </remarks>
        public static double HypotenuseFromSides(double side, double otherSide, int? precision = null) =>
            R(HypotenuseFromSidesInternal(side, otherSide), precision);

        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="otherSide"></param>
        /// <param name="hypotenuse"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>side^2 + otherSide^2 = hypotenuse^2</c></item>
        /// <item><c>side^2 = hypotenuse^2 - otherSide^2</c></item>
        /// <item><c>side = sqrt(hypotenuse^2 - otherSide^2)</c></item>
        /// </list>
        /// </remarks>
        public static double SideFromOtherSideAndHypotenuse(double otherSide, double hypotenuse, int? precision = null) =>
            R(SideFromOtherSideAndHypotenuseInternal(otherSide, hypotenuse), precision);

        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="radians"></param>
        /// <param name="opposite"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>sin(radians) = opposite / hypotenuse</c></item>
        /// <item><c>sin(radians) * hypotenuse = opposite</c></item>
        /// <item><c>hypotenuse = opposite / sin(radians)</c></item>
        /// </list>
        /// </remarks>
        public static double HypotenuseFromRadiansAndOpposite(double radians, double opposite, int? precision = null) =>
            R(HypotenuseFromRadiansAndOppositeInternal(radians, opposite), precision);

        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="radians"></param>
        /// <param name="adjacent"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>cos(radians) = adjacent / hypotenuse</c></item>
        /// <item><c>cos(radians) * hypotenuse = adjacent</c></item>
        /// <item><c>hypotenuse = adjacent / cos(radians)</c></item>
        /// </list>
        /// </remarks>
        public static double HypotenuseFromRadiansAndAdjacent(double radians, double adjacent, int? precision = null) =>
            R(HypotenuseFromRadiansAndAdjacentInternal(radians, adjacent), precision);

        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="radians"></param>
        /// <param name="adjacent"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>tan(radians) = opposite / adjacent</c></item>
        /// <item><c>opposite = tan(radians) * adjacent</c></item>
        /// </list>
        /// </remarks>
        public static double OppositeFromRadiansAndAdjacent(double radians, double adjacent, int? precision = null) =>
            R(OppositeFromRadiansAndAdjacentInternal(radians, adjacent), precision);

        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="radians"></param>
        /// <param name="hypotenuse"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>sin(radians) = opposite / hypotenuse</c></item>
        /// <item><c>opposite = sin(radians) * hypotenuse</c></item>
        /// </list>
        /// </remarks>
        public static double OppositeFromRadiansAndHypotenuse(double radians, double hypotenuse, int? precision = null) =>
            R(OppositeFromRadiansAndHypotenuseInternal(radians, hypotenuse), precision);

        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="radians"></param>
        /// <param name="opposite"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>tan(radians) = opposite / adjacent</c></item>
        /// <item><c>tan(radians) * adjacent = opposite</c></item>
        /// <item><c>adjacent = opposite / tan(radians)</c></item>
        /// </list>
        /// </remarks>
        public static double AdjacentFromRadiansAndOpposite(double radians, double opposite, int? precision = null) =>
            R(AdjacentFromRadiansAndOppositeInternal(radians, opposite), precision);

        /// <summary>
        /// <code>
        ///                /|
        ///               / |
        /// (hypotenuse) /  |
        ///             /   | (opposite)
        ///            /   _|
        ///           /)__|_| 
        ///          A    (adjacent)
        /// </code>
        /// </summary>
        /// <param name="radians"></param>
        /// <param name="hypotenuse"></param>
        /// <param name="precision">The decimal precision to use in the rounded return value, <c>-1</c> prevents rounding, default is <c>3</c>.</param>
        /// <remarks>
        /// <list type="table">
        /// <item><c>cos(radians) = adjacent / hypotenuse</c></item>
        /// <item><c>adjacent = cos(radians) * hypotenuse</c></item>
        /// </list>
        /// </remarks>
        public static double AdjacentFromRadiansAndHypotenuse(double radians, double hypotenuse, int? precision = null) =>
            R(AdjacentFromRadiansAndHypotenuseInternal(radians, hypotenuse), precision);

        // sin(A) = opposite / hypotenuse
        // A = sin^-1(opposite / hypotenuse)
        private static double RadiansFromOppositeAndHypotenuseInternal(double opposite, double hypotenuse) =>
            Math.Asin(opposite / hypotenuse);

        // cos(A) = adjacent / hypotenuse
        // A = cos^-1(adjacent / hypotenuse)
        private static double RadiansFromAdjacentAndHypotenuseInternal(double adjacent, double hypotenuse) =>
            Math.Acos(adjacent / hypotenuse);

        // tan(A) = opposite / adjacent
        // A = tan^-1(opposite / adjacent)
        private static double RadiansFromAdjacentAndOppositeInternal(double adjacent, double opposite) =>
            Math.Atan(opposite / adjacent);

        // side^2 + otherSide^2 = hypotenuse^2
        // hypotenuse = sqrt(side^2 + otherSide^2)
        private static double HypotenuseFromSidesInternal(double side, double otherSide) =>
            Math.Sqrt(Math.Pow(side, 2.0) + Math.Pow(otherSide, 2.0));

        // side^2 + otherSide^2 = hypotenuse^2
        // side^2 = hypotenuse^2 - otherSide^2
        // side = sqrt(hypotenuse^2 - otherSide^2)
        private static double SideFromOtherSideAndHypotenuseInternal(double otherSide, double hypotenuse) =>
            Math.Sqrt(Math.Pow(hypotenuse, 2.0) - Math.Pow(otherSide, 2.0));

        // sin(radians) = opposite / hypotenuse
        // sin(radians) * hypotenuse = opposite
        // hypotenuse = opposite / sin(radians)
        private static double HypotenuseFromRadiansAndOppositeInternal(double radians, double opposite) =>
            opposite / Math.Sin(radians);

        // cos(radians) = adjacent / hypotenuse
        // cos(radians) * hypotenuse = adjacent
        // hypotenuse = adjacent / cos(radians)
        private static double HypotenuseFromRadiansAndAdjacentInternal(double radians, double adjacent) =>
            adjacent / Math.Cos(radians);

        // tan(radians) = opposite / adjacent
        // opposite = tan(radians) * adjacent
        private static double OppositeFromRadiansAndAdjacentInternal(double radians, double adjacent) =>
            Math.Tan(radians) * adjacent;

        // sin(radians) = opposite / hypotenuse
        // opposite = sin(radians) * hypotenuse
        private static double OppositeFromRadiansAndHypotenuseInternal(double radians, double hypotenuse) =>
            Math.Sin(radians) * hypotenuse;

        // tan(radians) = opposite / adjacent
        // tan(radians) * adjacent = opposite
        // adjacent = opposite / tan(radians)
        private static double AdjacentFromRadiansAndOppositeInternal(double radians, double opposite) =>
            opposite / Math.Tan(radians);

        // cos(radians) = adjacent / hypotenuse
        // adjacent = cos(radians) * hypotenuse
        private static double AdjacentFromRadiansAndHypotenuseInternal(double radians, double hypotenuse) =>
            Math.Cos(radians) * hypotenuse;
    }
}
