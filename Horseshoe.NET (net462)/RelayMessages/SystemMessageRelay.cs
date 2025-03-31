using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Horseshoe.NET.Dotnet;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.RelayMessages
{
    /// <summary>
    /// This is where all of Horseshoe.NET relays its messages and exceptions
    /// </summary>
    public static class SystemMessageRelay
    {
        private static List<IMessageRelay> listeners;

        /// <summary>
        /// The programming language to which to render .NET objects.  Default is <c>C#</c>.
        /// </summary>
        public static DotnetLanguage Lang { get; set; }

        /// <summary>
        /// Include declaring type by default when relaying method info
        /// </summary>
        public static bool IncludeMethodDeclaringType { get; set; }

        /// <summary>
        /// Embed params by default when relaying method info
        /// </summary>
        public static bool IncludeMethodParams { get; set; }

        /// <summary>
        /// Get a peek at all of the message relay listeners registered in Horseshoe.NET
        /// </summary>
        /// <returns></returns>
        public static IMessageRelay[] GetListeners()
        {
            return listeners != null
                ? listeners.ToArray()
                : Array.Empty<IMessageRelay>();
        }

        /// <summary>
        /// Register a message relay listener in Horseshoe.NET.  See also <see cref="Listener"/>.
        /// </summary>
        /// <param name="listener">a message relay listener</param>
        public static void AddListener(IMessageRelay listener)
        {
            if (listeners == null)
                listeners = new List<IMessageRelay>();
            listeners.Add(listener);
        }

        /// <summary>
        /// Setting this value replaces all other previously registered listeners, if any, with this one.
        /// Getting this value will return only the first registered listener, if any.
        /// </summary>
        public static IMessageRelay Listener
        {
            get
            {
                return listeners?.FirstOrDefault();
            }
            set
            {
                listeners?.Clear();
                AddListener(value);
            }
        }

        /// <summary>
        /// The base mechanism for relaying system messages i.e. messages sent from within Horseshoe.NET[.Xxxxx] methods.
        /// </summary>
        /// <param name="message">A message to relay to calling code.</param>
        /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        /// <param name="indent">Instructions for indenting relayed messages.</param>
        public static void RelayMessage(string message, string group = null, int? id = null, Indent indent = null)
        {
            if (listeners == null)
                return;
            foreach (var listener in listeners)
            {
                listener.Message(message, group: group, id: id, indent: indent);
            }
        }

        /// <summary>
        /// The base mechanism for relaying system messages i.e. messages sent from within Horseshoe.NET[.Xxxxx] methods.
        /// </summary>
        /// <param name="messageFunc">A function/lambda which returns a message to relay to calling code.</param>
        /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        /// <param name="indent">Instructions for indenting relayed messages.</param>
        public static void RelayMessage(Func<string> messageFunc, string group = null, int? id = null, Indent indent = null)
        {
            if (listeners == null)
                return;

            foreach (var listener in listeners)
            {
                if (messageFunc != null)
                    listener.Message(messageFunc.Invoke(), group: group, id: id, indent: indent);
                else
                    listener.Message("[null-func]", group: group, id: id, indent: indent);
            }
        }

        /// <summary>
        /// The base mechanism for relaying system messages i.e. messages sent from within Horseshoe.NET[.Xxxxx] methods.
        /// </summary>
        /// <param name="name">A variable name or value name.</param>
        /// <param name="value">A value.</param>
        /// <param name="dump">If <c>true</c>, displays the object's properties in JSON-like format, default is <c>false</c>.</param>
        /// <param name="cropToLength">Reduce size of the relayed messages by cropping to this length.</param>
        /// <param name="cropPosition">Where in the relayed message to crop, default is <c>HorizontalPosition.Center</c>.</param>
        /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        /// <param name="indent">Instructions for indenting relayed messages.</param>
        public static void RelayValue(string name, object value, bool dump = false, int? cropToLength = null, HorizontalPosition cropPosition = HorizontalPosition.Center, string group = null, int? id = null, Indent indent = null)
        {
            if (listeners == null)
                return;

            var returnStr = dump
                ? ObjectUtil.DumpToString(value)
                : ValueUtil.Display(value);

            if (cropToLength.HasValue)
                returnStr = TextUtil.Crop(returnStr, cropToLength.Value, position: cropPosition, truncateMarker: TruncateMarker.LongEllipsis);

            RelayMessage
            (
                name + ": " + returnStr,
                group: group,
                id: id,
                indent: indent
            );
        }

        /// <summary>
        /// The base mechanism for relaying system messages i.e. messages sent from within Horseshoe.NET[.Xxxxx] methods.
        /// </summary>
        /// <param name="name">A variable name or value name.</param>
        /// <param name="valueFunc">A function/lambda which returns a value to relay to calling code.</param>
        /// <param name="dump">If <c>true</c>, displays the object's properties in JSON-like format, default is <c>false</c>.</param>
        /// <param name="cropToLength">Reduce size of the relayed messages by cropping to this length.</param>
        /// <param name="cropPosition">Where in the relayed message to crop, default is <c>HorizontalPosition.Center</c>.</param>
        /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        /// <param name="indent">Instructions for indenting relayed messages.</param>
        public static void RelayValue(string name, Func<object> valueFunc, bool dump = false, int? cropToLength = null, HorizontalPosition cropPosition = HorizontalPosition.Center, string group = null, int? id = null, Indent indent = null)
        {
            if (listeners == null)
                return;

            string returnStr;

            if (valueFunc != null)
                returnStr = dump
                    ? ObjectUtil.DumpToString(valueFunc.Invoke())
                    : ValueUtil.Display(valueFunc.Invoke());
            else
                returnStr = "[null-func]";

            if (cropToLength.HasValue)
                returnStr = TextUtil.Crop(returnStr, cropToLength.Value, position: cropPosition, truncateMarker: TruncateMarker.LongEllipsis);

            RelayMessage
            (
                name + ": " + returnStr,
                group: group,
                id: id,
                indent: indent
            );
        }

        /// <summary>
        /// Relays a message detailing the current method, e.g. <c>MyMethod(int myParam)</c>.
        /// This relayed message will, by default, increment indentation of subsequent messages.
        /// See also <see cref="RelayMethodReturnValue(object, bool, int?, HorizontalPosition, string, int?, Indent)"/>
        /// </summary>
        /// <param name="method">The current method.</param>
        /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        /// <param name="indent">Instructions for indenting relayed messages.</param>
        /// <param name="includeDeclaringType">If <c>true</c>, relays a message containing the fully qualified type, e.g. <c>(MyNamespace.MyClass)</c>, prior to relaying the method string.  Default is <c>false</c></param>
        /// <param name="includeParams">If <c>true</c>, the method's parameters (or a close approximation thereof) will be included in the generated output, default is <c>false</c>..</param>
        public static void RelayMethodInfo(MethodBase method, string group = null, int? id = null, Indent indent = null, bool? includeDeclaringType = false, bool? includeParams = false)
        {
            if (listeners == null)
                return;

            RelayMethodInfo
            (
                DotnetUtil.ReconstructMethod(method, includeDeclaringType: false, includeParams: includeParams ?? IncludeMethodParams),
                declaringTypeString: (includeDeclaringType ?? IncludeMethodDeclaringType) ? method.DeclaringType.FullName : null,
                group: group,
                id: id,
                indent: indent ?? Indent.IncrementNext
            );
        }

        /// <summary>
        /// Relays a message detailing the calling method, e.g. <c>MyMethod(int myParam)</c>. Optimization, only executes if there are listeners.
        /// This relayed message will, by default, increment indentation of subsequent messages.
        /// See also <see cref="RelayMethodReturnValue(object, bool, int?, HorizontalPosition, string, int?, Indent)"/>
        /// </summary>
        /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        /// <param name="indent">Instructions for indenting relayed messages.</param>
        /// <param name="includeDeclaringType">If <c>true</c>, relays a message containing the fully qualified type, e.g. <c>(MyNamespace.MyClass)</c>, prior to relaying the method string.  Default is <c>false</c></param>
        /// <param name="includeParams">If <c>true</c>, the method's parameters (or a close approximation thereof) will be included in the generated output, default is <c>false</c>..</param>
        public static void RelayMethodInfo(string group = null, int? id = null, Indent indent = null, bool? includeDeclaringType = false, bool? includeParams = false)
        {
            if (listeners == null)
                return;

            RelayMethodInfo
            (
                new StackTrace().GetFrame(1).GetMethod(), // ref: https://code-maze.com/csharp-how-to-find-caller-method/
                group: group,
                id: id,
                indent: indent ?? Indent.IncrementNext,
                includeDeclaringType: includeDeclaringType,
                includeParams: includeParams
            );
        }

        /// <summary>
        /// Relays a message detailing the current method.
        /// This relayed message will, by default, increment indentation of subsequent messages.
        /// See also <see cref="RelayMethodReturnValue(object, bool, int?, HorizontalPosition, string, int?, Indent)"/>
        /// </summary>
        /// <param name="declaringTypeString">The optional fully qualified declaring type.  If supplied, relays an additional message before the <c>methodString</c> message.</param>
        /// <param name="methodString">The current method including, by convention, declaring type and parameters.</param>
        /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        /// <param name="indent">Instructions for indenting relayed messages.</param>
        public static void RelayMethodInfo(string methodString, string declaringTypeString = null, string group = null, int? id = null, Indent indent = null)
        {
            if (!string.IsNullOrEmpty(declaringTypeString))
            {
                RelayMessage("(" + declaringTypeString + ")", group: group, indent: indent ?? Indent.IncrementNext);
                RelayMessage(methodString ?? TextConstants.Null, group: group, id: id);
            }
            else
            {
                RelayMessage(methodString ?? TextConstants.Null, group: group, id: id, indent: indent ?? Indent.IncrementNext);
            }
        }

        /// <summary>
        /// Relays a single method parameter, by convention this must directly follow a relayed method info or another method parameter.
        /// </summary>
        /// <param name="paramName">A parameter name</param>
        /// <param name="value">A parame</param>
        /// <param name="group">A message group optionally used by implementations for grouping, searching or filtering messages.</param>
        public static void RelayMethodParam(string paramName, object value, string group = null)
        {
            RelayMessage("   param: " + (paramName ?? TextConstants.Null) + " = " + DotnetUtil.GetSourceCodeFormattedValue(value, lang: Lang), group: group);
        }

        /// <summary>
        /// Relays a group of method parameters, by convention this must directly follow a relayed method info or other method parameters.
        /// </summary>
        /// <param name="params">A group of parameter names and values</param>
        /// <param name="group">A message group optionally used by implementations for grouping, searching or filtering messages.</param>
        public static void RelayMethodParams(IDictionary<string, object> @params, string group = null)
        {
            if (@params == null)
                return;
            foreach (var param in @params) 
            {
                RelayMethodParam(param.Key, param.Value, group: group);
            }
        }

        /// <summary>
        /// Relays the return value of a method, by convention this occurs at the end of a relayed method's scope.
        /// This relayed message will, by default, decrement indentation of subsequent messages.
        /// </summary>
        /// <param name="value">A return value</param>
        /// <param name="dump">If <c>true</c>, displays the object's properties in JSON-like format, default is <c>false</c>.</param>
        /// <param name="cropToLength">Reduce size of the relayed messages by cropping to this length.</param>
        /// <param name="cropPosition">Where in the relayed message to crop, default is <c>HorizontalPosition.Center</c>.</param>
        /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        /// <param name="indent">Instructions for indenting relayed messages.</param>
        public static void RelayMethodReturnValue(object value, bool dump = false, int? cropToLength = null, HorizontalPosition cropPosition = HorizontalPosition.Center, string group = null, int? id = null, Indent indent = null)
        {
            if (listeners == null)
                return;

            var returnStr = dump 
                ? ObjectUtil.DumpToString(value) 
                : ValueUtil.Display(value);

            if (cropToLength.HasValue)
                returnStr = TextUtil.Crop(returnStr, cropToLength.Value, position: cropPosition, truncateMarker: TruncateMarker.LongEllipsis);

            RelayMessage
            (
                "return " + returnStr,
                group: group,
                id: id,
                indent: indent ?? Indent.DecrementNext
            );
        }

        /// <summary>
        /// Relays the return value of a method, by convention this occurs at the end of a relayed method's scope.
        /// This relayed message will, by default, decrement indentation of subsequent messages.
        /// </summary>
        /// <param name="valueFunc">A function/lambda which returns the return value</param>
        /// <param name="dump">If <c>true</c>, displays the object's properties in JSON-like format, default is <c>false</c>.</param>
        /// <param name="cropToLength">Reduce size of the relayed messages by cropping to this length.</param>
        /// <param name="cropPosition">Where in the relayed message to crop, default is <c>HorizontalPosition.Center</c>.</param>
        /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        /// <param name="indent">Instructions for indenting relayed messages.</param>
        public static void RelayMethodReturnValue<T>(Func<T> valueFunc, bool dump = false, int? cropToLength = null, HorizontalPosition cropPosition = HorizontalPosition.Center, string group = null, int? id = null, Indent indent = null)
        {
            if (listeners == null)
                return;

            string returnStr;
            if (valueFunc != null)
                returnStr = "[null-func]";
            else if (dump)
                returnStr = ObjectUtil.DumpToString(valueFunc.Invoke());
            else
                returnStr = ValueUtil.Display(valueFunc.Invoke());

            if (cropToLength.HasValue)
                returnStr = TextUtil.Crop(returnStr, cropToLength.Value, position: cropPosition, truncateMarker: TruncateMarker.LongEllipsis);

            RelayMessage
            (
                "return " + returnStr,
                group: group,
                id: id,
                indent: indent ?? Indent.DecrementNext
            );
        }

        /// <summary>
        /// Relays the return of a method, by convention this occurs at the end of a relayed method's scope.
        /// This relayed message will, by default, decrement indentation of subsequent messages.
        /// </summary>
        /// <param name="returnDescription">An optional name for the return value.</param>
        /// <param name="group">A category that implementations may use for message grouping, searching or filtering.</param>
        /// <param name="id">An optional message ID that implementations may use for more granular and precise message handling.</param>
        /// <param name="indent">Instructions for indenting relayed messages.</param>
        public static void RelayMethodReturn(string returnDescription = null, string group = null, int? id = null, Indent indent = null)
        {
            if (listeners == null)
                return;

            var returnStr = "return";
            if (returnDescription != null)
            {
                returnStr += " " + returnDescription;
            }

            RelayMessage
            (
                returnStr,
                group: group,
                id: id,
                indent: indent ?? Indent.DecrementNext
            );
        }

        /// <summary>
        /// The base mechanism for relaying system exceptions i.e. exceptions thrown from within Horseshoe.NET[.Xxxxx] methods.
        /// </summary>
        /// <param name="exception">An exception to relay to calling code.</param>
        /// <param name="group">A message group optionally used by implementations for grouping, searching or filtering messages.</param>
        /// <param name="inlineWithMessages">
        /// If <c>true</c>, indents this exception at the same level as the last relayed message.  Default is <c>false</c>.  
        /// The only built-in implementation that is guaranteed to honor this argument is <c>RelayIndentedMessageBase</c>  
        /// (i.e. subclasses <c>RelayToConsole</c> and <c>RelayToFile</c>).
        /// </param>
        public static void RelayException(Exception exception, string group = null, bool inlineWithMessages = false)
        {
            if (listeners == null)
                return;
            foreach (var listener in listeners)
            {
                listener.Exception(exception, group: group, inlineWithMessages: inlineWithMessages);
            }
        }

        /// <summary>
        /// Increment indentation
        /// </summary>
        public static void IncrementIndentation()
        {
            if (listeners == null)
                return;
            foreach (var listener in listeners)
            {
                if (listener is IIndentedMessageRelay indentedListener)
                    indentedListener.IncrementIndentation();
            }
        }

        /// <summary>
        /// Decrement indentation
        /// </summary>
        public static void DecrementIndentation()
        {
            if (listeners == null)
                return;
            foreach (var listener in listeners)
            {
                if (listener is IIndentedMessageRelay indentedListener)
                    indentedListener.DecrementIndentation();
            }
        }
    }
}
