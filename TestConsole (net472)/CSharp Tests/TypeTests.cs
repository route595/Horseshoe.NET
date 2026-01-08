using System;
using System.Collections.Generic;
using System.Reflection;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.ObjectsTypesAndValues;

namespace TestConsole.CSharpTests
{
    class TypeTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "'New' properties",
                () =>
                {
                    ObjectFoo objectFoo = new ObjectFoo { Foo = "two" };
                    NumberFoo numberFoo = new NumberFoo { Foo = 2 };
                    string render(PropertyValue pv) => pv + " (" + pv.Property.PropertyType.Name + ") (" + pv.Property.DeclaringType.Name + (pv.Property.DeclaringType == pv.Property.ReflectedType ? "" : ">" + pv.Property.ReflectedType.Name) + ")";
                    RenderX.List(ObjectUtil.GetInstancePropertyValues(objectFoo), renderer: render, title: objectFoo + " properties");
                    RenderX.List(ObjectUtil.GetInstancePropertyValues(numberFoo), renderer: render, title: numberFoo + " properties");
                }
            ),
            BuildMenuRoutine
            (
                "'is' conversions",
                () =>
                {
                    eval(1);
                    Console.WriteLine();
                    eval(1.0);
                    Console.WriteLine();
                    eval(1.0m);

                    void eval(object obj)
                    {
                        Console.WriteLine($"{obj} is an instance of {obj.GetType().FullName}");
                        if (obj is byte)
                            Console.WriteLine($"    {obj} is a byte");
                        if (obj is short)
                            Console.WriteLine($"    {obj} is a short");
                        if (obj is int)
                            Console.WriteLine($"    {obj} is an int");
                        if (obj is long)
                            Console.WriteLine($"    {obj} is a long");
                        if (obj is decimal)
                            Console.WriteLine($"    {obj} is a decimal");
                        if (obj is float)
                            Console.WriteLine($"    {obj} is a float");
                        if (obj is double)
                            Console.WriteLine($"    {obj} is a double");
                        if (obj is DateTime)
                            Console.WriteLine($"    {obj} is a DateTime");
                        if (obj is bool)
                            Console.WriteLine($"    {obj} is a bool");
                        if (obj is string)
                            Console.WriteLine($"    {obj} is a string");
                        if (obj is object)
                            Console.WriteLine($"    {obj} is an object");
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Action and Func",
                () =>
                {
                    Action action = () => { Console.WriteLine(); };
                    Action<string> actionStr = (str) => { Console.WriteLine(str); };
                    Func<bool> func = () => true;
                    Func<string, bool> funcStr = (str) => string.IsNullOrEmpty(str);
                    foreach(var obj in new object[]{ action, actionStr, func, funcStr })
                    {
                        Console.WriteLine(obj.GetType().Name + " - " + obj.GetType().FullName);
                    }
                }
            )
        };

        class ObjectFoo 
        { 
            public object Foo { get; set; } 
            public override string ToString() 
            { 
                return GetType().Name + " (" + Foo + ")"; 
            } 
        }

        class NumberFoo : ObjectFoo 
        { 
            public new int Foo 
            { 
                get => (int)base.Foo; 
                set => base.Foo = value; 
            } 
        }
    }
}
