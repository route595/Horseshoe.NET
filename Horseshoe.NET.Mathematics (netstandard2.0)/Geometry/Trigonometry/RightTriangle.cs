using System;
using System.Text;

using Horseshoe.NET.RelayMessages;

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

        private int iteration = 0;
        private const int maxIterations = 3;

        /// <summary>
        /// Creates an empty <c>RightTriangle</c> instance.  
        /// <para>
        /// Note: This constructor exists primarily for serialization purposes.
        /// For a more complete experience please use the overloaded constructor.
        /// </para>
        /// </summary>
        public RightTriangle()
        {
            C = 90.0;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        /// <summary>
        /// Creates a <c>RightTriangle</c> instance
        /// </summary>
        /// <param name="a">Angle 'A'</param>
        /// <param name="b">Angle 'B'</param>
        /// <param name="c">Angle 'C', this is always the 90 degree angle even if the caller attempts to override.</param>
        /// <param name="ab">Side 'AB' (hypotenuse)</param>
        /// <param name="ac">Side 'AC' (adjacent to 'A')</param>
        /// <param name="bc">Side 'BC' (opposite 'A')</param>
        /// <param name="precision">Optional decimal rounding to use in calculations</param>
        /// <exception cref="Exception"></exception>
        public RightTriangle(double? a = null, double? b = null, double? c = 90.0, double? ab = null, double? ac = null, double? bc = null, int? precision = null) :
            base(a, b, 90.0, ab, ac, bc)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            Precision = precision;
            Init();

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
        }
