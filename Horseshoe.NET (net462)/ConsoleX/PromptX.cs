using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;

using Horseshoe.NET.Collections;
using Horseshoe.NET.ObjectsAndTypes;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A collection of methods for prompting users for different types of input on the console
    /// </summary>
    public static class PromptX
    {
        private static string _Input(char? mask, string quickText, TraceJournal journal)
        {
            return string.Join("", _BufferedInput(mask, quickText, journal));
        }

        // <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        // <exception cref="ConsoleNavigation.ControlCException"></exception>
        private static IList<char> _BufferedInput(char? mask, string quickText, TraceJournal journal)
        {
            // journaling
            journal.WriteEntry("PromptX._BufferedInput()");
            journal.Level++;

            // do stuff
            var buf = new List<char>();
            var treatControlCAsInput = Console.TreatControlCAsInput;
            ConsoleKeyInfo info;
            bool inputting = true;
            bool alt;
            bool ctrl;
            bool shift;
            while (inputting)
            {
                Console.TreatControlCAsInput = true;
                info = Console.ReadKey(true);
                alt = (info.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt;
                ctrl = (info.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control;
                shift = (info.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift;
                journal.WriteEntry((ctrl ? "Ctrl+" : "") + (alt ? "Alt+" : "") + (shift ? "Shift+" : "") + TextUtil.Reveal(info.KeyChar, RevealOptions.All));
                switch (info.Key)
                {
                    case ConsoleKey.Escape:
                        // finalize
                        journal.Level--;
                        ConsoleNavigation.CancelInputPrompt();
                        break;
                    case ConsoleKey.Enter:
                        journal.AppendLastEntry("<-" + ConsoleKey.Enter);
                        inputting = false;
                        break;
                    case ConsoleKey.Backspace:
                        journal.AppendLastEntry("<-" + ConsoleKey.Backspace);
                        if (buf.Count > 0)
                        {
                            buf.RemoveAt(buf.Count - 1);
                            Console.Write("\b \b");
                        }
                        else
                        {
                            Console.Beep();
                        }
                        break;
                    default:
                        if (info.KeyChar < 32)
                        {
                            if (info.KeyChar == 0)
                            {
                                journal.AppendLastEntry("<-" + info.Key);
                            }
                            Console.Beep();
                        }
                        else if (ctrl)
                        {
                            // Handle ctrl+C (exits routine)
                            if (info.Key == ConsoleKey.C)
                            {
                                Console.WriteLine();
                                journal.WriteEntry("ctrl+C was pressed");

                                // finalize
                                journal.Level--;
                                ConsoleNavigation.ExitRoutine();
                            }
                            else
                            {
                                Console.Beep();
                            }
                        }
                        else if (alt)
                        {
                            Console.Beep();
                        }
                        else
                        {
                            Console.Write(mask ?? info.KeyChar);
                            buf.Add(info.KeyChar);
                        }
                        break;
                }
                Console.TreatControlCAsInput = treatControlCAsInput;
            }
            if (quickText != null && !buf.Any(c => !char.IsWhiteSpace(c)))
            {
                buf.Clear();
                buf.AddRange(quickText.ToArray());
                Console.WriteLine(quickText);
            }
            else
                Console.WriteLine();

            // finalize
            journal.Level--;
            return buf;
        }

        ///// <summary>
        ///// Prompts for input, accepts free text as well as certain commands i.e. 'cancel', 'exit' by default
        ///// </summary>
        ///// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        ///// <param name="displayAsRequired">If <c>true</c>, suggests to the renderer to mark this input as required.</param>
        ///// <param name="quickText">an optional common or predictive input that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        ///// <param name="padBefore">The number of new lines to render before the prompt.</param>
        ///// <param name="padAfter">The number of new lines to render after the prompt.</param>
        ///// <param name="autoTrim">Whether to trim leading and trailing whitespaces, default is <c>false</c>.</param>
        ///// <param name="canCancel">Whether typing 'cancel' at the prompt can cancel the routine, default is <c>true</c>.</param>
        ///// <param name="canExitApp">Whether typing 'exit' at the prompt can exit the application, default is <c>true</c>.</param>
        ///// <returns>The text entered by the user</returns>
        //public static string Input
        //(
        //    bool required = false,
        //    bool displayAsRequired = false,
        //    string quickText = null, 
        //    int padBefore = 0, 
        //    int padAfter = 0, 
        //    bool autoTrim = false,
        //    bool canCancel = true,
        //    bool canExitApp = true
        //)
        //{
        //    return Input
        //    (
        //        null,
        //        required: required,
        //        displayAsRequired: displayAsRequired,
        //        quickText: quickText, 
        //        autoTrim: autoTrim,
        //        padBefore: padBefore, 
        //        padAfter: padAfter, 
        //        canCancel: canCancel, 
        //        canExitApp: canExitApp
        //    );
        //}

        ///// <summary>
        ///// Prompts for input, accepts free text as well as certain commands i.e. 'cancel', 'exit' by default
        ///// </summary>
        ///// <param name="prompt">The text to render at the prompt.</param>
        ///// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        ///// <param name="displayAsRequired">If <c>true</c>, suggests to the renderer to mark this input as required.</param>
        ///// <param name="quickText">an optional common or predictive input that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        ///// <param name="padBefore">The number of new lines to render before the prompt.</param>
        ///// <param name="padAfter">The number of new lines to render after the prompt.</param>
        ///// <param name="autoTrim">Whether to trim leading and trailing whitespaces, default is <c>false</c>.</param>
        ///// <param name="canCancel">Whether typing 'cancel' at the prompt can cancel the routine, default is <c>true</c>.</param>
        ///// <param name="canExitApp">Whether typing 'exit' at the prompt can exit the application, default is <c>true</c>.</param>
        ///// <returns>The text entered by the user</returns>
        //public static string Input
        //(
        //    string prompt, 
        //    bool required = false,
        //    bool displayAsRequired = false,
        //    string quickText = null,
        //    int padBefore = 0, 
        //    int padAfter = 0, 
        //    bool autoTrim = false,
        //    bool canCancel = true, 
        //    bool canExitApp = true
        //)
        //{
        //    RenderX.Pad(padBefore);
        //    if (!string.IsNullOrWhiteSpace(quickText))
        //    {
        //        Console.WriteLine("(Press 'Enter' to use \"" + quickText + "\")");
        //    }

        //    string input;
        //    while (true)
        //    {
        //        RenderX.Prompt(prompt, required: required || displayAsRequired);
        //        input = autoTrim 
        //            ? Console.ReadLine().Trim()
        //            : Console.ReadLine();
        //        if (input.Equals(""))
        //        {
        //            if (quickText != null)
        //            {
        //                input = quickText;
        //                RenderX.Prompt(prompt, required: required || displayAsRequired);
        //                Console.WriteLine(input);
        //            }
        //            else if (required)
        //            {
        //                RenderX.Alert("An input is required.");
        //                continue;
        //            }
        //        }
        //        else
        //        {
        //            switch (input.ToLower())
        //            {
        //                case "exit":
        //                    if (canExitApp)
        //                    {
        //                        Environment.Exit(0);
        //                    }
        //                    break;
        //                case "cancel":
        //                    if (canCancel)
        //                    {
        //                        ConsoleNavigation.CancelInput();
        //                    }
        //                    break;
        //            }
        //        }
        //        break;
        //    }
        //    RenderX.Pad(padAfter);
        //    return input;
        //}

        /// <summary>
        /// Prompts for input, accepts free text with no string trimming and no command recognition
        /// </summary>
        /// <param name="mask">Optional <c>char</c> to display on screen.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="displayAsRequired">If <c>true</c>, suggests to the renderer to mark this input as required.</param>
        /// <param name="quickText">Text to apply by pressing 'Enter', suggested before the prompt.</param>
        /// <param name="requiredMessage">The alert to display if a required input is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        /// <returns>The exact text entered by the user</returns>
        public static string RawInput
        (
            char? mask = null,
            bool required = false,
            bool displayAsRequired = false,
            string quickText = null,
            string requiredMessage = "Input is required.",
            int padBefore = 0, 
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            return RawInput
            (
                null,
                mask: mask,
                required: required,
                displayAsRequired: displayAsRequired,
                quickText: quickText,
                requiredMessage: requiredMessage,
                padBefore: padBefore, 
                padAfter: padAfter,
                journal: journal
            );
        }

        /// <summary>
        /// Prompts for input, accepts free text with no string trimming and no command recognition.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="mask">Optional <c>char</c> to display on screen.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="displayAsRequired">If <c>true</c>, suggests to the renderer to mark this input as required.</param>
        /// <param name="requiredMessage">The alert to display if a required input is not supplied.</param>
        /// <param name="quickText">Text to apply by pressing 'Enter', suggested before the prompt.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The exact text entered by the user.</returns>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static string RawInput
        (
            string prompt,
            char? mask = null,
            bool required = false,
            bool displayAsRequired = false,
            string quickText = null,
            string requiredMessage = "Input is required.",
            int padBefore = 0, 
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.RawInput()");
            journal.Level++;

            // do work
            string input;

            RenderX.Pad(padBefore);

            if (quickText != null)
            {
                Console.WriteLine("(Press 'Enter' to input \"" + quickText + "\")");
            }

            try
            {
                RenderX.Prompt(prompt, required: required || displayAsRequired);
                input = _Input(mask, quickText, journal);
                while (string.IsNullOrWhiteSpace(input) && required)
                {
                    RenderX.Alert(TextUtil.Reveal(requiredMessage));
                    RenderX.Prompt(prompt, required: required || displayAsRequired);
                    input = _Input(mask, quickText, journal);
                }

                //// apply the quick text  -->>  _InputBuffered()
                //if (string.IsNullOrWhiteSpace(input) && quickText != null)
                //{
                //    input = quickText;
                //    RenderX.Prompt(prompt, required: required || displayAsRequired);
                //    Console.WriteLine(input);
                //}
            }
            catch (ConsoleNavigation.CancelInputPromptException)
            {
                throw;
            }
            catch (ConsoleNavigation.ControlCException)
            {
                throw;
            }
            finally
            {
                // finalize
                journal.Level--;
                RenderX.Pad(padAfter);
            }

            return input;
        }

        /// <summary>
        /// Prompts for console input of type <c>T</c> as well as certain commands i.e. 'exit' by default.
        /// Strings are zapped (trimmed and then, if blank, converted to <c>null</c>).
        /// Client code may wrap this call in try-block to handle <see cref="ConsoleNavigation.CancelInputPromptException" />.
        /// </summary>
        /// <typeparam name="T">a reference type</typeparam>
        /// <param name="parser">An optional custom text-to-value converter.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="numberStyle">Applies to <c>Value&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>Value&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>Value&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>Value&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>Value&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>Value&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="inheritedType">An optional type constraint - the type to which the returned <c>Type</c> must be assignable.</param>
        /// <param name="ignoreCase">Applies to <c>Value&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static T Value<T>
        (
            Func<string, object> parser = null,
            bool required = false,
            string requiredMessage = "A value is required.",
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            Type inheritedType = null,
            bool ignoreCase = false,
            QuickValue<T> quickValue = null,
            Action<T> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        ) 
        {
            return Value<T>
            (
                null,
                parser: parser,
                required: required,
                requiredMessage: requiredMessage,
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
                journal: journal
            );
        }

        /// <summary>
        /// Prompts for console input of type <c>T</c> as well as certain commands i.e. 'exit' by default.
        /// Strings are zapped (trimmed and then, if blank, converted to <c>null</c>).
        /// Client code may wrap this call in try-block to handle <see cref="ConsoleNavigation.CancelInputPromptException" />.
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="parser">An optional custom text-to-value converter.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="numberStyle">Applies to <c>Value&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>Value&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>Value&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>Value&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>Value&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>Value&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="inheritedType">An optional type constraint - the type to which the returned <c>Type</c> must be assignable.</param>
        /// <param name="ignoreCase">Applies to <c>Value&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static T Value<T>
        (
            string prompt,
            Func<string, object> parser = null,
            bool required = false,
            string requiredMessage = "A value is required.",
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            Type inheritedType = null,
            bool ignoreCase = false,
            QuickValue<T> quickValue = null,
            Action<T> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.Value<T>() T=" + typeof(T).FullName);
            journal.Level++;

            try
            {
                // scenario 1 of 4 - enum list
                if (typeof(T).IsEnumType())
                {
                    // finalize
                    journal.Level--;

                    return Enum<T>(prompt, required: required, padBefore: padBefore, padAfter: padAfter);
                }

                if (prompt != null && (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?)))
                {
                    prompt += " [y/n]";
                }

                while (true)
                {
                    string input = Zap.String
                    (
                        RawInput
                        (
                            prompt,
                            required: required && quickValue == null,
                            displayAsRequired: required,
                            quickText: quickValue?.ToString(),
                            requiredMessage: requiredMessage,
                            padBefore: padBefore,
                            padAfter: padAfter,
                            journal: journal
                        )
                    );

                    // scenario 2 of 4 - null input
                    if (input == null)
                    {
                        // finalize
                        journal.Level--;

                        if (quickValue != null)
                        {
                            return quickValue.Value;
                        }
                        return default;
                    }

                    T parsedValue;

                    // scenario 3 of 4 - supplied parser
                    if (parser != null)
                    {
                        parsedValue = (T)parser.Invoke(input);
                    }

                    // scenario 4 of 4 - built-in parser
                    else
                    {
                        parsedValue = Zap.To<T>
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
                        );
                    }

                    try
                    {
                        validator?.Invoke(parsedValue);
                    }
                    catch (Exception ex)
                    {
                        RenderX.Alert(ex.Message.Replace(AssertionFailedException.MESSAGE_PREFIX + ": ", ""));
                        continue;
                    }

                    // finalize
                    journal.Level--;
                    return parsedValue;
                }
            }
            catch (ConsoleNavigation.CancelInputPromptException)
            {
                throw;
            }
            catch (ConsoleNavigation.ControlCException)
            {
                throw;
            }
        }

        /// <summary>
        /// Prompts for <c>enum</c> value, also accepts certain commands i.e. 'cancel', 'exit' by default
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="title">An optional list title.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="only">Restrict the list of choices to these <c>enum</c> values, if supplied.</param>
        /// <param name="except">Omit these <c>enum</c> values from the list of choices.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user.</returns>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static T Enum<T>
        (
            Title? title = null, 
            bool required = false,
            IEnumerable<T> only = null,
            IEnumerable<T> except = null,
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
                .Cast<T>();
            if (only != null)
                enumValues = enumValues.Where(e => e.In(only));
            else if (except != null)
                enumValues = enumValues.Where(e => !e.In(except));

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
            var menuSelection = Menu(null as IEnumerable<T>, customItemsToAppend: customItems, title: title, padBefore: padBefore, padAfter: padAfter);

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
        /// Prompts a user to press any key to exit.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
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
        /// Prompts a user to securely enter a password.
        /// </summary>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static string Password
        (
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            return Password(null, padBefore: padBefore, padAfter: padAfter, journal: journal);
        }

        /// <summary>
        /// Prompts a user to securely enter a password.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static string Password
        (
            string prompt,
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.Password()");
            journal.Level++;

            // do stuff
            var password = RawInput
            (
                prompt,
                mask: '*',
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            // finalize
            journal.Level--;
            return password;
        }

        /// <summary>
        /// Prompts a user to securely enter a secure password.
        /// </summary>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static SecureString SecurePassword
        (
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            return SecurePassword(null, padBefore: padBefore, padAfter: padAfter, journal: journal);
        }

        /// <summary>
        /// Prompts a user to securely enter a secure password.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static SecureString SecurePassword
        (
            string prompt,
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.SecurePassword()");
            journal.Level++;

            // do stuff
            var secureString = new SecureString();
            RenderX.Pad(padBefore);

            RenderX.Prompt(prompt);
            var buf = _BufferedInput('*', null, journal);
            foreach (var c in buf)
            {
                secureString.AppendChar(c);
            }
            secureString.MakeReadOnly();

            RenderX.Pad(padAfter + 1);

            // finalize
            journal.Level--;
            return secureString;
        }

        /// <summary>
        /// Prompts a user to choose from a collection of items.
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="collection">A collection.</param>
        /// <param name="title">A collection title.</param>
        /// <param name="indexPolicy">Whether to display an index and whether it is 0-based.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="renderer">An alternative to <c>object.ToString()</c> for displaying collection items.</param>
        /// <param name="padBefore">The number of new lines to render before the collection.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static ListSelection<T> List<T>
        (
            IEnumerable<T> collection,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
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
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="padBefore">The number of new lines to render before the list</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static ListSelection<T> List<T>
        (
            IList<T> list,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
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
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="padBefore">The number of new lines to render before the list</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static ListSelection<T> List<T>
        (
            IList<T> list,
            out int selectedIndex,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
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
                var index = Value<int>
                (
                    required: required,
                    requiredMessage: "A selection is required.",
                    validator: (i) => Assert.InRange(i, 0 + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? 0 : 1), list.Count + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? -1 : 0))
                );
                RenderX.Pad(padAfter);

                choice = list[index + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? 0 : -1)];
                selectedIndex = index;
            }
            else
            {
                choice = default;
                selectedIndex = -1;
            }

            return new ListSelection<T> { SelectedItem = choice, SelectedIndex = selectedIndex };
        }

        /// <summary>
        /// Prompts a user to choose a selection from a menu.
        /// </summary>
        /// <typeparam name="T">type of menu item</typeparam>
        /// <param name="menuItems">a collection items, special treatment of <c>MenuItem</c>s</param>
        /// <param name="customItemsToPrepend">custom items to list before the regular menu items</param>
        /// <param name="customItemsToAppend">custom items to list after the regular menu items</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="title">a title</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="columns">the number of columns in which to render the list</param>
        /// <param name="configureTextGrid">exposes a reference to the underlying <c>TextGrid</c> for further configuration</param>
        /// <param name="allowArbitraryInput">allow arbitrary text in addition to menu item selection</param>
        /// <param name="allowMultipleSelection">allow entry of multiple list items such as: 1-3, 5, etc.</param>
        /// <param name="onMenuSelecting">an action to perform when user enters a menu selection</param>
        /// <param name="onMenuSelection">an action to perfrom after menu selection is complete</param>
        /// <param name="onRoutineAutoRunComplete">an action to perform after selection and autorun of a routine</param>
        /// <param name="onRoutineAutoRunError">an action to perform after an autorun routine throws an exception</param>
        /// <returns>The selected menu selection(s)</returns>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.ControlCException"></exception>
        public static MenuSelection<T> Menu<T>
        (
            IEnumerable<T> menuItems,
            IList<MenuObject> customItemsToPrepend = null,
            IList<MenuObject> customItemsToAppend = null,
            Func<T, string> renderer = null,
            Title? title = null,
            int padBefore = 0,
            int padAfter = 1,
            int columns = 1,
            Action<TextGrid> configureTextGrid = null,
            bool allowArbitraryInput = false,
            bool allowMultipleSelection = false,
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
                title: title            
            );

            MenuSelection<T> menuSelection = null;

            while (!listConfigurator.IsNotSelectable)
            {
                var rawInput = RawInput(padAfter: padAfter);
                onMenuSelecting?.Invoke(rawInput);
                var input = rawInput.Trim();

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
                if (menuItems != null && menuItems.Any())
                {
                    var nonHeaderMenuItems = menuItems
                        .Where(mi => !(mi is MenuHeader))
                        .ToList();
                    if (int.TryParse(input, out int index))
                    {
                        Assert.InRange(index, 1, nonHeaderMenuItems.Count);
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
                    menuSelection = new MenuSelection<T> { ArbitraryInput = rawInput };
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
    }
}
