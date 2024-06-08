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
        private static string _Keystrokes(char? mask, object quickValue, TraceJournal journal)
        {
            // journaling
            journal.WriteEntry("PromptX._KeyStrokes()");
            journal.Level++;

            // do stuff
            string bufferedKeystrokes = string.Join("", _BufferedKeystrokes(mask, quickValue, journal));
            journal.WriteEntry(nameof(bufferedKeystrokes) + " = \"" + bufferedKeystrokes + "\"");

            // finalize
            journal.Level--;
            return bufferedKeystrokes;
        }

        private static IList<char> _BufferedKeystrokes(char? mask, object quickValue, TraceJournal journal)
        {
            // journaling
            journal.WriteEntry("PromptX._BufferedKeyStrokes()");
            journal.Level++;

            // do stuff
            var buf = new List<char>();
            //var treatControlCAsInput = Console.TreatControlCAsInput;
            ConsoleKeyInfo info;
            bool inputting = true;
            bool alt;
            bool ctrl;
            bool shift;
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
            Console.WriteLine();
            if (quickValue != null && !buf.Any(c => !char.IsWhiteSpace(c)))
            {
                buf.Clear();
                Console.WriteLine(quickValue);
            }

            // finalize
            journal.Level--;
            return buf;
        }

        private static string _ConsoleLine(object quickValue, bool suppressEcho, TraceJournal journal)
        {
            // journaling
            journal.WriteEntry("PromptX._ConsoleLine()");
            journal.Level++;

            // do stuff
            var rawLine = Console.ReadLine();
            journal.WriteEntry(nameof(rawLine) + " = \"" + rawLine + "\"");

            if (quickValue != null && string.IsNullOrWhiteSpace(rawLine) && !suppressEcho)
            {
                journal.WriteEntry("user selected quick value: " + quickValue);
                Console.WriteLine(quickValue);
            }

            // finalize
            journal.Level--;
            return rawLine;
        }

        /// <summary>
        /// Prompts for input, accepts free text with no string trimming.  Command recognition: 'exit'.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">Value to apply by pressing 'Enter', suggested before the prompt.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="requiredMessage">The alert to display if a required input is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The exact text entered by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutine"></exception>
        public static string RawConsoleInput
        (
            string prompt,
            bool required = false,
            object quickValue = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.RawConsoleInput()");
            journal.Level++;

            // do work
            string input;

            RenderX.Pad(padBefore);

            //if (quickText != null)
            //{
            //    Console.WriteLine("(Press 'Enter' to input \"" + quickText + "\")");
            //}

            try
            {
                RenderX.Prompt(prompt, required: required && !suppressRequiredPrompt, quickValue: quickValue);
                //input = _KeyStrokes(mask, quickText, journal);
                input = _ConsoleLine(quickValue, suppressEcho, journal);
                while (input.Length == 0 && required && quickValue == null)
                {
                    RenderX.Alert(requiredMessage);
                    RenderX.Prompt(prompt, required: !suppressRequiredPrompt);
                    //input = _KeyStrokes(mask, quickText, journal);
                    input = _ConsoleLine(quickValue, suppressEcho, journal);
                }
            }
            //catch (ConsoleNavigation.CtrlCException)
            //{
            //    throw;
            //}
            finally
            {
                // finalize
                journal.Level--;
                RenderX.Pad(padAfter);
            }

            return input;
        }

        /// <summary>
        /// Prompts for input, accepts free text with no string trimming.  Command recognition: 'exit'.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="mask">An optional character to display instead of the typed key.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">Value to apply by pressing 'Enter'.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="requiredMessage">The alert to display if a required input is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The exact text entered by the user.</returns>
        /// <exception cref="ConsoleNavigation.CtrlCException"></exception>
        public static string RawKeystrokeInput
        (
            string prompt,
            char? mask = null,
            bool required = false,
            object quickValue = null,
            bool suppressRequiredPrompt = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.RawKeystrokeInput()");
            journal.Level++;

            // do work
            string input;

            RenderX.Pad(padBefore);

            //if (quickText != null)
            //{
            //    Console.WriteLine("(Press 'Enter' to input \"" + quickText + "\")");
            //}

            try
            {
                RenderX.Prompt(prompt, required: required && !suppressRequiredPrompt, quickValue: quickValue);
                input = _Keystrokes(mask, quickValue, journal);
                //input = _ConsoleLine(quickValue, suppressEcho, journal);
                while (input.Length == 0 && required && quickValue == null)
                {
                    RenderX.Alert(TextUtil.Reveal(requiredMessage));
                    RenderX.Prompt(prompt, required: !suppressRequiredPrompt);
                    input = _Keystrokes(mask, quickValue, journal);
                    //input = _ConsoleLine(quickValue, suppressEcho, journal);
                }
            }
            //catch (ConsoleNavigation.CtrlCException)
            //{
            //    throw;
            //}
            finally
            {
                // finalize
                journal.Level--;
                RenderX.Pad(padAfter);
            }

            return input;
        }

        /// <summary>
        /// Prompts for console input of type <c>T</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// Strings are trimmed. (whitespace trimmed and then blank converts to <c>null</c>).
        /// Client code may wrap this call in try-block to handle <see cref="ConsoleNavigation.CancelInputPromptException" />.
        /// </summary>
        /// <typeparam name="T">A runtime type.</typeparam>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="parser">A required text-to-value converter.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="dateTimeStyle">Applies to <c>To&lt;[datetime]&gt;()</c>. If supplied, indicates the expected date/time format.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="trueValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="encoding">Applies to <c>To&lt;byte[]&gt;()</c>. An optional text encoding, e.g. UTF8.</param>
        /// <param name="inheritedType">An optional type constraint - the type to which the returned <c>Type</c> must be assignable.</param>
        /// <param name="ignoreCase">Applies to <c>To&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        /// <exception cref="ConversionException"></exception>
        public static T Value<T>
        (
            string prompt,
            Func<string,T> parser = null,
            bool required = false,
            T quickValue = default,
            DateTimeStyles? dateTimeStyle = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            Encoding encoding = null,
            Type inheritedType = null,
            bool ignoreCase = false,
            Action<T> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.Value<T>()");
            journal.Level++;

            // do stuff
            string input;
            T value;

            while (true)
            {
                input = RawConsoleInput
                (
                    prompt,
                    required: required,
                    quickValue: quickValue,
                    suppressRequiredPrompt: suppressRequiredPrompt,
                    suppressEcho: suppressEcho,
                    requiredMessage: requiredMessage,
                    padBefore: padBefore,
                    padAfter: padAfter,
                    journal: journal
                ).Trim();

                // use quick value or return null, if applicable
                if (input.Length == 0) // if required, and no quick value was supplied, length will not equal 0
                {
                    // finalize
                    journal.Level--;

                    return quickValue;  // quick value or, simply, null
                }

                // process commands, if applicable
                if (!suppressCommands)
                {
                    if (input.Trim().Equals("/exit", StringComparison.CurrentCultureIgnoreCase))
                        Environment.Exit(0);
                    if (input.Trim().Equals("/back", StringComparison.CurrentCultureIgnoreCase))
                        ConsoleNavigation.BackoutRoutine();
                }

                // parse
                value = parser != null
                    ? parser.Invoke(input)
                    : Zap.To<T>(input, dateTimeStyle: dateTimeStyle, numberStyle: numberStyle, provider: provider, locale: locale, trueValues: trueValues, falseValues: falseValues, encoding: encoding, inheritedType: inheritedType, ignoreCase: ignoreCase);

                // validate input, if applicable
                try
                {
                    validator?.Invoke(value);
                    break;
                }
                catch (AssertionFailedException ex)
                {
                    RenderX.Alert(ex.Message.Replace(AssertionFailedException.MESSAGE_PREFIX + ": ", ""));
                }
            }

            // finalize
            journal.Level--;
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>string</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// Strings are zapped (whitespace trimmed and then blank converts to <c>null</c>).
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static string String
        (
            string prompt,
            bool required = false,
            string quickValue = null,
            Action<string> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.String()");
            journal.Level++;

            // do stuff
            var value = Value
            (
                prompt,
                required: required,
                quickValue: quickValue,
                validator: validator,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressEcho: suppressEcho,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            // finalize
            journal.Level--;
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>int?</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static int? NInt
        (
            string prompt,
            bool required = false,
            int? quickValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<int?> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.NInt()");
            journal.Level++;

            // do stuff
            var value = Value
            (
                prompt,
                required: required,
                quickValue: quickValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            // finalize
            journal.Level--;
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>int</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static int Int
        (
            string prompt,
            bool required = false,
            int defaultValue = default,
            int? quickValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<int> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.Int()");
            journal.Level++;

            // do stuff
            var nValue = NInt
            (
                prompt,
                required: required,
                quickValue: quickValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: convertValidator(validator),
                suppressEcho: suppressEcho,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            Action<int?> convertValidator(Action<int> _validator)
            {
                if(_validator != null)
                {
                    return (nNum) => _validator.Invoke(nNum ?? defaultValue);
                }
                return null;
            }

            // finalize
            journal.Level--;
            return nValue ?? defaultValue;
        }

        /// <summary>
        /// Prompts for console input of type <c>double?</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static double? NDouble
        (
            string prompt,
            bool required = false,
            double? quickValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<double?> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.NDouble()");
            journal.Level++;

            // do stuff
            var value = Value
            (
                prompt,
                required: required,
                quickValue: quickValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressEcho: suppressEcho,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            // finalize
            journal.Level--;
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>double</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static double Double
        (
            string prompt,
            bool required = false,
            double defaultValue = default,
            double? quickValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<double> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.Double()");
            journal.Level++;

            // do stuff
            var nValue = NDouble
            (
                prompt,
                required: required,
                quickValue: quickValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: convertValidator(validator),
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressEcho: suppressEcho,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            Action<double?> convertValidator(Action<double> _validator)
            {
                if (_validator != null)
                {
                    return (nNum) => _validator.Invoke(nNum ?? defaultValue);
                }
                return null;
            }

            // finalize
            journal.Level--;
            return nValue ?? defaultValue;
        }

        /// <summary>
        /// Prompts for console input of type <c>decimal?</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static decimal? NDecimal
        (
            string prompt,
            bool required = false,
            decimal? quickValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<decimal?> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.NDecimal()");
            journal.Level++;

            // do stuff
            var value = Value
            (
                prompt,
                required: required,
                quickValue: quickValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: validator,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressEcho: suppressEcho,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            // finalize
            journal.Level--;
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>decimal</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="numberStyle">Applies to <c>To&lt;[numeric-type]&gt;()</c>. If supplied, indicates the expected number format.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static decimal Decimal
        (
            string prompt,
            bool required = false,
            decimal defaultValue = default,
            decimal? quickValue = null,
            NumberStyles? numberStyle = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<decimal> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.Decimal()");
            journal.Level++;

            // do stuff
            var nValue = NDecimal
            (
                prompt,
                required: required,
                quickValue: quickValue,
                numberStyle: numberStyle,  // numeric formatting
                provider: provider,        // numeric formatting
                locale: locale,            // numeric formatting
                validator: convertValidator(validator),
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressEcho: suppressEcho,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            Action<decimal?> convertValidator(Action<decimal> _validator)
            {
                if (_validator != null)
                {
                    return (nInt) => _validator.Invoke(nInt ?? defaultValue);
                }
                return null;
            }

            // finalize
            journal.Level--;
            return nValue ?? defaultValue;
        }

        /// <summary>
        /// Prompts for console input of type <c>bool?</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="trueValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="ignoreCase">Applies to <c>To&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static bool? NBool
        (
            string prompt,
            bool required = false,
            bool? quickValue = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            bool ignoreCase = false,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.NBool()");
            journal.Level++;

            // do stuff
            var value = Value
            (
                prompt,
                required: required,
                quickValue: quickValue,
                trueValues: trueValues,
                falseValues: falseValues,
                ignoreCase: ignoreCase,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressEcho: suppressEcho,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            // finalize
            journal.Level--;
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>bool</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="trueValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">Applies to <c>To&lt;bool&gt;()</c>. A pipe delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="ignoreCase">Applies to <c>To&lt;[enum-type-or-bool]&gt;()</c>. If <c>true</c>, the letter case of an enum value <c>string</c> is ignored when converting to the actual <c>enum</c> value, default is <c>false</c>.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static bool Bool
        (
            string prompt,
            bool required = false,
            bool defaultValue = default,
            bool? quickValue = null,
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            bool ignoreCase = false,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.Bool()");
            journal.Level++;

            // do stuff
            var nValue = NBool
            (
                prompt,
                required: required,
                quickValue: quickValue,
                trueValues: trueValues,
                falseValues: falseValues,
                ignoreCase: ignoreCase,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressEcho: suppressEcho,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            // finalize
            journal.Level--;
            return nValue ?? defaultValue;
        }

        /// <summary>
        /// Prompts for console input of type <c>DateTime?</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static DateTime? NDateTime
        (
            string prompt,
            bool required = false,
            DateTime? quickValue = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<DateTime?> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.NDateTime()");
            journal.Level++;

            // do stuff
            var value = Value
            (
                prompt,
                required: required,
                quickValue: quickValue,
                provider: provider,        // date formatting
                locale: locale,            // date formatting
                validator: validator,
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressEcho: suppressEcho,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            // finalize
            journal.Level--;
            return value;
        }

        /// <summary>
        /// Prompts for console input of type <c>DateTime</c>.  Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        /// <param name="quickValue">An optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'.</param>
        /// <param name="provider">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">Applies to <c>To&lt;[numeric-type-or-datetime]&gt;()</c>. An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <param name="validator">An optional validation routine to be run on the parsed input value.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="suppressEcho">Instructs the console to not print the quick value if selected.</param>
        /// <param name="suppressCommands">Instucts the console to ignore, rather than process, commands like 'exit'.</param>
        /// <param name="requiredMessage">The alert to display if a required value is not supplied.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <returns>The value input by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
        public static DateTime DateTime
        (
            string prompt,
            bool required = false,
            DateTime defaultValue = default,
            DateTime? quickValue = null,
            IFormatProvider provider = null,
            string locale = null,
            Action<DateTime> validator = null,
            bool suppressRequiredPrompt = false,
            bool suppressEcho = false,
            bool suppressCommands = false,
            string requiredMessage = "Input is required.",
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("PromptX.DateTime()");
            journal.Level++;

            // do stuff
            var nValue = NDateTime
            (
                prompt,
                required: required,
                quickValue: quickValue,
                provider: provider,        // date formatting
                locale: locale,            // date formatting
                validator: convertValidator(validator),
                suppressRequiredPrompt: suppressRequiredPrompt,
                suppressEcho: suppressEcho,
                suppressCommands: suppressCommands,
                requiredMessage: requiredMessage,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );

            Action<DateTime?> convertValidator(Action<DateTime> _validator)
            {
                if (_validator != null)
                {
                    return (nDate) => _validator.Invoke(nDate ?? defaultValue);
                }
                return null;
            }

            // finalize
            journal.Level--;
            return nValue ?? defaultValue;
        }

        /// <summary>
        /// Prompts for <c>enum</c> value. Accepts certain commands i.e. '/exit' and '/back' by default.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <param name="title">An optional list title.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="only">Restrict the list of choices to these <c>enum</c> values, if supplied.</param>
        /// <param name="except">Omit these <c>enum</c> values from the list of choices.</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user.</returns>
        /// <exception cref="ConsoleNavigation.BackoutRoutineException"></exception>
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
                title = enumTypeForList.Name + (required ? " (required)" : "");
            }
            else if (required)
            {
                title += " (required)";
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
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.CtrlCException"></exception>
        public static string Password
        (
            bool required = false,
            bool suppressRequiredPrompt = false,
            int padBefore = 0,
            int padAfter = 0,
            TraceJournal journal = null
        )
        {
            return Password
            (
                "password",
                required: required,
                suppressRequiredPrompt: suppressRequiredPrompt,
                padBefore: padBefore,
                padAfter: padAfter,
                journal: journal
            );
        }

        /// <summary>
        /// Prompts a user to securely enter a password.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.CtrlCException"></exception>
        public static string Password
        (
            string prompt,
            bool required = false,
            bool suppressRequiredPrompt = false,
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

            RenderX.Pad(padBefore);

            RenderX.Prompt(prompt, required: required && !suppressRequiredPrompt);
            var password = _Keystrokes('*', null, journal);

            RenderX.Pad(padAfter);

            // finalize
            journal.Level--;
            return password;
        }

        /// <summary>
        /// Prompts a user to securely enter a secure password.
        /// </summary>
        /// <param name="prompt">The text to render at the prompt.</param>
        /// <param name="required">If <c>true</c>, forces non-blank input, default is <c>false</c>.</param>
        /// <param name="suppressRequiredPrompt">Instructs the console to not label the prompt as "(required)".</param>
        /// <param name="padBefore">The number of new lines to render before the prompt.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <param name="journal">A trace journal to which steps within complex methods can be logged.</param>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.CtrlCException"></exception>
        public static SecureString SecurePassword
        (
            string prompt,
            bool required = false,
            bool suppressRequiredPrompt = false,
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

            RenderX.Prompt(prompt, required: required && !suppressRequiredPrompt);
            var buf = _BufferedKeystrokes('*', null, journal);
            for (int i = 0; i < buf.Count; i++)
            {
                secureString.AppendChar(buf[i]);
                buf[i] = '\0';
            }
            secureString.MakeReadOnly();

            RenderX.Pad(padAfter);

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
        /// <param name="columns">The number of columns with which to render the list.</param>
        /// <param name="padBefore">The number of new lines to render before the collection.</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.CtrlCException"></exception>
        public static ListSelection<T> List<T>
        (
            IEnumerable<T> collection,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            Func<T, string> renderer = null,
            int columns = 1,
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
                columns: columns,
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
        /// <param name="columns">The number of columns with which to render the list.</param>
        /// <param name="padBefore">The number of new lines to render before the list</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.CtrlCException"></exception>
        public static ListSelection<T> List<T>
        (
            IList<T> list,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            Func<T, string> renderer = null,
            int columns = 1,
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
                columns: columns,
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
        /// <param name="columns">The number of columns with which to render the list.</param>
        /// <param name="padBefore">The number of new lines to render before the list</param>
        /// <param name="padAfter">The number of new lines to render after the prompt.</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="ConsoleNavigation.CancelInputPromptException"></exception>
        /// <exception cref="ConsoleNavigation.CtrlCException"></exception>
        public static ListSelection<T> List<T>
        (
            IList<T> list,
            out int selectedIndex,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            Func<T, string> renderer = null,
            int columns = 1,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            if (indexPolicy != ListIndexPolicy.DisplayZeroBased && indexPolicy != ListIndexPolicy.DisplayOneBased)
            {
                throw new ValidationException("Invalid indexPolicy: " + indexPolicy);
            }

            T choice;

            var listConfigurator = new MenuAndListRealtimeConfigurator();
            RenderX.List(list, title: title, indexPolicy: indexPolicy, renderer: renderer, listConfigurator: listConfigurator, columns: columns, padBefore: padBefore, padAfter: 0);

            if (listConfigurator.IsNotSelectable)
            {
                selectedIndex = -1;
                RenderX.Alert("Selection canceled (list not selectable).");
                Continue();
                return new ListSelection<T> { SelectedIndex = -1 };
            }

            var index = Int
            (
                null,
                required: required,
                suppressRequiredPrompt: true,
                requiredMessage: "A selection is required.",
                validator: (i) => Assert.InRange(i, 0 + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? 0 : 1), list.Count + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? -1 : 0))
            );
            RenderX.Pad(padAfter);

            choice = list[index + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? 0 : -1)];
            selectedIndex = index;

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
        /// <exception cref="ConsoleNavigation.CtrlCException"></exception>
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

            if (listConfigurator.IsNotSelectable)
            {
                RenderX.Alert("Selection canceled (menu not selectable).");
                Continue();
                return new MenuSelection<T> { SelectedIndex = -1 };
            }

            MenuSelection<T> menuSelection = null;

            while (true)
            {
                var input = String(null, padAfter: padAfter);
                onMenuSelecting?.Invoke(input);

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
                        if (!Assert.TryInRange(index, 1, nonHeaderMenuItems.Count, out string message))
                        {
                            RenderX.Alert(message);
                            continue;
                        }
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
                    menuSelection = new MenuSelection<T> { ArbitraryInput = input };
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
