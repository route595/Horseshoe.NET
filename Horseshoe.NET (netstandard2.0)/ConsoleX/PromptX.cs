using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

using Horseshoe.NET.Collections;
using Horseshoe.NET.DateAndTime;
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
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickText">an optional common or predictive input that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="autoTrim">whether to trim leading and trailing whitespaces, default is <c>true</c></param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The text entered by the user</returns>
        public static string Input
        (
            bool required = false, 
            string requiredIndicator = "*",
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
                requiredIndicator: requiredIndicator,
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
        /// <param name="prompt">The text to render at the prompt</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickText">an optional common or predictive input that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="autoTrim">whether to trim leading and trailing whitespaces, default is <c>true</c></param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The text entered by the user</returns>
        public static string Input
        (
            string prompt, 
            bool required = false, 
            string requiredIndicator = "*",
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
                RenderX.Prompt(prompt, required: required, requiredIndicator: requiredIndicator);
                input = autoTrim 
                    ? Console.ReadLine().Trim()
                    : Console.ReadLine();
                if (input.Equals(""))
                {
                    if (quickText != null)
                    {
                        input = quickText;
                        RenderX.Prompt(prompt, required: required, requiredIndicator: requiredIndicator);
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
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <returns>The exact text entered by the user</returns>
        public static string InputVerbatim
        (
            bool required = false,
            string requiredIndicator = "*",
            int padBefore = 0, 
            int padAfter = 0
        )
        {
            return InputVerbatim
            (
                null,
                required: required,
                requiredIndicator: requiredIndicator,
                padBefore: padBefore, 
                padAfter: padAfter
            );
        }

        /// <summary>
        /// Prompts for input, accepts free text with no string trimming and no command recognition
        /// </summary>
        /// <param name="prompt">The text to render at the prompt</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <returns>The exact text entered by the user</returns>
        public static string InputVerbatim
        (
            string prompt,
            bool required = false,
            string requiredIndicator = "*",
            int padBefore = 0, 
            int padAfter = 0
        )
        {
            RenderX.Pad(padBefore);
            RenderX.Prompt(prompt, required: required, requiredIndicator: requiredIndicator);
            string input = Console.ReadLine();
            RenderX.Pad(padAfter);
            return input;
        }

        /// <summary>
        /// Prompts for input, blanks become <c>null</c>s, accepts free text as well as certain commands i.e. 'cancel', 'exit' by default
        /// </summary>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
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
        /// <param name="prompt">The text to render at the prompt</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
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
        /// <param name="parser">A custom text-to-value converter</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static T Value<T>
        (
            Func<string, object> parser = null,
            bool required = false,
            string requiredIndicator = "*",
            T quickValue = null,
            Action<T> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        ) where T : class
        {
            return Value
            (
                null,
                parser: parser,
                required: required,
                requiredIndicator: requiredIndicator,
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
        /// <typeparam name="T">a reference type</typeparam>
        /// <param name="prompt">The text to render at the prompt</param>
        /// <param name="parser">A custom text-to-value converter</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static T Value<T>
        (
            string prompt,
            Func<string, object> parser = null,
            bool required = false,
            string requiredIndicator = "*",
            T quickValue = null,
            Action<T> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        ) where T : class
        {
            while (true)
            {
                string input = Input
                (
                    prompt,
                    required: required,
                    requiredIndicator: requiredIndicator,
                    quickText: quickValue?.ToString(),
                    padBefore: padBefore,
                    padAfter: padAfter,
                    autoTrim: true,
                    canCancel: canCancel,
                    canExitApp: canExitApp
                );

                try
                {
                    var value = input.Length > 0
                        ? ConvertString.To<T>(input, converter: parser)
                        : quickValue;
                    if (value == null)
                        return null;
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
        /// Prompts for typesafe input, accepts <c>T</c> values only as well as certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <typeparam name="T">a value type</typeparam>
        /// <param name="parser">A custom text-to-value converter</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static T Value<T>
        (
            Func<string, object> parser = null,
            bool required = false,
            string requiredIndicator = "*",
            T? quickValue = null,
            Action<T> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        ) where T : struct
        {
            return Value
            (
                null,
                parser: parser,
                required: required,
                requiredIndicator: requiredIndicator,
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
        /// <typeparam name="T">a value type</typeparam>
        /// <param name="prompt">The text to render at the prompt</param>
        /// <param name="parser">A custom text-to-value converter</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static T Value<T>
        (
            string prompt,
            Func<string, object> parser = null,
            bool required = false,
            string requiredIndicator = "*",
            T? quickValue = null,
            Action<T> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        ) where T : struct
        {
            while (true)
            {
                string input = Input
                (
                    prompt,
                    required: required,
                    requiredIndicator: requiredIndicator,
                    quickText: quickValue?.ToString(),
                    padBefore: padBefore,
                    padAfter: padAfter,
                    autoTrim: true,
                    canCancel: canCancel,
                    canExitApp: canExitApp
                );

                try
                {
                    var value = input.Length > 0
                        ? ConvertString.To<T>(input, converter: parser)
                        : quickValue ?? default;
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
        /// Prompts for nullable typesafe input, accepts <c>T</c> values only as well as certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <typeparam name="T">a value type</typeparam>
        /// <param name="parser">A custom text-to-value converter</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user or <c>null</c></returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static T? NValue<T>
        (
            Func<string, object> parser = null,
            Action<T> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        ) where T : struct
        {
            return NValue<T>
            (
                null,
                parser: parser,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for nullable typesafe input, accepts <c>T</c> values only as well as certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <typeparam name="T">a value type</typeparam>
        /// <param name="prompt">The text to render at the prompt</param>
        /// <param name="parser">A custom text-to-value converter</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user or <c>null</c></returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static T? NValue<T>
        (
            string prompt,
            Func<string, object> parser = null,
            Action<T> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        ) where T : struct
        {
            while (true)
            {
                var input = Input
                (
                    prompt,
                    padBefore: padBefore,
                    padAfter: padAfter,
                    canCancel: canCancel,
                    canExitApp: canExitApp
                );

                if (input.Length == 0)
                {
                    return null;
                }

                try
                {
                    var value = ConvertString.To<T>(input, converter: parser);
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
        /// Prompts for a <c>true</c>/<c>false</c> value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="trueValues">pipe delimited list of <c>true</c> values, default = 'y|yes|t|true|1')</param>
        /// <param name="falseValues">pipe delimited list of <c>false</c> values, default = 'n|no|f|false|0')</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static bool Bool
        (
            bool required = false, 
            string requiredIndicator = "*",
            string trueValues = "y|yes|t|true|1", 
            string falseValues = "n|no|f|false|0",
            bool? quickValue = null,
            int padBefore = 0, 
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Value<bool>
            (
                parser: (input) => Zap.Bool(input, trueValues: trueValues, falseValues: falseValues, ignoreCase: true),
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a <c>true</c>/<c>false</c> value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="trueValues">pipe delimited list of <c>true</c> values, default = 'y|yes|t|true|1')</param>
        /// <param name="falseValues">pipe delimited list of <c>false</c> values, default = 'n|no|f|false|0')</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static bool Bool
        (
            string prompt, 
            bool required = false, 
            string requiredIndicator = "*",
            string trueValues = "y|yes|t|true|1", 
            string falseValues = "n|no|f|false|0", 
            bool? quickValue = null, 
            int padBefore = 0, 
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Value<bool>
            (
                prompt,
                parser: (input) => Zap.Bool(input, trueValues: trueValues, falseValues: falseValues, ignoreCase: true),
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable <c>true</c>/<c>false</c> value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="trueValues">pipe delimited list of <c>true</c> values, default = 'y|yes|t|true|1')</param>
        /// <param name="falseValues">pipe delimited list of <c>false</c> values, default = 'n|no|f|false|0')</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static bool? NBool
        (
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0",
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NValue<bool>
            (
                parser: (input) => Zap.NBool(input, trueValues: trueValues, falseValues: falseValues, ignoreCase: true),
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable <c>true</c>/<c>false</c> value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="trueValues">pipe delimited list of <c>true</c> values, default = 'y|yes|t|true|1')</param>
        /// <param name="falseValues">pipe delimited list of <c>false</c> values, default = 'n|no|f|false|0')</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        public static bool? NBool
        (
            string prompt, 
            string trueValues = "y|yes|t|true|1", 
            string falseValues = "n|no|f|false|0", 
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NValue<bool>
            (
                prompt,
                parser: (input) => Zap.NBool(input, trueValues: trueValues, falseValues: falseValues, ignoreCase: true),
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Int
        (
            bool required = false,
            string requiredIndicator = "*",
            int? quickValue = null,
            Action<int> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Int
            (
                null,
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Int
        (
            string prompt,
            bool required = false,
            string requiredIndicator = "*",
            int? quickValue = null,
            Action<int> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Value<int>
            (
                prompt,
                parser: (input) => Convert.ToInt32(input),
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Int
        (
            bool required = false,
            string requiredIndicator = "*",
            int? min = null,
            int? max = null,
            int? quickValue = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Int
            (
                null,
                required: required,
                requiredIndicator: requiredIndicator,
                min: min,
                max: max,
                quickValue: quickValue,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Int
        (
            string prompt,
            bool required = false,
            string requiredIndicator = "*",
            int? min = null,
            int? max = null,
            int? quickValue = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            // validation
            if (min.HasValue && max.HasValue && min > max)
                throw new ValidationException("min value may not exceed max value: " + min + " !< " + max);

            return Int
            (
                prompt,
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                validator: (intValue) =>
                {
                    if (min.HasValue && intValue < min)
                        throw new ArgumentOutOfRangeException(nameof(intValue), "input (" + intValue + ") may not be less than " + min);
                    if (max.HasValue && intValue > max)
                        throw new ArgumentOutOfRangeException(nameof(intValue), "input (" + intValue + ") may not exceed " + max);
                },
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int? NInt
        (
            Action<int> validator,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NInt
            (
                null,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int? NInt
        (
            string prompt,
            Action<int> validator,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NValue<int>
            (
                prompt,
                parser: (input) => Convert.ToInt32(input),
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int? NInt
        (
            int? min = null,
            int? max = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NInt
            (
                null,
                min: min,
                max: max,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int? NInt
        (
            string prompt,
            int? min = null,
            int? max = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            // validation
            if (min.HasValue && max.HasValue && min > max)
                throw new ValidationException("min value may not exceed max value: " + min + " !< " + max);

            return NInt
            (
                prompt,
                validator: (intValue) =>
                {
                    if (min.HasValue && intValue < min)
                        throw new ArgumentOutOfRangeException(nameof(intValue), "input (" + intValue + ") may not be less than " + min);
                    if (max.HasValue && intValue > max)
                        throw new ArgumentOutOfRangeException(nameof(intValue), "input (" + intValue + ") may not exceed " + max);
                },
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static decimal Decimal
        (
            bool required = false,
            string requiredIndicator = "*",
            decimal? quickValue = null,
            Action<decimal> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Decimal
            (
                null,
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static decimal Decimal
        (
            string prompt,
            bool required = false,
            string requiredIndicator = "*",
            decimal? quickValue = null,
            Action<decimal> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Value<decimal>
            (
                prompt,
                parser: (input) => Convert.ToDecimal(input),
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static decimal Decimal
        (
            bool required = false,
            string requiredIndicator = "*",
            decimal? min = null,
            decimal? max = null,
            decimal? quickValue = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Decimal
            (
                null,
                required: required,
                requiredIndicator: requiredIndicator,
                min: min,
                max: max,
                quickValue: quickValue,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static decimal Decimal
        (
            string prompt,
            bool required = false,
            string requiredIndicator = "*",
            decimal? min = null,
            decimal? max = null,
            decimal? quickValue = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            // validation
            if (min.HasValue && max.HasValue && min > max)
                throw new ValidationException("min value may not exceed max value: " + min + " !< " + max);

            return Decimal
            (
                prompt,
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                validator: (decimalValue) =>
                {
                    if (min.HasValue && decimalValue < min)
                        throw new ArgumentOutOfRangeException(nameof(decimalValue), "input (" + decimalValue + ") may not be less than " + min);
                    if (max.HasValue && decimalValue > max)
                        throw new ArgumentOutOfRangeException(nameof(decimalValue), "input (" + decimalValue + ") may not exceed " + max);
                },
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static decimal? NDecimal
        (
            Action<decimal> validator,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NDecimal
            (
                null,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static decimal? NDecimal
        (
            string prompt,
            Action<decimal> validator,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NValue<decimal>
            (
                prompt,
                parser: (input) => Convert.ToDecimal(input),
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static decimal? NDecimal
        (
            decimal? min = null,
            decimal? max = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NDecimal
            (
                null,
                min: min,
                max: max,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static decimal? NDecimal
        (
            string prompt,
            decimal? min = null,
            decimal? max = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            // validation
            if (min.HasValue && max.HasValue && min > max)
                throw new ValidationException("min value may not exceed max value: " + min + " !< " + max);

            return NDecimal
            (
                prompt,
                validator: (decimalValue) =>
                {
                    if (min.HasValue && decimalValue < min)
                        throw new ArgumentOutOfRangeException(nameof(decimalValue), "input (" + decimalValue + ") may not be less than " + min);
                    if (max.HasValue && decimalValue > max)
                        throw new ArgumentOutOfRangeException(nameof(decimalValue), "input (" + decimalValue + ") may not exceed " + max);
                },
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateTime DateTime
        (
            bool required = false,
            string requiredIndicator = "*",
            DateTime? quickValue = null,
            Action<DateTime> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return DateTime
            (
                null,
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateTime DateTime
        (
            string prompt,
            bool required = false,
            string requiredIndicator = "*",
            DateTime? quickValue = null,
            Action<DateTime> validator = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Value<DateTime>
            (
                prompt,
                parser: (input) => Convert.ToDateTime(input),
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateTime DateTime
        (
            bool required = false,
            string requiredIndicator = "*",
            DateTime? min = null,
            DateTime? max = null,
            DateTime? quickValue = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return DateTime
            (
                null,
                required: required,
                requiredIndicator: requiredIndicator,
                min: min,
                max: max,
                quickValue: quickValue,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for an integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="quickValue">an optional common or predictive value that may be entered by the user simply pressing 'Enter' or 'Return'</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateTime DateTime
        (
            string prompt,
            bool required = false,
            string requiredIndicator = "*",
            DateTime? min = null,
            DateTime? max = null,
            DateTime? quickValue = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            // validation
            if (min.HasValue && max.HasValue && min > max)
                throw new ValidationException("min value may not exceed max value: " + min.Value.ToFlexStringUS() + " !< " + max.Value.ToFlexStringUS());

            return DateTime
            (
                prompt,
                required: required,
                requiredIndicator: requiredIndicator,
                quickValue: quickValue,
                validator: (dateValue) =>
                {
                    if (min.HasValue && dateValue < min)
                        throw new ArgumentOutOfRangeException(nameof(dateValue), "input (" + dateValue.ToFlexStringUS() + ") may not be less than " + min.Value.ToFlexStringUS());
                    if (max.HasValue && dateValue > max)
                        throw new ArgumentOutOfRangeException(nameof(dateValue), "input (" + dateValue.ToFlexStringUS() + ") may not exceed " + max.Value.ToFlexStringUS());
                },
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateTime? NDateTime
        (
            Action<DateTime> validator,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NDateTime
            (
                null,
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="validator">an optional validation routine to be run on the parsed input value</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateTime? NDateTime
        (
            string prompt,
            Action<DateTime> validator,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NValue<DateTime>
            (
                prompt,
                parser: (input) => Convert.ToDateTime(input),
                validator: validator,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateTime? NDateTime
        (
            DateTime? min = null,
            DateTime? max = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return NDateTime
            (
                null,
                min: min,
                max: max,
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for a nullable integer value, also accepts certain commands i.e. 'cancel', 'exit' by default.
        /// Always wrap in try block to catch <see cref="ConsoleNavigation.PromptCanceledException" />.
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="min">the minimum acceptable value, optional</param>
        /// <param name="max">the maximum acceptable value, optional</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="canCancel">whether typing 'cancel' at the prompt can cancel the prompt, default is <c>true</c></param>
        /// <param name="canExitApp">whether typing 'exit' at the prompt can exit the application, default is <c>true</c></param>
        /// <returns>The value entered by the user</returns>
        /// <exception cref="ConsoleNavigation.PromptCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateTime? NDateTime
        (
            string prompt,
            DateTime? min = null,
            DateTime? max = null,
            int padBefore = 0,
            int padAfter = 0,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            // validation
            if (min.HasValue && max.HasValue && min > max)
                throw new ValidationException("min value may not exceed max value: " + min.Value.ToFlexStringUS() + " !< " + max.Value.ToFlexStringUS());

            return NDateTime
            (
                prompt,
                validator: (dateValue) =>
                {
                    if (min.HasValue && dateValue < min)
                        throw new ArgumentOutOfRangeException(nameof(dateValue), "input (" + dateValue.ToFlexStringUS() + ") may not be less than " + min.Value.ToFlexStringUS());
                    if (max.HasValue && dateValue > max)
                        throw new ArgumentOutOfRangeException(nameof(dateValue), "input (" + dateValue.ToFlexStringUS() + ") may not exceed " + max.Value.ToFlexStringUS());
                },
                padBefore: padBefore,
                padAfter: padAfter,
                canCancel: canCancel,
                canExitApp: canExitApp
            );
        }

        /// <summary>
        /// Prompts for <c>enum</c> value, also accepts certain commands i.e. 'cancel', 'exit' by default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title">The list title</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="except">Any <c>enum</c> values you want to omit from the list of choices</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <returns>The value entered by the user</returns>
        public static T Enum<T>
        (
            Title? title = null, 
            bool required = false, 
            string requiredIndicator = "*", 
            IList<T> except = null, 
            int padBefore = 0, 
            int padAfter = 0
        ) where T : struct, Enum
        {
            return NEnum<T>
            (
                title: title, 
                required: required, 
                requiredIndicator: requiredIndicator, 
                except: except, 
                padBefore: padBefore, 
                padAfter: padAfter
            ) ?? default;
        }

        /// <summary>
        /// Prompts for <c>enum</c> value, also accepts certain commands i.e. 'cancel', 'exit' by default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title">The list title</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="except">Any <c>enum</c> values you want to omit from the list of choices</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <returns>The value entered by the user or <c>null</c></returns>
        public static T? NEnum<T>
        (
            Title? title = null, 
            bool required = false, 
            string requiredIndicator = "*", 
            IList<T> except = null, 
            int padBefore = 0,
            int padAfter = 0
        ) where T : struct, Enum
        {
            T? choice = null;
            var enumValues = System.Enum.GetValues(typeof(T))
                .Cast<T?>()
                .Where(e => !e.In(except?.Select(_e => (T?)_e)));

            // create / configure custom menu
            var customItems = enumValues
                .Select(e => RoutineX.BuildCustomRoutine(e.ToString(), () => choice = e, ((int)(object)e).ToString()) as MenuObject)
                .ToList();
            if (title == null)
            {
                title = typeof(T).Name;
            }
            if (requiredIndicator == null)
            {
                requiredIndicator = required ? "*" : "";
            }
            if (!required)
            {
                customItems.Add(RoutineX.BuildCustomRoutine("Press 'Enter' to skip...", () => { }));
            }

            // prompt custom menu for choice selection
            MenuSelection<T?> menuSelection = null;
            while (true)
            {
                menuSelection = Menu(null as IList<T?>, customItemsToAppend: customItems, title: requiredIndicator + title, padBefore: padBefore, padAfter: padAfter);
                if (menuSelection != null)
                {
                    break;
                }
                RenderX.Alert("A selection is required.");
            }

            return choice;
        }

        /// <summary>
        /// Prompt a user to choose from a collection of items
        /// </summary>
        /// <typeparam name="T">type of item</typeparam>
        /// <param name="collection">a collection</param>
        /// <param name="title">a collection title</param>
        /// <param name="indexPolicy">whether to display an index and whether it is 0-based</param>
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="choicePrompt">the text to render at the prompt</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying collection items</param>
        /// <param name="padBefore">the number of new lines to render before the collection</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        public static T List<T>
        (
            IEnumerable<T> collection,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            string requiredIndicator = "*",
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
                requiredIndicator: requiredIndicator, 
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
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="choicePrompt">the text to render at the prompt</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="padBefore">the number of new lines to render before the list</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <returns>The value selected by the user</returns>
        /// <remarks><seealso cref="Enum"/></remarks>
        public static T List<T>
        (
            IList<T> list,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            string requiredIndicator = "*",
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
                requiredIndicator: requiredIndicator,
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
        /// <param name="required"><c>true</c> to force non-blank input</param>
        /// <param name="requiredIndicator">input prompt decoration for required inputs</param>
        /// <param name="choicePrompt">the text to render at the prompt</param>
        /// <param name="renderer">an alternative to <c>object.ToString()</c> for displaying list items</param>
        /// <param name="padBefore">the number of new lines to render before the list</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
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
            string requiredIndicator = "*",
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

            if (requiredIndicator == null)
            {
                requiredIndicator = required ? "*" : "";
            }

            RenderX.List(list, title: title, indexPolicy: indexPolicy, renderer: renderer, requiredIndicator: requiredIndicator, padBefore: padBefore, padAfter: 0);

            if (list != null && list.Any())
            {
                // required indicator would have been added to the title (if any), if so blank it out here
                if (title != null)
                {
                    requiredIndicator = "";
                }
                var index = Int(choicePrompt, required: required, min: indexPolicy == ListIndexPolicy.DisplayZeroBased ? 0 : 1, max: list.Count + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? -1 : 0), requiredIndicator: requiredIndicator);
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
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
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
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
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
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="cancelable">whether to allow escape to throw a <see cref="ConsoleNavigation.CancelPasswordException"/>, remember to catch the exception if <c>true</c>!</param>
        /// <exception cref="ConsoleNavigation.CancelPasswordException"></exception>
        public static string Password
        (
            int padBefore = 0,
            int padAfter = 0,
            bool cancelable = false
        )
        {
            return Password(null, padBefore: padBefore, padAfter: padAfter);
        }

        /// <summary>
        /// Prompts a user to enter a password (the password is hidden)
        /// </summary>
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
        /// <param name="cancelable">whether to allow escape to throw a <see cref="ConsoleNavigation.CancelPasswordException"/>, remember to catch the exception if <c>true</c>!</param>
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
            RenderX.Prompt(prompt, required: false, requiredIndicator: null);

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
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
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
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
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
            RenderX.Prompt(prompt, required: false, requiredIndicator: null);

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
        /// <param name="prompt">the text to render at the prompt</param>
        /// <param name="padBefore">the number of new lines to render before the prompt</param>
        /// <param name="padAfter">the number of new lines to render after the prompt</param>
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
    }
}
