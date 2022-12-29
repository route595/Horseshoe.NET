using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;

using Horseshoe.NET.Collections;
using Horseshoe.NET.ObjectsAndTypes;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A collection of methods for prompting users for different types of input on the console
    /// </summary>
    public static class PromptX
    {
        /// <summary>
        /// Prompts for input, accepts free text as well as certain commands i.e. 'cancel', 'exit' by default
        /// </summary>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="displayAsRequired">If <c>true</c>, suggests to the renderer to mark this input as required.</param>
        /// <param name="quickText">an optional common or predictive input that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="autoTrim">whether to trim leading and trailing whitespaces, default is <c>true</c></param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The text entered by the user</returns>
        public static string Input
        (
            bool required = false,
            bool displayAsRequired = false,
            string quickText = null, 
            int padBefore = 0, 
            int padAfter = 0, 
            bool autoTrim = true,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Input
            (
                null,
                required: required,
                displayAsRequired: displayAsRequired,
                quickText: quickText, 
                autoTrim: autoTrim,
                padBefore: padBefore, 
                padAfter: padAfter, 
                canCancel: canCancel, 
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for input, accepts free text as well as certain commands i.e. 'cancel', 'exit' by default
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="displayAsRequired">If <c>true</c>, suggests to the renderer to mark this input as required.</param>
        /// <param name="quickText">an optional common or predictive input that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="autoTrim">whether to trim leading and trailing whitespaces, default is <c>true</c></param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The text entered by the user</returns>
        public static string Input
        (
            string prompt, 
            bool required = false,
            bool displayAsRequired = false,
            string quickText = null,
            int padBefore = 0, 
            int padAfter = 0, 
            bool autoTrim = true,
            bool canCancel = true, 
            bool canExitApp = true
        )
        {
            RenderX.Pad(padBefore);
            if (!string.IsNullOrWhiteSpace(quickText))
            {
                Console.WriteLine("(Press 'Enter' to use \"" + quickText + "\")");
            }

            string input;
            while (true)
            {
                RenderX.Prompt(prompt, required: required || displayAsRequired);
                input = autoTrim 
                    ? Console.ReadLine().Trim()
                    : Console.ReadLine();
                if (input.Equals(""))
                {
                    if (quickText != null)
                    {
                        input = quickText;
                        RenderX.Prompt(prompt, required: required || displayAsRequired);
                        Console.WriteLine(input);
                    }
                    else if (required)
                    {
                        RenderX.Alert("An input is required.");
                        continue;
                    }
                }
                else
                {
                    switch (input.ToLower())
                    {
                        case "exit":
                            if (canExitApp)
                            {
                                Environment.Exit(0);
                            }
                            break;
                        case "cancel":
                            if (canCancel)
                            {
                                ConsoleNavigation.CancelPrompt();
                            }
                            break;
                    }
                }
                break;
            }
            RenderX.Pad(padAfter);
            return input;
        }

        /// <summary>
        /// Prompts for input, accepts free text with no string trimming and no command recognition
        /// </summary>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="displayAsRequired">If <c>true</c>, suggests to the renderer to mark this input as required.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The exact text entered by the user</returns>
        public static string InputVerbatim
        (
            bool required = false,
            bool displayAsRequired = false,
            int padBefore = 0, 
            int padAfter = 0
        )
        {
            return InputVerbatim
            (
                null,
                required: required,
                displayAsRequired: displayAsRequired,
                padBefore: padBefore, 
                padAfter: padAfter
            );
        }

        /// <summary>
        /// Prompts for input, accepts free text with no string trimming and no command recognition.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="displayAsRequired">If <c>true</c>, suggests to the renderer to mark this input as required.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The exact text entered by the user</returns>
        public static string InputVerbatim
        (
            string prompt,
            bool required = false,
            bool displayAsRequired = false,
            int padBefore = 0, 
            int padAfter = 0
        )
        {
            RenderX.Pad(padBefore);
            RenderX.Prompt(prompt, required: required || displayAsRequired);
            string input = Console.ReadLine();
            RenderX.Pad(padAfter);
            return input;
        }

        /// <summary>
        /// Prompts for input, blanks become <c>null</c>s, accepts free text as well as certain commands i.e. 'cancel', 'exit' by default
        /// </summary>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The text entered by the user (or <c>null</c>)</returns>
        public static string NInput
        (
            int padBefore = 0, 
            int padAfter = 0,
            bool canCancel = true, 
            bool canExitApp = true
        )
        {
            return Zap.String
            (
                Input
                (
                    padBefore: padBefore, 
                    padAfter: padAfter,
                    canCancel: canCancel, 
                    canExitApp: canExitApp
               )
            );
        }

        /// <summary>
        /// Prompts for input, blanks become <c>null</c>s, accepts free text as well as certain commands i.e. 'cancel', 'exit' by default
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The text entered by the user (or <c>null</c>)</returns>
        public static string NInput
        (
            string prompt, 
            int padBefore = 0, 
            int padAfter = 0,
            bool canCancel = true, 
            bool canExitApp = true
        )
        {
            return Zap.String
            (
                Input
                (
                    prompt, 
                    padBefore: padBefore, 
                    padAfter: padAfter,
                    canCancel: canCancel, 
                    canExitApp: canExitApp
                )
            );
        }

        /// <summary>
        /// Prompts for typesafe input, accepts <c>T</c> values only as well as certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <typeparam name="T">a reference type</typeparam>
        /// <param name="parser">An optional custom text-to-value converter</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="numberStyle">Applies to <c>Value&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>Value&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>Value&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>Value&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>Value&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>Value&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="inheritedType">An optional type constraint - the type to which the returned <c>Type</c> must be assignable.</param>
        /// <param name="ignoreCase">Applies to <c>Value&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static T Value<T>
        (
            Func<string, object> parser = null,
            bool required = false,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            Type inheritedType = null,
            bool ignoreCase = false,
            T quickValue = default,
            Action<T> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        ) 
        {
            return Value
            (
                null,
                parser: parser,
                required: required,
                numberStyle: numberStyle,
                provider: provider,
                locale: locale,
                trueValues: trueValues,
                falseValues: falseValues,
                encoding: encoding,
                inheritedType: inheritedType,
                ignoreCase: ignoreCase,
                quickValue: quickValue,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for typesafe input, accepts <c>T</c> values only as well as certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="parser">An optional custom text-to-value converter</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="numberStyle">Applies to <c>Value&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>Value&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>Value&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>Value&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>Value&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>Value&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="inheritedType">An optional type constraint - the type to which the returned <c>Type</c> must be assignable.</param>
        /// <param name="ignoreCase">Applies to <c>Value&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static T Value<T>
        (
            string prompt,
            Func<string, object> parser = null,
            bool required = false,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            Type inheritedType = null,
            bool ignoreCase = false,
            T quickValue = default,
            Action<T> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            if (typeof(T).IsEnumType())
                return Enum<T>(prompt, required: required, padBefore: padBefore, padAfter: padAfter);

            while (true)
            {
                string input = Input
                (
                    prompt,
                    required: required,
                    quickText: Equals(quickValue, default(T)) ? null : quickValue?.ToString(),
                    padBefore: padBefore,
                    padAfter: padAfter,
                    autoTrim: true,
                    canCancel: canCancel,
                    canExitApp: canExitApp
                );

                try
                {
                    if (parser != null)
                        return (T)parser.Invoke(input);
                    var value = input.Length > 0
                        ? Zap.To<T>
                          (
                            input,
                            numberStyle: numberStyle,
                            provider: provider,
                            locale: locale,
                            trueValues: trueValues,
                            falseValues: falseValues,
                            encoding: encoding,
                            inheritedType: inheritedType,
                            ignoreCase: ignoreCase
                          )
                        : quickValue;
                    if (value == null)
                        return default;
                    validator?.Invoke(value);
                    return value;
                }
                catch (Exception ex)
                {
                    RenderX.Alert(ex.RenderMessage(typeRendering: ExceptionTypeRenderingPolicy.FqnExceptSystem));
                    if (ex is ConversionException cex && cex.IsConverterNotSupplied)
                    {
                        ConsoleNavigation.CancelPrompt(cex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Prompts for <c>enum</c> value, also accepts certain commands i.e. 'cancel', 'exit' by default
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="title">An optional list title.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="except">Any <c>enum</c> values you want to omit from the list of choices.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value entered by the user</returns>
        public static T Enum<T>
        (
            Title? title = null, 
            bool required = false, 
            IList<T> except = null, 
            int padBefore = 0, 
            int padAfter = 0
        )
        {
            // validation
            if (!typeof(T).IsEnumType())
                throw new ValidationException("Not an enum type: " + typeof(T).FullName);

            // transform to non-nullable
            Type enumTypeForList = typeof(T);
            if (typeof(T).TryGetUnderlyingType(out Type underlyingType))
                enumTypeForList = underlyingType;

            // prepare variables
            T choice = default;
            var enumValues = System.Enum.GetValues(enumTypeForList)
                .Cast<T>()
                .Where(e => !e.In(except));

            // create / configure custom menu
            if (title == null)
            {
                title = enumTypeForList.Name + (required ? RenderX.RequiredIndicator : "");
            }
            else if (required)
            {
                title += (required ? RenderX.RequiredIndicator : "");
            }

            // create / customize menu
            var customItems = enumValues
                .Select(e => RoutineX.BuildCustomRoutine(e.ToString(), () => choice = e, ((int)(object)e).ToString()) as MenuObject)
                .ToList();

            if (underlyingType != null)
            {
                customItems.Insert(0, RoutineX.BuildCustomRoutine("Press 'Enter' for null...", () => { }));
            }

            // prompt custom menu for choice selection
            var menuSelection = Menu(null as IList<T>, customItemsToAppend: customItems, title: title, padBefore: padBefore, padAfter: padAfter);

            return choice;
        }

        /// <summary>
        /// Prompt a user to choose from a collection of items
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="collection">a collection</param>
        /// <param name="title">a collection title</param>
        /// <param name="indexPolicy">whether to display an index and whether it is 0-based</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="choicePrompt">the text to render at the prompt.</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying collection items</param>
        /// <param name="padBefore">The number of new lines to render before the collection</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        public static T List<T>
        (
            IEnumerable<T> collection,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            string choicePrompt = ">",
            Func<T, string> renderer = null,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            return List
            (
                collection?.ToList(), 
                title: title, 
                indexPolicy: indexPolicy, 
                required: required, 
                choicePrompt: choicePrompt, 
                renderer: renderer, 
                padBefore: padBefore, 
                padAfter: padAfter
            );
        }

        /// <summary>
        /// Prompt a user to choose from a list of items
        /// </summary>
        /// <typeparam name="T">type of item</typeparam>
        /// <param name="list">a list</param>
        /// <param name="title">a list title</param>
        /// <param name="indexPolicy">whether to display an index and whether it is 0-based</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="choicePrompt">the text to render at the prompt.</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="padBefore">The number of new lines to render before the list</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        public static T List<T>
        (
            IList<T> list,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            string choicePrompt = null,
            Func<T, string> renderer = null,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            return List
            (
                list,
                out _,
                title: title,
                indexPolicy: indexPolicy,
                required: required,
                choicePrompt: choicePrompt,
                renderer: renderer,
                padBefore: padBefore,
                padAfter: padAfter
            );
        }

        /// <summary>
        /// Prompt a user to choose from a list of items
        /// </summary>
        /// <typeparam name="T">type of item</typeparam>
        /// <param name="list">a list</param>
        /// <param name="selectedIndex">returns the index selected by the user</param>
        /// <param name="title">a list title</param>
        /// <param name="indexPolicy">whether to display an index and whether it is 0-based</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="choicePrompt">the text to render at the prompt.</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="padBefore">The number of new lines to render before the list</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <exception cref="ValidationException"></exception>
        /// <remarks><seealso cref="Enum"/></remarks>
        public static T List<T>
        (
            IList<T> list,
            out int selectedIndex,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            string choicePrompt = null,
            Func<T, string> renderer = null,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            if (indexPolicy != ListIndexPolicy.DisplayZeroBased && indexPolicy != ListIndexPolicy.DisplayOneBased)
            {
                throw new ValidationException("Invalid indexPolicy: " + indexPolicy);
            }

            T choice;

            RenderX.List(list, title: title, indexPolicy: indexPolicy, renderer: renderer, padBefore: padBefore, padAfter: 0);

            if (list != null && list.Any())
            {
                var index = Value<int>(choicePrompt, required: required, validator: (i) => AssertIntInRange(i, 0 + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? 0 : 1), list.Count + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? 0 : 1)));
                RenderX.Pad(padAfter);

                choice = list[index + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? 0 : -1)];
                selectedIndex = index;
            }
            else
            {
                choice = default;
                selectedIndex = -1;
            }

            return choice;
        }

        /// <summary>
        /// Prompts a user to press any key to continue
        /// </summary>
        /// <param name="prompt">the text to render at the prompt.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        public static void Continue
        (
            string prompt = "Press any key to continue...",
            int padBefore = 1,
            int padAfter = 1
        )
        {
            RenderX.Pad(padBefore);
            Console.WriteLine(prompt);
            Console.ReadKey();
            RenderX.Pad(padAfter);
        }

        /// <summary>
        /// Prompts a user to press any key to exit
        /// </summary>
        /// <param name="prompt">the text to render at the prompt.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        public static void Exit
        (
            string prompt = "Press any key to exit...",
            int padBefore = 0
        )
        {
            RenderX.Pad(padBefore);
            Console.WriteLine(prompt);
            Console.ReadKey();
            Environment.Exit(0);
        }

        /// <summary>
        /// Prompts a user to enter a password (the password is hidden)
        /// </summary>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="cancelable">whether to allow escape to throw a <see cref="ConsoleNavigation.CancelPasswordException"/>, remember to catch the exception if <c>true</c>!</param>
        /// <exception cref="ConsoleNavigation.CancelPasswordException"></exception>
        public static string Password
        (
            int padBefore = 0,
            int padAfter = 0,
            bool cancelable = false
        )
        {
            return Password(null, padBefore: padBefore, padAfter: padAfter, cancelable: cancelable);
        }

        /// <summary>
        /// Prompts a user to enter a password (the password is hidden)
        /// </summary>
        /// <param name="prompt">the text to render at the prompt.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="cancelable">Whether to allow escape to throw a <see cref="ConsoleNavigation.CancelPasswordException"/>, remember to catch the exception if <c>true</c>!</param>
        /// <exception cref="ConsoleNavigation.CancelPasswordException"></exception>
        public static string Password
        (
            string prompt,
            int padBefore = 0,
            int padAfter = 0,
            bool cancelable = false
        )
        {
            var plainTextBuilder = new StringBuilder();

            RenderX.Pad(padBefore);
            RenderX.Prompt(prompt, required: false);

            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    if (cancelable)
                    {
                        Console.WriteLine();
                        RenderX.Pad(padAfter);
                        ConsoleNavigation.CancelPassword();
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (plainTextBuilder.Length > 0)
                    {
                        plainTextBuilder.Length -= 1;
                        Console.Write("\b \b");
                    }
                    else
                    {
                        Console.Beep();
                    }
                }
                else if (keyInfo.IsCursorNavigation())
                {
                    Console.Beep();
                }
                else if (keyInfo.IsLetterOrDigit() || keyInfo.IsPunctuation() || keyInfo.IsSpecialCharacter() || keyInfo.IsSpace())
                {
                    plainTextBuilder.Append(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    RenderX.Alert("Unexpected input: " + keyInfo.Key + ".", padBefore: 2, padAfter: 2);
                    return Password(prompt, padBefore: padBefore, padAfter: padAfter);
                }
            }
            RenderX.Pad(padAfter);
            return plainTextBuilder.ToString();
        }

        /// <summary>
        /// Prompts a user to enter a secure password (the password is hidden)
        /// </summary>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="cancelable">whether to allow escape to throw a <see cref="ConsoleNavigation.CancelPasswordException"/>, remember to catch the exception if <c>true</c>!</param>
        /// <exception cref="ConsoleNavigation.CancelPasswordException"></exception>
        public static SecureString PasswordSecure
        (
            int padBefore = 0,
            int padAfter = 0,
            bool cancelable = false
        )
        {
            return PasswordSecure(null, padBefore: padBefore, padAfter: padAfter);
        }

        /// <summary>
        /// Prompts a user to enter a secure password (the password is hidden)
        /// </summary>
        /// <param name="prompt">the text to render at the prompt.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="cancelable">whether to allow escape to throw a <see cref="ConsoleNavigation.CancelPasswordException"/>, remember to catch the exception if <c>true</c>!</param>
        /// <exception cref="ConsoleNavigation.CancelPasswordException"></exception>
        public static SecureString PasswordSecure
        (
            string prompt,
            int padBefore = 0,
            int padAfter = 0,
            bool cancelable = false
        )
        {
            var secureString = new SecureString();

            RenderX.Pad(padBefore);
            RenderX.Prompt(prompt, required: false);

            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    if (cancelable)
                    {
                        Console.WriteLine();
                        RenderX.Pad(padAfter);
                        ConsoleNavigation.CancelPassword();
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (secureString.Length > 0)
                    {
                        secureString.RemoveAt(secureString.Length - 1);
                        Console.Write("\b \b");
                    }
                    else
                    {
                        Console.Beep();
                    }
                }
                else if (keyInfo.IsCursorNavigation())
                {
                    Console.Beep();
                }
                else if (keyInfo.IsLetterOrDigit() || keyInfo.IsPunctuation() || keyInfo.IsSpecialCharacter() || keyInfo.IsSpace())
                {
                    secureString.AppendChar(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    RenderX.Alert("Unexpected input: " + keyInfo.Key + ".", padBefore: 2, padAfter: 2);
                    return PasswordSecure(prompt, padBefore: padBefore, padAfter: padAfter);
                }
            }
            RenderX.Pad(padAfter);
            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Prompts a user to choose a selection from a menu
        /// </summary>
        /// <typeparam name="T">type of menu item</typeparam>
        /// <param name="menuItems">a collection items, special treatment of <c>MenuItem</c>s</param>
        /// <param name="customItemsToPrepend">custom items to list before the regular menu items</param>
        /// <param name="customItemsToAppend">custom items to list after the regular menu items</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="title">a title</param>
        /// <param name="prompt">the text to render at the prompt.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="columns">the number of columns in which to render the list</param>
        /// <param name="configureTextGrid">exposes a reference to the underlying <c>TextGrid</c> for further configuration</param>
        /// <param name="allowArbitraryInput">allow arbitrary text in addition to menu item selection</param>
        /// <param name="allowMultipleSelection">allow entry of multiple list items such as: 1-3, 5, etc.</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <param name="onMenuSelecting">an action to perform when user enters a menu selection</param>
        /// <param name="onMenuSelection">an action to perfrom after menu selection is complete</param>
        /// <param name="onRoutineAutoRunComplete">an action to perform after selection and autorun of a routine</param>
        /// <param name="onRoutineAutoRunError">an action to perform after an autorun routine throws an exception</param>
        /// <returns>The selected menu selection(s)</returns>
        public static MenuSelection<T> Menu<T>
        (
            IEnumerable<T> menuItems,
            IList<MenuObject> customItemsToPrepend = null,
            IList<MenuObject> customItemsToAppend = null,
            Func<T, string> renderer = null,
            Title? title = null,
            string prompt = ">",
            int padBefore = 0,
            int padAfter = 1,
            int columns = 1,
            Action<TextGrid> configureTextGrid = null,
            bool allowArbitraryInput = false,
            bool allowMultipleSelection = false,
            bool canCancel = true,
            bool canExitApp = true,
            Action<string> onMenuSelecting = null,
            Action<MenuSelection<T>> onMenuSelection = null,
            Action<RoutineX> onRoutineAutoRunComplete = null,
            Action<RoutineX, Exception> onRoutineAutoRunError = null
        )
        {
            var listConfigurator = new MenuAndListRealtimeConfigurator();
            RenderX.Menu
            (
                menuItems?.ToList(),
                customItemsToPrepend: customItemsToPrepend,
                customItemsToAppend: customItemsToAppend,
                columns: columns,
                configureTextGrid: configureTextGrid,
                renderer: renderer,
                listConfigurator: listConfigurator,
                padBefore: padBefore,
                title: title,
                padAfter: padAfter
            );

            MenuSelection<T> menuSelection = null;

            while (!listConfigurator.IsNotSelectable)
            {
                string input, _input;
                try
                {
                    _input = Input(prompt, canCancel: canCancel, canExitApp: canExitApp) ?? "";
                    onMenuSelecting?.Invoke(_input);
                    input = _input.Trim();
                }
                catch (ConsoleNavigation.PromptCanceledException)
                {
                    break;
                }

                // selection type 1 of 4 - custom menu items
                var customMenuItems = CollectionUtil.Combine(customItemsToPrepend, customItemsToAppend)
                    .Where(mo => mo is RoutineX)
                    .Select(mo => (RoutineX)mo);
                RoutineX selectedCustomMenuItem = null;
                foreach (var custItem in customMenuItems)
                {
                    if (string.Equals(input, custItem.Command, StringComparison.CurrentCultureIgnoreCase))
                    {
                        selectedCustomMenuItem = custItem;
                        break;
                    }
                }
                if (selectedCustomMenuItem != null)
                {
                    menuSelection = new MenuSelection<T> { CustomMenuItem = selectedCustomMenuItem };
                    break;
                }


                // selection type 2 of 4 - menu items
                if (menuItems != null)
                {
                    var nonHeaderMenuItems = menuItems
                        .Where(mi => !(mi is MenuHeader))
                        .ToList();
                    if (int.TryParse(input, out int index) && index > 0 && index <= nonHeaderMenuItems.Count)
                    {
                        menuSelection = new MenuSelection<T>
                        {
                            SelectedItem = nonHeaderMenuItems[index - 1],  // index is 1-based
                            SelectedIndex = index
                        };
                        break;
                    }
                }

                // selection type 3 of 4 - multiple selections
                if (allowMultipleSelection && input.Length != 0)
                {
                    try
                    {
                        var _menuItems = new List<T>(menuItems);
                        var selectedIndices = MenuSelection.ParseMultipleIndexes(input, _menuItems.Count, out bool all);   // 1-based indices
                        var dict = new Dictionary<int, T>();
                        foreach (var selIndex in selectedIndices)
                        {
                            dict.Add(selIndex, _menuItems[selIndex - 1]);
                        }
                        menuSelection = new MenuSelection<T>
                        {
                            MultipleSelection = dict,
                            SelectedAll = all
                        };
                        break;
                    }
                    catch (BenignException ex)
                    {
                        RenderX.Alert(ex.Message);
                        continue;
                    }
                }

                // selection type 4 of 4 - arbitrary input
                if (allowArbitraryInput)
                {
                    menuSelection = new MenuSelection<T> { ArbitraryInput = _input };
                    break;
                }

                RenderX.Alert("Invalid menu selection.");
            }

            // process menu selection
            if (menuSelection != null)
            {
                onMenuSelection?.Invoke(menuSelection);
                RenderX.Pad(padAfter);
                if (menuSelection.SelectedRoutine != null)
                {
                    bool autoRunComplete;
                    try
                    {
                        menuSelection.SelectedRoutine.Run();
                        autoRunComplete = true;
                    }
                    catch (ConsoleNavigation)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        autoRunComplete = false;
                        if (onRoutineAutoRunError != null)
                        {
                            onRoutineAutoRunError.Invoke(menuSelection.SelectedRoutine, ex);
                        }
                        else throw;
                    }
                    if (autoRunComplete)
                    {
                        onRoutineAutoRunComplete?.Invoke(menuSelection.SelectedRoutine);
                    }
                }
            }
            return menuSelection;
        }

        /// <summary>
        /// Throws an <c>AssertionFailedException</c> if <c>value</c> is not in the specified range.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void AssertIntInRange(int value, int? min = null, int? max = null)
        {
            if (min.HasValue && value < min)
                throw new AssertionFailedException("The integer " + value + " is less than the minimum: " + min);
            if (max.HasValue && value > max)
                throw new AssertionFailedException("The integer " + value + " is greater than the maximum: " + max);
        }
    }
}
