namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A suite of benign exceptions (expected to be caught immediately) for performing basic app navigation.
    /// </summary>
    public class ConsoleNavigation : BenignException
    {
        /// <summary>
        /// Raises a specific subclass of <c>ConsoleNavigation</c> exception listened for in <c>RoutineX</c> to skip to the end of the routine and naturally start over.
        /// </summary>
        /// <exception cref="RestartRoutineException"></exception>
        public static void RestartRoutine()
        {
            throw new RestartRoutineException();
        }

        /// <summary>
        /// Raises a subclass of <c>ConsoleNavigation</c> exception (listened for in <c>RoutineX</c>) that exits the routine and returns to the previous menu.
        /// </summary>
        /// <exception cref="ExitRoutineException"></exception>
        public static void ExitRoutine()
        {
            throw new ExitRoutineException();
        }

        /// <summary>
        /// Raises a specific subclass of <c>ConsoleNavigation</c> exception listened for in <c>ConsoleXApp</c> exit the application.
        /// </summary>
        /// <exception cref="ExitAppException"></exception>
        public static void ExitApp()
        {
            throw new ExitAppException();
        }

        /// <summary>
        /// Exits input prompt.
        /// </summary>
        /// <exception cref="CancelInputPromptException"></exception>
        public static void CancelInputPrompt()
        {
            throw new CancelInputPromptException();
        }

        ///// <summary>
        ///// Exits routine or application due to Ctrl+C has been pressed.
        ///// </summary>
        ///// <exception cref="CtrlCException"></exception>
        //public static void CtrlCHasBeenPressed()
        //{
        //    throw new CtrlCException();
        //}

        /// <summary>
        /// Create a new <c>ConsoleNavigation</c> instance.
        /// </summary>
        public ConsoleNavigation() : base() { }

        /// <summary>
        /// Create a new <c>ConsoleNavigation</c> instance.
        /// </summary>
        /// <param name="message">a message to carry through the exception</param>
        public ConsoleNavigation(string message) : base(message) { }

        /// <summary>
        /// This subclass of <c>ConsoleNavigation</c> is listened for in routines which then skips to the end then naturally loops back to the beginning.
        /// </summary>
        public class RestartRoutineException : ConsoleNavigation
        {
            /// <summary>
            /// Creates a new <c>RestartRoutineException</c>.
            /// </summary>
            public RestartRoutineException() : base("Restarting routine...") { }
        }

        /// <summary>
        /// This subclass of <c>ConsoleNavigation</c> is listened for in routines which then exits the loop and return to the previous menu / routine
        /// </summary>
        public class ExitRoutineException : ConsoleNavigation
        {
            /// <summary>
            /// Creates a new <c>ExitRoutineException</c>.
            /// </summary>
            public ExitRoutineException() : base("Exiting routine...") { }
        }

        /// <summary>
        /// This subclass of <c>ConsoleNavigation</c> is listened for in <c>ConsoleXApp</c>s which then exits the application
        /// </summary>
        public class ExitAppException : ConsoleNavigation
        {
            /// <summary>
            /// Creates a new <c>ExitAppException</c>.
            /// </summary>
            public ExitAppException() : base("Exiting application...") { }
        }

        ///// <summary>
        ///// This subclass of <c>ConsoleNavigation</c> is listened for in <c>RoutineX</c> which exits the routine and in <c>ConsoleXApp</c>s which then exits the application.
        ///// </summary>
        //public class CtrlCException : ConsoleNavigation
        //{
        //    /// <summary>
        //    /// Creates a new <c>ControlCException</c>.
        //    /// </summary>
        //    public CtrlCException() : base("Ctrl+C has been pressed...") { }
        //}

        /// <summary>
        /// This subclass of <c>ConsoleNavigation</c> is triggered during console input when Escape has been pressed.
        /// </summary>
        public class CancelInputPromptException : ConsoleNavigation
        {
            /// <summary>
            /// Creates a new <c>CancelInputPromptException</c>.
            /// </summary>
            public CancelInputPromptException() : base("Cancelling input prompt...") { }
        }
    }
}
