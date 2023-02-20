namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// A suite of benign exceptions (expected to be caught immediately) for performing basic app navigation
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
        /// Exits password prompts promptly.
        /// </summary>
        /// <exception cref="CancelPasswordException"></exception>
        public static void xCancelPassword()
        {
            throw new xCancelPasswordException();
        }

        /// <summary>
        /// Cancels input prompt.
        /// </summary>
        /// <exception cref="PromptCanceledException"></exception>
        public static void xCancelPrompt()
        {
            throw new xPromptCanceledException();
        }

        /// <summary>
        /// Exits input prompt.
        /// </summary>
        /// <exception cref="CancelInputPromptException"></exception>
        public static void CancelInputPrompt()
        {
            throw new CancelInputPromptException();
        }

        /// <summary>
        /// Exits routine or application due to Ctrl+C has been pressed.
        /// </summary>
        /// <exception cref="ControlCException"></exception>
        public static void ControlCHasBeenPressed()
        {
            throw new ControlCException();
        }

        /// <summary>
        /// Exits prompts
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="PromptCanceledException"></exception>
        public static void xCancelPrompt(string message)
        {
            throw new xPromptCanceledException(message);
        }

        /// <summary>
        /// Create a new <c>ConsoleNavigation</c> instance
        /// </summary>
        public ConsoleNavigation() : base() { }

        /// <summary>
        /// Create a new <c>ConsoleNavigation</c> instance
        /// </summary>
        /// <param name="message">a message to carry through the exception</param>
        public ConsoleNavigation(string message) : base(message) { }

        /// <summary>
        /// This subclass of <c>ConsoleNavigation</c> is listened for in routines which then skips to the end then naturally loops back to the beginning
        /// </summary>
        public class RestartRoutineException : ConsoleNavigation
        {
            /// <summary>
            /// Create a new <c>RestartRoutineException</c>
            /// </summary>
            public RestartRoutineException() : base("Restarting routine...") { }
        }

        /// <summary>
        /// This subclass of <c>ConsoleNavigation</c> is listened for in routines which then exits the loop and return to the previous menu / routine
        /// </summary>
        public class ExitRoutineException : ConsoleNavigation
        {
            /// <summary>
            /// Create a new <c>ExitRoutineException</c>
            /// </summary>
            public ExitRoutineException() : base("Exiting routine...") { }
        }

        /// <summary>
        /// This subclass of <c>ConsoleNavigation</c> is listened for in <c>ConsoleXApp</c>s which then exits the application
        /// </summary>
        public class ExitAppException : ConsoleNavigation
        {
            /// <summary>
            /// Create a new <c>ExitAppException</c>
            /// </summary>
            public ExitAppException() : base("Exiting application...") { }
        }

        /// <summary>
        /// This subclass of <c>ConsoleNavigation</c> is listened for in <c>ConsoleXApp</c>s which then exits the application
        /// </summary>
        public class ControlCException : ConsoleNavigation
        {
            /// <summary>
            /// Creates a new <c>ControlCException</c>.
            /// </summary>
            public ControlCException() : base("Ctrl+C has been pressed...") { }
        }

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

        /// <summary>
        /// This subclass of <c>ConsoleNavigation</c> must be listened for in client code when <c>cancelable == true</c> in <c>PromptX.Password[Secure]()</c>
        /// </summary>
        public class xCancelPasswordException : ConsoleNavigation
        {
            /// <summary>
            /// Create a new <c>CancelPasswordException</c>
            /// </summary>
            public xCancelPasswordException() : base("Cancelling password input...") { }
        }

        /// <summary>
        /// This subclass of <c>ConsoleNavigation</c> is listened for in <c>PromptX.List()</c>s which then exits the prompt for a list value
        /// </summary>
        public class xPromptCanceledException : ConsoleNavigation
        {
            /// <summary>
            /// Create a new <c>PromptCanceledException</c>
            /// </summary>
            public xPromptCanceledException() { }

            /// <summary>
            /// Create a new <c>PromptCanceledException</c>
            /// </summary>
            /// <param name="message">a message</param>
            public xPromptCanceledException(string message) : base(message) { }
        }
    }
}
