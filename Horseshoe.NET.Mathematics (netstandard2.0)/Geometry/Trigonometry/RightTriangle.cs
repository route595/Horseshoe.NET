using System;
using System.Text;

namespace Horseshoe.NET.Mathematics.Geometry.Trigonometry
{
    /// <summary>
    /// A triangle with exactly one right angle where theta is generally represented by angle, the object of the study of trigonometry.
    /// </summary>
    /// <remarks>
    /// <code>
    ///                 B
    ///                /|
    ///               / |
    /// (hypotenuse) /  |
    ///          AB /   | BC
    ///            /    | (opposite A)
    ///           /     |
    ///       |  /     _|
    ///       V /)____|_| 
    ///   --> A    AC    C
    ///        (adjacent to A)
    /// </code>
    /// </remarks>
    public class RightTriangle : Triangle
    {
        /// <summary>
        /// The decimal rounding to use in calculations
        /// </summary>
        public int? Precision { get; }

        private int iterations = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">Angle 'A'</param>
        /// <param name="b">Angle 'B'</param>
        /// <param name="c">Angle 'C', this is the 90 degree angle</param>
        /// <param name="ab">Side 'AB' (hypotenuse)</param>
        /// <param name="ac">Side 'AC' (adjacent to 'A')</param>
        /// <param name="bc">Side 'BC' (opposite 'A')</param>
        /// <param name="precision">Optional decimal rounding to use in calculations</param>
        /// <exception cref="Exception"></exception>
        public RightTriangle(double? a = null, double? b = null, double? c = 90.0, double? ab = null, double? ac = null, double? bc = null, int? precision = null) :
            base(a, b, c, ab, ac, bc)
        {
            Precision = precision;
            Init();
        }

