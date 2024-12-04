using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

using Horseshoe.NET.IO;
using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents compiled XML documentation (e.g. XML produced by the C# compiler) featuring the ability to parse XML documentation 
    /// </summary>
    public class XmlDoc
    {
        /// <summary>
        /// Represents the singleton XML &lt;assembly&gt; element
        /// </summary>
        public Assembly Assembly { get; } = new Assembly();

        /// <summary>
        /// Represents the 0 or more XML &lt;member&gt; elements that may be in the XML documentation
        /// </summary>
        public IList<Member> Members { get; } = new List<Member>();

        /// <summary>
        /// Derives the distinct namespaces referenced in the parsed XML documentation
        /// </summary>
        /// <returns></returns>
        public IList<string> GetDocumentedNamespaces()
        {
            return Members
                .Where(m => m.MemberType != null && m.MemberType.Namespace != null)
                .Select(m => m.MemberType.Namespace)
                .Distinct()
                .OrderBy(ns => ns)
                .ToList();
        }

        /// <summary>
        /// Namespace cache used internally to validate parsed XML data. Applies to situations where the assembly referenced in the XML docs is available at runtime.
        /// </summary>
        public IList<string> VerifiedNamespaces { get; } = new List<string>();


        /// <summary>
        /// Type cache used internally to validate parsed XML data. Applies to situations where the assembly referenced in the XML docs is available at runtime.
        /// </summary>
        public IDictionary<string,System.Type> VerifiedTypes { get; } = new Dictionary<string, System.Type>();

        /// <summary>
        /// List of warnings reported by the <c>Fill()</c> method
        /// </summary>
        public IList<string> Warnings { get; } = new List<string>();

        /// <summary>
        /// Load an XLM documenation file to populate this <c>XmlDoc</c> instance
        /// </summary>
        /// <param name="fileUri">the file from which to load the XML documentation</param>
        /// <param name="assemblyDllUri">optional, the assembly dll corresponding to the documentation - can also load by adding assembly the traditional ways (e.g. install NuGet package or manually place in the home directory prior to app start)</param>
        /// <param name="credentials">credentials needed to access web hosted xml file</param>
        /// <param name="fillInMissingTypes">Adds an undocumented type implied by the existence of a member of that type</param>
        /// <param name="journal">a custom journal, if ommitted you can still view <c>TraceJournal.DefaultEntries</c> after method completion</param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public XmlDoc Fill
        (
            FilePath fileUri,
            FilePath? assemblyDllUri = null,
            NetworkCredential credentials = null,
            bool fillInMissingTypes = false, 
            TraceJournal journal = null
        )
        {
            // variable declaration
            Member member;

            // init journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("[XmlDoc.Fill()]");
            journal.Level++;

            // cleanup
            Assembly.Name = null;
            Members.Clear();
            VerifiedNamespaces.Clear();
            VerifiedTypes.Clear();
            Warnings.Clear();

            // optional assembly startup
            if (assemblyDllUri.HasValue && assemblyDllUri.Value.FullName != null)
            {
                _ = System.Reflection.Assembly.LoadFile(assemblyDllUri.Value.FullName);
            }

            // settings incl. creds
            var settings = new XmlReaderSettings();
            if (credentials != null)
            {
                var resolver = new XmlUrlResolver
                {
                    Credentials = credentials
                };
                settings.XmlResolver = resolver;
            }

            // iterate XML file
            using (var reader = XmlReader.Create(fileUri.FullName, settings))
            {
                journal.WriteEntry(reader.BaseURI);

                if (!reader.Read())
                    return this;

                // xml declaration e.g. <?xml version="1.0"?>
                if (reader.NodeType == XmlNodeType.XmlDeclaration)
                {
                    journal.WriteEntry("XML: " + reader.Value);
                }

                // assembly name 
                if (!reader.ReadToFollowing("assembly"))
                    return this;
                if (!reader.ReadToDescendant("name"))
                    return this;
                journal.WriteEntry("assembly name: " + reader.ReadInnerXml().Trim());
                journal.WriteEntry("[start looping <member> elements]");
                journal.Level++;

                // scan <member> elements
                while (reader.ReadToFollowing("member"))  // [outer loop]
                {
                    member = ParseMember(reader.GetAttribute("name"), fillInMissingTypes: fillInMissingTypes, journal: journal);
                    Members.Add(member);
                    journal.WriteEntry(member.ToString());
                    journal.Level++;

                    // scan elements inside each <member> element
                    bool goToNextMember = false;
                    while (reader.Read() && !goToNextMember)  // [inner loop]
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "summary":
                                        member.Summary = reader.ReadInnerXml().Trim();
                                        journal.WriteEntry("summary: " + member.Summary);
                                        break;
                                    case "typeparam":
                                        {
                                            if (member is Type type)
                                            {
                                                type.TypeParams.Add(new Param(reader.GetAttribute("name"), reader.ReadInnerXml().Trim()));
                                                journal.WriteEntry("type" + type.TypeParams.Last());
                                            }
                                            else if (member is Method method)
                                            {
                                                method.TypeParams.Add(new Param(reader.GetAttribute("name"), reader.ReadInnerXml().Trim()));
                                                journal.WriteEntry("type" + method.TypeParams.Last());
                                            }
                                            else if (reader is IXmlLineInfo lineInfo)
                                            {
                                                Warnings.Add("could not add <typeparam> to incompatible member at line " + lineInfo.LineNumber + ":" + lineInfo.LinePosition + ": " + member);
                                                journal.WriteEntry("warning: " + Warnings.Last());
                                            }
                                            else
                                            {
                                                Warnings.Add("could not add <typeparam> to incompatible member: " + member);
                                                journal.WriteEntry("warning: " + Warnings.Last());
                                            }
                                        }
                                        break;
                                    case "param":
                                        {
                                            if (member is Method method)
                                            {
                                                method.Params.Add(new Param(reader.GetAttribute("name"), reader.ReadInnerXml().Trim()));
                                                journal.WriteEntry(method.Params.Last().ToString());
                                            }
                                            else if (reader is IXmlLineInfo lineInfo)
                                            {
                                                Warnings.Add("could not add <param> to incompatible member at line " + lineInfo.LineNumber + ":" + lineInfo.LinePosition + ": " + member);
                                                journal.WriteEntry("warning: " + Warnings.Last());
                                            }
                                            else
                                            {
                                                Warnings.Add("could not add <param> to incompatible member: " + member);
                                                journal.WriteEntry("warning: " + Warnings.Last());
                                            }
                                        }
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
                                                method.Exceptions.Add(new Exception(cref, reader.ReadInnerXml().Trim()));
                                                journal.WriteEntry(method.Exceptions.Last().ToString());
                                            }
                                            else if (reader is IXmlLineInfo lineInfo)
                                            {
                                                Warnings.Add("could not add <exception> to incompatible member at line " + lineInfo.LineNumber + ":" + lineInfo.LinePosition + ": " + member);
                                                journal.WriteEntry("warning: " + Warnings.Last());
                                            }
                                            else
                                            {
                                                Warnings.Add("could not add <exception> to incompatible member: " + member);
                                                journal.WriteEntry("warning: " + Warnings.Last());
                                            }
                                        }
                                        break;
                                }
                                break;
                            case XmlNodeType.EndElement:
                                switch (reader.Name)
                                {
                                    case "member":
                                        goToNextMember = true;  // effectively breaks out of inner loop on continue
                                        continue;

                                }
                                break;
                        }
                    }
                    journal.Level--;
                    journal.WriteEntry("/");
                }
                journal.Level--;
                journal.WriteEntry("[end looping <member> elements]");
                journal.Level--;
                journal.WriteEntry("[end XmlDoc.Fill()]");
            }

            // finalize
            (VerifiedNamespaces as List<string>).Sort();

            return this;
        }

        private static Regex _nameAndParamsPattern = new Regex("(?<=([a-z0-9_.]+\\.)?)[#]?[a-z0-9_`]+(\\([a-z0-9_.,`@\\{\\}\\[\\]]*\\))?$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Parses the "name" attribute from a &lt;member&gt; element into an XmlDoc <c>Member</c> object
        /// </summary>
        /// <param name="rawName">raw name</param>
        /// <param name="fillInMissingTypes">Adds an undocumented type implied by the existence of a member of that type</param>
        /// <param name="journal">a journal</param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public Member ParseMember(string rawName, bool fillInMissingTypes = false, TraceJournal journal = null)
        {
            Member member;
            var parts = rawName.Split(':');
            var match = _nameAndParamsPattern.Match(parts[1]);
            if (!match.Success)
                throw new ValidationException("Cannot parse member name: " + parts[1]);

            // init journaling
            journal = journal ?? new TraceJournal();

            var name = match.Value;
            var prename = parts[1].Length == match.Value.Length
                ? null
                : parts[1].Substring(0, parts[1].Length - match.Value.Length - 1);

            switch (parts[0])
            {
                case "T":  // type     (name should not contain parentheses or params unless otherwise indicated)
                    return ParseType(parts[1], journal: journal);
                case "P":  // property
                    member = new Property(ParseType(prename, journal: journal), name);
                    if (fillInMissingTypes && (!Members.Any() || Members.Last().MemberType != member.MemberType))
                    {
                        Members.Add(member.MemberType);
                        journal.WriteEntry(member.MemberType + " [!missing]");
                    }
                    return member;
                case "M":  // method     (name nay contain parentheses and params)
                    member = new Method(ParseType(prename, journal: journal), name);
                    if (fillInMissingTypes && (!Members.Any() || Members.Last().MemberType != member.MemberType))
                    {
                        Members.Add(member.MemberType);
                        journal.WriteEntry(member.MemberType + " [!missing]");
                    }
                    return member;
                case "F":  // enum value
                    member = new EnumValue(ParseType(prename, journal: journal), name);
                    if (fillInMissingTypes && (!Members.Any() || Members.Last().MemberType != member.MemberType))
                    {
                        Members.Add(member.MemberType);
                        journal.WriteEntry(member.MemberType + " [!missing]");
                    }
                    return member;
                default:
                    throw new ValidationException("Unrecognized member type indicator: " + parts[0] + " (try one of T, P, M, F)");
            }
        }

        /// <summary>
        /// Takes a type string from XML and returns an XmlDoc <c>Type</c> object representing that type
        /// </summary>
        /// <param name="rawType">a type string from an XML "name" attribute</param>
        /// <param name="journal">a journal</param>
        /// <returns>XmlDoc <c>Type</c></returns>
        /// <exception cref="ValidationException"></exception>
        public Type ParseType(string rawType, TraceJournal journal = null)
        {
            // init journaling
            journal = journal ?? new TraceJournal();

            // try to reuse already parsed XmlDoc Type
            Type xdType = Members.FirstOrDefault(m => m is Type t && t.ToOriginalString() == rawType) as Type;
            if (xdType != null)
            {
                journal.WriteEntry("REUSED TYPE MEMBER - based on raw type");
                return xdType;
            }

            // try to detect type info via app domain assemblies
            System.Type _type = ParseSystemType(rawType);
            if (_type != null)
            {
                xdType = Members.FirstOrDefault(m => m is Type t && t.ToOriginalString() == _type.FullName) as Type;
                if (xdType != null)
                {
                    journal.WriteEntry("REUSED TYPE MEMBER - based on parsed system type");
                    return xdType;
                }
            }

            // return XmlDoc Type based on System.Type, if applicable, reverting nested type notation from MyClass+MyNestedClass back to MyClass.MyNestedClass
            if (_type != null)
            {
                if (!_type.IsClass)
                    return new Class(_type.Namespace, _type.FullName.Substring(_type.Namespace.Length + 1).Replace("+", "."));
                if (_type.IsEnum)
                    return new Enum(_type.Namespace, _type.FullName.Substring(_type.Namespace.Length + 1).Replace("+", "."));
                if (_type.IsInterface)
                    return new Interface(_type.Namespace, _type.FullName.Substring(_type.Namespace.Length + 1).Replace("+", "."));
                return new Struct(_type.Namespace, _type.FullName.Substring(_type.Namespace.Length + 1).Replace("+", "."));
            }

            // divide the type string into a namespace and name, last ditch best guess
            var match = _nameAndParamsPattern.Match(rawType);
            if (!match.Success)
                throw new ValidationException("Cannot parse type name: " + rawType);
            var name = match.Value;
            var @namespace = rawType.Length == match.Value.Length
                ? null
                : rawType.Substring(0, rawType.Length - match.Value.Length - 1);
            return new Type(@namespace, name);
        }

        /// <summary>
        /// Takes a type string from XML and attempts to look up an actual matching system type via app domain assemblies
        /// </summary>
        /// <param name="rawType">a type string from an XML "name" attribute</param>
        /// <param name="journal">a journal</param>
        /// <returns>XmlDoc <c>Type</c></returns>
        /// <exception cref="ValidationException"></exception>
        public System.Type ParseSystemType(string rawType, TraceJournal journal = null)
        {
            // init journaling
            journal = journal ?? new TraceJournal();

            // try to reuse already verified type
            if (VerifiedTypes.ContainsKey(rawType))
            {
                journal.WriteEntry("REUSED VERIFIED SYSTEM TYPE - based on raw type");
                return VerifiedTypes[rawType];
            }
            
            System.Type _type = TypeUtil.GetType(rawType, assemblyName: Assembly.Name);

            // if lookup fails try shifting '.' to '+' rtl to see if classes are nested e.g. MyNamespace.MyClass+MyNestedClass
            var _rawType = rawType;
            while (_type == null)
            {
                var pos = _rawType.LastIndexOf('.');
                if (pos == -1)
                    break;

                _rawType = _rawType.Substring(0, pos) + "+" + _rawType.Substring(pos + 1);
                if (VerifiedTypes.ContainsKey(_rawType))
                {
                    journal.WriteEntry("REUSED VERIFIED SYSTEM TYPE - based on raw type (. -> +)");
                    return VerifiedTypes[_rawType];
                }
                _type = TypeUtil.GetType(_rawType, assemblyName: Assembly.Name);

                // update the type cache for the modified type string, if applicable
                if (_type != null)
                {
                    VerifiedTypes.Add(_rawType, _type);
                    journal.WriteEntry("ADDED VERIFIED SYSTEM TYPE - based on raw type (. -> +)");
                }
            }

            // update both caches for the original type string, if applicable
            if (_type != null)
            {
                VerifiedTypes.Add(rawType, _type);
                journal.WriteEntry("ADDED VERIFIED SYSTEM TYPE - based on raw type");
                if (!VerifiedNamespaces.Contains(_type.Namespace))
                {
                    VerifiedNamespaces.Add(_type.Namespace);
                    journal.WriteEntry("ADDED VERIFIED NAMESPACE");
                }
            }
            return _type;
        }
    }
}
