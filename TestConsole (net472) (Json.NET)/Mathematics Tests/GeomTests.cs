using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Comparison;
using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Mathematics;
using Horseshoe.NET.Mathematics.Geometry;
using Horseshoe.NET.Mathematics.Geometry.Trigonometry;

namespace TestConsole.MathematicsTests
{
    class GeomTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Calculations",
                () =>
                {
                    int precision = 3;
                    var data = new double[]
                    {
                        Trig.RadiansFromOppositeAndHypotenuse(4, 5, precision: -1).RadiansToDegrees(precision: precision),
                        Trig.RadiansFromOppositeAndHypotenuse(3, 5, precision: -1).RadiansToDegrees(precision: precision),
                        Trig.RadiansFromAdjacentAndHypotenuse(3, 5, precision: -1).RadiansToDegrees(precision: precision),
                        Trig.RadiansFromAdjacentAndHypotenuse(4, 5, precision: -1).RadiansToDegrees(precision: precision),
                        Trig.RadiansFromAdjacentAndOpposite(3, 4, precision: -1).RadiansToDegrees(precision: precision),
                        Trig.RadiansFromAdjacentAndOpposite(4, 3, precision: -1).RadiansToDegrees(precision: precision)
                    };
                    Console.WriteLine($"sine    - angle from opposite and hypotenuse: 4, 5 -> {data[0]:N3}");
                    Console.WriteLine($"sine    - angle from opposite and hypotenuse: 3, 5 -> {data[1]:N3}");
                    Console.WriteLine($"                                                   -> {data[0] + data[1]:N3}");
                    Console.WriteLine($"cosine  - angle from adjacent and hypotenuse: 3, 5 -> {data[2]:N3}");
                    Console.WriteLine($"cosine  - angle from adjacent and hypotenuse: 4, 5 -> {data[3]:N3}");
                    Console.WriteLine($"                                                   -> {data[2] + data[3]:N3}");
                    Console.WriteLine($"tangent - angle from adjacent and opposite:   3, 4 -> {data[4]:N3}");
                    Console.WriteLine($"tangent - angle from adjacent and opposite:   4, 3 -> {data[5]:N3}");
                    Console.WriteLine($"                                                   -> {data[4] + data[5]:N3}");
               }
            ),
            BuildMenuRoutine
            (
                "Right Triangles",
                () =>
                {
                    RightTriangle rightTriangle = new RightTriangle(a: 60, ac: 45);
                    Console.WriteLine(rightTriangle);
                    rightTriangle = new RightTriangle(a: 60, ac: 45, precision: 4);
                    Console.WriteLine(rightTriangle);
                }
            ),
            BuildMenuRoutine
            (
                "Right Triangle w/ message relay",
                () =>
                {
                    // Register listener
                    var listener = SystemMessageRelay.Subscribe.Console(MathConstants.MessageRelayGroup, LikeMode.StartsWith);

                    RightTriangle rightTriangle = new RightTriangle(a: 60, ac: 45, precision: 4);
                    Console.WriteLine(rightTriangle);

                    // De-register listener
                    SystemMessageRelay.Unsubscribe.Listener(listener);
                }
            )
        };
    }
}
