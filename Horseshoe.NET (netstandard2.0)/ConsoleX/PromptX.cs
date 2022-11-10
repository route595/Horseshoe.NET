using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text.TextClean;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.ConsoleX
{
    public static class PromptX
    {
        public static string Input
        (
            bool required = false, 
            int padBefore = 0, 
            int padAfter = 0, 
            string predictiveText = null, 
            string requiredIndicator = null,
            bool autoTrim = true,
            bool canCancel = true,
            bool canExitApp = true
        )
        {
            return Input
            (
                null, 
                required: required, 
                padBefore: padBefore, 
                padAfter: padAfter, 
                predictiveText: predictiveText, 
                requiredIndicator: requiredIndicator,
                autoTrim: autoTrim,
                canCancel: canCancel, 
                canExitApp: canExitApp
            );
        }

        public static string Input
        (
            string prompt, 
            bool required = false, 
            int padBefore = 0, 
            int padAfter = 0, 
            string predictiveText = null,
            string requiredIndicator = null,
            bool autoTrim = true,
            bool canCancel = true, 
            bool canExitApp = true
        )
        {
            RenderX.Pad(padBefore);
            if (!string.IsNullOrWhiteSpace(predictiveText))
            {
                Console.WriteLine("(Press 'Enter' to use \"" + predictiveText + "\")");
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
                    if (predictiveText != null)
                    {
                        input = predictiveText;
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

        public static string InputVerbatim(int padBefore = 0, int padAfter = 0)
        {
            return InputVerbatim(null, padBefore: padBefore, padAfter: padAfter);
        }

        public static string InputVerbatim(string prompt, int padBefore = 0, int padAfter = 0)
        {
            RenderX.Pad(padBefore);
            RenderX.Prompt(prompt, required: false);
            string input = Console.ReadLine();
            RenderX.Pad(padAfter);
            return input;
        }

        public static string NInput
        (
            int padBefore = 0, 
            int padAfter = 0,
            string predictiveText = null, 
            bool canCancel = true, 
            bool canExitApp = true
        )
        {
            return Zap.String
            (
                Input
                (
                    required: false, 
                    padBefore: padBefore, 
                    padAfter: padAfter,
                    predictiveText: predictiveText, 
                    canCancel: canCancel, 
                    canExitApp: canExitApp
               )
            );
        }

        public static string NInput
        (
            string prompt, 
            int padBefore = 0, 
            int padAfter = 0,
            string predictiveText = null, 
            bool canCancel = true, 
            bool canExitApp = true
        )
        {
            return Zap.String
            (
                Input
                (
                    prompt, 
                    required: false, 
                    padBefore: padBefore, 
                    padAfter: padAfter,
                    predictiveText: predictiveText, 
                    canCancel: canCancel, 
                    canExitApp: canExitApp
                )
            );
        }

        public static E Value<E>
        (
            bool required = false, 
            E valueIfOmitted = default, 
            int padBefore = 0, 
            int padAfter = 0, 
            string predictiveText = null,
            bool canCancel = true,
            string requiredIndicator = null
        ) where E : struct
        {
            return Value
            (
                null, 
                null, 
                required: required, 
                valueIfOmitted: valueIfOmitted, 
                padBefore: padBefore, 
                padAfter: padAfter, 
                predictiveText: predictiveText, 
                canCancel: canCancel,
                requiredIndicator: requiredIndicator
            );
        }

        public static E Value<E>
        (
            string prompt, 
            bool required = false, 
            E valueIfOmitted = default, 
            int padBefore = 0, 
            int padAfter = 0,
            string predictiveText = null, 
            bool canCancel = true,
            string requiredIndicator = null
        ) where E : struct
        {
            return Value
            (
                prompt, 
                null, 
                required: required, 
                valueIfOmitted: valueIfOmitted, 
                padBefore: padBefore, 
                padAfter: padAfter, 
                predictiveText: predictiveText, 
                canCancel: canCancel,
                requiredIndicator: requiredIndicator
            );
        }

        public static E Value<E>
        (
            Func<string, object> converter, 
            bool required = false, 
            E valueIfOmitted = default, 
            int padBefore = 0, 
            int padAfter = 0,
            string predictiveText = null, 
            bool canCancel = true,
            string requiredIndicator = null
        ) where E : struct
        {
            return Value
            (
                null, 
                converter, 
                required: required, 
                valueIfOmitted: valueIfOmitted, 
                padBefore: padBefore, 
                padAfter: padAfter, 
                predictiveText: predictiveText,
                canCancel: canCancel,
                requiredIndicator: requiredIndicator
            );
        }

        public static E Value<E>
        (
            string prompt, 
            Func<string, object> converter, 
            bool required = false, 
            E valueIfOmitted = default, 
            int padBefore = 0, 
            int padAfter = 0,
            string predictiveText = null, 
            bool canCancel = true,
            string requiredIndicator = null
        ) where E : struct
        {
            while (true)
            {
                string input = Input
                (
                    prompt, 
                    required: required, 
                    padBefore: padBefore, 
                    padAfter: padAfter, 
                    predictiveText: predictiveText,
                    canCancel: canCancel,
                    requiredIndicator: requiredIndicator
                );
                if (input.Length == 0)
                {
                    return valueIfOmitted;
                }
                try
                {
                    return ConvertString.To<E>(input, converter: converter);
                }
                catch (Exception ex)
                {
                    RenderX.Alert(TextClean.CombineWhitespace(ex.Message));
                    if (ex is ConversionException cex && cex.IsConverterNotSupplied)
                    {
                        ConsoleNavigation.CancelPrompt(cex.Message);
                    }
                }
            }
        }

        public static E? NValue<E>
        (
            E? valueIfOmitted = null, 
            int padBefore = 0, 
            int padAfter = 0,
            string predictiveText = null, 
            bool canCancel = true,
            Func<string, object> converter = null
        ) where E : struct
        {
            return NValue
            (
                null, 
                valueIfOmitted: valueIfOmitted, 
                padBefore: padBefore, 
                padAfter: padAfter, 
                predictiveText: predictiveText,
                canCancel: canCancel,
                converter: converter
            );
        }

        public static E? NValue<E>
        (
            string prompt, 
            E? valueIfOmitted = null, 
            int padBefore = 0, 
            int padAfter = 0,
            string predictiveText = null, 
            bool canCancel = true,
            Func<string, object> converter = null
        ) where E : struct
        {
            while (true)
            {
                string input = NInput
                (
                    prompt, 
                    padBefore: padBefore, 
                    padAfter: padAfter,
                    predictiveText: predictiveText,
                    canCancel: canCancel
                );
                if (input == null)
                {
                    return valueIfOmitted;
                }
                try
                {
                    return ConvertString.To<E>(input, converter: converter);
                }
                catch (Exception ex)
                {
                    RenderX.Alert(TextClean.CombineWhitespace(ex.Message));
                    if (ex is ConversionException cex && cex.IsConverterNotSupplied)
                    {
                        ConsoleNavigation.CancelPrompt(cex.Message);
                    }
                }
            }
        }

        public static E Reference<E>
        (
            Func<string, object> converter,
            bool required = false, 
            E valueIfOmitted = null, 
            int padBefore = 0, 
            int padAfter = 0, 
            string predictiveText = null, 
            string requiredIndicator = null
        ) where E : class
        {
            return Reference
            (
                null, 
                converter, 
                required: required, 
                valueIfOmitted: valueIfOmitted, 
                padBefore: padBefore, 
                padAfter: padAfter, 
                predictiveText: predictiveText, 
                requiredIndicator: requiredIndicator
            );
        }

        public static E Reference<E>
        (
            string prompt, 
            Func<string, object> converter, 
            bool required = false, 
            E valueIfOmitted = null, 
            int padBefore = 0, 
            int padAfter = 0, 
            string predictiveText = null, 
            string requiredIndicator = null
        ) where E : class
        {
            while (true)
            {
                string input = Input
                (
                    prompt, 
                    required: required, 
                    padBefore: padBefore, 
                    padAfter: padAfter, 
                    predictiveText: predictiveText, 
                    requiredIndicator: requiredIndicator
                );
                if (input.Length == 0)
                {
                    return valueIfOmitted;
                }
                try
                {
                    return (E)converter.Invoke(input);
                }
                catch (Exception ex)
                {
                    RenderX.Alert(TextClean.CombineWhitespace(ex.Message));
                    if (ex is ConversionException cex && cex.IsConverterNotSupplied)
                    {
                        ConsoleNavigation.CancelPrompt(cex.Message);
                    }
                }
            }
        }

        public static bool Bool
        (
            bool required = false, 
            bool valueIfOmitted = false, 
            string trueValues = "y|yes|t|true|1", 
            string falseValues = "n|no|f|false|0", 
            bool ignoreCase = true, 
            int padBefore = 0, 
            int padAfter = 0, 
            string requiredIndicator = null
        )
        {
            return Bool
            (
                null,
                required: required,
                valueIfOmitted: valueIfOmitted,
                trueValues: trueValues,
                falseValues: falseValues,
                ignoreCase: ignoreCase,
                padBefore: padBefore,
                padAfter: padAfter,
                requiredIndicator: requiredIndicator
            );
        }

        public static bool Bool
        (
            string prompt, 
            bool required = false, 
            bool valueIfOmitted = false, 
            string trueValues = "y|yes|t|true|1", 
            string falseValues = "n|no|f|false|0", 
            bool ignoreCase = true, 
            int padBefore = 0, 
            int padAfter = 0, 
            string requiredIndicator = null
        )
        {
            return Value<bool>
            (
                prompt,
                required: required,
                valueIfOmitted: valueIfOmitted,
                padBefore: padBefore,
                padAfter: padAfter,
                requiredIndicator: requiredIndicator,
                converter: (input) => Zap.NBool(input, trueValues: trueValues, falseValues: falseValues, ignoreCase: ignoreCase)
            );
        }

        public static bool? NBool
        (
            bool? valueIfOmitted = null, 
            string trueValues = "y|yes|t|true|1",
            string falseValues = "n|no|f|false|0", 
            bool ignoreCase = false, 
            int padBefore = 0, 
            int padAfter = 0
        )
        {
            return NBool
            (
                null,
                valueIfOmitted: valueIfOmitted,
                trueValues: trueValues,
                falseValues: falseValues,
                ignoreCase: ignoreCase,
                padBefore: padBefore,
                padAfter: padAfter
            );
        }

        public static bool? NBool
        (
            string prompt, 
            bool? valueIfOmitted = null, 
            string trueValues = "y|yes|t|true|1", 
            string falseValues = "n|no|f|false|0", 
            bool ignoreCase = false, 
            int padBefore = 0,
            int padAfter = 0
        )
        {
            return NValue<bool>
            (
                prompt,
                valueIfOmitted: valueIfOmitted,
                padBefore: padBefore,
                padAfter: padAfter,
                converter: (input) => Zap.NBool(input, trueValues: trueValues, falseValues: falseValues, ignoreCase: ignoreCase)
            );
        }

        public static T Numeric<T>
        (
            bool required = false,
            T valueIfOmitted = default,
            T? min = null,
            T? max = null,
            int padBefore = 0,
            int padAfter = 0,
            string predictiveText = null,
            bool canCancel = true,
            string requiredIndicator = null
        ) where T :
            struct,
            IComparable,
            IComparable<T>,
            IConvertible,
            IEquatable<T>,
            IFormattable
        {
            return Numeric<T>
            (
                null,
                required: required,
                valueIfOmitted: valueIfOmitted,
                min: min,
                max: max,
                padBefore: padBefore,
                padAfter: padAfter,
                predictiveText: predictiveText,
                canCancel: canCancel,
                requiredIndicator: requiredIndicator
            );
        }

        public static T Numeric<T>
        (
            string prompt,
            bool required = false,
            T valueIfOmitted = default,
            T? min = null,
            T? max = null,
            int padBefore = 0,
            int padAfter = 0,
            string predictiveText = null,
            bool canCancel = true,
            string requiredIndicator = null
        ) where T :
            struct,
            IComparable,
            IComparable<T>,
            IConvertible,
            IEquatable<T>,
            IFormattable
        {
            var result = Value<T>
            (
                prompt,
                required: required,
                valueIfOmitted: valueIfOmitted,
                padBefore: padBefore,
                padAfter: padAfter,
                predictiveText: predictiveText,
                canCancel: canCancel,
                requiredIndicator: requiredIndicator
            );

            if (NumberUtil.LessThan(result, min))
            {
                throw new ArgumentOutOfRangeException(nameof(result), "Invalid input - value is less than range min (" + min + ").");
            }
            if (NumberUtil.GreaterThan(result, max))
            {
                throw new ArgumentOutOfRangeException(nameof(result), "Invalid input - value exceeds range max (" + max + ").");
            }
            return result;
        }

        public static T? NNumeric<T>
        (
            T? valueIfOmitted = null,
            T? min = null,
            T? max = null,
            int padBefore = 0,
            int padAfter = 0,
            string predictiveText = null,
            bool canCancel = false
        ) where T :
            struct,
            IComparable,
            IComparable<T>,
            IConvertible,
            IEquatable<T>,
            IFormattable
        {
            return NNumeric<T>
            (
                null,
                valueIfOmitted: valueIfOmitted,
                min: min,
                max: max,
                padBefore: padBefore,
                padAfter: padAfter,
                predictiveText: predictiveText,
                canCancel: canCancel
            );
        }

        public static T? NNumeric<T>
        (
            string prompt,
            T? valueIfOmitted = null,
            T? min = null,
            T? max = null,
            int padBefore = 0,
            int padAfter = 0,
            string predictiveText = null,
            bool canCancel = false
        ) where T :
            struct,
            IComparable,
            IComparable<T>,
            IConvertible,
            IEquatable<T>,
            IFormattable
        {
            var result = NValue<T>
            (
                prompt,
                valueIfOmitted: valueIfOmitted,
                padBefore: padBefore,
                padAfter: padAfter,
                predictiveText: predictiveText,
                canCancel: canCancel
            );

            if (NumberUtil.LessThan(result, min))
            {
                throw new ArgumentOutOfRangeException(nameof(result), "Invalid input - value is less than range min (" + min + ").");
            }
            if (NumberUtil.GreaterThan(result, max))
            {
                throw new ArgumentOutOfRangeException(nameof(result), "Invalid input - value exceeds range max (" + max + ").");
            }
            return result;
        }

        public static E Enum<E>(Title? title = null, bool required = false, string requiredIndicator = null, IList<E> except = null, int padBefore = 0, int padAfter = 0) where E : struct, Enum
        {
            return NEnum<E>(title: title, required: required, requiredIndicator: requiredIndicator, except: except, padBefore: padBefore, padAfter: padAfter) ?? default;
        }

        public static E? NEnum<E>(Title? title = null, bool required = false, string requiredIndicator = null, IList<E> except = null, int padBefore = 0, int padAfter = 0) where E : struct, Enum
        {
            E? choice = null;
            var enumValues = System.Enum.GetValues(typeof(E))
                .Cast<E?>()
                .Where(e => !e.In(except?.Select(_e => (E?)_e)));

            // create / configure custom menu
            var customItems = enumValues
                .Select(e => RoutineX.BuildCustomRoutine(e.ToString(), () => choice = e, ((int)(object)e).ToString()) as MenuObject)
                .ToList();
            if (title == null)
            {
                title = typeof(E).Name;
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
            MenuSelection<E?> menuSelection = null;
            while (true)
            {
                menuSelection = Menu(null as IList<E?>, customItemsToAppend: customItems, title: requiredIndicator + title, padBefore: padBefore, padAfter: padAfter);
                if (menuSelection != null)
                {
                    break;
                }
                RenderX.Alert("A selection is required.");
            }

            return choice;
        }

        public static E List<E>
        (
            IEnumerable<E> collection,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            string requiredIndicator = null,
            string choicePrompt = ">",
            Func<E, string> renderer = null,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            return List(collection?.ToList(), title: title, indexPolicy: indexPolicy, required: required, requiredIndicator: requiredIndicator, choicePrompt: choicePrompt, renderer: renderer, padBefore: padBefore, padAfter: padAfter);
        }

        public static E List<E>
        (
            IList<E> list,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            string choicePrompt = null,
            Func<E, string> renderer = null,
            int padBefore = 0,
            int padAfter = 0,
            string requiredIndicator = null
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
                padAfter: padAfter,
                requiredIndicator: requiredIndicator
            );
        }

        public static E List<E>
        (
            IList<E> list,
            out int selectedIndex,
            Title? title = null,
            ListIndexPolicy indexPolicy = ListIndexPolicy.DisplayOneBased,
            bool required = false,
            string choicePrompt = null,
            Func<E, string> renderer = null,
            int padBefore = 0,
            int padAfter = 0,
            string requiredIndicator = null
        )
        {
            if (indexPolicy != ListIndexPolicy.DisplayZeroBased && indexPolicy != ListIndexPolicy.DisplayOneBased)
            {
                throw new ValidationException("Invalid indexPolicy: " + indexPolicy);
            }

            E choice;

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
                var index = Numeric<int>(choicePrompt, required: required, min: indexPolicy == ListIndexPolicy.DisplayZeroBased ? 0 : 1, max: list.Count + (indexPolicy == ListIndexPolicy.DisplayZeroBased ? -1 : 0), requiredIndicator: requiredIndicator);
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

        public static string Password
        (
            int padBefore = 0,
            int padAfter = 0
        )
        {
            return Password(null, padBefore: padBefore, padAfter: padAfter);
        }

        public static string Password
        (
            string prompt,
            int padBefore = 0,
            int padAfter = 0
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
                    Console.WriteLine();
                    RenderX.Pad(padAfter);
                    ConsoleNavigation.CancelPassword();
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

        public static SecureString PasswordSecure
        (
            int padBefore = 0,
            int padAfter = 0
        )
        {
            return PasswordSecure(null, padBefore: padBefore, padAfter: padAfter);
        }

        public static SecureString PasswordSecure
        (
            string prompt,
            int padBefore = 0,
            int padAfter = 0
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
                    Console.WriteLine();
                    RenderX.Pad(padAfter);
                    ConsoleNavigation.CancelPassword();
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

        public static MenuSelection<E> Menu<E>
        (
            IEnumerable<E> menuItems,
            IList<MenuObject> customItemsToPrepend = null,
            IList<MenuObject> customItemsToAppend = null,
            Func<E, string> renderer = null,
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
            Action<MenuSelection<E>> onMenuSelection = null,
            Action<RoutineX> onRoutineAutoRunComplete = null,
            Action<RoutineX, Exception> onRoutineAutoRunError = null
        )
        {
            var renderMessages = new RenderMessages();
            RenderX.Menu
            (
                menuItems?.ToList(),
                customItemsToPrepend: customItemsToPrepend,
                customItemsToAppend: customItemsToAppend,
                columns: columns,
                configureTextGrid: configureTextGrid,
                renderer: renderer,
                renderMessages: renderMessages,
                padBefore: padBefore,
                title: title,
                padAfter: padAfter
            );

            MenuSelection<E> menuSelection = null;

            while (!renderMessages.IsNotSelectable)
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
                    menuSelection = new MenuSelection<E> { CustomMenuItem = selectedCustomMenuItem };
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
                        menuSelection = new MenuSelection<E>
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
                        var _menuItems = new List<E>(menuItems);
                        var selectedIndices = MenuSelection.ParseMultipleIndexes(input, _menuItems.Count, out bool all);   // 1-based indices
                        var dict = new Dictionary<int, E>();
                        foreach (var selIndex in selectedIndices)
                        {
                            dict.Add(selIndex, _menuItems[selIndex - 1]);
                        }
                        menuSelection = new MenuSelection<E>
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
                    menuSelection = new MenuSelection<E> { ArbitraryInput = _input };
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
                    catch (Exception)
                    {
                        autoRunComplete = false;
                        throw;
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
