using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
//using System.Xml;

using Horseshoe.NET.IO;
using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.XmlDoc
{
    /// <summary>
    /// Represents compiled XML documentation (e.g. XML produced by the C# compiler) featuring the ability to parse XML documentation 
    /// </summary>
    public static class XmlDocBuilder
    {
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
        public static XmlDoc Build
        (
            FilePath fileUri,
            FilePath? assemblyDllUri = null,
            NetworkCredential credentials = null,
            bool fillInMissingTypes = false, 
            TraceJournal journal = null
        )
        {
            // variable declaration
            XmlDoc xmlDoc = new XmlDoc();
            Member member;

            // init journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("[XmlDoc.Fill()]");
            journal.Level++;

            // cleanup
            xmlDoc.Assembly.Name = null;
            xmlDoc.Members.Clear();
            xmlDoc.VerifiedNamespaces.Clear();
            xmlDoc.VerifiedTypes.Clear();
            xmlDoc.Warnings.Clear();

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
                    return xmlDoc;

                // xml declaration e.g. <?xml version="1.0"?>
                if (reader.NodeType == XmlNodeType.XmlDeclaration)
                {
                    journal.WriteEntry("XML: " + reader.Value);
                }

                // assembly name 
                if (!reader.ReadToFollowing("assembly"))
                    return xmlDoc;
                if (!reader.ReadToDescendant("name"))
                    return xmlDoc;
                journal.WriteEntry("assembly name: " + reader.ReadInnerXml().Trim());
                journal.WriteEntry("[start looping <member> elements]");
                journal.Level++;

                // scan <member> elements
                while (reader.ReadToFollowing("member"))  // [outer loop]
                {
                    member = ParseMember(xmlDoc, reader.GetAttribute("name"), fillInMissingTypes: fillInMissingTypes, journal: journal);
                    xmlDoc.Members.Add(member);
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
                                                xmlDoc.Warnings.Add("could not add <typeparam> to incompatible member at line " + lineInfo.LineNumber + ":" + lineInfo.LinePosition + ": " + member);
                                                journal.WriteEntry("warning: " + xmlDoc.Warnings.Last());
                                            }
                                            else
                                            {
                                                xmlDoc.Warnings.Add("could not add <typeparam> to incompatible member: " + member);
                                                journal.WriteEntry("warning: " + xmlDoc.Warnings.Last());
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
                                                xmlDoc.Warnings.Add("could not add <param> to incompatible member at line " + lineInfo.LineNumber + ":" + lineInfo.LinePosition + ": " + member);
                                                journal.WriteEntry("warning: " + xmlDoc.Warnings.Last());
                                            }
                                            else
                                            {
                                                xmlDoc.Warnings.Add("could not add <param> to incompatible member: " + member);
                                                journal.WriteEntry("warning: " + xmlDoc.Warnings.Last());
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
                                                xmlDoc.Warnings.Add("could not add <exception> to incompatible member at line " + lineInfo.LineNumber + ":" + lineInfo.LinePosition + ": " + member);
                                                journal.WriteEntry("warning: " + xmlDoc.Warnings.Last());
                                            }
                                            else
                                            {
                                                xmlDoc.Warnings.Add("could not add <exception> to incompatible member: " + member);
                                                journal.WriteEntry("warning: " + xmlDoc.Warnings.Last());
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
            (xmlDoc.VerifiedNamespaces as List<string>).Sort();

            return xmlDoc;
        }

        private static Regex _nameAndParamsPattern = new Regex("(?<=([a-z0-9_.]+\\.)?)[#]?[a-z0-9_`]+(\\([a-z0-9_.,`@\\{\\}\\[\\]]*\\))?$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Parses the "name" attribute from a &lt;member&gt; element into an XmlDoc <c>Member</c> object.
        /// </summary>
        /// <param name="xmlDoc">An <c>XmlDoc</c> instance.</param>
        /// <param name="rawName">A raw XML member name.</param>
        /// <param name="fillInMissingTypes">Adds an undocumented type implied by the existence of a member of that type.</param>
        /// <param name="journal">A journal.</param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public static Member ParseMember(XmlDoc xmlDoc, string rawName, bool fillInMissingTypes = false, TraceJournal journal = null)
        {
            Member member;
            var parts = rawName.Split(':');
            var match = _nameAndParamsPattern.Match(parts[1]);
            if (!match.Success)
                throw new ValidationException("Cannot parse member name: " + parts[1]);
           
            // init journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }

            var name = match.Value;
            var prename = parts[1].Length == match.Value.Length
                ? null
                : parts[1].Substring(0, parts[1].Length - match.Value.Length - 1);

            switch (parts[0])
            {
                case "T":  // type     (name should not contain parentheses or params unless otherwise indicated)
                    return ParseType(xmlDoc, parts[1], journal: journal);
                case "P":  // property
                    member = new Property(ParseType(xmlDoc, prename, journal: journal), name);
                    if (fillInMissingTypes && (!xmlDoc.Members.Any() || xmlDoc.Members.Last().MemberType != member.MemberType))
                    {
                        xmlDoc.Members.Add(member.MemberType);
                        journal.WriteEntry(member.MemberType + " [!missing]");
                    }
                    return member;
                case "M":  // method     (name nay contain parentheses and params)
                    member = new Method(ParseType(xmlDoc, prename, journal: journal), name);
                    if (fillInMissingTypes && (!xmlDoc.Members.Any() || xmlDoc.Members.Last().MemberType != member.MemberType))
                    {
                        xmlDoc.Members.Add(member.MemberType);
                        journal.WriteEntry(member.MemberType + " [!missing]");
                    }
                    return member;
                case "F":  // enum value
                    member = new EnumValue(ParseType(xmlDoc, prename, journal: journal), name);
                    if (fillInMissingTypes && (!xmlDoc.Members.Any() || xmlDoc.Members.Last().MemberType != member.MemberType))
                    {
                        xmlDoc.Members.Add(member.MemberType);
                        journal.WriteEntry(member.MemberType + " [!missing]");
                    }
                    return member;
                default:
                    throw new ValidationException("Unrecognized member type indicator: " + parts[0] + " (try one of T, P, M, F)");
            }
        }

        /// <summary>
        /// Takes a type string from XML and returns an XmlDoc <c>Type</c> object representing that type.
        /// </summary>
        /// <param name="xmlDoc">An <c>XmlDoc</c> instance.</param>
        /// <param name="rawType">A type string from an XML "name" attribute.</param>
        /// <param name="journal">A journal.</param>
        /// <returns>XmlDoc <c>Type</c></returns>
        /// <exception cref="ValidationException"></exception>
        public static Type ParseType(XmlDoc xmlDoc, string rawType, TraceJournal journal = null)
        {
            // init journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }

            // try to reuse already parsed XmlDoc Type
            Type xdType = xmlDoc.Members.FirstOrDefault(m => m is Type t && t.ToOriginalString() == rawType) as Type;
            if (xdType != null)
            {
                journal.WriteEntry("REUSED TYPE MEMBER - based on raw type");
                return xdType;
            }

            // try to detect type info via app domain assemblies
            System.Type _type = ParseSystemType(xmlDoc, rawType);
            if (_type != null)
            {
                xdType = xmlDoc.Members.FirstOrDefault(m => m is Type t && t.ToOriginalString() == _type.FullName) as Type;
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
        /// <param name="xmlDoc">An <c>XmlDoc</c> instance.</param>
        /// <param name="rawType">A type string from an XML "name" attribute.</param>
        /// <param name="journal">A journal.</param>
        /// <returns>XmlDoc <c>Type</c></returns>
        /// <exception cref="ValidationException"></exception>
        public static System.Type ParseSystemType(XmlDoc xmlDoc, string rawType, TraceJournal journal = null)
        {
            // init journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }

            // try to reuse already verified type
            if (xmlDoc.VerifiedTypes.ContainsKey(rawType))
            {
                journal.WriteEntry("REUSED VERIFIED SYSTEM TYPE - based on raw type");
                return xmlDoc.VerifiedTypes[rawType];
            }
            
            System.Type _type = TypeUtil.GetType(rawType, assemblyName: xmlDoc.Assembly.Name);

            // if lookup fails try shifting '.' to '+' rtl to see if classes are nested e.g. MyNamespace.MyClass+MyNestedClass
            var _rawType = rawType;
            while (_type == null)
            {
                var pos = _rawType.LastIndexOf('.');
                if (pos == -1)
                    break;

                _rawType = _rawType.Substring(0, pos) + "+" + _rawType.Substring(pos + 1);
                if (xmlDoc.VerifiedTypes.ContainsKey(_rawType))
                {
                    journal.WriteEntry("REUSED VERIFIED SYSTEM TYPE - based on raw type (. -> +)");
                    return xmlDoc.VerifiedTypes[_rawType];
                }
                _type = TypeUtil.GetType(_rawType, assemblyName: xmlDoc.Assembly.Name);

                // update the type cache for the modified type string, if applicable
                if (_type != null)
                {
                    xmlDoc.VerifiedTypes.Add(_rawType, _type);
                    journal.WriteEntry("ADDED VERIFIED SYSTEM TYPE - based on raw type (. -> +)");
                }
            }

            // update both caches for the original type string, if applicable
            if (_type != null)
            {
                xmlDoc.VerifiedTypes.Add(rawType, _type);
                journal.WriteEntry("ADDED VERIFIED SYSTEM TYPE - based on raw type");
                if (!xmlDoc.VerifiedNamespaces.Contains(_type.Namespace))
                {
                    xmlDoc.VerifiedNamespaces.Add(_type.Namespace);
                    journal.WriteEntry("ADDED VERIFIED NAMESPACE");
                }
            }
            return _type;
        }
    }
}
