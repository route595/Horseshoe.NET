using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Collections;
using Horseshoe.NET.ObjectsAndTypes;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// The heart of <c>ConsoleX</c> applications including app launching logic.
    /// </summary>
    public abstract class ConsoleXApp
    {
        /// <summary>
        /// Sample welcome banner text.
        /// </summary>
        public static StringValues DefaultWelcomeValues => new[]
        {
            "Welcome to " + Assembly.GetEntryAssembly().GetDisplayName() + "!",
            typeof(ConsoleXApp).Namespace + " v" + typeof(ConsoleXApp).Assembly.GetDisplayVersion(),
            Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
        };

        /// <summary>
        /// Launches the app and, if <c>MainMenu</c> is implemented, starts menu automation.
        /// </summary>
        /// <param name="app">A <c>ConsoleXApp</c> instance</param>
        public static void StartConsoleApp(ConsoleXApp app)
        {
            app.Run();
        }

        /// <summary>
        /// Launches the app and, if <c>MainMenu</c> is implemented, starts menu automation.
        /// </summary>
        /// <typeparam name="T">Subclass of <c>ConsoleXApp</c> (typically Program.cs)</typeparam>
        public static void StartConsoleApp<T>() where T : ConsoleXApp
        {
            TypeUtil.GetDefaultInstance<T>().Run();
        }

        /// <summary>
        /// Gets the loop mode (i.e. 'Continuous', 'ClearScreen'), default is 'Continuous'. Overridable in implementing classes.
        /// </summary>
        public virtual LoopMode LoopMode { get; } = LoopMode.Continuous;

        /// <summary>
        /// String or array of strings to display at app startup.
        /// </summary>
        public virtual StringValues WelcomeMessage { get; }

        /// <summary>
        /// The title to display above the main menu, if applicable.
        /// </summary>
        public virtual Title MainMenuTitle => "Main Menu";

        /// <summary>
        /// Collection of initial routines to choose from at app startup.
        /// </summary>
        public virtual IList<MenuObject> MainMenu { get; }

        /// <summary>
        /// The number of columns in which to render the main menu.
        /// </summary>
        public virtual int MainMenuColumns => 1;

        /// <summary>
        /// A mechanism for configuring the menu's rendering <c>TextGrid</c>.
        /// </summary>
        public virtual Action<TextGrid> ConfigureTextGrid { get; }

        private bool ApplicationExiting { get; set; }

        /// <summary>
        /// Creates a configuration for main menu routines.
        /// </summary>
        public virtual Action<RoutineX> ConfigureMainMenuRoutines { get; }

        /// <summary>
        /// Action to perform when user selects a menu item from the main menu.
        /// </summary>
        public virtual Action<string> OnMainMenuSelecting { get; }

        /// <summary>
        /// Action to perform when a <c>MainMenu</c> routine completes.
        /// </summary>
        public virtual Action<RoutineX> OnMainMenuRoutineAutoRunComplete { get; }

        /// <summary>
        /// Action to perform when a <c>MainMenu</c> routine throws an exception.
        /// </summary>
        public virtual Action<Exception> OnMainMenuRoutineError { get; }

        /// <summary>
        /// Override this for a non-interactive console app experience.
        /// </summary>
        public virtual void Run()
        {
            if (MainMenu == null)
            {
                throw new UtilityException("ConsoleApp requires one of the following overrides: MainMenu or Run()");
            }
            if (WelcomeMessage.Count > 0)
            {
                RenderX.Welcome(WelcomeMessage);
            }
            while (!ApplicationExiting)
            {
                _RunImpl();
                switch (LoopMode)
                {
                    case LoopMode.Continuous:
                        RenderX.Pad(1);
                        break;
                }
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
            catch (ConsoleNavigation.CancelInputPromptException)   // if reached this far, handle gracefully
            {
                RenderX.Alert("ConsoleXApp: Input prompt cancelled.", padBefore: 1);
#if DEBUG
                RenderX.Alert("Developer debug note: Please catch ConsoleNavigation.CancelInputPromptException.");
#endif
                PromptX.Continue();
            }
            catch (Exception ex)
            {
                RenderX.Exception(ex, padBefore: 1);
                PromptX.Continue();
            }
        }

        /// <summary>
        /// Build a non-interactive <c>RoutineX</c> as an item for the main menu.
        /// </summary>
        /// <param name="text">A title.</param>
        /// <param name="action">The action to execute when this routine is run.</param>
        /// <param name="configure">An action to custom configure the this routine.</param>
        /// <param name="onError">An action to custom handle uncaught exceptions.</param>
        /// <returns>A <c>RoutineX</c> instance.</returns>
        public RoutineX BuildMenuRoutine(string text, Action action, Action<RoutineX> configure = null, Action<Exception> onError = null)
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
        /// Searches the calling assembly for subclasses of <c>RoutineX</c> and instantiates them into a menu list in alpha order.
        /// </summary>
        /// <param name="namespacesToMatch">Select routines only in one of these namespace, if provided.</param>
        /// <returns></returns>
        protected IList<MenuObject> FindRoutines(params string[] namespacesToMatch)
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
                var routine = (RoutineX)TypeUtil.GetInstance(type);
                routine.OnError = OnMainMenuRoutineError;
                routine.Configure(ConfigureMainMenuRoutines);
                list.Add(routine);
            }
            return list;
        }
    }
}
