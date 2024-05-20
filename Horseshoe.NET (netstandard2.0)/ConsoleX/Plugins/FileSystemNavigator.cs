using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Collections;
using Horseshoe.NET.ConsoleX.Plugins.FileSystemNavigatorTypes;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.ConsoleX.Plugins
{
    /// <summary>
    /// A <c>ConsoleX</c> plugin for traversing the filesystem with customizable actions enabling folder size calculation, recursive deletion, etc.
    /// </summary>
    public class FileSystemNavigator : RoutineX
    {
        /// <summary>
        /// <c>FileSystemNavigator</c> options
        /// </summary>
        public FileSystemNavigatorOptions Options { get; }

        /// <summary>
        /// The currently visited file or directory path
        /// </summary>
        public string SelectedPath { get; set; }
        private bool IsFinalSelection { get; set; }
        private string StartDirectory => NormalizeDirectoryPath(Options.StartDirectory ?? Environment.CurrentDirectory);
        private string DelayedAlert { get; set; }

        /// <summary>
        /// Action to perform when a path has been selected
        /// </summary>
        public Action<string> OnPathSelected { get; set; }

        /// <summary>
        /// Create a new instance of the <c>FileSystemNavigator</c> plugin for <c>ConsoleX</c>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="options"></param>
        /// <exception cref="ValidationException"></exception>
        public FileSystemNavigator(string text, FileSystemNavigatorOptions options = null) : base(text ?? "File Search")
        {
            if (options != null)
            {
                if (options.StartDirectory != null && !Directory.Exists(options.StartDirectory))
                {
                    throw new ValidationException("Start directory does not exist: " + options.StartDirectory);
                }
                Options = options;
            }
            else
            {
                Options = new FileSystemNavigatorOptions();
            }
        }

        /// <summary>
        /// Start crawling the filesystem beginning at the starting directory
        /// </summary>
        public override void Run()
        {
            IsFinalSelection = false;
            SelectedPath = StartDirectory;
            var firstRun = true;
            var isExited = false;
            while (!IsFinalSelection && !isExited)
            {
                if (!firstRun)
                {
                    OnBeforeNextIteration?.Invoke(this);
                }

                if (DelayedAlert != null)
                {
                    RenderX.Alert(DelayedAlert);
                    DelayedAlert = null;
                    PromptX.Continue();
                }

                try
                {
                    _RunImpl();
                }
                catch (ConsoleNavigation.BackoutRoutineException)
                {
                    isExited = true;
                }
                firstRun = false;
            }
            if (IsFinalSelection)
            {
                OnPathSelected?.Invoke(SelectedPath);
            }
            if (isExited)
            {
                OnRoutineExiting?.Invoke(this);
            }
            OnRoutineEnded?.Invoke(this);
        }

        void _RunImpl()
        {
            var list = new List<MenuObject>();
            var currentAction = "Start impl, SelectedPath?";
            if (SelectedPath == null)
                return;
            try
            {
                // list subdirectories
                currentAction = "Listing subdirectories of '" + SelectedPath + "'";
                var dirs = Directory.GetDirectories(SelectedPath)
                    .Select(d => new FSDirectory(d, d.Substring(SelectedPath.Length + 1)));
                //var maxDirPathLength = dirs.Select(fsd => fsd.Text.Length).Max();

                //if (!Options.DirectoryModeOn && dirs.Any())
                //{
                //    list.Add(new MenuHeader("DIRECTORIES"));
                //}
                list.AddRange(dirs);

                if (!Options.DirectoryModeOn)
                {
                    currentAction = "Listing files in '" + SelectedPath + "'";
                    var files = Directory.GetFiles(SelectedPath)
                        .Select(d => new FSFile(d, d.Substring(SelectedPath.Length + 1)));
                    //list.Add(new MenuHeader("FILES"));
                    list.AddRange(files);
                }

                Console.WriteLine("Current Directory");
                Console.WriteLine("-----------------");
                Console.WriteLine(SelectedPath);
                Console.WriteLine();

                var menuSelection = PromptX.Menu
                (
                    list,
                    customItemsToPrepend:
                        new List<MenuObject>()
                            .AppendIf
                            (
                                Options.DirectoryModeOn,
                                BuildCustomRoutine
                                (
                                    "Choose the current directory",
                                    () =>
                                    {
                                        IsFinalSelection = true;
                                    },
                                    command: "."
                                )
                            )
                            .Append
                            (
                                BuildCustomRoutine
                                (
                                    "(Go up one directory)",
                                    () =>
                                    {
                                        if (_rootPathPattern.IsMatch(SelectedPath))
                                        {
                                            DelayedAlert = "You cannot traverse past the root directory";
                                        }
                                        else if (SelectedPath.Length == StartDirectory.Length && !Options.AllowTraversalOutsideStartDirectory)
                                        {
                                            DelayedAlert = "You cannot traverse outside the start directory (see FileSearchOptions.AllowTraversalOutsideStartDirectory)";
                                        }
                                        else
                                        {
                                            SelectedPath = NormalizeDirectoryPath(SelectedPath.Substring(0, SelectedPath.LastIndexOf(Path.PathSeparator)));
                                        }
                                    },
                                    command: ".."
                                )
                            ),
                    customItemsToAppend: new[]
                    {
                        CreateExitRoutineMenuItem(text: "Cancel")
                    },
                    title: "Choose or type/paste a directory",
                    renderer: (mo) => mo is FSDirectory
                        ? mo.Text.PadRight(55) + "(dir)"
                        : mo.Text,
                    allowArbitraryInput: true
                );

                currentAction = "Continue impl, menuSelection?";
                if (menuSelection == null)
                    return;
                if (menuSelection.SelectedItem is FSDirectory fsDirectory)
                {
                    SelectedPath = NormalizeDirectoryPath(fsDirectory.Path);
                }
                else if (menuSelection.SelectedItem is FSFile fsFile)  // assumes Options.DirectoryModeOn == true
                {
                    SelectedPath = fsFile.Path;
                    IsFinalSelection = true;
                }
                else if (menuSelection.ArbitraryInput != null)
                {
                    if (!Directory.Exists(menuSelection.ArbitraryInput))
                    {
                        DelayedAlert = "Is not a directory or does not exist.";
                    }
                    else
                    {
                        SelectedPath = NormalizeDirectoryPath(menuSelection.ArbitraryInput);
                    }
                }
            }
            catch (ConsoleNavigation.BackoutRoutineException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Current action: " + currentAction);
                RenderX.Exception(ex);
            }
        }

        /// <summary>
        /// Ensures a path does not end in a path separator (except root paths)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizeDirectoryPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ValidationException("path cannot be null or blank");
            }
            path = path.Trim();
            if (!_rootPathPattern.IsMatch(path) && path.Last() == Path.DirectorySeparatorChar)
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }

        private static Regex _rootPathPattern = new Regex(@"^([A-Z]:\\|\/)$", RegexOptions.IgnoreCase);
    }
}
