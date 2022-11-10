using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class XMLTests : RoutineX
    {
        public override IList<MenuObject> Menu => new MenuObject[]
        {
            new MenuHeader("USER ROUTINES"),
            BuildMenuRoutine
            (
                "Parse documentation XML",
                () =>
                {
                    new XmlDoc2().Fill("Horseshoe.NET.xml");
                    Console.WriteLine("Journal");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, TraceJournal.DefaultEntries));
                }
            )
        };

        public class XmlDoc2
        {
            public Assembly _Assembly { get; set; }

            public IList<Member> Members { get; } = new List<Member>();

            public IEnumerable<string> GetNamespaces()
            {
                var distinct = Members.Select(m => m.NameSpace).Distinct().ToList();
                distinct.Sort();
                return distinct;
            }

            public void Fill(string fileUri, TraceJournal journal = null)
            {
                Member member = null;
                string text;
                DocElement docElement = default;
                NestedDocElement nestedDocElement = default;
                if (journal == null)
                {
                    journal = TraceJournal.ResetDefault();
                }
                journal.WriteEntry("XmlDoc.Fill()");
                journal.Level++;
                using (var reader = XmlReader.Create(fileUri, new XmlReaderSettings { IgnoreWhitespace = true }))
                {
                    while (reader.Read() && !reader.EOF)
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.XmlDeclaration:
                                journal.WriteEntry("Base URI: " + reader.BaseURI);
                                journal.WriteEntry("XML: " + reader.Value);
                                break;
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "doc":
                                        journal.WriteEntry("-doc begin-");
                                        journal.Level++;
                                        break;
                                    case "assembly":
                                        _Assembly = new Assembly();
                                        docElement = DocElement.Assembly;
                                        journal.WriteEntry("Assembly: " + reader.Value);
                                        break;
                                    case "name":
                                        if (docElement == DocElement.Assembly)
                                        {
                                            nestedDocElement = NestedDocElement.Name;
                                        }
                                        break;
                                    case "member":
                                        member = ParseMember(reader.GetAttribute("name"));
                                        docElement = DocElement.Member;
                                        journal.WriteEntry("Parsing " + member.FullName + "...");
                                        journal.Level++;
                                        break;
                                    case "summary":
                                        nestedDocElement = NestedDocElement.Summary;
                                        break;
                                    case "remarks":
                                        nestedDocElement = NestedDocElement.Remarks;
                                        break;
                                    case "typeparam":
                                        {
                                            if (member is Type type)
                                            {
                                                type.TypeParams.Add(new Param(reader.GetAttribute("name")));
                                                journal.WriteEntry("type param " + type.TypeParams.Last().Name);
                                            }
                                            if (member is Method method)
                                            {
                                                method.TypeParams.Add(new Param(reader.GetAttribute("name")));
                                                journal.WriteEntry("type param " + method.TypeParams.Last().Name);
                                            }
                                        }
                                        nestedDocElement = NestedDocElement.TypeParam;
                                        break;
                                    case "param":
                                        {
                                            if (member is Method method)
                                            {
                                                method.Params.Add(new Param(reader.GetAttribute("name")));
                                                journal.WriteEntry("param " + method.Params.Last().Name);
                                            }
                                        }
                                        nestedDocElement = NestedDocElement.Param;
                                        break;
                                    case "returns":
                                        nestedDocElement = NestedDocElement.Returns;
                                        break;
                                    case "exception":
                                        {
                                            if (member is Method method)
                                            {
                                                var cref = reader.GetAttribute("cref") ?? "";
                                                if (cref.Contains(":"))
                                                {
                                                    var split = cref.Split(':');
                                                    cref = split[1];
                                                }
                                                method.Exceptions.Add(new Exception(cref));
                                                journal.WriteEntry("exception " + method.Exceptions.Last().Cref);
                                            }
                                        }
                                        nestedDocElement = NestedDocElement.Exception;
                                        break;
                                }
                                break;
                            case XmlNodeType.EndElement:
                                switch (reader.Name)
                                {
                                    case "doc":
                                        journal.Level--;
                                        journal.WriteEntry("-doc end-");
                                        break;
                                    case "assembly":
                                        docElement = default;
                                        break;
                                    case "name":
                                        nestedDocElement = default;
                                        break;
                                    case "member":
                                        Members.Add(member);
                                        member = null;
                                        docElement = default;
                                        journal.Level--;
                                        break;
                                    case "summary":
                                        nestedDocElement = default;
                                        break;
                                    case "remarks":
                                        nestedDocElement = default;
                                        break;
                                    case "typeparam":
                                        nestedDocElement = default;
                                        break;
                                    case "param":
                                        nestedDocElement = default;
                                        break;
                                    case "returns":
                                        nestedDocElement = default;
                                        break;
                                    case "exception":
                                        nestedDocElement = default;
                                        break;
                                }
                                break;
                            case XmlNodeType.Text:
                                text = reader.Value.Trim();
                                switch(docElement)
                                {
                                    case DocElement.Assembly:
                                        switch (nestedDocElement)
                                        {
                                            case NestedDocElement.Name:
                                                journal.WriteEntry("Assembly: " + reader.Value.Trim());
                                                continue;
                                        }
                                        break;
                                    case DocElement.Member:
                                        switch (nestedDocElement)
                                        {
                                            case NestedDocElement.Summary:
                                                member.Summary = reader.Value.Trim();
                                                journal.WriteEntry("summary " + member.Summary);
                                                continue;
                                            case NestedDocElement.Remarks:
                                                member.Remarks = reader.Value.Trim();
                                                journal.WriteEntry("remarks " + member.Summary);
                                                continue;
                                            case NestedDocElement.TypeParam:
                                                {
                                                    if (member is Type type)
                                                    {
                                                        type.TypeParams.Last().Description = reader.Value.Trim();
                                                        journal.WriteEntry("type param " + type.TypeParams.Last().Description);
                                                        continue;
                                                    }
                                                    if (member is Method method)
                                                    {
                                                        method.TypeParams.Last().Description = reader.Value.Trim();
                                                        journal.WriteEntry("type param " + method.TypeParams.Last().Description);
                                                        continue;
                                                    }
                                                }
                                                break;
                                            case NestedDocElement.Param:
                                                {
                                                    if (member is Method method)
                                                    {
                                                        method.Params.Last().Description = reader.Value.Trim();
                                                        journal.WriteEntry("param " + method.Params.Last().Description);
                                                        continue;
                                                    }
                                                }
                                                break;
                                            case NestedDocElement.Returns:
                                                {
                                                    if (member is Method method)
                                                    {
                                                        method.Returns = reader.Value.Trim();
                                                        journal.WriteEntry("returns " + method.Returns);
                                                        continue;
                                                    }
                                                }
                                                break;
                                            case NestedDocElement.Exception:
                                                {
                                                    if (member is Method method)
                                                    {
                                                        method.Exceptions.Last().Description = reader.Value.Trim();
                                                        journal.WriteEntry("exception " + method.Exceptions.Last().Description);
                                                        continue;
                                                    }
                                                }
                                                break;
                                        }
                                        break;
                                }
                                throw new ValidationException("Text not assignable" + (member != null ? " to " + member.GetType().Name : "") + (docElement != default ? (nestedDocElement != default ? " from " + docElement + " > " + nestedDocElement : " from " + docElement) : "") + ": " + text);
                        }
                    }
                }
            }

            private static Regex _nameAndParamsPattern = new Regex("[#]?[a-z0-9_`]+(\\([a-z0-9_.,`@\\{\\}\\[\\]]*\\))?$", RegexOptions.IgnoreCase);

            public static Member ParseMember(string rawName)
            {
                var parts = rawName.Split(':');
                var match = _nameAndParamsPattern.Match(parts[1]);
                if (!match.Success)
                    throw new ValidationException("Cannot parse name: " + parts[1]);
                var name = match.Value;
                var @namespace = parts[1].Length == match.Value.Length
                    ? null
                    : parts[1].Substring(0, parts[1].Length - match.Value.Length - 1);
                switch (parts[0])
                {
                    case "T":
                        System.Type type = ObjectUtil.GetType(parts[1], suppressErrors: true);
                        if (type != null)
                        {
                            if (type.IsEnum)
                                return new Enum(type.Namespace, type.Name);
                            if (type.IsInterface)
                                return new Interface(type.Namespace, type.Name);
                            if (!type.IsClass)
                                return new Struct(type.Namespace, type.Name);
                            return new Class(type.Namespace, type.Name);
                        }
                        return new Type(@namespace, name);
                    case "P":
                        return new Property(@namespace, name);
                    case "F":
                        return new EnumValue(@namespace, name);
                    case "M":
                        return new Method(@namespace, name);
                    default:
                        throw new ValidationException("Unrecognized member type indicator: recognized - T, P, F, M");
                }
            }

            public enum DocElement
            {
                None,
                Assembly,
                Member
            }

            public enum NestedDocElement
            {
                None,
                Exception, // has "cref" attribute
                Name,
                Param,     // has "name" attribute
                Remarks,
                Returns,
                Summary,
                TypeParam  // has "name" attribute
            }

            public class Assembly
            {
                public string Name { get; set; }
                public override string ToString() => TextUtil.Reveal(Name);
            }

            public abstract class Member
            {
                public string NameSpace { get; }
                public string Name { get; }
                public string FullName => NameSpace == null
                    ? Name 
                    : NameSpace + "." + Name;
                public string Summary { get; set; }
                public string Remarks { get; set; }

                public Member(string name)
                {
                    Name = name;
                }

                public Member(string @namespace, string name) : this(name)
                {
                    NameSpace = @namespace;
                }

                public override string ToString()
                {
                    return NameSpace != null
                        ? NameSpace + "." + Name
                        : Name;
                }
            }

            public class Type : Member
            {
                public IList<Param> TypeParams { get; } = new List<Param>();

                public Type(string @namespace, string name) : base(@namespace, name)
                {
                }
            }

            public class Class : Type
            {
                public IList<Property> Properties { get; } = new List<Property>();
                public IList<Method> Functions { get; } = new List<Method>();
                
                public Class(string @namespace, string name) : base(@namespace, name)
                {
                }
            }

            public class Struct : Type
            {
                public IList<Property> Properties { get; } = new List<Property>();
                public IList<Method> Functions { get; } = new List<Method>();

                public Struct(string @namespace, string name) : base(@namespace, name)
                {
                }
            }

            public class Interface : Type
            {
                public IList<Property> Properties { get; } = new List<Property>();
                public IList<Method> Functions { get; } = new List<Method>();

                public Interface(string @namespace, string name) : base(@namespace, name)
                {
                }
            }

            public class Enum : Type
            {
                public IList<Value> Values { get; } = new List<Value>();

                public Enum(string @namespace, string name) : base(@namespace, name)
                {
                }

                public class Value : Member
                {
                    public Value(string name) : base(name)
                    {
                    }
                }
            }

            public class Property : Member
            {
                public Property(string @namespace, string name) : base(@namespace, name)
                {
                }
            }

            public class Method : Member
            {
                public IList<Param> TypeParams { get; } = new List<Param>();
                public IList<Param> Params { get; } = new List<Param>();
                public string Returns { get; set; }
                public IList<Exception> Exceptions { get; } = new List<Exception>();

                public Method(string @namespace, string name) : base(@namespace, name)
                {
                }
            }

            public class EnumValue : Member
            {
                public EnumValue(string @namespace, string name) : base(@namespace, name)
                {
                }
            }

            public class Param
            {
                public string Name { get; } // e.g. "T"
                public string Description { get; set; } // e.g. "Type of item"

                public Param(string name)
                {
                    Name = name;
                }
            }

            public class Exception
            {
                public string Cref { get; } // e.g. "T"
                public string Description { get; set; } // e.g. "Type of item"

                public Exception(string cref)
                {
                    Cref = cref;
                }
            }

            //public class ControlInterface
            //{
            //    public bool SubMember
            //}
        }
    }
}