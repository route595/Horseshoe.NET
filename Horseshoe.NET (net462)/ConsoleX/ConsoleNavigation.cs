namespace Horseshoe.NET.ConsoleX
{
    public class ConsoleNavigation : BenignException
    {
        public static void RestartRoutine()
        {
            throw new RestartRoutineException();
        }

        public static void ExitRoutine()
        {
            throw new ExitRoutineException();
        }

        public static void ExitApp()
        {
            throw new ExitAppException();
        }

        public static void CancelPassword()
        {
            throw new CancelPasswordException();
        }

        public static void CancelPrompt()
        {
            throw new PromptCanceledException();
        }

        public static void CancelPrompt(string message)
        {
            throw new PromptCanceledException(message);
        }

        public ConsoleNavigation() : base() { }

        public ConsoleNavigation(string message) : base(message) { }

        public class RestartRoutineException : ConsoleNavigation
        {
            public RestartRoutineException() : base("Restarting routine...") { }
        }

        public class ExitRoutineException : ConsoleNavigation
        {
            public ExitRoutineException() : base("Exiting routine...") { }
        }

        public class ExitAppException : ConsoleNavigation
        {
            public ExitAppException() : base("Exiting application...") { }
        }

        public class CancelPasswordException : ConsoleNavigation
        {
            public CancelPasswordException() : base("Cancelling password input... Note, it is preferrable to wrap PromptPassword[Secure]() in a try-catch block.") { }
        }

        public class PromptCanceledException : ConsoleNavigation
        {
            public PromptCanceledException() { }
            public PromptCanceledException(string message) : base(message) { }
        }
    }
}
