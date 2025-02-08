![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.Configuration

A configuration utility for .NET.  Seeks to unify the experience between .NET Framework and Core to the extent possible.

## Code Examples

```xml
<appSettings>
  <add key="myInt" value="90" />
  <add key="myHexInt" value="5a" />
  <add key="myInt_Annotation" value="5a[hex]" />
  <add key="myInt_Format" value="0x5a" />
</appSettings>
```

```c#
// Getting an int...
Config.Get<int>("myInt");              // value="90" -> 90

// Getting a hex formatted int...
Config.Get<int>("myHexInt",            // value="5a" -> 90
    numberStyle: NumberStyles.HexNumber);

// Getting a hex formatted int w/ key annotation...
Config.Get<int>("myHexInt[hex]");      // value="5a" -> 90

// Getting a hex formatted int w/ data annotation or data format...
Config.Get<int>("myInt_Annotation");   // value="5a[hex]" -> 90
Config.Get<int>("myInt_Format");       // value="0x5a" -> 90

// Before Horseshoe.NET... 
var stringValue = ConfigurationManager.AppSettings["myInt"];
if (stringValue != null)
    return int.Parse(stringValue);
```
