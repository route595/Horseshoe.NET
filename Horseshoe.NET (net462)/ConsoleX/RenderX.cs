using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextGrid;
using Horseshoe.NET.Text.TextClean;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A collection of methods for rendering content to the console
    /// </summary>
    public static class RenderX
    {
        /// <summary>
        /// The current with of the console window (in characters) minus a padding value of 2
        /// </summary>
        public static int ConsoleWidth => Console.WindowWidth - 2;

        private static string requiredIndicator;

        /// <summary>
        /// Gets or sets what marks required fields, default is <c>"*"</c>.
        /// </summary>
        public static string RequiredIndicator { get => requiredIndicator ?? "*"; set => requiredIndicator = value; }

        /// <summary>
        /// Global exception rendering preferences
        /// </summary>
        public static ExceptionRendering ExceptionRendering { get; } = new ExceptionRendering();

        /// <summary>
        /// Render a large banner and welcome message, typically at program start
        /// </summary>
        /// <param name="message">A <c>string</c> or <c>string[]</c> to display in the welcome banner</param>
        /// <param name="padBefore">the number of new lines to render before the banner</param>
        /// <param name="padAfter">the number of new lines to render after the banner</param>
        public static void Welcome(StringValues message, int padBefore = 1, int padAfter = 2)
        {
            var list = new List<string>();
            foreach (var line in message)
            {
                list.AddRange(line.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'));
            }
            Pad(padBefore);
            Console.WriteLine(" »╔" + "".PadLeft(ConsoleWidth - 4, '═') + "╗«");
            Console.WriteLine(" »║" + "".PadLeft(ConsoleWidth - 4, ' ') + "║«");
            foreach (var line in list)
            {
                Console.WriteLine(" »║" + TextUtil.Pad(line, ConsoleWidth - 4, position: HorizontalPosition.Center) + "║«");
            }
            Console.WriteLine(" »║" + "".PadLeft(ConsoleWidth - 4, ' ') + "║«");
            Console.WriteLine(" »╚" + "".PadLeft(ConsoleWidth - 4, '═') + "╝«");
            Pad(padAfter);
        }

        /// <summary>
        /// Render a small banner such as those used in displaying <c>Routine</c> titles
        /// </summary>
        /// <param name="title"><c>Title</c> or text</param>
        /// <param name="padBefore">the number of new lines to render before the banner</param>
        /// <param name="padAfter">the number of new lines to render after the banner</param>
        public static void RoutineTitle(Title title, int padBefore = 1, int padAfter = 1)
        {
            Pad(padBefore);
            Console.WriteLine(" ╔" + "".PadLeft(ConsoleWidth - 2, '═') + "╗");
            Console.WriteLine(" ║" + TextUtil.Pad(title.ToString(), ConsoleWidth - 2, position: HorizontalPosition.Center) + '║');
            Console.WriteLine(" ╚" + "".PadLeft(ConsoleWidth - 2, '═') + "╝");
            Pad(padAfter);
        }

        /// <summary>
        /// Renders the specified number of new lines
        /// </summary>
        /// <param name="pad">the number of new lines to render, default is 1</param>
        /// <param name="altText">an optional, alternate string to render on each new line in the pad</param>
        public static void Pad(int pad = 1, string altText = null)
        {
            for (int i = 0; i < pad; i++)
            {
                if (altText != null)
                    Console.WriteLine(altText);
                else
                    Console.WriteLine();
            }
        }

        /// <summary>
        /// Render a title underlined with dashes (-)
        /// </summary>
        /// <param name="title"><c>Title</c> or text</param>
        /// <param name="padBefore">the number of new lines to render before the title</param>
        /// <param name="padAfter">the number of new lines to render after the title</param>
        public static void ListTitle(Title? title, int padBefore = 0, int padAfter = 0)
        {
            Pad(padBefore);
            if (title.HasValue)
            {
                Console.WriteLine(title);
                Console.WriteLine(new string('-', title.Value.Text.Length));
            }
            Pad(padAfter);
        }

        /// <summary>
        /// Render a collection of items to the console with or without indexes based on <c>indexStyle</c>
        /// </summary>
        /// <typeparam name="T">type of item</typeparam>
        /// <param name="list">a collection of items</param>
        /// <param name="title"><c>Title</c> or text</param>
        /// <param name="indexStyle">whether to display an index and whether it is 0-based</param>
        /// <param name="renderer">alternative to <c>ToString()</c></param>
        /// <param name="listConfigurator">internal mechanism for preventing prompt deadlock</param>
        /// <param name="columns">the number of columns in which to render the collection</param>
        /// <param name="padBefore">the number of new lines to render before the collection</param>
        /// <param name="padAfter">the number of new lines to render after the collection</param>
        /// <param name="configureTextGrid">exposes a reference to the underlying <c>TextGrid</c> for further configuration</param>
        public static void List<T>
        (
            IEnumerable<T> list,
            Title? title = null,
            ListIndexStyle indexStyle = default,
            Func<T, string> renderer = null,
            MenuAndListRealtimeConfigurator listConfigurator = null,
            int columns = 1,
            int padBefore = 0,
            int padAfter = 0,
            Action<TextGrid> configureTextGrid = null
        )
        {
            List(CollectionUtil.ToList(list, optimize: Optimization.ReuseCollection), title: title, indexStyle: indexStyle, renderer: renderer, listConfigurator: listConfigurator, columns: columns, padBefore: padBefore, padAfter: padAfter, configureTextGrid: configureTextGrid);
        }

        /// <summary>
        /// Render a list of items to the console with or without indexes based on <c>indexStyle</c>
        /// </summary>
        /// <typeparam name="T">type of item</typeparam>
        /// <param name="list">a list of items</param>
        /// <param name="title"><c>Title</c> or text</param>
        /// <param name="indexStyle">whether to display an index and whether it is 0-based</param>
        /// <param name="renderer">alternative to <c>ToString()</c></param>
        /// <param name="listConfigurator">internal mechanism for preventing prompt deadlock</param>
        /// <param name="columns">The number of columns with which to render the list.</param>
        /// <param name="padBefore">the number of new lines to render before the list</param>
        /// <param name="padAfter">the number of new lines to render after the list</param>
        /// <param name="configureTextGrid">exposes a reference to the underlying <c>TextGrid</c> for further configuration</param>
        public static void List<T>
        (
            IList<T> list,
            Title? title = null,
            ListIndexStyle indexStyle = default,
            Func<T, string> renderer = null,
            MenuAndListRealtimeConfigurator listConfigurator = null,
            int columns = 1,
            int padBefore = 0,
            int padAfter = 0,
            Action<TextGrid> configureTextGrid = null
        )
        {
            Pad(padBefore);
            ListTitle(title: title, padBefore: 0, padAfter: 0);
            if (list == null)
            {
                Console.WriteLine("[null list]");
                listConfigurator?.SetNotSelectable();
            }
            else if (!list.Any())
            {
                Console.WriteLine("[empty list]");
                listConfigurator?.SetNotSelectable();
            }
            else
            {
                var count = list.Count();
                var strb = new StringBuilder();
                var renderedList = new List<string>();
                var indexSize = (count + (indexStyle == ListIndexStyle.OneBased ? 1 : 0)).ToString().Length;

                string renderListItem(string index, T item)
                {
                    strb.Clear();
                    if (index != null)
                    {
                        strb.Append("[" + index.PadLeft(indexSize) + "] ");
                    }
                    strb.Append
                    (
                        renderer != null
                            ? renderer.Invoke(item)
                            : item?.ToString() ?? "[null]"
                    );
                    return strb.ToString();
                }

                // build intermediate list
                for (int i = 0; i < count; i++)
                {
                    string index = null;
                    switch (indexStyle)
                    {
                        case ListIndexStyle.ZeroBased:  // e.g.  [0] First Item
                            index = i.ToString();
                            break;
                        case ListIndexStyle.OneBased:   // e.g.  [1] First Item
                            index = (i + 1).ToString();
                            break;
                    }
                    renderedList.Add(renderListItem(index, list[i]));
                }

                // build and render text grid
                var textGrid = new TextGrid(renderedList, columns: columns);
                configureTextGrid?.Invoke(textGrid);
                Console.Write(textGrid.Render());
            }
            Pad(padAfter);
        }

        /// <summary>
        /// Render a list of items to the console with or without indexes based on <c>indexStyle</c>
        /// </summary>
        /// <typeparam name="T">type of item</typeparam>
        /// <param name="list">a list of items</param>
        /// <param name="title"><c>Title</c> or text</param>
        /// <param name="indexStyle">whether to display an index and whether it is 0-based</param>
        /// <param name="renderer">alternative to <c>ToString()</c></param>
        /// <param name="listConfigurator">internal mechanism for preventing prompt deadlock</param>
        /// <param name="columns">The number of columns with which to render the list.</param>
        /// <param name="padBefore">the number of new lines to render before the list</param>
        /// <param name="padAfter">the number of new lines to render after the list</param>
        /// <param name="configureTextGrid">exposes a reference to the underlying <c>TextGrid</c> for further configuration</param>
        /// <exception cref="ValidationException"></exception>
        public static void List<T>
        (
            IEnumerable<KeyValuePair<int, T>> list,
            Title? title = null,
            ListIndexStyle indexStyle = default,
            Func<T, string> renderer = null,
            MenuAndListRealtimeConfigurator listConfigurator = null,
            int columns = 1,
            int padBefore = 0,
            int padAfter = 0,
            Action<TextGrid> configureTextGrid = null
        )
        {
            if (indexStyle != ListIndexStyle.ZeroBased)
                throw new ValidationException("Invalid indexStyle: " + indexStyle);

            Pad(padBefore);
            ListTitle(title: title, padBefore: 0, padAfter: 0);
            if (list == null)
            {
                Console.WriteLine("[null list]");
                listConfigurator?.SetNotSelectable();
            }
            else if (!list.Any())
            {
                Console.WriteLine("[empty list]");
                listConfigurator?.SetNotSelectable();
            }
            else
            {
                var count = list.Count();
                var strb = new StringBuilder();
                var indexSize = list.Max(_kvp => _kvp.Key).ToString().Length;

                string renderListItem(string index, T item)
                {
                    strb.Clear();
                    if (index != null)
                    {
                        strb.Append("[" + index.PadLeft(indexSize) + "] ");
                    }
                    strb.Append
                    (
                        renderer != null
                            ? renderer.Invoke(item)
                            : item?.ToString() ?? "[null]"
                    );
                    return strb.ToString();
                }

                // build intermediate lists
                var renderedList = new List<string>();
                foreach (var kvp in list)
                {
                    renderedList.Add(renderListItem(kvp.Key.ToString(), kvp.Value));
                }

                // build and render text grid
                var textGrid = new TextGrid(renderedList, columns: columns);
                configureTextGrid?.Invoke(textGrid);
                Console.Write(textGrid.Render());
            }
            Pad(padAfter);
        }

        /// <summary>
        /// Render a menu to the console
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="menuItems">list of items including <c>MenuItem</c>s</param>
        /// <param name="title">a title to render above the menu</param>
        /// <param name="customItemsToPrepend">custom items to list before the regular menu items</param>
        /// <param name="customItemsToAppend">custom items to list after the regular menu items</param>
        /// <param name="columns">the number of columns in which to render the list</param>
        /// <param name="configureTextGrid">exposes a reference to the underlying <c>TextGrid</c> for further configuration</param>
        /// <param name="renderer">alternative to <c>ToString()</c></param>
        /// <param name="listConfigurator">internal mechanism for preventing prompt deadlock</param>
        /// <param name="padBefore">the number of new lines to render before the list</param>
        /// <param name="padAfter">the number of new lines to render after the list</param>
        public static void Menu<T>
        (
            IEnumerable<T> menuItems,
            Title? title = null,
            IEnumerable<MenuObject> customItemsToPrepend = null,
            IEnumerable<MenuObject> customItemsToAppend = null,
            int columns = 1,
            Action<TextGrid> configureTextGrid = null,
            Func<T, string> renderer = null,
            MenuAndListRealtimeConfigurator listConfigurator = null,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            Pad(padBefore);
            ListTitle(title: title, padBefore: 0, padAfter: 0);
            if (!CollectionUtil.ContainsAny(customItemsToPrepend) && !CollectionUtil.ContainsAny(customItemsToAppend))
            {
                if (menuItems == null)
                {
                    Console.WriteLine("[null menu]");
                    Pad(padAfter);
                    listConfigurator?.SetNotSelectable();
                    return;
                }
                else if (!menuItems.Any())
                {
                    Console.WriteLine("[empty menu]");
                    Pad(padAfter);
                    listConfigurator?.SetNotSelectable();
                    return;
                }
            }

            // build intermediate lists
            var commandList = new List<string>();
            var itemList = new List<string>();

            // step 1 of 2 -- source list 1 of 3 -- custom items to prepend
            if (customItemsToPrepend != null)
            {
                foreach (var item in customItemsToPrepend)
                {
                    if (item is RoutineX routine)
                    {
                        commandList.Add(Zap.String(routine.Command) ?? "<┘");
                        itemList.Add(routine.Text);
                    }
                    else
                    {
                        commandList.Add(null);
                        itemList.Add(item.Text);
                    }
                }
            }

            // step 1 of 2 -- source list 2 of 3 -- menu items
            if (menuItems != null)
            {
                var counter = 0;
                foreach (var item in menuItems)
                {
                    if (item is MenuHeader header)
                    {
                        commandList.Add(null);
                        itemList.Add(header.Text);
                    }
                    else if (item != null)
                    {
                        commandList.Add((++counter).ToString());
                        itemList.Add(renderer != null ? renderer.Invoke(item) : item.ToString());
                    }
                }
            }

            // step 1 of 2 -- source list 3 of 3 -- custom items to append
            if (customItemsToAppend != null)
            {
                foreach (var item in customItemsToAppend)
                {
                    if (item is RoutineX routine)
                    {
                        commandList.Add(Zap.String(routine.Command) ?? "<┘");
                        itemList.Add(routine.Text);
                    }
                    else
                    {
                        commandList.Add(null);
                        itemList.Add(item.Text);
                    }
                }
            }

            // step 2 of 2 -- combine commands and items, e.g. { "1", "First Menu Item" } -> "[1] First Menu Item"
            var commandSize = commandList.Max(s => s?.Length ?? 0);
            for (int i = 0; i < commandList.Count; i++)
            {
                itemList[i] = (commandList[i] != null
                    ? "[" + commandList[i].PadLeft(commandSize) + "] "
                    : "".PadLeft(commandSize + 3)) + itemList[i];
            }

            // build and render text grid
            var textGrid = new TextGrid(itemList, columns: columns);
            configureTextGrid?.Invoke(textGrid);
            Console.Write(textGrid.Render());
        }

        //private static Regex WordOrPhrasePattern { get; } = new Regex("[a-z0-9 ]", RegexOptions.IgnoreCase);
        //private static Regex AlphaNumericPattern { get; } = new Regex("[a-z0-9]", RegexOptions.IgnoreCase);

        /// <summary>
        /// Renders a prompt for user input e.g. menu selection, etc.
        /// </summary>
        /// <param name="promptType">Ways <c>ConsoleX</c> can render an input prompt</param>
        /// <param name="required">If <c>true</c>, adds an indicator to the prompt to suggest an input is required, default is <c>false</c>.</param>
        /// <param name="quickValue">A value to suggest to the user who then can press 'Enter' to input it.</param>
        public static void Prompt(PromptType promptType, bool required = false, object quickValue = null)
        {
            Prompt(null, promptType: promptType, required: required, quickValue: quickValue);
        }

        /// <summary>
        /// Renders a prompt for user input e.g. free text, etc.
        /// </summary>
        /// <param name="prompt">An input prompt.</param>
        /// <param name="promptType">Ways <c>ConsoleX</c> can render an input prompt</param>
        /// <param name="required">If <c>true</c>, adds an indicator to the prompt to suggest an input is required, default is <c>false</c>.</param>
        /// <param name="quickValue">A value to suggest to the user who then can press 'Enter' to input it.</param>
        /// <param name="defaultValue">The value to use in case of blank input.</param>
        public static void Prompt(string prompt, PromptType promptType = PromptType.Auto, bool required = false, object quickValue = null, object defaultValue = null)
        {
            switch (promptType)
            {
                case PromptType.Literal:
                    Console.Write(prompt ?? "");               // e.g. "  M:y mickEY moU$e   CluB "
                    return;
                case PromptType.MenuOrList:
                    Console.Write("> ");                       // e.g. "> "
                    return;
            }

            if (string.IsNullOrEmpty(prompt))
            {
                if (required)
                {
                    Console.Write("(required) ");              // e.g. "(required) > "
                }
                Console.Write("> ");                           // e.g. "> "
            }
            else
            {
                prompt = TextClean.Remove(prompt, '\t', '\r', '\n').TrimEnd(' ', ':');
                Console.Write(prompt);                         // e.g. "First name", "First name ", "First name:" or "First name: " => "First name: "
                if (required)
                {
                    Console.Write(" (required)");              // "First name"  => "First name (required): "
                }
                if (promptType == PromptType.Bool)
                {
                    if (defaultValue is bool boolValue)
                    {
                        if (boolValue)
                            Console.Write(" [Y/n]");            // "Yankees fan"  => "Yankees fan [Y/n]: "
                        else
                            Console.Write(" [y/N]");            // "Yankees fan"  => "Yankees fan [y/N]: "
                    }
                    else
                    {
                        Console.Write(" [y/n]");                // "Yankees fan"  => "Yankees fan [y/n]: "
                    }
                }
                else if (quickValue != null)
                {
                    Console.Write(" [" + quickValue + "]");    // "First name" => "First name [Jackie]: " or "First name (required) [Jackie]: "
                }
                Console.Write(": ");
            }
        }

        /// <summary>
        /// Render an alert message to the console
        /// </summary>
        /// <param name="message">a message</param>
        /// <param name="centered"><c>true</c> to center on screen</param>
        /// <param name="padBefore">the number of new lines to render before the alert</param>
        /// <param name="padAfter">the number of new lines to render after the alert</param>
        public static void Alert(string message, bool centered = false, int padBefore = 0, int padAfter = 0)
        {
            message = "** " + (message?.Trim() ?? "Alert!") + " **";
            if (centered)
            {
                message = TextUtil.Pad(message, ConsoleWidth, position: HorizontalPosition.Center);
            }
            Pad(padBefore);
            Console.WriteLine(message);
            Pad(padAfter);
        }

        /// <summary>
        /// Render an exception to the console
        /// </summary>
        /// <param name="ex">an exception </param>
        /// <param name="typeRendering">preferences for rendering the exception class name</param>
        /// <param name="includeDateTime"><c>true</c> to include date/time</param>
        /// <param name="includeMachineName"><c>true</c> to include machine name</param>
        /// <param name="includeStackTrace"><c>true</c> to include stack trace</param>
        /// <param name="indent">how deep to indent new lines</param>
        /// <param name="recursive"><c>true</c> to include all the inner exceptions recursively</param>
        /// <param name="padBefore">the number of new lines to render before the exception</param>
        /// <param name="padAfter">the number of new lines to render after the exception</param>
        /// <remarks><seealso cref="ExceptionRendering"/></remarks>
        public static void Exception
        (
            ExceptionInfo ex,
            ExceptionTypeRenderingPolicy? typeRendering = null,
            bool? includeDateTime = null,
            bool? includeMachineName = null,
            bool? includeStackTrace = null,
            int? indent = null,
            bool? recursive = null,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            Pad(padBefore);
            Console.WriteLine
            (
                ex.Render(
                    typeRendering: typeRendering ?? ExceptionRendering.TypeRendering, 
                    includeDateTime: includeDateTime ?? ExceptionRendering.IncludeDateTime, 
                    includeMachineName: includeMachineName ?? ExceptionRendering.IncludeMachineName, 
                    includeStackTrace: includeStackTrace ?? ExceptionRendering.IncludeStackTrace, 
                    indent: indent ?? ExceptionRendering.Indent, 
                    recursive: recursive ?? ExceptionRendering.Recursive
                )
            );
            Pad(padAfter);
        }
    }
}
