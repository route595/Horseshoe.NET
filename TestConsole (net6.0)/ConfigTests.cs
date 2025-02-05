using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

using Horseshoe.NET.Configuration;
using Horseshoe.NET.ConsoleX;

namespace TestConsole
{
    class ConfigTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Get ints from app settings",
                () =>
                {
                    Console.WriteLine("// Getting an int...");
                    Console.Write("Config.Get<int>(\"myInt\");              // value=\"90\"");
                    Console.WriteLine(" -> " + Config.Get<int>("myInt") + "");
                    Console.WriteLine();
                    Console.WriteLine("// Getting a hex formatted int...");
                    Console.Write("Config.Get<int>(\"myHexInt\",            // value=\"5a\"");
                    Console.WriteLine(" -> " + Config.Get<int>("myHexInt", numberStyle: NumberStyles.HexNumber));
                    Console.WriteLine("    numberStyle: NumberStyles.HexNumber);");
                    Console.WriteLine();
                    Console.WriteLine("// Getting a hex formatted int w/ key annotation...");
                    Console.Write("Config.Get<int>(\"myHexInt[hex]\");      // value=\"5a\"");
                    Console.WriteLine(" -> " + Config.Get<int>("myHexInt[hex]"));
                    Console.WriteLine();
                    Console.WriteLine("// Getting a hex formatted int w/ data annotation or data format...");
                    Console.Write("Config.Get<int>(\"myInt_Annotation\");   // value=\"5a[hex]\"");
                    Console.WriteLine(" -> " + Config.Get<int>("myInt_Annotation"));
                    Console.Write("Config.Get<int>(\"myInt_Format\");       // value=\"0x5a\"");
                    Console.WriteLine(" -> " + Config.Get<int>("myInt_Format"));
                }
            ),
            BuildMenuRoutine
            (
                "Get \"myPlanet\" and \"mySolarSystem\" from " + Program.CONFIG_FILE_NAME,
                () =>
                {
                    var myPlanet = Config.ParseSection<Planet>("myPlanet");
                    Console.WriteLine("[myPlanet]");
                    Console.WriteLine(myPlanet);
                    var mySolarSystem = Config.ParseSection<SolarSystem>("mySolarSystem");
                    Console.WriteLine();
                    Console.WriteLine("[mySolarSystem]");
                    foreach (var planet in mySolarSystem)
                        Console.WriteLine(planet);
                }
            )
        };
    }

    class SolarSystem : List<Planet>
    {
    }

    class Planet
    {
        public string Name { get; set; } = "unk-planet";

        public double Diameter { get; set; }

        public string Color { get; set; } = "unk-color";

        public IEnumerable<Moon>? Moons { get; set; }

        public IEnumerable<string>? MoonNames => Moons?
            .Select(m => m.Name);

        public override string ToString()
        {
            return "I am " + Name + ", a " + Color + " planet measuring " + Diameter + " km in diameter. Moons: " +
                (MoonNames == null ? "[null]" : (MoonNames.Any() ? string.Join(", ", MoonNames) : "[empty]"));
        }
    }

    public class Moon
    {
        public string Name { get; set; } = "unk-moon";
    }
}