#pragma warning restore IDE0060 // Remove unused parameter

        private void Init()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            // try to fill all missing values, max three rounds
            while (++iteration <= maxIterations)
            {
                if (IsComplete())
                    break;

                SystemMessageRelay.RelayMessage($"iteration: {iteration} of up to {maxIterations}", group: MessageRelayGroup);

                if (!A.HasValue)
                {
                    if (AC.HasValue && BC.HasValue)
                    {
                        A = Trig.RadiansFromAdjacentAndOpposite(AC.Value, BC.Value, precision: -1).RadiansToDegrees(precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting angle 'A' = {R(A.Value)}° (from adjacent side 'AC': {R(AC.Value)}, side 'BC' [opposite]: {R(BC.Value)})", group: MessageRelayGroup);
                        if (!HasAllAngles())
                        {
                            TryCompleteAngles();
                        }
                    }
                    else if (AC.HasValue && AB.HasValue)
                    {
                        A = Trig.RadiansFromAdjacentAndHypotenuse(AC.Value, AB.Value, precision: -1).RadiansToDegrees(precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting angle 'A' = {R(A.Value)}° (from adjacent side 'AC': {R(AC.Value)}, hypotenuse 'AB': {R(AB.Value)})", group: MessageRelayGroup);
                        if (!HasAllAngles())
                        {
                            TryCompleteAngles();
                        }
                    }
                    else if (BC.HasValue && AB.HasValue)
                    {
                        A = Trig.RadiansFromOppositeAndHypotenuse(BC.Value, AB.Value, precision: -1).RadiansToDegrees(precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting angle 'A' = {R(A.Value)}° (from opposite side 'BC': {R(BC.Value)}, hypotenuse 'AB': {R(AB.Value)})", group: MessageRelayGroup);
                        if (!HasAllAngles())
                        {
                            TryCompleteAngles();
                        }
                    }
                }

                if (!B.HasValue)
                {
                    if (BC.HasValue && AC.HasValue)
                    {
                        B = Trig.RadiansFromAdjacentAndOpposite(BC.Value, AC.Value, precision: -1).RadiansToDegrees(precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting angle 'B' = {R(B.Value)}° (from adjacent side 'BC': {R(BC.Value)}, opposite side 'AC': {R(AC.Value)})", group: MessageRelayGroup);
                        if (!HasAllAngles())
                        {
                            TryCompleteAngles();
                        }
                    }
                    else if (BC.HasValue && AB.HasValue)
                    {
                        B = Trig.RadiansFromAdjacentAndHypotenuse(BC.Value, AB.Value, precision: -1).RadiansToDegrees(precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting angle 'B' = {R(B.Value)}° (from adjacent side 'BC': {R(BC.Value)}, hypotenuse 'AB': {R(AB.Value)})", group: MessageRelayGroup);
                        if (!HasAllAngles())
                        {
                            TryCompleteAngles();
                        }
                    }
                    else if (AC.HasValue && AB.HasValue)
                    {
                        B = Trig.RadiansFromOppositeAndHypotenuse(AC.Value, AB.Value, precision: -1).RadiansToDegrees(precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting angle 'B' = {R(B.Value)}° (from opposite side 'AC': {R(AC.Value)}, hypotenuse 'AB': {R(AB.Value)})", group: MessageRelayGroup);
                        if (!HasAllAngles())
                        {
                            TryCompleteAngles();
                        }
                    }
                }

                if (!AB.HasValue)
                {
                    if (AC.HasValue && BC.HasValue)
                    {
                        AB = Trig.HypotenuseFromSides(AC.Value, BC.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting hypotenuse 'AB' = {R(AB.Value)} (from sides 'AC', 'BC': {R(AC.Value)}, {R(BC.Value)})", group: MessageRelayGroup);
                    }
                    else if (A.HasValue && AC.HasValue)
                    {
                        AB = Trig.HypotenuseFromRadiansAndAdjacent(A.Value.DegreesToRadians(), AC.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting hypotenuse 'AB' = {R(AB.Value)} (from angle 'A': {R(A.Value)}°, adjacent side 'AC': {R(AC.Value)})", group: MessageRelayGroup);
                    }
                    else if (A.HasValue && BC.HasValue)
                    {
                        AB = Trig.HypotenuseFromRadiansAndOpposite(A.Value.DegreesToRadians(), BC.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting hypotenuse 'AB' = {R(AB.Value)} (from angle 'A': {R(A.Value)}°, opposite side 'BC': {R(BC.Value)})", group: MessageRelayGroup);
                    }
                    else if (B.HasValue && BC.HasValue)
                    {
                        AB = Trig.HypotenuseFromRadiansAndAdjacent(B.Value.DegreesToRadians(), BC.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting hypotenuse 'AB' = {R(AB.Value)} (from angle 'B': {R(B.Value)}°, adjacent side 'BC': {R(BC.Value)})", group: MessageRelayGroup);
                    }
                    else if (B.HasValue && AC.HasValue)
                    {
                        AB = Trig.HypotenuseFromRadiansAndOpposite(B.Value.DegreesToRadians(), AC.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting hypotenuse 'AB' = {R(AB.Value)} (from angle 'B': {R(B.Value)}°, opposite side 'AC': {R(AC.Value)})", group: MessageRelayGroup);
                    }
                }

                if (!AC.HasValue)
                {
                    if (BC.HasValue && AB.HasValue)
                    {
                        AC = Trig.SideFromOtherSideAndHypotenuse(BC.Value, AB.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting side 'AC' = {R(AC.Value)} (from side 'BC': {R(BC.Value)}, hypotenuse 'AB': {R(AB.Value)})", group: MessageRelayGroup);
                    }
                    else if (A.HasValue && BC.HasValue)
                    {
                        AC = Trig.AdjacentFromRadiansAndOpposite(A.Value.DegreesToRadians(), BC.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting side 'AC' = {R(AC.Value)} (from angle 'A': {R(A.Value)}°, opposite side 'BC': {R(BC.Value)})", group: MessageRelayGroup);
                    }
                    else if (A.HasValue && AB.HasValue)
                    {
                        AC = Trig.AdjacentFromRadiansAndHypotenuse(A.Value.DegreesToRadians(), AB.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting side 'AC' = {R(AC.Value)} (from angle 'A': {R(A.Value)}°,  hypotenuse 'AB': {R(AB.Value)})", group: MessageRelayGroup);
                    }
                    else if (B.HasValue && BC.HasValue)
                    {
                        AC = Trig.OppositeFromRadiansAndAdjacent(B.Value.DegreesToRadians(), BC.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting side 'AC' = {R(AC.Value)} (from angle 'B': {R(B.Value)}°, adjacent side 'BC': {R(BC.Value)})", group: MessageRelayGroup);
                    }
                    else if (B.HasValue && AB.HasValue)
                    {
                        AC = Trig.OppositeFromRadiansAndHypotenuse(B.Value.DegreesToRadians(), AB.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting side 'AC' = {R(AC.Value)} (from angle 'B': {R(B.Value)}°,  hypotenuse 'AB': {R(AB.Value)})", group: MessageRelayGroup);
                    }
                }

                if (!BC.HasValue)
                {
                    if (AC.HasValue && AB.HasValue)
                    { 
                        BC = Trig.SideFromOtherSideAndHypotenuse(AC.Value, AB.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting side 'BC' = {R(BC.Value)} (from side 'AC': {R(AC.Value)}, hypotenuse 'AB': {R(AB.Value)})", group: MessageRelayGroup);
                    }
                    else if (B.HasValue && AC.HasValue)
                    {
                        BC = Trig.AdjacentFromRadiansAndOpposite(B.Value.DegreesToRadians(), AC.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting side 'BC' = {R(BC.Value)} (from angle 'B': {R(B.Value)}°, opposite side 'AC': {R(AC.Value)})", group: MessageRelayGroup);
                    }
                    else if (B.HasValue && AB.HasValue)
                    {
                        BC = Trig.AdjacentFromRadiansAndHypotenuse(B.Value.DegreesToRadians(), AB.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting side 'BC' = {R(BC.Value)} (from angle 'B': {R(B.Value)}°,  hypotenuse 'AB': {R(AB.Value)})", group: MessageRelayGroup);
                    }
                    else if (A.HasValue && AC.HasValue)
                    {
                        BC = Trig.OppositeFromRadiansAndAdjacent(A.Value.DegreesToRadians(), AC.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting side 'BC' = {R(BC.Value)} (from angle 'A': {R(A.Value)}°, adjacent side 'AC': {R(AC.Value)})", group: MessageRelayGroup);
                    }
                    else if (A.HasValue && AB.HasValue)
                    {
                        BC = Trig.OppositeFromRadiansAndHypotenuse(A.Value.DegreesToRadians(), AB.Value, precision: Precision);
                        SystemMessageRelay.RelayMessage($"setting side 'BC' = {R(BC.Value)} (from angle 'A': {R(A.Value)}°, hypotenuse 'AB': {R(AB.Value)})", group: MessageRelayGroup);
                    }
                }
            }

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
        }

        /// <inheritdoc cref="ShapeBase.CalculateArea"/>
        public override double CalculateArea()
        {
            if (AC.HasValue && BC.HasValue)
                return 0.5 * AC.Value * BC.Value;
            throw new GeomException("Insufficient information to calculate area relative to base side 'AC'");
        }

        /// <summary>
        /// Checks the lengths of the sides of this <c>RightTriangle</c> for equality.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GeomException"></exception>
        public override bool IsIsoscolese()
        {
            if (!AC.HasValue && !BC.HasValue)
                throw new GeomException("the lengths of sides 'AC' and/or 'BC' have not been set");
            return AC == BC;
        }

        /// <summary>
        /// Displays a diagram of the triangle labeled with angle degrees and side lengths, if applicable
        /// </summary>
        public override string ToString()
        {
            string a = (A.HasValue ? "A:" + A.ToString() + "°" : "").PadLeft(10);
            string b = B.HasValue ? B.ToString() + "°" : "";
            string ac = AC.HasValue ? (UnitsOfMeasurement == null ? "len=" + AC : AC + " " + UnitsOfMeasurement) : "";
            string bc = BC.HasValue ? (UnitsOfMeasurement == null ? "len=" + BC : BC + " " + UnitsOfMeasurement) : "";
            string ab = (AB.HasValue ? (UnitsOfMeasurement == null ? "len=" + AB : AB + " " + UnitsOfMeasurement) : "").PadLeft(11);
            return new StringBuilder()
                .AppendLine((IsComplete() ? "Complete" : "Incomplete") + $" triangle")
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
