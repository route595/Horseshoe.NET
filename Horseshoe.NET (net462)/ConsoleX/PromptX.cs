using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Dotnet;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A collection of methods for prompting users for different types of input on the console
    /// </summary>
    public static class PromptX
    {
        private static readonly string MessageRelayGroup = typeof(PromptX).Namespace;

        private static int tryAgainCount = 0;

        private static string _Keystrokes(char? mask/*, object quickValue, bool suppressEcho*/)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            string bufferedKeystrokes = string.Join("", _BufferedKeystrokes(mask/*, quickValue, suppressEcho*/));
            //SystemMessageRelay.RelayMessage(nameof(bufferedKeystrokes) + " = \"" + bufferedKeystrokes + "\"", group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
            return bufferedKeystrokes;
        }

        private static IList<char> _BufferedKeystrokes(char? mask/*, object quickValue, bool suppressEcho*/)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var buf = new List<char>();
            //var treatControlCAsInput = Console.TreatControlCAsInput;
            ConsoleKeyInfo info;
            bool inputting = true;
            bool alt;
            bool ctrl;
            bool shift;
            var strbInputsToRelay = new StringBuilder();
            while (inputting)
            {
                //Console.TreatControlCAsInput = true;

                //// check if Ctrl+C was pressed 
                //if (ConsoleXApp.WasCtrlCPressed())
                //{
                //    ConsoleNavigation.CtrlCHasBeenPressed();
                //}

                info = Console.ReadKey(true);

                //// check if Ctrl+C was pressed 
                //if (ConsoleXApp.WasCtrlCPressed())
                //{
                //    ConsoleNavigation.CtrlCHasBeenPressed();
                //}
 
                alt = (info.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt;
                ctrl = (info.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control;
                shift = (info.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift;
                strbInputsToRelay
                    .AppendIf(ctrl, "Ctrl+")
                    .AppendIf(alt, "Alt+")
                    .AppendIf(shift, "Shift+")
                    .Append(info.Key)
                    .Append(' ');
                switch (info.Key)
                {
                    case ConsoleKey.Escape:
                        ConsoleNavigation.CancelInputPrompt();
                        break;
                    case ConsoleKey.Enter:
                        inputting = false;
                        break;
                    case ConsoleKey.Backspace:
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
                            Console.Beep();
                        }
                        else if (ctrl)
                        {
                            // Handle ctrl+C (exits routine)
                            //if (info.Key == ConsoleKey.C)
                            //{
                            //    Console.WriteLine();
                            //    journal.WriteEntry("ctrl+C was pressed");

                            //    // finalize
                            //    journal.Level--;
                            //    ConsoleNavigation.ExitRoutine();
                            //}
                            //else
                            //{
                                Console.Beep();
                            //}
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
                //Console.TreatControlCAsInput = treatControlCAsInput;
            }
            //if (quickValue != null && !buf.Any(c => !char.IsWhiteSpace(c)))
            //{
            //    // quickValue does not become the input value until Value<T>()
            //    buf.Clear();
            //    SystemMessageRelay.RelayMessage("quick value: " + ValueUtil.Display(quickValue), group: MessageRelayGroup);
            //    if (!suppressEcho)
            //    {
            //        Console.WriteLine(quickValue);
            //    }
            //}
            Console.WriteLine();

            SystemMessageRelay.RelayMethodReturn(returnDescription: "char list (buffer): length = " + buf.Count, group: MessageRelayGroup);

            return buf;
        }

        private static string _ConsoleLine(/*object quickValue, bool suppressEcho*/)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var rawLine = Console.ReadLine();

            //if (quickValue != null && string.IsNullOrWhiteSpace(rawLine))
            //{
            //    // quickValue does not become the input value until Value<T>()
            //    SystemMessageRelay.RelayMessage("quick value: " + ValueUtil.Display(quickValue), group: MessageRelayGroup);
            //    if (!suppressEcho)
            //    {
            //        Console.WriteLine(quickValue);
            //    }
            //}

            SystemMessageRelay.RelayMethodReturnValue(rawLine, group: MessageRelayGroup);
            return rawLine;
        }

        /// <summary>
        /// Prompts for input, accepts free text with no string trimming.  Command recognition: 'exit'.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">Value to apply by pressing 'Enter', suggested before the prompt.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The exact text entered by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutine"></exception>
        public static string RawConsoleInput
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            object quickValue = null,
            object defaultValue = null,
            bool suppressRequiredPrompt = false,
            //bool suppressEcho = false,
            //string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            RenderX.Pad(padBefore);
            RenderX.Prompt(prompt, promptType: promptType, required: required && !suppressRequiredPrompt, quickValue: quickValue, defaultValue: defaultValue);

            var input = _ConsoleLine();
            //while (input.Length == 0 && required && quickValue == null)
            //{
            //    RenderX.Alert(requiredMessage);
            //    RenderX.Prompt(prompt, required: !suppressRequiredPrompt);
            //    //input = _KeyStrokes(mask, quickText, journal);
            //    input = _ConsoleLine(quickValue, suppressEcho);
            //}

            RenderX.Pad(padAfter);

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return input;
        }

        /// <summary>
        /// Prompts for input, accepts free text with no string trimming.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="mask">An optional character to display instead of the typed key.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">Value to apply by pressing 'Enter'.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The exact text entered by the user.</returns>
        public static string RawKeystrokeInput
        (
            string prompt,
            char? mask = null,
            bool required = false,
            object quickValue = null,
            bool suppressRequiredPrompt = false,
            //string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            RenderX.Pad(padBefore);
            RenderX.Prompt(prompt, required: required && !suppressRequiredPrompt, quickValue: quickValue);

            string input = _Keystrokes(mask/*, quickValue*/);
            //while (input.Length == 0 && required && quickValue == null)
            //{
            //    RenderX.Alert(TextUtil.Reveal(requiredMessage));
            //    RenderX.Prompt(prompt, required: !suppressRequiredPrompt);
            //    input = _Keystrokes(mask/*, quickValue*/);
            //    //input = _ConsoleLine(quickValue, suppressEcho, journal);
            //}

            RenderX.Pad(padAfter);

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);

            return input;
        }

        /// <summary>
        /// Prompts for console input.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// Leading and trailing whitespace is trimmed and blanks are converted to <c>null</c>.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        /// <exception cref="ConversionException"></exception>
        private static string _String
        (
            string prompt,
            PromptType promptType,
            bool required,
            string requiredMessage,
            bool suppressRequiredPrompt,
            bool suppressCommands,
            int padBefore,
            int padAfter
        ) 
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            string input;

            while (true)
            {
                input = Zap.String(RawConsoleInput
                (
                    prompt,
                    promptType: promptType,
                    required: required,
                    suppressRequiredPrompt: suppressRequiredPrompt,
                    padBefore: padBefore,
                    padAfter: padAfter
                ));

                if (input == null)
                {
                    // complain and retry if input is required
                    if (required)
                    {
                        SystemMessageRelay.RelayMessage("required and null, retrying...", group: MessageRelayGroup);
                        RenderX.Alert(requiredMessage);
                        continue;
                    }
                    return null;
                }

                // process commands, if applicable
                if (!suppressCommands)
                {
                    if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase))
                    {
                        SystemMessageRelay.RelayMessage("command: \"/exit\"", group: MessageRelayGroup);
                        Environment.Exit(0);
                    }
                    if (input.Equals("/back", StringComparison.OrdinalIgnoreCase))
                    {
                        SystemMessageRelay.RelayMessage("command: \"/back\"", group: MessageRelayGroup, indent: Indent.DecrementNext);
                        ConsoleNavigation.BackoutRoutine();
                    }
                }

                SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
                return input;
            }
        }

        /// <summary>
        /// Prompts for console input of a value type (e.g. <c>int</c>).  
        /// Accepts certain commands by default i.e. '/exit' and '/back'.
        /// Leading and trailing whitespace is trimmed before converting to <c>T</c> and blanks are converted to <c>null</c>s.
        /// Client code may wrap this call in try-block to handle <see cref="ConsoleNavigation.CancelInputPromptException" />.
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="parser">A required text-to-value converter.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="dateTimeStyle">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates the expected date/time format.</param>
        /// <param name="dateFormat">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates the exact date/time format.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>To&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="ignoreCase">Applies to <c>To&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.  Must throw <c>AssertionFailedException</c> if validation fails.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        /// <exception cref="ConversionException"></exception>
        public static T Value<T>
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            Func<string, T> parser = null,
            T? quickValue = null,
            T defaultValue = default,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            DateTimeStyles? dateTimeStyle = null,
            string dateFormat = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            bool ignoreCase = false,
            Action<T> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        ) where T : struct
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            while (true)
            {
                var input = _String
                (
                    prompt,
                    promptType,
                    required,
                    requiredMessage,
                    suppressRequiredPrompt,
                    suppressCommands,
                    padBefore,
                    padAfter
                );

                T value;

                try
                {
                    if (input == null)
                    {
                        // use quick value, if applicable
                        if (quickValue.HasValue)
                        {
                            SystemMessageRelay.RelayMessage("applying quick value: " + ValueUtil.Display(quickValue), group: MessageRelayGroup);
                            if (!suppressEcho)
                            {
                                Console.WriteLine(quickValue);
                            }
                            return quickValue.Value;
                        }

                        if (required)
                        {
                            throw new ThisShouldNeverHappenException("This required input is null");
                        }

                        // fall back to default value (will run through the validator)
                        value = defaultValue;
                    }
                    else
                    {
                        // parse value
                        value = parser != null
                            ? parser.Invoke(input)
                            : Zap.To<T>(input, numberStyle: numberStyle, provider: provider, dateTimeStyle: dateTimeStyle, dateFormat: dateFormat, locale: locale, trueValues: trueValues, falseValues: falseValues, encoding: encoding, ignoreCase: ignoreCase);
                    }

                    // validate input, if applicable
                    if (validator != null)
                    {
                        SystemMessageRelay.RelayMessage("validating...", group: MessageRelayGroup);
                        validator.Invoke(value);
                        SystemMessageRelay.RelayMessage("validation successful", group: MessageRelayGroup);
                    }

                    SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
                    return value;
                }
                catch (ConversionException ex)
                {
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup, inlineWithMessages: true);
                    RenderX.Alert(ex.Message);
                    throw;
                }
                catch (AssertionFailedException ex)
                {
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup, inlineWithMessages: true);
                    SystemMessageRelay.RelayMessage("retrying...", group: MessageRelayGroup);
                    RenderX.Alert(ex.Message.Replace(AssertionConstants.EXCEPTION_MESSAGE_PREFIX, ""));
                    continue;
                }
                catch (Exception ex)
                {
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup, inlineWithMessages: true);
                    SystemMessageRelay.RelayMessage("retrying...", group: MessageRelayGroup);
                    RenderX.Alert(ex.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// Prompts for console input of a nullable value type (e.g. <c>int?</c>).  
        /// Accepts certain commands by default i.e. '/exit' and '/back'.
        /// Leading and trailing whitespace is trimmed before converting to <c>T?</c> and blanks are converted to <c>null</c>s.
        /// Validators are not executed when the return value is <c>null</c>.
        /// Client code may wrap this call in try-block to handle <see cref="ConsoleNavigation.CancelInputPromptException" />.
        /// </summary>
        /// <typeparam name="T">A value type.</typeparam>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="parser">A required text-to-value converter.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="dateTimeStyle">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates the expected date/time format.</param>
        /// <param name="dateFormat">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates the exact date/time format.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>To&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="ignoreCase">Applies to <c>To&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.  Must throw <c>AssertionFailedException</c> if validation fails.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        /// <exception cref="ConversionException"></exception>
        public static T? NValue<T>
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            Func<string, T> parser = null,
            T? quickValue = null,
            T? defaultValue = default,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            DateTimeStyles? dateTimeStyle = null,
            string dateFormat = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            bool ignoreCase = false,
            Action<T> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        ) where T : struct
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            while (true)
            {
                var input = _String
                (
                    prompt,
                    promptType,
                    required,
                    requiredMessage,
                    suppressRequiredPrompt,
                    suppressCommands,
                    padBefore,
                    padAfter
                );

                T value;

                try
                {
                    if (input == null)
                    {
                        // use quick value, if applicable
                        if (quickValue.HasValue)
                        {
                            SystemMessageRelay.RelayMessage("applying quick value: " + ValueUtil.Display(quickValue), group: MessageRelayGroup);
                            if (!suppressEcho)
                            {
                                Console.WriteLine(quickValue);
                            }
                            return quickValue.Value;
                        }

                        if (required)
                        {
                            throw new ThisShouldNeverHappenException("This required input is null");
                        }

                        // fall back to default value, which may be null (otherwise will run through the validator)
                        if (!defaultValue.HasValue)
                            return null;
                        value = defaultValue.Value;
                    }
                    else
                    {
                        // parse value
                        value = parser != null
                            ? parser.Invoke(input)
                            : Zap.To<T>(input, numberStyle: numberStyle, provider: provider, dateTimeStyle: dateTimeStyle, dateFormat: dateFormat, locale: locale, trueValues: trueValues, falseValues: falseValues, encoding: encoding, ignoreCase: ignoreCase);
                    }

                    // validate input, if applicable
                    if (validator != null)
                    {
                        SystemMessageRelay.RelayMessage("validating...", group: MessageRelayGroup);
                        validator.Invoke(value);
                        SystemMessageRelay.RelayMessage("validation successful", group: MessageRelayGroup);
                    }

                    SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
                    return value;
                }
                catch (ConversionException ex)
                {
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup, inlineWithMessages: true);
                    RenderX.Alert(ex.Message);
                    throw;
                }
                catch (AssertionFailedException ex)
                {
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup, inlineWithMessages: true);
                    SystemMessageRelay.RelayMessage("retrying...", group: MessageRelayGroup);
                    RenderX.Alert(ex.Message.Replace(AssertionConstants.EXCEPTION_MESSAGE_PREFIX, ""));
                    continue;
                }
                catch (Exception ex)
                {
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup, inlineWithMessages: true);
                    SystemMessageRelay.RelayMessage("retrying...", group: MessageRelayGroup);
                    RenderX.Alert(ex.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// Prompts for console input of a reference type (e.g. <c>string</c>).  
        /// Accepts certain commands by default i.e. '/exit' and '/back'.
        /// Leading and trailing whitespace is trimmed before converting to <c>T</c> and blanks are converted to <c>null</c>s.
        /// Validators are not executed when the return value is <c>null</c>.
        /// Client code may wrap this call in try-block to handle <see cref="ConsoleNavigation.CancelInputPromptException" />.
        /// </summary>
        /// <typeparam name="T">A reference type.</typeparam>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="parser">A required text-to-value converter.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.  Must throw <c>AssertionFailedException</c> if validation fails.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        /// <exception cref="ConversionException"></exception>
        public static T Value<T>
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            Func<string, T> parser = null,
            T quickValue = null,
            T defaultValue = null,
            Action<T> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        ) where T : class
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            while (true)
            {
                var input = _String
                (
                    prompt,
                    promptType,
                    required,
                    requiredMessage,
                    suppressRequiredPrompt,
                    suppressCommands,
                    padBefore,
                    padAfter
                );

                T value;

                try
                {
                    if (input == null)
                    {
                        // use quick value, if applicable
                        if (quickValue != null)
                        {
                            SystemMessageRelay.RelayMessage("applying quick value: " + ValueUtil.Display(quickValue), group: MessageRelayGroup);
                            if (!suppressEcho)
                            {
                                Console.WriteLine(quickValue);
                            }
                            return quickValue;
                        }

                        if (required)
                        {
                            throw new ThisShouldNeverHappenException("This required input is null");
                        }

                        // fall back to default value, which may be null (otherwise will run through the validator)
                        if (defaultValue == null)
                            return null;
                        value = defaultValue;
                    }
                    else
                    {
                        // parse value
                        value = parser != null
                            ? parser.Invoke(input)
                            : Zap.To<T>(input);
                    }

                    // validate input, if applicable
                    if (validator != null)
                    {
                        SystemMessageRelay.RelayMessage("validating...", group: MessageRelayGroup);
                        validator.Invoke(value);
                        SystemMessageRelay.RelayMessage("validation successful", group: MessageRelayGroup);
                    }

                    SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
                    return value;
                }
                catch (ConversionException ex)
                {
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup, inlineWithMessages: true);
                    RenderX.Alert(ex.Message);
                    throw;
                }
                catch (AssertionFailedException ex)
                {
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup, inlineWithMessages: true);
                    SystemMessageRelay.RelayMessage("retrying...", group: MessageRelayGroup);
                    RenderX.Alert(ex.Message.Replace(AssertionConstants.EXCEPTION_MESSAGE_PREFIX, ""));
                    continue;
                }
                catch (Exception ex)
                {
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup, inlineWithMessages: true);
                    SystemMessageRelay.RelayMessage("retrying...", group: MessageRelayGroup);
                    RenderX.Alert(ex.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// Prompts for console input of type <c>string</c> (shortcut for <c>Value&lt;string&gt;()</c>).
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// Strings are zapped (whitespace trimmed and then blank converts to <c>null</c>).
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static string String
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            string requiredMessage = "Input is required.",
            string quickValue = null,
            Action<string> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = Value
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                validator: validator,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressEcho: suppressEcho,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>int?</c> (shortcut for <c>NValue&lt;int&gt;()</c>).
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input, default is <c>null</c>.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static int? NInt
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            int? quickValue = null,
            int? defaultValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<int> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = NValue
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                defaultValue: defaultValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>int</c> (shortcut for <c>Value&lt;int&gt;()</c>). 
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static int Int
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            int? quickValue = null,
            int defaultValue = default,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<int> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = Value
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                defaultValue: defaultValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>double?</c> (shortcut for <c>NValue&lt;double&gt;()</c>).
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input, default is <c>null</c>.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static double? NDouble
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            double? quickValue = null,
            double? defaultValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<double> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = NValue
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                defaultValue: defaultValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>double</c> (shortcut for <c>Value&lt;double&gt;()</c>). 
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static double Double
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            double? quickValue = null,
            double defaultValue = default,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<double> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = Value
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                defaultValue: defaultValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>decimal?</c> (shortcut for <c>NValue&lt;decimal&gt;()</c>).
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input, default is <c>null</c>.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static decimal? NDecimal
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            decimal? quickValue = null,
            decimal? defaultValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<decimal> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = NValue
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                defaultValue: defaultValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>decimal</c> (shortcut for <c>Value&lt;decimal&gt;()</c>). 
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static decimal Decimal
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            decimal? quickValue = null,
            decimal defaultValue = default,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<decimal> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = Value
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                defaultValue: defaultValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>DateTime?</c> (shortcut for <c>NValue&lt;DateTime&gt;()</c>).
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input, default is <c>null</c>.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static DateTime? NDateTime
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            DateTime? quickValue = null,
            DateTime? defaultValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<DateTime> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = NValue
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                defaultValue: defaultValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>DateTime</c> (shortcut for <c>Value&lt;DateTime&gt;()</c>). 
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static DateTime DateTime
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            DateTime? quickValue = null,
            DateTime defaultValue = default,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<DateTime> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = Value
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                defaultValue: defaultValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>bool?</c> (shortcut for <c>NValue&lt;bool&gt;()</c>).
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input, default is <c>null</c>.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static bool? NBool
        (
            string prompt,
            PromptType promptType = PromptType.Bool,
            bool required = false,
            bool? quickValue = null,
            bool? defaultValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<bool> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = NValue
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                defaultValue: defaultValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>bool</c> (shortcut for <c>Value&lt;bool&gt;()</c>). 
        /// Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter'.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like '/exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static bool Bool
        (
            string prompt,
            PromptType promptType = PromptType.Bool,
            bool required = false,
            bool? quickValue = null,
            bool defaultValue = default,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<bool> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var value = Value
            (
                prompt,
                promptType: promptType,
                required: required,
                quickValue: quickValue,
                defaultValue: defaultValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(value, group: MessageRelayGroup);
            return value;
        }

        /// <summary>
        /// Prompts for an <c>enum</c> value. Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="defaultValue">An optional default value to use if <c>null</c> selection since this method cannot return <c>null</c>.</param>
        /// <param name="title">An optional list title.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="only">Restrict the list of choices to these <c>enum</c> values, if supplied.</param>
        /// <param name="except">Omit these <c>enum</c> values from the list of choices.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The <c>enum</c> selected by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static T Enum<T>
        (
            T defaultValue = default,
            Title? title = null,
            bool required = false,
            IEnumerable<T> only = null,
            IEnumerable<T> except = null,
            int padBefore = 0,
            int padAfter = 0
        ) where T : struct, Enum
        {
            var selection = NEnumInternal
            (
                title: title,
                required: required,
                nullable: false,
                only: only,
                except: except,
                padBefore: padBefore,
                padAfter: padAfter
            );
            if (selection == null)
                return defaultValue;
            return (T)selection;
        }

        /// <summary>
        /// Prompts for a nullable <c>enum</c> value. Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="title">An optional list title.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="only">Restrict the list of choices to these <c>enum</c> values, if supplied.</param>
        /// <param name="except">Omit these <c>enum</c> values from the list of choices.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The nullable <c>enum</c> selected by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static T? NEnum<T>
        (
            Title? title = null,
            bool required = false,
            IEnumerable<T> only = null,
            IEnumerable<T> except = null,
            int padBefore = 0,
            int padAfter = 0
        ) where T : struct, Enum
        {
            return NEnumInternal
            (
                title: title,
                required: required,
                nullable: true,
                only: only,
                except: except,
                padBefore: padBefore,
                padAfter: padAfter
            );
        }

        private static T? NEnumInternal<T>
        (
            Title? title,
            bool required,
            bool nullable,
            IEnumerable<T> only,
            IEnumerable<T> except,
            int padBefore,
            int padAfter
        ) where T : struct, Enum
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            Type type = typeof(T);

            //// handle nullable - not relevant when type constraints include struct
            //bool nullable = false;
            //if (type.TryGetUnderlyingType(out Type underlyingType))
            //{
            //    nullable = true;
            //    type = underlyingType;
            //}

            SystemMessageRelay.RelayMethodParams
            (
                DictionaryUtil.From<string, object>("T", DotnetUtil.GetSourceCodeFormattedName(type))
                    .AppendRTL(nameof(title), title)
                    .AppendRTL(nameof(required), required),
                group: MessageRelayGroup
            );

            //// validation - not needed if type constraints include Enum
            //if (!typeof(T).IsEnumType())
            //{
            //    var vex = new ValidationException("Not an enum type: " + typeof(T).FullName);
            //    SystemMessageRelay.RelayException(vex, group: MessageRelayGroup);
            //    throw vex;
            //}

            // create / configure custom menu
            if (title == null)
            {
                title = type.Name;
            }
            if (required)
            {
                title += " (required)";
            }

            // prepare variables
            T? choice = null;
            var enumValues = only != null && only.Any() 
                ? only 
                : System.Enum.GetValues(type).Cast<T>();
            if (except != null)
                enumValues = enumValues.Where(e => !e.In(except));

            // create / customize menu
            var customMenuItems = ListUtil.AppendIf
            (
                enumValues.Select(e => RoutineX.BuildCustomRoutine
                (
                    e.ToString() + (Equals(e, default(T)) ? " (default)" : ""), 
                    () => choice = e, 
                    command: ((int)(object)e).ToString()) as MenuObject
                ),
                nullable && !required, 
                RoutineX.BuildCustomRoutine(TextConstants.Null, () => { /* do nothing - choice remains null */ }, command: "N")
            );

            // prompt custom menu for choice selection
            var menuSelection = Menu
            (
                null as IEnumerable<T>,
                customItemsToAppend: customMenuItems,
                title: title,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturnValue(choice, group: MessageRelayGroup);
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
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        public static string Password
        (
            PromptType promptType = PromptType.Auto,
            bool required = false,
            bool suppressRequiredPrompt = false,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var password = Password
            (
                "password",
                promptType: promptType,
                required: required,
                suppressRequiredPrompt: suppressRequiredPrompt,
                padBefore: padBefore,
                padAfter: padAfter
            );

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
            return password;
        }

        /// <summary>
        /// Prompts a user to securely enter a password.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        public static string Password
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            bool suppressRequiredPrompt = false,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            RenderX.Pad(padBefore);

            RenderX.Prompt(prompt, promptType: promptType, required: required && !suppressRequiredPrompt);
            var password = _Keystrokes('*');

            RenderX.Pad(padAfter);

            SystemMessageRelay.RelayMethodReturnValue(new string('*', password.Length), group: MessageRelayGroup);
            return password;
        }

        /// <summary>
        /// Prompts a user to securely enter a secure password.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="promptType">How<c>ConsoleX</c> should render the input prompt</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        public static SecureString SecurePassword
        (
            string prompt,
            PromptType promptType = PromptType.Auto,
            bool required = false,
            bool suppressRequiredPrompt = false,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var secureString = new SecureString();

            RenderX.Pad(padBefore);

            RenderX.Prompt(prompt, promptType: promptType, required: required && !suppressRequiredPrompt);
            var buf = _BufferedKeystrokes('*');
            for (int i = 0; i < buf.Count; i++)
            {
                secureString.AppendChar(buf[i]);
                buf[i] = '\0';   // zero out the buffer as the secure string is built
            }
            secureString.MakeReadOnly();

            RenderX.Pad(padAfter);

            SystemMessageRelay.RelayMethodReturnValue(new string('*', buf.Count), group: MessageRelayGroup);
            return secureString;
        }

        /// <summary>
        /// Prompt a user to choose from a list of items.  By default the index style is 1-based.
        /// </summary>
        /// <typeparam name="T">type of item</typeparam>
        /// <param name="collection">a collection</param>
        /// <param name="title">a list title</param>
        /// <param name="indexStyle">Whether the displayed index and the index input by the user is 0 or 1-based (default is 1-based).</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="selectionMode">Defines whether an input is required and whether to enable syntax for multiselection.</param>
        /// <param name="columns">The number of columns with which to render the list.</param>
        /// <param name="padBefore">The number of new lines to render before the list</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        public static ListSelection<T> List<T>
        (
            IEnumerable<T> collection,
            Title? title = null,
            Func<T, string> renderer = null,
            ListIndexStyle indexStyle = ListIndexStyle.OneBased,
            ListSelectionMode selectionMode = default,
            int columns = 1,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            if (indexStyle != ListIndexStyle.ZeroBased && indexStyle != ListIndexStyle.OneBased)
                throw new ValidationException("Invalid indexStyle: " + indexStyle);

            var list = CollectionUtil.ToList(collection, optimize: Optimization.ReuseCollection);
            var listSelection = new ListSelection<T>();
            var listConfigurator = new MenuAndListRealtimeConfigurator();

            RenderX.List(list, title: title, indexStyle: indexStyle, renderer: renderer, listConfigurator: listConfigurator, columns: columns, padBefore: padBefore, padAfter: 0);

            if (listConfigurator.IsNotSelectable)
            {
                RenderX.Alert("Selection canceled (list not selectable).");
                Continue();
                return listSelection; // nothing selected -> selected index = -1
            }

            var input = String
            (
                null,
                promptType: PromptType.MenuOrList,
                required: selectionMode == ListSelectionMode.ExactlyOne || selectionMode == ListSelectionMode.OneOrMore,
                requiredMessage: "A selection is required.",
                suppressRequiredPrompt: true
            );

            if (selectionMode == ListSelectionMode.ZeroOrOne || selectionMode == ListSelectionMode.ExactlyOne)  // not multiple
            {
                try
                {
                    if (input != null)
                    {
                        var index = int.Parse(input);
                        AssertValue.InRange(index, indexStyle == ListIndexStyle.ZeroBased ? 0 : 1, list.Count - (indexStyle == ListIndexStyle.ZeroBased ? 1 : 0));
                        listSelection.Selection.Add(index, list[index - (indexStyle == ListIndexStyle.ZeroBased ? 0 : 1)]);
                    }
                    RenderX.Pad(padAfter);
                    tryAgainCount = 0;
                    return listSelection;
                }
                catch (Exception afex)
                {
                    RenderX.Alert(afex.Message.Replace(AssertionConstants.EXCEPTION_MESSAGE_PREFIX, ""));
                    Continue(prompt: "Press any key to try again..." + (++tryAgainCount > 2 ? "  (you can enter '/back' at the prompt to exit the routine)" : ""));
                    return List(collection, title: title, renderer: renderer, indexStyle: indexStyle, selectionMode: selectionMode, columns: columns, padBefore: padBefore, padAfter: padAfter);
                }
            }

            try
            {
                var selectedIndices = UserInputUtil.ParseMultipleIndices(input, list.Count, indexStyle: indexStyle); 
                listSelection.Selection.Append(selectedIndices.Select(i => new KeyValuePair<int, T>(i, list[i - (indexStyle == ListIndexStyle.ZeroBased ? 0 : 1)])));
                RenderX.Pad(padAfter);
                tryAgainCount = 0;
                return listSelection;
            }
            catch (Exception ex)
            {
                RenderX.Alert(ex.Message);
                Continue(prompt: "Press any key to try again..." + (++tryAgainCount > 2 ? "  indexStyle: indexStyle, (you can enter '/back' at the prompt to exit the routine)" : ""));
                return List(collection, title: title, renderer: renderer, selectionMode: selectionMode, columns: columns, padBefore: padBefore, padAfter: padAfter);
            }
        }

        /// <summary>
        /// Prompt a user to choose from a list of items
        /// </summary>
        /// <typeparam name="T">type of item</typeparam>
        /// <param name="mappings">a list of key/value pairs</param>
        /// <param name="title">a list title</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="selectionMode">Defines whether an input is required and whether to enable syntax for multiselection.</param>
        /// <param name="columns">The number of columns with which to render the list.</param>
        /// <param name="padBefore">The number of new lines to render before the list</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static ListSelection<T> List<T>
        (
            IEnumerable<KeyValuePair<int,T>> mappings,
            Title? title = null,
            Func<T, string> renderer = null,
            ListSelectionMode selectionMode = default,
            int columns = 1,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            var dict = DictionaryUtil.From(mappings, optimize: Optimization.ReuseCollection);
            var listSelection = new ListSelection<T>();
            var listConfigurator = new MenuAndListRealtimeConfigurator();

            RenderX.List(dict, title: title, indexStyle: ListIndexStyle.OneBased, renderer: renderer, listConfigurator: listConfigurator, columns: columns, padBefore: padBefore, padAfter: 0);

            if (listConfigurator.IsNotSelectable)
            {
                RenderX.Alert("Selection canceled (list not selectable).");
                Continue();
                return listSelection; // nothing selected -> selected index = -1
            }

            var input = String
            (
                null,
                promptType: PromptType.MenuOrList,
                required: selectionMode == ListSelectionMode.ExactlyOne || selectionMode == ListSelectionMode.OneOrMore,
                requiredMessage: "A selection is required.",
                suppressRequiredPrompt: true
            );

            if (selectionMode == ListSelectionMode.ZeroOrOne || selectionMode == ListSelectionMode.ExactlyOne)  // not multiple
            {
                try
                {
                    if (input != null)
                    {
                        var index = int.Parse(input);
                        AssertValue.InSet(index, dict.Keys);
                        listSelection.Selection.Add(index, dict[index]);
                    }
                    RenderX.Pad(padAfter);
                    tryAgainCount = 0;
                    return listSelection;
                }
                catch (Exception afex)
                {
                    RenderX.Alert(afex.Message.Replace(AssertionConstants.EXCEPTION_MESSAGE_PREFIX, ""));
                    Continue(prompt: "Press any key to try again..." + (++tryAgainCount > 2 ? "  (you can enter '/back' at the prompt to exit the routine)" : ""));
                    return List(dict, title: title, renderer: renderer, selectionMode: selectionMode, columns: columns, padBefore: padBefore, padAfter: padAfter);
                }
            }

            try
            {
                var selectedIndices = UserInputUtil.ParseMultipleIndices(input, dict.Keys);
                listSelection.Selection.Append(dict.Where(kvp => selectedIndices.Contains(kvp.Key)));
                RenderX.Pad(padAfter);
                tryAgainCount = 0;
                return listSelection;
            }
            catch (Exception ex)
            {
                RenderX.Alert(ex.Message);
                Continue(prompt: "Press any key to try again..." + (++tryAgainCount > 2 ? "  (you can enter '/back' at the prompt to exit the routine)" : ""));
                return List(dict, title: title, renderer: renderer, selectionMode: selectionMode, columns: columns, padBefore: padBefore, padAfter: padAfter);
            }
        }

        /// <summary>
        /// Prompts a user to choose a selection from a menu.
        /// </summary>
        /// <typeparam name="T">type of menu item</typeparam>
        /// <param name="menuItems">a collection items, special treatment of <c>MenuItem</c>s</param>
        /// <param name="customItemsToPrepend">custom items to list before the regular menu items</param>
        /// <param name="customItemsToAppend">custom items to list after the regular menu items</param>
        /// <param name="selectDefault">Optional logic for selecting an item when no selection is made (e.g. input is blank)</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="title">a title</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="columns">the number of columns in which to render the list</param>
        /// <param name="configureTextGrid">exposes a reference to the underlying <c>TextGrid</c> for further configuration</param>
        /// <param name="allowArbitraryInput">allow arbitrary text in addition to menu item selection</param>
        /// <param name="allowMultipleSelection">allow entry of multiple list items such as: 1-3, 5, etc.</param>
        /// <param name="onMenuSelection">an action to perfrom after menu selection is complete</param>
        /// <param name="onRoutineAutoRunComplete">an action to perform after selection and autorun of a routine</param>
        /// <param name="onRoutineAutoRunError">an action to perform after an autorun routine throws an exception</param>
        /// <returns>The selected menu selection(s)</returns>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        public static MenuSelection<T> Menu<T>
        (
            IEnumerable<T> menuItems,
            IList<MenuObject> customItemsToPrepend = null,
            IList<MenuObject> customItemsToAppend = null,
            Func<T> selectDefault = null,
            Func<T, string> renderer = null,
            Title? title = null,
            int padBefore = 0,
            int padAfter = 1,
            int columns = 1,
            Action<TextGrid> configureTextGrid = null,
            bool allowArbitraryInput = false,
            bool allowMultipleSelection = false,
            Action<MenuSelection<T>> onMenuSelection = null,
            Action<RoutineX> onRoutineAutoRunComplete = null,
            Action<RoutineX, Exception> onRoutineAutoRunError = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var menuSelection = new MenuSelection<T>();
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

            if (listConfigurator.IsNotSelectable)
            {
                RenderX.Alert("Selection canceled (menu not selectable).");
                Continue();
                return menuSelection; // nothing selected -> selected index = -1
            }

            while (true)
            {
                var input = String(null, promptType: PromptType.MenuOrList, padAfter: padAfter);

                // selection type 1 of 5 - no input
                if (input == null)
                {
                    if (selectDefault != null)
                    {
                        menuSelection.Selection.Add(default, selectDefault());
                        SystemMessageRelay.RelayMessage("selection type: no input", group: MessageRelayGroup);
                        break;
                    }
                    input = "";
                }

                // selection type 2 of 5 - custom menu items
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
                    menuSelection.CustomMenuItem = selectedCustomMenuItem;
                    SystemMessageRelay.RelayMessage("selection type: custom menu item", group: MessageRelayGroup);
                    break;
                }

                // selection type 3 of 5 - menu items
                if (menuItems != null && menuItems.Any())
                {
                    var nonHeaderMenuItems = menuItems
                        .Where(mi => !(mi is MenuHeader))
                        .ToList();
                    if (int.TryParse(input, out int index))
                    {
                        if (!AssertValue.TryInRange(index, 1, nonHeaderMenuItems.Count, out string message))
                        {
                            RenderX.Alert(message);
                            continue;
                        }
                        menuSelection.Selection.Add(index, nonHeaderMenuItems[index - 1]);  // menu indices is 1-based
                        SystemMessageRelay.RelayMessage("selection type: menu item", group: MessageRelayGroup);
                        break;
                    }
                }

                // selection type 4 of 5 - multiple selections
                if (allowMultipleSelection && input.Length != 0)
                {
                    SystemMessageRelay.RelayMessage("selection type: multiple selections", group: MessageRelayGroup);
                    try
                    {
                        var _menuItems = new List<T>(menuItems);
                        var selectedIndices = UserInputUtil.ParseMultipleIndices(input, _menuItems.Count);   // 1-based indices default
                        foreach (var selIndex in selectedIndices)
                        {
                            menuSelection.Selection.Add(selIndex, _menuItems[selIndex - 1]);
                        }
                        break;
                    }
                    catch (BenignException ex)
                    {
                        RenderX.Alert(ex.Message);
                        continue;
                    }
                }

                // selection type 5 of 5 - arbitrary input
                if (allowArbitraryInput)
                {
                    menuSelection.ArbitraryInput = input;
                    SystemMessageRelay.RelayMessage("selection type: arbitrary input", group: MessageRelayGroup);
                    break;
                }

                RenderX.Alert("Invalid menu selection.");
            }

            // process menu selection
            onMenuSelection?.Invoke(menuSelection);
            RenderX.Pad(padAfter);
            if (menuSelection.SelectedRoutine != null)
            {
                bool autoRunComplete;
                try
                {
                    SystemMessageRelay.RelayMessage("auto run routine \"" + menuSelection.SelectedRoutine.Text + "\"...", group: MessageRelayGroup);
                    menuSelection.SelectedRoutine.Run();
                    autoRunComplete = true;
                    SystemMessageRelay.RelayMessage("auto run complete", group: MessageRelayGroup);
                }
                catch (ConsoleNavigation cn)  // e.g. exit, back
                {
                    SystemMessageRelay.RelayMessage(cn.Message, group: MessageRelayGroup, indent: Indent.DecrementNext);
                    throw;
                }
                catch (Exception ex)
                {
                    autoRunComplete = false;
                    if (onRoutineAutoRunError != null)
                    {
                        SystemMessageRelay.RelayException(ex, group: MessageRelayGroup, inlineWithMessages: true);
                        onRoutineAutoRunError.Invoke(menuSelection.SelectedRoutine, ex);
                    }
                    else
                    {
                        SystemMessageRelay.RelayException(ex, group: MessageRelayGroup);
                        throw;
                    }
                }
                if (autoRunComplete)
                {
                    onRoutineAutoRunComplete?.Invoke(menuSelection.SelectedRoutine);
                }
            }

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
            return menuSelection;
        }
    }
}
