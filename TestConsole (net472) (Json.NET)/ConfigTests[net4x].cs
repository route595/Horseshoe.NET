﻿using System;
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
                    Console.WriteLine("<appSettings>");
                    foreach (var key in ConfigurationManager.AppSettings.AllKeys.Where(k => k.Contains("Int")))
                    {
                        Console.WriteLine("  <add key=\"" + key + "\" value=\"" + Config.Get(key) + "\" />");
                    }
                    Console.WriteLine("</appSettings>");
                    Console.WriteLine();
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
                "Get \"MyPlanet\" from configuration",
                () =>
                {
                    Console.WriteLine("Getting \"myPlanet\" from config...");
                    var myPlanet = Config.ParseSection<PlanetSection>("myPlanet");
                    Console.WriteLine(myPlanet);
                }
            )
        };
    }

    class PlanetSection : ConfigurationSection
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name => (string)base["name"];

        [ConfigurationProperty("diameter")]
        public double Diameter => (double)base["diameter"];

        [ConfigurationProperty("color")]
        public string Color => (string)base["color"];

        [ConfigurationProperty("moons")]
        [ConfigurationCollection(typeof(MoonCollection))]
        public MoonCollection Moons => (MoonCollection)base["moons"];

        public IEnumerable<string> MoonNames => Moons?
            .Cast<MoonElement>()
            .Select(m => m.Name);

        public override string ToString()
        {
            return "I am " + Name + ", a " + Color + " planet measuring " + Diameter + " km in diameter. Moons: " +
                (MoonNames == null ? "[null]" : (MoonNames.Any() ? string.Join(", ", MoonNames) : "[empty]"));
        }
    }

    class SolarSystemSection : ConfigurationSection
    {
        [ConfigurationProperty("planets")]
        [ConfigurationCollection(typeof(PlanetCollection))]
        public PlanetCollection Planets => (PlanetCollection)base["planets"];
    }

    class MoonCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MoonElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MoonElement)element).Name;
        }

        public MoonElement this[int index]
        {
            get
            {
                return (MoonElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public MoonElement this[string Name]
        {
            get
            {
                return (MoonElement)BaseGet(Name);
            }
        }

        public int IndexOf(MoonElement element)
        {
            return BaseIndexOf(element);
        }

        public void Add(MoonElement element)
        {
            BaseAdd(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(MoonElement element)
        {
            if (BaseIndexOf(element) >= 0)
            {
                BaseRemove(element);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

    class PlanetCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PlanetElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PlanetElement)element).Name;
        }

        public PlanetElement this[int index]
        {
            get
            {
                return (PlanetElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public PlanetElement this[string Name]
        {
            get
            {
                return (PlanetElement)BaseGet(Name);
            }
        }

        public int IndexOf(PlanetElement element)
        {
            return BaseIndexOf(element);
        }

        public void Add(PlanetElement element)
        {
            BaseAdd(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(PlanetElement element)
        {
            if (BaseIndexOf(element) >= 0)
            {
                BaseRemove(element);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

    class PlanetElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("diameter")]
        public double Diameter
        {
            get
            {
                return (double)this["diameter"];
            }
            set
            {
                this["diameter"] = value;
            }
        }

        [ConfigurationProperty("color")]
        public string Color
        {
            get
            {
                return (string)this["color"];
            }
            set
            {
                this["color"] = value;
            }
        }

        [ConfigurationProperty("moons")]
        [ConfigurationCollection(typeof(MoonCollection))]
        public MoonCollection Moons => (MoonCollection)base["moons"];

        public IEnumerable<string> MoonNames => Moons?
            .Cast<MoonElement>()
            .Select(m => m.Name);

        public override string ToString()
        {
            return "I am " + Name + ", a " + Color + " planet measuring " + Diameter + " km in diameter. Moons: " +
                (MoonNames == null ? "[null]" : (MoonNames.Any() ? string.Join(", ", MoonNames) : "[empty]"));
        }
    }

    public class MoonElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
    }
}
