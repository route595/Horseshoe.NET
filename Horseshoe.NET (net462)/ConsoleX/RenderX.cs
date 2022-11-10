using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.ConsoleX
{
    public static class RenderX
    {
        public static int ConsoleWidth => Console.WindowWidth - 2;

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
                Console.WriteLine(" »║" + line.PadCenter(ConsoleWidth - 4) + "║«");
            }
            Console.WriteLine(" »║" + "".PadLeft(ConsoleWidth - 4, ' ') + "║«");
            Console.WriteLine(" »╚" + "".PadLeft(ConsoleWidth - 4, '═') + "╝«");
            Pad(padAfter);
        }

        public static void RoutineTitle(Title title, int padBefore = 1, int padAfter = 1)
        {
            Pad(padBefore);
            Console.WriteLine(" ╔" + "".PadLeft(ConsoleWidth - 2, '═') + "╗");
            Console.WriteLine(" ║" + title.ToString().PadCenter(ConsoleWidth - 2) + '║');
            Console.WriteLine(" ╚" + "".PadLeft(ConsoleWidth - 2, '═') + "╝");
            Pad(padAfter);
        }

        public static void Pad(int pad, string altText = null)
        {
            for (int i = 0; i < pad; i++)
            {
                Console.WriteLine(altText);
            }
        }

        public static void ListTitle(Title? title, string requiredIndicator = null, int padBefore = 0, int padAfter = 0)
        {
            Pad(padBefore);
            if (title.HasValue)
            {
                if (string.IsNullOrEmpty(requiredIndicator))
                {
                    Console.WriteLine(title);
                    Console.WriteLine(new string('-', title.Value.Text.Length));
                }
                else
                {
                    Console.WriteLine(requiredIndicator + title.Value.Text + (title.Value.Xtra ?? ""));
                    Console.WriteLine(new string('-', title.Value.Text.Length + requiredIndicator.Length));
                }
            }
            Pad(padAfter);
        }

        public static void List<E>
        (
            IEnumerable<E> list,
            Title? title = null,
            ListIndexPolicy indexPolicy = default,
            Func<E, string> renderer = null,
            int columns = 1,
            int padBefore = 0,
            int padAfter = 0,
            string requiredIndicator = null,
            Action<TextGrid> configureTextGrid = null
        )
        {
            List(list?.ToList(), title: title, indexPolicy: indexPolicy, renderer: renderer, columns: columns, padBefore: padBefore, padAfter: padAfter, requiredIndicator: requiredIndicator, configureTextGrid: configureTextGrid);
        }

        public static void List<E>
        (
            IList<E> list,
            Title? title = null,
            ListIndexPolicy indexPolicy = default,
            Func<E, string> renderer = null,
            RenderMessages renderMessages = null,
            int columns = 1,
            int padBefore = 0,
            int padAfter = 0,
            string requiredIndicator = null,
            Action<TextGrid> configureTextGrid = null
        )
        {
            Pad(padBefore);
            ListTitle(title: title, requiredIndicator: requiredIndicator, padBefore: 0, padAfter: 0);
            if (list == null)
            {
                Console.WriteLine("[null list]");
                renderMessages?.SetNotSelectable();
            }
            else if (!list.Any())
            {
                Console.WriteLine("[empty list]");
                renderMessages?.SetNotSelectable();
            }
            else
            {
                var count = list.Count();
                var strb = new StringBuilder();
                var renderedList = new List<string>();
                var indexSize = (count + (indexPolicy == ListIndexPolicy.DisplayOneBased ? 1 : 0)).ToString().Length;

                string renderListItem(string index, E item)
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
                    switch (indexPolicy)
                    {
                        case ListIndexPolicy.DisplayZeroBased:  // e.g.  [0] First Item
                            index = i.ToString();
                            break;
                        case ListIndexPolicy.DisplayOneBased:   // e.g.  [1] First Item
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

        public static void Menu<E>
        (
            IList<E> menuItems,
            Title? title = null,
            IList<MenuObject> customItemsToPrepend = null,
            IList<MenuObject> customItemsToAppend = null,
            int columns = 1,
            Action<TextGrid> configureTextGrid = null,
            Func<E, string> renderer = null,
            RenderMessages renderMessages = null,
            string requiredIndicator = null,
            int padBefore = 0,
            int padAfter = 0
        )
        {
            Pad(padBefore);
            ListTitle(title: title, requiredIndicator: requiredIndicator, padBefore: 0, padAfter: 0);
            if (!CollectionUtil.ContainsAny(customItemsToPrepend) && !CollectionUtil.ContainsAny(customItemsToAppend))
            {
                if (menuItems == null)
                {
                    Console.WriteLine("[null menu]");
                    Pad(padAfter);
                    renderMessages?.SetNotSelectable();
                    return;
                }
                else if (!menuItems.Any())
                {
                    Console.WriteLine("[empty menu]");
                    Pad(padAfter);
                    renderMessages?.SetNotSelectable();
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

        private static Regex WordOrPhrasePattern { get; } = new Regex("[a-z0-9 ]", RegexOptions.IgnoreCase);

        public static void Prompt(string prompt, bool required = false, string requiredIndicator = null)
        {
            if (required)
            {
                if (requiredIndicator == null)
                {
                    requiredIndicator = required ? "*" : "";
                }
            }
            else
            {
                requiredIndicator = "";
            }

            if (prompt == null)
            {
                prompt = ">" + requiredIndicator;
            }
            else if (WordOrPhrasePattern.IsMatch(prompt))
            {
                prompt = prompt + requiredIndicator + ":";
            }
            else
            {
                prompt += requiredIndicator;
            }
            Console.Write(prompt + " ");
        }

        public static void Alert(string message, bool centered = false, int padBefore = 0, int padAfter = 0)
        {
            message = "** " + (message ?? "Alert!") + " **";
            if (centered)
            {
                message = message.PadCenter(ConsoleWidth - 2);
            }
            Pad(padBefore);
            Console.WriteLine(message);
            Pad(padAfter);
        }

        public static ExceptionRendering ExceptionRendering { get; } = new ExceptionRendering();

        public static void Exception
        (
            Exception ex,
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
