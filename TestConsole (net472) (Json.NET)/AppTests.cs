using System;
using System.Collections.Generic;
using System.Text;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class AppTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Console Properties",
                () =>
                {
                    var consoleProperties = typeof(Console).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    foreach (var prop in consoleProperties)
                    {
                        Console.Write(prop.Name.PadRight(25) + " ");
                        try
                        {
                            Console.WriteLine(TextUtil.Reveal(prop.GetValue(null)));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.GetType().Name);
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Environment Properties",
                () =>
                {
                    var environmentProperties = typeof(Environment).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    foreach (var prop in environmentProperties)
                    {
                        Console.Write(prop.Name.PadRight(25) + " ");
                        try
                        {
                            if (prop.Name.Equals("NewLine"))
                            {
                                Console.WriteLine(TextUtil.Reveal(prop.GetValue(null)?.ToString(), new RevealOptions{ RevealWhitespaces = true }));
                            }
                            else if (prop.Name.Equals("StackTrace"))
                            {
                                Console.WriteLine(((string)prop.GetValue(null)).Substring(0, 70) + "...");
                            }
                            else
                            {
                                Console.WriteLine(TextUtil.Reveal(prop.GetValue(null)));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.GetType().Name);
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "AppDomain Properties",
                () =>
                {
                    var appDomainProperties = typeof(AppDomain).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    foreach (var prop in appDomainProperties)
                    {
                        Console.Write(prop.Name.PadRight(25) + " ");
                        try
                        {
                            if (prop.Name.Equals("PermissionSet"))
                            {
                                Console.WriteLine(TextUtil.Reveal(prop.GetValue(AppDomain.CurrentDomain)?.ToString().Replace(Environment.NewLine, " ").Trim()));
                            }
                            else
                            {
                                Console.WriteLine(TextUtil.Reveal(prop.GetValue(AppDomain.CurrentDomain)));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.GetType().Name);
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Assemblies",
                () =>
                {
                    RenderX.List(AppDomain.CurrentDomain.GetAssemblies(), renderer: (a) => a.FullName);
                }
            //),
            //BuildMenuRoutine
            //(
            //    "Detect App Type",
            //    () =>
            //    {
            //        var sb = new StringBuilder();
            //        var appType = ClientApp.DetectAppType(sb);
            //        RenderX.ListTitle("Detected: " + (appType?.ToString() ?? "[null]"));
            //        Console.WriteLine(sb);
            //    }
            )
        };
    }
}
