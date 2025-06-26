using System;

using Horseshoe.NET.Mathematics.Geometry.Trigonometry;

namespace Horseshoe.NET.Mathematics.Geometry
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <code>
    ///                B                                        B                        B
    ///               /\                                      //                        /|
    ///              /  \                                  /  /                        / |
    ///             /    \ (opposite)                   /    /           (hypotenuse) /  |
    /// (adjacent) /      \ BC        -or-        AB /      /     -or-            AB /   |
    ///        AB /        \                      /        / BC                     /    | (opposite)
    ///          /          \                  /          /                        /     | BC
    ///       | /            \              /            /                      | /     _|
    ///       V/)_____________\          /)_____________/                       V/)____|_| 
    ///   --> A       AC       C        A       AC      C                   --> A   AC    C
    ///         (adjacent)                                                     (adjacent)
    /// </code>
    /// </remarks>
    public class Triangle : CalcBase
    {
        /// <summary>
        /// Angle 'A' in degrees
        /// </summary>
        public double? A { get; internal set; }

        /// <summary>
        /// Angle 'B' in degrees
        /// </summary>
        public double? B { get; internal set; }

        /// <summary>
        /// Angle 'C' in degrees (90 degrees in right triangles)
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

        /// <summary>
        /// Units of measurement
        /// </summary>
        public string UnitsOfLength { get; }

        /// <summary>
        /// Triangle constructor.  All parameters are optional, supply the ones you know and the rest will attempt to be calculated.
        /// </summary>
        /// <param name="a">Angle 'A' in degrees</param>
        /// <param name="b">Angle 'B' in degrees</param>
        /// <param name="c">Angle 'C' in degrees</param>
        /// <param name="ab">Length of side 'A' to 'B'</param>
        /// <param name="ac">Length of side 'A' to 'C'</param>
        /// <param name="bc">Length of side 'B' to 'C'</param>
        /// <param name="unitsOfLength">Units of length used for display, e.g. 'cm' -&gt; '34 cm'.  Default is <c>null</c> -&gt; 'len=34'.</param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Triangle (double? a = null, double? b = null, double? c = null, double? ab = null, double? ac = null, double? bc = null, string unitsOfLength = null)
        {
            // basic angle validation
            A = !a.HasValue || (a > 0.0 && a < 180.0) ? a : throw new ArgumentOutOfRangeException(nameof(a), "Angle 'A' of this triangle may only be between 0 and 180 degrees (exclusive)");
            B = !b.HasValue || (b > 0.0 && b < 180.0) ? b : throw new ArgumentOutOfRangeException(nameof(b), "Angle 'B' of this triangle may only be between 0 and 180 degrees (exclusive)");
            C = !c.HasValue || (c > 0.0 && c < 180.0) ? c : throw new ArgumentOutOfRangeException(nameof(c), "Angle 'C' of this triangle may only be between 0 and 180 degrees (exclusive)");

            // basic side validation
            AB = !ab.HasValue || ab > 0.0 ? ab : throw new ArgumentOutOfRangeException(nameof(ab), "Side 'AB' of this triangle cannot be <= 0 units of length");
            AC = !ac.HasValue || ac > 0.0 ? ac : throw new ArgumentOutOfRangeException(nameof(ac), "Side 'AC' of this triangle cannot be <= 0 units of length");
            BC = !bc.HasValue || bc > 0.0 ? bc : throw new ArgumentOutOfRangeException(nameof(bc), "Side 'BC' of this triangle cannot be <= 0 units of length");

            // units of lengths
            UnitsOfLength = unitsOfLength;

            Init();
        }

        private void Init()
        {
            // More angle validation
            if ((!A.HasValue && R((B ?? 0.0) + (C ?? 0.0)) >= 180.0) || (!B.HasValue && R((A ?? 0.0) + (C ?? 0.0)) >= 180.0) || (!C.HasValue && R((A ?? 0.0) + (B ?? 0.0)) >= 180.0))
                throw new Exception("The partial sum of angles of a triangle may not >= 180 degrees");
            if (A.HasValue && B.HasValue && C.HasValue && R(A.Value + B.Value + C.Value) != 180.0)
                throw new Exception("The sum of all angles of a triangle must exactly = 180 degrees");

            // Calculate third angle given the other two, if applicable
            if (!A.HasValue && B.HasValue && C.HasValue)
                A = R(180.0 - B.Value - C.Value);
            else if (!B.HasValue && A.HasValue && C.HasValue)
                B = R(180.0 - A.Value - C.Value);
            else if (!C.HasValue && A.HasValue && B.HasValue)
                C = R(180.0 - A.Value - B.Value);

            // Cannot continue if no sides
            if (!BC.HasValue && !AC.HasValue && !AB.HasValue)
                return;

            // Unilateral triangle based on equal angles
            if (A == 60.0 && B == 60.0 && C == 60.0)
            {
                double side = (BC ?? AC ?? AB).Value;  // guaranteed to have value, see 'Cannot continue if no sides' above
                if (!BC.HasValue)
                    BC = side;
                else if (BC != side)
                    throw new Exception($"Side 'BC' must be equal in length to the other sides given this is an equilateral triangle: {BC}, {side}");
                if (!AC.HasValue)
                    AC = side;
                else if (AC != side)
                    throw new Exception($"Side 'AC' must be equal in length to the other sides given this is an equilateral triangle: {AC}, {side}");
                if (!AB.HasValue)
                    AB = side;
                else if (AB != side)
                    throw new Exception($"Side 'AB' must be equal in length to the other sides given this is an equilateral triangle: {AB}, {side}");
                return;
            }

            // if all sides present
            if (BC.HasValue && AC.HasValue && AB.HasValue)
            {
                // unilateral triangle based on equal sides
                if (BC == AC && AC == AB)
                {
                    if (!A.HasValue)
                        A = 60.0;
                    else if (A != 60.0)
                        throw new Exception($"Angle 'A' must = 60 degrees given this is an equilateral triangle: {A}");
                    if (!B.HasValue)
                        B = 60.0;
                    else if (B != 60.0)
                        throw new Exception($"Angle 'B' must = 60 degrees given this is an equilateral triangle: {B}");
                    if (!C.HasValue)
                        C = 60.0;
                    else if (C != 60.0)
                        throw new Exception($"Angle 'C' must = 60 degrees given this is an equilateral triangle: {C}");
                    return;
                }

                // pythagorean theorum
                double potentialHypotenuse = Math.Max(BC.Value, Math.Max(AC.Value, AB.Value));

                if (BC.Value == potentialHypotenuse)
                {
                    if (this is RightTriangle)
                        throw new Exception("Please make side 'AB' the hypotenuse instead of 'BC'");
                    if (R(Math.Pow(AC.Value, 2.0) + Math.Pow(AB.Value, 2.0)) == R(Math.Pow(BC.Value, 2.0)))
                    {
                        if (!A.HasValue)
                            A = 90.0;
                        else if (A != 90.0)
                            throw new Exception($"Side lengths indicate a right triangle, however, the angle opposite the hypotenuse is not 90 degrees: {A}");
                        B = 45;
                        C = 45;
                        return;
                    }
                }
                else if (AC.Value == potentialHypotenuse)
                {
                    if (this is RightTriangle)
                        throw new Exception("Please make side 'c' the long side instead of 'b'");
                    if (R(Math.Pow(BC.Value, 2.0) + Math.Pow(AB.Value, 2.0)) == R(Math.Pow(AC.Value, 2.0)))
                    {
                        if (!B.HasValue)
                            B = 90.0;
                        else if (B != 90.0)
                            throw new Exception($"Side lengths indicate a right triangle, however, the angle opposite the hypotenuse is not 90 degrees: {B}");
                        A = 45;
                        C = 45;
                        return;
                    }
                }
                else if (AB.Value == potentialHypotenuse)
                {
                    if (R(Math.Pow(AC.Value, 2.0) + Math.Pow(AB.Value, 2.0)) == R(Math.Pow(BC.Value, 2.0)))
                    {
                        if (!C.HasValue)
                            C = 90.0;
                        else if (C != 90.0)
                            throw new Exception($"Side lengths indicate a right triangle, however, the angle opposite the hypotenuse is not 90 degrees: {C}");
                        A = 45;
                        B = 45;
                        return;
                    }
                }
            }

            // Cannot continue if no angles
            if (!A.HasValue && !B.HasValue && !C.HasValue)
                return;
        }

        public bool IsComplete()
        {
            if (!A.HasValue) return false;
            if (!B.HasValue) return false;
            if (!C.HasValue) return false;
            if (!BC.HasValue) return false;
            if (!AC.HasValue) return false;
            if (!AB.HasValue) return false;
            return true;
        }

        public bool IsIsoscolese()
        {
            if (!BC.HasValue)
            {
                if (!AC.HasValue) throw new Exception("length of sides 'a' and 'b' have not been set");
                if (!AB.HasValue) throw new Exception("length of sides 'a' and 'c' have not been set");
                return AC == AB;
            }
            else if (!AC.HasValue)
            {
                if (!AB.HasValue) throw new Exception("length of sides 'b' and 'c' have not been set");
                return BC == AB;
            }
            return BC == AC;
        }

        public bool IsEquilateral()
        {
            if (!BC.HasValue) throw new Exception("length of side 'a' has not been set");
            if (!AC.HasValue) throw new Exception("length of side 'b' has not been set");
            if (!AB.HasValue) throw new Exception("length of side 'c' has not been set");
            return BC == AC && AC == AB;
        }
    }
}
