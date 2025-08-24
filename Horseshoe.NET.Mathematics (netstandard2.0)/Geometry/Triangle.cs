using System;

using Horseshoe.NET.Mathematics.Geometry.Trigonometry;
using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Mathematics.Geometry
{
    /// <summary>
    /// Describes a triangle, which is a polygon with three edges and three vertices. 
    /// Contains some calculation logic to determine missing angles and sides.
    /// </summary>
    /// <remarks>
    /// <code>
    ///                B                                        B                        B
    ///               /\                                      //                        /|
    ///              /  \                                  /  /                        / |
    ///             /    \                              /    /           (hypotenuse) /  |
    ///            /      \ BC        -or-        AB /      /     -or-            AB /   |
    ///        AB /        \                      /        / BC                     /    | (opposite)
    ///          /          \                  /          /                        /     | BC
    ///       | /            \          |   /            /                      | /     _|
    ///       V/)____________(\         V/)_____________/                       V/)____|_| 
    ///   --> A       AC       C    --> A       AC      C                   --> A   AC    C
    /// </code>
    /// </remarks>
    public class Triangle : ShapeBase
    {
        /// <summary>
        /// Angle 'A' in°
        /// </summary>
        public double? A { get; internal set; }

        /// <summary>
        /// Angle 'B' in°
        /// </summary>
        public double? B { get; internal set; }

        /// <summary>
        /// Angle 'C' in° (90° in right triangles)
        /// </summary>
        public double? C { get; internal set; }

        /// <summary>
        /// Length of side 'A' to 'B' (adjacent to 'A' and also the hypoteneuse in right triangles)
        /// </summary>
        public double? AB { get; internal set; }

        /// <summary>
        /// Length of side 'A' to 'C' (adjacent to 'A')
        /// </summary>
        public double? AC { get; internal set; }

        /// <summary>
        /// Length of side 'B' to 'C' (opposite 'A')
        /// </summary>
        public double? BC { get; internal set; }

        /// <inheritdoc cref="ShapeBase.NumberOfAngles"/>
        public override double NumberOfAngles => 3;

        /// <inheritdoc cref="ShapeBase.NumberOfSides"/>
        public override double NumberOfSides => 3;

        /// <summary>
        /// Triangle constructor.  All parameters are optional, supply the ones you know and the rest will attempt to be calculated.
        /// </summary>
        /// <param name="a">Angle 'A' in°</param>
        /// <param name="b">Angle 'B' in°</param>
        /// <param name="c">Angle 'C' in°</param>
        /// <param name="ab">Length of side 'A' to 'B'</param>
        /// <param name="ac">Length of side 'A' to 'C'</param>
        /// <param name="bc">Length of side 'B' to 'C'</param>
        /// <param name="unitsOfMeasurement">Units of length used for display, e.g. 'cm' -&gt; '34 cm'.  Default is <c>null</c> -&gt; 'len=34'.</param>
        /// <exception cref="GeomException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Triangle (double? a = null, double? b = null, double? c = null, double? ab = null, double? ac = null, double? bc = null, string unitsOfMeasurement = null)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            // geom measurements
            // basic angle validation
            A = !a.HasValue || (a > 0.0 && a < 180.0) ? a : throw new ArgumentOutOfRangeException(nameof(a), "Angle 'A' of this triangle may only be between 0° and 180° (exclusive)");
            B = !b.HasValue || (b > 0.0 && b < 180.0) ? b : throw new ArgumentOutOfRangeException(nameof(b), "Angle 'B' of this triangle may only be between 0° and 180° (exclusive)");
            C = !c.HasValue || (c > 0.0 && c < 180.0) ? c : throw new ArgumentOutOfRangeException(nameof(c), "Angle 'C' of this triangle may only be between 0° and 180° (exclusive)");

            // basic side validation
            AB = !ab.HasValue || ab > 0.0 ? ab : throw new ArgumentOutOfRangeException(nameof(ab), "Side 'AB' of this triangle cannot be <= 0 units of length");
            AC = !ac.HasValue || ac > 0.0 ? ac : throw new ArgumentOutOfRangeException(nameof(ac), "Side 'AC' of this triangle cannot be <= 0 units of length");
            BC = !bc.HasValue || bc > 0.0 ? bc : throw new ArgumentOutOfRangeException(nameof(bc), "Side 'BC' of this triangle cannot be <= 0 units of length");

            // units of lengths
            UnitsOfMeasurement = unitsOfMeasurement;

            Init();
            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
        }

        private void Init()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            // More angle validation
            if ((!A.HasValue && R((B ?? 0.0) + (C ?? 0.0)) >= 180.0) || (!B.HasValue && R((A ?? 0.0) + (C ?? 0.0)) >= 180.0) || (!C.HasValue && R((A ?? 0.0) + (B ?? 0.0)) >= 180.0))
                throw new GeomException("The partial sum of angles of a triangle may not >= 180°");
            if (A.HasValue && B.HasValue && C.HasValue && R(A.Value + B.Value + C.Value) != 180.0)
                throw new GeomException("The sum of all angles of a triangle must exactly = 180°");

            if (!HasAllAngles())
            {
                TryCompleteAngles();
            }

            // Cannot continue if no sides
            if (!BC.HasValue && !AC.HasValue && !AB.HasValue)
            {
                SystemMessageRelay.RelayMethodReturn("exiting due to no sides defined", group: MessageRelayGroup);
                return;
            }

            // Equilateral triangle based on equal angles
            if (A == 60.0 && B == 60.0 && C == 60.0)
            {
                double side = (BC ?? AC ?? AB).Value;  // guaranteed to have value, see 'Cannot continue if no sides' above
                if (!AB.HasValue)
                {
                    AB = side;
                    SystemMessageRelay.RelayMessage($"set side 'AB' = {R(AB.Value)} (equilateral sides based on equal angles)", group: MessageRelayGroup);
                }
                else if (AB != side)
                    throw new GeomException($"Side 'AB' must be equal in length to the other sides given this is an equilateral triangle based on equal angles: {AB}, {side}");
                if (!AC.HasValue)
                {
                    AC = side;
                    SystemMessageRelay.RelayMessage($"set side 'AC' = {R(AC.Value)} (equilateral sides based on equal angles)", group: MessageRelayGroup);
                }
                else if (AC != side)
                    throw new GeomException($"Side 'AC' must be equal in length to the other sides given this is an equilateral triangle based on equal angles: {AC}, {side}");
                if (!BC.HasValue)
                {
                    BC = side;
                    SystemMessageRelay.RelayMessage($"set side 'BC' = {R(BC.Value)} (equilateral sides based on equal angles)", group: MessageRelayGroup);
                }
                else if (BC != side)
                    throw new GeomException($"Side 'BC' must be equal in length to the other sides given this is an equilateral triangle based on equal angles: {BC}, {side}");
                
                SystemMessageRelay.RelayMethodReturn("detected unilateral triangle based on equal angles", group: MessageRelayGroup);
                return;
            }

            // if all sides present
            if (AB.HasValue && AC.HasValue && BC.HasValue)
            {
                // unilateral triangle based on equal sides
                if (AB == AC && AC == BC)
                {
                    if (!A.HasValue)
                    {
                        A = 60.0;
                        SystemMessageRelay.RelayMessage($"set angle 'A' = {R(A.Value)} (equal angles based on equal sides)", group: MessageRelayGroup);
                    }
                    else if (A != 60.0)
                        throw new GeomException($"Angle 'A' must = 60° given this is an equilateral triangle based on equal sides: {A}");
                    if (!B.HasValue)
                    {
                        B = 60.0;
                        SystemMessageRelay.RelayMessage($"set angle 'B' = {R(B.Value)} (equal angles based on equal sides)", group: MessageRelayGroup);
                    }
                    else if (B != 60.0)
                        throw new GeomException($"Angle 'B' must = 60° given this is an equilateral triangle based on equal sides: {B}");
                    if (!C.HasValue)
                    {
                        C = 60.0;
                        SystemMessageRelay.RelayMessage($"set angle 'C' = {R(C.Value)} (equal angles based on equal sides)", group: MessageRelayGroup);
                    }
                    else if (C != 60.0)
                        throw new GeomException($"Angle 'C' must = 60° given this is an equilateral triangle based on equal sides: {C}");
                    
                    SystemMessageRelay.RelayMethodReturn("detected unilateral triangle based on equal sides", group: MessageRelayGroup);
                    return;
                }

                // right triangle calculations - defer actual instances of RightTriangle to RightTriangle.Init()
                if (!(this is RightTriangle) && HasPythagoreanRatio())
                {
                    SystemMessageRelay.RelayMessage("side lengths indicate this is a right triangle (pythagorean ratio)", group: MessageRelayGroup);

                    double longestSide = Math.Max(BC.Value, Math.Max(AC.Value, AB.Value));
                    double calculatedAngle;

                    if (AB.Value == longestSide)
                    {
                        SystemMessageRelay.RelayMessage("side 'AB' is hypotenuse", group: MessageRelayGroup);

                        if (!C.HasValue)
                        {
                            C = 90.0;
                            SystemMessageRelay.RelayMessage("set angle 'C' = 90°", group: MessageRelayGroup);
                            if (!HasAllAngles())
                            {
                                TryCompleteAngles();
                            }
                        }
                        else if (C != 90.0)
                            throw new GeomException($"Angle 'C' = {R(C.Value)}°, however, the right triangle-based calculation would be 90°");

                        calculatedAngle = Trig.RadiansFromAdjacentAndHypotenuse(AC.Value, AB.Value).RadiansToDegrees();
                        if (!A.HasValue)
                        {
                            A = calculatedAngle;
                            SystemMessageRelay.RelayMessage($"set angle 'A' = {R(calculatedAngle)}°", group: MessageRelayGroup);
                            if (!HasAllAngles())
                            {
                                TryCompleteAngles();
                            }
                        }
                        else if (R(A.Value) != R(calculatedAngle))
                            throw new GeomException($"Angle 'A' = {R(A.Value)}°, however, the right triangle-based calculation would be {R(calculatedAngle)}°");

                        calculatedAngle = Trig.RadiansFromAdjacentAndHypotenuse(BC.Value, AB.Value).RadiansToDegrees();
                        if (!B.HasValue)
                        {
                            B = calculatedAngle;
                            SystemMessageRelay.RelayMessage($"set angle 'B' = {R(calculatedAngle)}°", group: MessageRelayGroup);
                            if (!HasAllAngles())
                            {
                                TryCompleteAngles();
                            }
                        }
                        else if (R(B.Value) != R(calculatedAngle))
                            throw new GeomException($"Angle 'B' = {R(B.Value)}°, however, the right triangle-based calculation would be {R(calculatedAngle)}°");

                        SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                        return;
                    }

                    if (AC.Value == longestSide)
                    {
                        SystemMessageRelay.RelayMessage("side 'AC' is hypotenuse", group: MessageRelayGroup);

                        if (!B.HasValue)
                        {
                            B = 90.0;
                            SystemMessageRelay.RelayMessage("set angle 'B' = 90°", group: MessageRelayGroup);
                            if (!HasAllAngles())
                            {
                                TryCompleteAngles();
                            }
                        }
                        else if (B != 90.0)
                            throw new GeomException($"Angle 'B' = {R(B.Value)}°, however, the right triangle-based calculation would be 90°");

                        calculatedAngle = Trig.RadiansFromAdjacentAndHypotenuse(AB.Value, AC.Value).RadiansToDegrees();
                        if (!A.HasValue)
                        {
                            A = calculatedAngle;
                            SystemMessageRelay.RelayMessage($"set angle 'A' = {R(calculatedAngle)}°", group: MessageRelayGroup);
                            if (!HasAllAngles())
                            {
                                TryCompleteAngles();
                            }
                        }
                        else if (R(A.Value) != R(calculatedAngle))
                            throw new GeomException($"Angle 'A' = {R(A.Value)}°, however, the right triangle-based calculation would be {R(calculatedAngle)}°");

                        calculatedAngle = Trig.RadiansFromAdjacentAndHypotenuse(BC.Value, AC.Value).RadiansToDegrees();
                        if (!C.HasValue)
                        {
                            C = calculatedAngle;
                            SystemMessageRelay.RelayMessage($"set angle 'C' = {R(calculatedAngle)}°", group: MessageRelayGroup);
                            if (!HasAllAngles())
                            {
                                TryCompleteAngles();
                            }
                        }
                        else if (R(C.Value) != R(calculatedAngle))
                            throw new GeomException($"Angle 'C' = {R(C.Value)}°, however, the right triangle-based calculation would be {R(calculatedAngle)}°");

                        SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                        return;
                    }

                    if (BC.Value == longestSide)
                    {
                        SystemMessageRelay.RelayMessage("side 'BC' is hypotenuse", group: MessageRelayGroup);

                        if (!A.HasValue)
                        {
                            A = 90.0;
                            SystemMessageRelay.RelayMessage("set angle 'A' = 90°", group: MessageRelayGroup);
                            if (!HasAllAngles())
                            {
                                TryCompleteAngles();
                            }
                        }
                        else if (A != 90.0)
                            throw new GeomException($"Angle 'A' = {R(A.Value)}°, however, the right triangle-based calculation would be 90°");

                        calculatedAngle = Trig.RadiansFromAdjacentAndHypotenuse(AB.Value, BC.Value).RadiansToDegrees();
                        if (!B.HasValue)
                        {
                            B = calculatedAngle;
                            SystemMessageRelay.RelayMessage($"set angle 'B' = {R(calculatedAngle)}°", group: MessageRelayGroup);
                            if (!HasAllAngles())
                            {
                                TryCompleteAngles();
                            }
                        }
                        else if (R(B.Value) != R(calculatedAngle))
                            throw new GeomException($"Angle 'B' = {R(B.Value)}°, however, the right triangle-based calculation would be {R(calculatedAngle)}°");

                        calculatedAngle = Trig.RadiansFromAdjacentAndHypotenuse(AC.Value, BC.Value).RadiansToDegrees();
                        if (!C.HasValue)
                        {
                            C = calculatedAngle;
                            SystemMessageRelay.RelayMessage($"set angle 'C' = {R(calculatedAngle)}°", group: MessageRelayGroup);
                            if (!HasAllAngles())
                            {
                                TryCompleteAngles();
                            }
                        }
                        else if (R(C.Value) != R(calculatedAngle))
                            throw new GeomException($"Angle 'C' = {R(C.Value)}°, however, the right triangle-based calculation would be {R(calculatedAngle)}°");

                        SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                        return;
                    }
                }
            }

            //// Cannot continue if no angles
            //if (!A.HasValue && !B.HasValue && !C.HasValue)
            //{
            //    return;
            //}
            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
        }

        /// <inheritdoc cref="ShapeBase.CalculateArea"/>
        public override double CalculateArea()
        {
            if (AC.HasValue)
                return 0.5 * AC.Value * CalculateHeight();
            throw new GeomException("Insufficient information to calculate area relative to base side 'AC'");
        }

        /// <inheritdoc cref="ShapeBase.CalculatePerimeter"/>
        public override double CalculatePerimeter()
        {
            if (HasAllSides())
                return AB.Value + AC.Value + BC.Value;
            throw new GeomException("Insufficient information to calculate perimeter");
        }

        protected void TryCompleteAngles()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            // Calculate third angle given the other two, if applicable
            if (!A.HasValue && B.HasValue && C.HasValue)
            {
                A = R(180.0 - B.Value - C.Value);
                SystemMessageRelay.RelayMessage($"set angle 'A' = {R(A.Value)}° (from known angles: {R(B.Value)}°, {R(C.Value)}°)", group: MessageRelayGroup);
            }
            else if (!B.HasValue && A.HasValue && C.HasValue)
            {
                B = R(180.0 - A.Value - C.Value);
                SystemMessageRelay.RelayMessage($"set angle 'B' = {R(B.Value)}° (from known angles: {R(A.Value)}°, {R(C.Value)}°)", group: MessageRelayGroup);
            }
            else if (!C.HasValue && A.HasValue && B.HasValue)
            {
                C = R(180.0 - A.Value - B.Value);
                SystemMessageRelay.RelayMessage($"set angle 'C' = {R(C.Value)}° (from known angles: {R(A.Value)}°, {R(B.Value)}°)", group: MessageRelayGroup);
            }

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
        }

        /// <summary>
        /// Checks if all the angles of this <c>Triangle</c> have been set or calculated.
        /// </summary>
        public bool HasAllAngles()
        {
            return A.HasValue && B.HasValue && C.HasValue;
        }

        /// <summary>
        /// Checks if all the sides of this <c>Triangle</c> have been set or calculated.
        /// </summary>
        public bool HasAllSides()
        {
            return AB.HasValue && AC.HasValue && BC.HasValue;
        }

        /// <summary>
        /// Checks if all the sides and angles of this <c>Triangle</c> have been set or calculated.
        /// </summary>
        public bool IsComplete()
        {
            return HasAllAngles() && HasAllSides();
        }

        /// <summary>
        /// Checks the lengths of the sides of this <c>Triangle</c> for equality.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GeomException"></exception>
        public virtual bool IsIsoscolese()
        {
            if (!BC.HasValue)
            {
                if (!AB.HasValue) throw new GeomException("the lengths of sides 'AB' and 'BC' have not been set");
                if (!AC.HasValue) throw new GeomException("the lengths of sides 'AC' and 'BC' have not been set");
                return AC == AB;
            }
            else if (!AC.HasValue)
            {
                if (!AB.HasValue) throw new GeomException("the lengths of sides 'AB' and 'AC' have not been set");
                return AB == AC;
            }
            return BC == AC;
        }

        /// <summary>
        /// Checks the side lengths of this <c>Triangle</c> for equality.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GeomException"></exception>
        public bool IsEquilateral()
        {
            if (!AB.HasValue) throw new GeomException("the length of side 'AB' has not been set");
            if (!AC.HasValue) throw new GeomException("the length of side 'AC' has not been set");
            if (!BC.HasValue) throw new GeomException("the length of side 'BC' has not been set");
            return AB == AC && AC == BC;
        }

        /// <summary>
        /// Checks the side length ratios of this <c>Triangle</c> using the Pythagorean theorum.  
        /// This can indicate whether the current <c>Triangle</c> is a right triangle.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GeomException"></exception>
        public bool HasPythagoreanRatio()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (HasAllSides())
            {
                var longestSide = MathUtil.Max(AB.Value, AC.Value, BC.Value);
                if (AB == longestSide)
                    return R(Math.Pow(AC.Value, 2.0) + Math.Pow(BC.Value, 2.0)) == R(Math.Pow(AB.Value, 2.0));
                if (AC == longestSide)
                    return R(Math.Pow(AB.Value, 2.0) + Math.Pow(BC.Value, 2.0)) == R(Math.Pow(AC.Value, 2.0));
                if (BC == longestSide)
                    return R(Math.Pow(AB.Value, 2.0) + Math.Pow(AC.Value, 2.0)) == R(Math.Pow(BC.Value, 2.0));
            }
            if (!AB.HasValue) throw new GeomException("the length of side 'AB' has not been set");
            if (!AC.HasValue) throw new GeomException("the length of side 'AC' has not been set");
            if (!BC.HasValue) throw new GeomException("the length of side 'BC' has not been set");

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
            return false;
        }

        /// <summary>
        /// Calculates the height of this triangle relative to base side 'AC'.
        /// <code>
        ///                B                 B                                      B
        ///               /|\               |\\                                    /|
        ///              /   \                \  \                                / |
        ///             /  |  \             |  \    \                            /  |
        ///          AB/       \BC  -or-        \      \            -or-      AB/   |BC/HT
        ///           /    |HT  \         HT|    \        \BC                  /    | 
        ///          /           \             AB \          \                /     |
        ///         /     _|      \         |_     \ (>90°)     \            /     _|
        ///        /)____|_|______(\        |_| _ _ \)____________(\        /)____|_| 
        ///    -> A       AC        C            -> A      AC       C   -> A    AC   C
        ///       ^      base                       ^     base             ^   base
        /// </code>
        /// </summary>
        public double CalculateHeight()
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            double returnValue;
            //if (!IsComplete())
            //    throw new GeomException("cannot calculate triangle height until all angles and sides are known");

            if (AC.HasValue)
            {
                if (A.HasValue && AB.HasValue)
                {
                    if (A < 90.0)
                        returnValue = Trig.OppositeFromRadiansAndHypotenuse(A.Value.DegreesToRadians(), AB.Value);
                    else if (A > 90.0)
                        returnValue = Trig.OppositeFromRadiansAndHypotenuse(180 - A.Value.DegreesToRadians(), AB.Value);
                    else 
                        returnValue = BC.Value;
                    SystemMessageRelay.RelayMethodReturnValue(returnValue, group: MessageRelayGroup);
                    return returnValue;
                }
                else if (C.HasValue && BC.HasValue)
                {
                    if (C < 90.0)
                        returnValue = Trig.OppositeFromRadiansAndHypotenuse(C.Value.DegreesToRadians(), BC.Value);
                    else if (C > 90.0)
                        returnValue = Trig.OppositeFromRadiansAndHypotenuse(180 - C.Value.DegreesToRadians(), BC.Value);
                    else
                        returnValue = AB.Value;
                    SystemMessageRelay.RelayMethodReturnValue(returnValue, group: MessageRelayGroup);
                    return returnValue;
                }
            }
            throw new GeomException("Insufficient information to calculate height relative to base side 'AC'");
        }
    }
}
