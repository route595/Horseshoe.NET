using System;
using System.Collections.Generic;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// Represents configurable, interactive or non-interactive synchronous processes or routines
    /// which are the building blocks of <c>ConsoleX</c> applications
    /// </summary>
    public abstract class RoutineX : MenuObject
    {
        /// <summary>
        /// An alternate banner for this routine (except if non-interactive)
        /// </summary>
        public virtual string AltBannerText { get; set; }

        /// <summary>
        /// A title assigned to the built-in menu
        /// </summary>
        public virtual Title MenuTitle => Text + " Menu";

        /// <summary>
        /// A built-in menu to display when this routine is run provided it has no <c>Action</c> (one or the other is required)
        /// </summary>
        public virtual IList<MenuObject> Menu { get; }

        /// <summary>
        /// The number of columns in which to render the menu
        /// </summary>
        public virtual int MenuColumns => 1;

        /// <summary>
        /// A mechanism for configuring the menu's rendering <c>TextGrid</c>
        /// </summary>
        public virtual Action<TextGrid> ConfigureTextGrid { get; }

        /// <summary>
        /// Custom menu items to prepend
        /// </summary>
        public virtual IList<MenuObject> CustomMenuItemsToPrepend { get; set; }

        /// <summary>
        /// Custom menu items to append
        /// </summary>
        public virtual IList<MenuObject> CustomMenuItemsToAppend { get; set; }

        /// <summary>
        /// The command to trigger custom menu items (routines)
        /// </summary>
        public virtual string Command { get; set; }

        /// <summary>
        /// The code to execute when then routine is run, if not implemented the <c>Menu</c> is triggered instead (one or the other is required)
        /// </summary>
        public virtual Action Action { get; set; }

        /// <summary>
        /// If <c>true</c>, adds a menu item for exiting the routine to the built-in menu, defaults to <c>AutoAppendExitRoutineMenuItemByDefault</c> which defaults to <c>false</c>
        /// </summary>
        public virtual bool AutoAppendExitRoutineMenuItem { get; set; }

        /// <summary>
        /// An event to hook into
        /// </summary>
        public virtual Action<Exception> OnError { get; set; }

        /// <summary>
        /// An event to hook into
        /// </summary>
        public virtual Action<string> OnMenuSelecting { get; set; }

        /// <summary>
        /// An event to hook into
        /// </summary>
        public virtual Action<MenuSelection<MenuObject>> OnMenuSelection { get; set; }

        /// <summary>
        /// An event to hook into
        /// </summary>
        public virtual Action<RoutineX> OnRoutineAutoRunComplete { get; set; }

        /// <summary>
        /// An event to hook into
        /// </summary>
        public virtual Action<RoutineX> OnBeforeNextIteration { get; set; }

        /// <summary>
        /// An event to hook into
        /// </summary>
        public virtual Action<RoutineX> OnRoutineRestarting { get; set; }

        /// <summary>
        /// An event to hook into
        /// </summary>
        public virtual Action<RoutineX> OnRoutineExiting { get; set; }

        /// <summary>
        /// An event to hook into
        /// </summary>
        public virtual Action<RoutineX> OnRoutineEnded { get; set; }

        private bool IsRestarted { get; set; }

        /// <summary>
        /// Is <c>true</c> if exiting the application (e.g. <c>'exit'</c> has been entered at the menu prompt and <c>allowExit == true</c> 
        /// </summary>
        public bool IsExited { get; set; }

        /// <summary>
        /// Create a new routine
        /// </summary>
        public RoutineX() : base()
        {
            Configure(GlobalConfiguration);
        }

        /// <summary>
        /// Create a new routine with display text
        /// </summary>
        /// <param name="text"></param>
        public RoutineX(string text) : base(text)
        {
            Configure(GlobalConfiguration);
        }

        /// <summary>
        /// Override Run() to implement a non-interactive routine
        /// </summary>
        public virtual void Run()
        {
            if (Menu != null)
            {
                if (Action != null)
                {
                    throw new UtilityException("must override Run() or exactly one of the following properties: Action, Menu");
                }
            }
            else if (Action == null)
            {
                throw new UtilityException("must override Run() or one of the following properties: Action, Menu");
            }
            bool looping = Menu != null;
            IsRestarted = false;
            IsExited = false;
            bool firstRun = true;
            RenderX.RoutineTitle(AltBannerText ?? Text);
            while ((looping || IsRestarted || firstRun) && !IsExited)
            {
                IsRestarted = false;
                if (!firstRun)
                { 
                    OnBeforeNextIteration?.Invoke(this);
                    RenderX.Pad(1);
                }
                _RunImpl();
                firstRun = false;
            }
            OnRoutineEnded?.Invoke(this);
        }

        private void _RunImpl()
        {
            try
            {
                if (Action != null)
                {
                    Action.Invoke();
                }
                else if (Menu != null)
                {
                    PromptX.Menu<MenuObject>
                    (
                        Menu, 
                        title: MenuTitle,
                        customItemsToPrepend: CustomMenuItemsToPrepend,
                        customItemsToAppend: BuildCustomMenuItems(CustomMenuItemsToAppend, AutoAppendExitRoutineMenuItem),
                        columns: MenuColumns,
                        configureTextGrid: ConfigureTextGrid,
                        onMenuSelecting: OnMenuSelecting,
                        onMenuSelection: OnMenuSelection,
                        onRoutineAutoRunComplete: OnRoutineAutoRunComplete
                    );
                }
            }
            catch (ConsoleNavigation.RestartRoutineException)
            {
                OnRoutineRestarting?.Invoke(this);
                IsRestarted = true;
            }
            catch (ConsoleNavigation.ExitRoutineException)
            {
                OnRoutineExiting?.Invoke(this);
                IsExited = true; 
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError.Invoke(ex);
                }
                else
                {
                    RenderX.Exception(ex);
                }
            }
        }

        private IList<MenuObject> BuildCustomMenuItems(IList<MenuObject> customMenuItems, bool autoAppendExitRoutineMenuItem)
        {
            if (!(autoAppendExitRoutineMenuItem || autoAppendExitRoutineMenuItem))
            {
                return customMenuItems;
            }
            return new List<MenuObject>(customMenuItems ?? new MenuObject[0])
                .AppendIf(autoAppendExitRoutineMenuItem, CreateExitRoutineMenuItem());
        }

        /// <summary>
        /// Action to perform at routine create time whose purpose is to configure the routine
        /// </summary>
        /// <param name="configure"></param>
        public virtual void Configure(Action<RoutineX> configure)
        {
            configure?.Invoke(this);
        }

        /// <summary>
        /// Action to perform configurations globally on all reoutines
        /// </summary>
        public static Action<RoutineX> GlobalConfiguration { get; set; }

        /// <summary>
        /// Restarts the routine (typically by typing '/' at the prompt if <c>AutoAppendRestartRoutineMenuItem == true</c>
        /// </summary>
        public static void Restart()
        {
            ConsoleNavigation.RestartRoutine();
        }

        /// <summary>
        /// Exits the routing (typically by typing 
        /// </summary>
        public static void Exit()
        {
            ConsoleNavigation.ExitRoutine();
        }

        /// <summary>
        /// Create a specialized menu item that restarts the routine
        /// </summary>
        /// <param name="text">The menu item text</param>
        /// <param name="command">The command to type</param>
        /// <param name="beforeRestart">Additional action to perform when menu item is selected</param>
        /// <returns></returns>
        protected static RoutineX CreateRestartRoutineMenuItem(string text = "Restart", string command = "R", Action beforeRestart = null)
        {
            return BuildCustomRoutine
            (
                text,
                () =>
                {
                    beforeRestart?.Invoke();
                    Restart();
                },
                command: command
            );
        }

        /// <summary>
        /// Create a specialized menu item that exits the routine and goes back to the previous menu
        /// </summary>
        /// <param name="text"></param>
        /// <param name="command"></param>
        /// <param name="beforeExit"></param>
        /// <remarks><seealso cref="AutoAppendExitRoutineMenuItem"/></remarks>
        /// <returns></returns>
        protected static RoutineX CreateExitRoutineMenuItem(string text = "Go Back", string command = "/", Action beforeExit = null)
        {
            return BuildCustomRoutine
            (
                text,
                () =>
                {
                    beforeExit?.Invoke();
                    Exit();
                },
                command: command
            );
        }

        /// <summary>
        /// Build a non-interactive <c>Routine</c>.  Note: It is common to override <see cref="Menu"/> with an array of routines built by this method.
        /// </summary>
        /// <param name="text">A title</param>
        /// <param name="action">The action to execute when this routine is run</param>
        /// <param name="configure">An action to custom configure this routine</param>
        /// <param name="onError">An action to custom handle uncaught exceptions</param>
        /// <returns>A <c>Routine</c> instance</returns>
        public static RoutineX BuildMenuRoutine(string text, Action action, Action<RoutineX> configure = null, Action<Exception> onError = null)
        {
            var routine = new NonInteractiveRoutine(text)
            {
                OnRun = action,
                OnError = onError
            };
            routine.Configure(configure);
            return routine;
        }

        /// <summary>
        /// Build a custom, non-interactive <c>Routine</c>
        /// </summary>
        /// <param name="text">A title</param>
        /// <param name="action">The action to execute when this routine is run</param>
        /// <param name="command">The text to type to activate this routine</param>
        /// <param name="configure">An action to custom configure the this routine</param>
        /// <returns>A <c>Routine</c> instance</returns>
        public static RoutineX BuildCustomRoutine(string text, Action action, string command = "", Action<RoutineX> configure = null)
        {
            var routine = new NonInteractiveRoutine(text)
            {
                Command = command,
                OnRun = action
            };
            routine.Configure(configure);
            return routine;
        }

        internal class InteractiveRoutine : RoutineX
        {
            internal InteractiveRoutine(string text) : base(text) { }
        }

        internal class NonInteractiveRoutine : RoutineX
        {
            public Action OnRun { get; set; }

            internal NonInteractiveRoutine(string text) : base(text) { }

            public override void Run()
            {
                OnRun?.Invoke();
            }
        }
    }
}