        private void Init()
        {
            if (C != 90.0)
                throw new Exception("In this right triangle representation, angle C must be 90 degrees");

            // try to fill all missing values, max three rounds
            for (int i = 0; i < 3; i++)
            {
                if (IsComplete())
                    break;

                if (!A.HasValue)
                {
                    if (AC.HasValue && BC.HasValue)
                        A = Trig.RadiansFromAdjacentAndOpposite(AC.Value, BC.Value, precision: -1).RadiansToDegrees(precision: Precision);
                    else if (AC.HasValue && AB.HasValue)
                        A = Trig.RadiansFromAdjacentAndHypotenuse(AC.Value, AB.Value, precision: -1).RadiansToDegrees(precision: Precision);
                    else if (BC.HasValue && AB.HasValue)
                        A = Trig.RadiansFromOppositeAndHypotenuse(BC.Value, AB.Value, precision: -1).RadiansToDegrees(precision: Precision);
                }

                if (!B.HasValue)
                {
                    if (BC.HasValue && AC.HasValue)
                        B = Trig.RadiansFromAdjacentAndOpposite(BC.Value, AC.Value, precision: -1).RadiansToDegrees(precision: Precision);
                    else if (BC.HasValue && AB.HasValue)
                        B = Trig.RadiansFromAdjacentAndHypotenuse(BC.Value, AB.Value, precision: -1).RadiansToDegrees(precision: Precision);
                    else if (AC.HasValue && AB.HasValue)
                        B = Trig.RadiansFromOppositeAndHypotenuse(AC.Value, AB.Value, precision: -1).RadiansToDegrees(precision: Precision);
                }

                if (!AB.HasValue)
                {
                    if (AC.HasValue && BC.HasValue)
                        AB = Trig.HypotenuseFromSides(AC.Value, BC.Value, precision: Precision);
                    else if (A.HasValue && AC.HasValue)
                        AB = Trig.HypotenuseFromRadiansAndAdjacent(A.Value.DegreesToRadians(), AC.Value, precision: Precision);
                    else if (A.HasValue && BC.HasValue)
                        AB = Trig.HypotenuseFromRadiansAndOpposite(A.Value.DegreesToRadians(), BC.Value, precision: Precision);
                    else if (B.HasValue && BC.HasValue)
                        AB = Trig.HypotenuseFromRadiansAndAdjacent(B.Value.DegreesToRadians(), BC.Value, precision: Precision);
                    else if (B.HasValue && AC.HasValue)
                        AB = Trig.HypotenuseFromRadiansAndOpposite(B.Value.DegreesToRadians(), AC.Value, precision: Precision);
                }

                if (!AC.HasValue)
                {
                    if (BC.HasValue && AB.HasValue)
                        AC = Trig.SideFromOtherSideAndHypotenuse(BC.Value, AB.Value, precision: Precision);
                    else if (A.HasValue && BC.HasValue)
                        AC = Trig.AdjacentFromRadiansAndOpposite(A.Value.DegreesToRadians(), BC.Value, precision: Precision);
                    else if (A.HasValue && AB.HasValue)
                        AC = Trig.AdjacentFromRadiansAndHypotenuse(A.Value.DegreesToRadians(), AB.Value, precision: Precision);
                    else if (B.HasValue && BC.HasValue)
                        AC = Trig.OppositeFromRadiansAndAdjacent(B.Value.DegreesToRadians(), BC.Value, precision: Precision);
                    else if (B.HasValue && AB.HasValue)
                        AC = Trig.OppositeFromRadiansAndHypotenuse(B.Value.DegreesToRadians(), AB.Value, precision: Precision);
                }

                if (!BC.HasValue)
                {
                    if (AC.HasValue && AB.HasValue)
                        BC = Trig.SideFromOtherSideAndHypotenuse(AC.Value, AB.Value, precision: Precision);
                    else if (B.HasValue && AC.HasValue)
                        BC = Trig.AdjacentFromRadiansAndOpposite(B.Value.DegreesToRadians(), AC.Value, precision: Precision);
                    else if (B.HasValue && AB.HasValue)
                        BC = Trig.AdjacentFromRadiansAndHypotenuse(B.Value.DegreesToRadians(), AB.Value, precision: Precision);
                    else if (A.HasValue && AC.HasValue)
                        BC = Trig.OppositeFromRadiansAndAdjacent(A.Value.DegreesToRadians(), AC.Value, precision: Precision);
                    else if (A.HasValue && AB.HasValue)
                        BC = Trig.OppositeFromRadiansAndHypotenuse(A.Value.DegreesToRadians(), AB.Value, precision: Precision);
                }

                iterations++;
            }
        }

        /// <summary>
        /// Displays a diagram of the triangle labeled with values of angles (degrees) and sides, if applicable
        /// </summary>
        public override string ToString()
        {
            string a = (A.HasValue ? "A:" + A.ToString() + "°" : "").PadLeft(10);
            string b = B.HasValue ? B.ToString() + "°" : "";
            string ac = AC.HasValue ? (UnitsOfLength == null ? "len=" + AC : AC + " " + UnitsOfLength) : "";
            string bc = BC.HasValue ? (UnitsOfLength == null ? "len=" + BC : BC + " " + UnitsOfLength) : "";
            string ab = (AB.HasValue ? (UnitsOfLength == null ? "len=" + AB : AB + " " + UnitsOfLength) : "").PadLeft(11);
            return new StringBuilder()
                .AppendLine((IsComplete() ? "Complete" : "Incomplete") + $" triangle: ({iterations} {(iterations == 1 ? "iteration" : "iterations")})")
                .AppendLine($"              B:{b}")
                .AppendLine( "               /|")
                .AppendLine( " (hypotenuse) / |")
                .AppendLine( "          AB /  | BC")
                .AppendLine(       $"{ab} /   | {bc}")
                .AppendLine( "           /   _|")
                .AppendLine( "          /)__|_|")
                .AppendLine(       $"{a}  AC   C:90°")
                .AppendLine($"            {ac}")
                .ToString();
        }
    }
}
