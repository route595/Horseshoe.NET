﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Objects;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.ConsoleX
{
    public abstract class ConsoleXApp
    {
        public static StringValues DefaultWelcomeValues => new[]
        {
            "Welcome to " + Assembly.GetEntryAssembly().GetDisplayName() + "!",
            typeof(ConsoleXApp).Namespace + " v" + typeof(ConsoleXApp).Assembly.GetDisplayVersion(),
            Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
        };

        /// <summary>
        /// Starts the menu automation, requires <c>MainMenu</c> to be overridden
        /// </summary>
        /// <param name="app">A ConsoleApp instance</param>
        public static void StartConsoleApp(ConsoleXApp app)
        {
            app.Run();
        }

        /// <summary>
        /// Starts the menu automation, requires <c>MainMenu</c> to be overridden
        /// </summary>
        /// <typeparam name="T">Type of the startup class (usually located in Program.cs)</typeparam>
        public static void StartConsoleApp<T>() where T : ConsoleXApp
        {
            ObjectUtil.GetDefaultInstance<T>().Run();
        }

        /// <summary>
        /// Set the loop mode (i.e. 'Continuous', 'ClearScreen'), default is 'Continuous'
        /// </summary>
        public virtual LoopMode LoopMode { get; } = LoopMode.Continuous;

        /// <summary>
        /// Displays whether looping is on (i.e. LoopMode = 'Continuous' or 'ClearScreen')
        /// </summary>
        public bool Looping => LoopMode.In(LoopMode.Continuous, LoopMode.ClearScreen);

        /// <summary>
        /// String or array of strings to display at app startup
        /// </summary>
        public virtual StringValues WelcomeMessage { get; }

        /// <summary>
        /// The title to display above the main menu, if applicable
        /// </summary>
        public virtual Title MainMenuTitle => "Main Menu";

        /// <summary>
        /// Collection of initial routines to choose from at app startup
        /// </summary>
        public virtual IList<MenuObject> MainMenu { get; }

        /// <summary>
        /// The number of columns in which to render the main menu
        /// </summary>
        public virtual int MainMenuColumns => 1;

        /// <summary>
        /// A mechanism for configuring the menu's rendering <c>TextGrid</c>
        /// </summary>
        public virtual Action<TextGrid> ConfigureTextGrid { get; }

        private bool ApplicationExiting { get; set; }

        /// <summary>
        /// Creates a configuration for main menu routines
        /// </summary>
        public virtual Action<RoutineX> ConfigureMainMenuRoutines { get; }

        public virtual Action<string> OnMainMenuSelecting { get; }

        public virtual Action<RoutineX> OnMainMenuRoutineAutoRunComplete { get; }

        public virtual Action<Exception> OnMainMenuRoutineError { get; }

        /// <summary>
        /// Override this for a non-interactive console app expereience
        /// </summary>
        public virtual void Run()
        {
            if (MainMenu == null)
            {
                throw new UtilityException("ConsoleApp requires one of the following overrides: MainMenu or Run()");
            }
            bool firstRun = true;
            while ((firstRun || Looping) && !ApplicationExiting)
            {
                if (!firstRun)
                {
                    switch (LoopMode)
                    {
                        case LoopMode.Continuous:
                            RenderX.Pad(1);
                            break;
                            //case LoopMode.ClearScreen:
                            //    Console.Clear();
                            //    break;
                    }
                }
                else if (WelcomeMessage.Count > 0)
                {
                    RenderX.Welcome(WelcomeMessage);
                }
                _RunImpl();
                firstRun = false;
            }
        }

        private void _RunImpl()
        {
            try
            {
                PromptX.Menu
                (
                    MainMenu,
                    title: MainMenuTitle,
                    columns: MainMenuColumns,
                    configureTextGrid: ConfigureTextGrid,
                    onMenuSelecting: LoopMode == LoopMode.ClearScreen
                        ? (menuSelection) =>
                        {
                            Console.Clear();
                            OnMainMenuSelecting?.Invoke(menuSelection);
                        }
                        : OnMainMenuSelecting,
                    onRoutineAutoRunComplete: LoopMode == LoopMode.ClearScreen
                        ? (routine) =>
                        {
                            if (!routine.IsExited)
                            {
                                PromptX.Continue();
                            }
                            Console.Clear();
                            OnMainMenuRoutineAutoRunComplete?.Invoke(routine);
                        }
                        : OnMainMenuRoutineAutoRunComplete
                );
            }
            catch (ConsoleNavigation.ExitAppException)   // exit gracefully
            {
                ApplicationExiting = true;
            }
            catch (Exception ex)
            {
                RenderX.Exception(ex, padBefore: 1);
                PromptX.Continue();
            }
        }

        /// <summary>
        /// Build a non-interactive <c>Routine</c> as an item for the main menu
        /// </summary>
        /// <param name="text">A title</param>
        /// <param name="action">The action to execute when this routine is run</param>
        /// <param name="configure">An action to custom configure the this routine</param>
        /// <param name="onError">An action to custom handle uncaught exceptions</param>
        /// <returns>A <c>Routine</c> instance</returns>
        public RoutineX BuildMainMenuRoutine(string text, Action action, Action<RoutineX> configure = null, Action<Exception> onError = null)
        {
            return RoutineX.BuildMenuRoutine
            (
                text,
                action,
                configure: CombineConfigureAction(configure, ConfigureMainMenuRoutines),
                onError: onError
            );
        }

        private Action<RoutineX> CombineConfigureAction(Action<RoutineX> action1, Action<RoutineX> action2)
        {
            return (routine) =>
            {
                action1?.Invoke(routine);
                action2?.Invoke(routine);
                if (LoopMode == LoopMode.ClearScreen)
                {
                    var onRoutineEnded = routine.OnRoutineEnded;
                    routine.OnRoutineEnded = (_routine) =>
                    {
                        onRoutineEnded?.Invoke(_routine);
                        if (!_routine.IsExited)
                        {
                            PromptX.Continue();
                        }
                    };
                }
            };
        }

        /// <summary>
        /// Search the calling assembly for subclasses of RoutineX and instantiate them into a menu list in alpha order
        /// </summary>
        /// <param name="namespaceToMatch">Select routines only in this namespace, if provided</param>
        /// <returns></returns>
        protected IList<MenuObject> FindMainMenuRoutines(params string[] namespacesToMatch)
        {
            if (namespacesToMatch == null)
                namespacesToMatch = new string[0];
            var assembly = Assembly.GetCallingAssembly();
            var routineTypes = assembly.GetTypes()
                .Where(t =>
                    t.IsSubclassOf(typeof(RoutineX)) &&
                    (!namespacesToMatch.Any() || t.Namespace.In(namespacesToMatch))
                )
                .OrderBy(t => t.Name);
            var list = new List<MenuObject>();
            foreach (var type in routineTypes)
            {
                var routine = ObjectUtil.GetInstance<RoutineX>(type);
                routine.OnError = OnMainMenuRoutineError;
                routine.Configure(ConfigureMainMenuRoutines);
                list.Add(routine);
            }
            return list;
        }
    }
}