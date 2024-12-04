using System;

namespace Horseshoe.NET.RelayProgress
{
    /// <summary>
    /// Utility class for basic handling of <c>RelayProgress</c> instances in console applications.
    /// <example>
    /// <para>
    /// Library Code Example
    /// </para>
    /// <code>
    /// using Horseshoe.NET.RelayProgress;
    /// 
    /// public class FooLib
    /// {
    ///     // For relaying messages to the user of this library
    ///     public (static) IProgressRelay Relay { get; set; }
    ///     
    ///     public void LibMethod()
    ///     {
    ///         Relay?.Progress(description: "Initializing...");
    ///         // - or -
    ///         Relay?.Message("Initializing...");
    ///         // initialize
    ///         Relay?.Progress(taskNumber: 0, totalTasks: 4, description: "1 - Downloading...");
    ///         // download 
    ///         Relay?.Progress(taskNumber: 1, totalTasks: 4, description: "2 - Unpacking...");
    ///         // unzip 
    ///         Relay?.Progress(taskNumber: 2, totalTasks: 4, description: "3 - Installing...");
    ///         // install 
    ///         Relay?.Progress(description: "4 - Optimizing...", indeterminate: true);
    ///         // optimize 
    ///         Relay?.Progress(taskNumber: 4, totalTasks: 4, description: "Installed successfully");
    ///         // - or -
    ///         Relay?.Message("Installed successfully");
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// <para>
    /// Client Code Example
    /// </para>
    /// <code>
    /// using Horseshoe.NET.RelayProgress;
    /// 
    /// public class MyCode
    /// {
    ///     private IProgressRelay Relay { get; } = new RelayProgressToConsole_Basic(indentInterval: 3, resetIndentationOnExceptionsByDefault: true);
    ///     private FooLib Foo { get; } = new FooLib { Relay = this.Relay };
    ///     
    ///     public void MyMethod()
    ///     {
    ///         Relay.Message("Start time: " + DateTime.Now, indent: 0);
    ///         Foo.LibMethod();   // library messages and progress are also being relayed
    ///         Relay.Message("End time: " + DateTime.Now, indent: 0);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class RelayProgressToConsole_Basic : RelayProgressBase
    {
        private string _cachedProgressOutput;
        private bool _lastOutputWasProgress;
        //private string _longestLength;  // used to era

        /// <inheritdoc cref="RelayProgressBase.RenderProgress(int?, int?, double?, string, string, bool)" />
        public override void RenderProgress(int? taskNumber, int? totalTasks, double? progress, string category, string description, bool indeterminate)
        {
            var lastLength = 0;

            // backspace to the beginning of current line, if applicable
            // note, does not erase, the idea is to overwrite
            if (_cachedProgressOutput != null && _lastOutputWasProgress)
            {
                Console.Write(new string('\b', _cachedProgressOutput.Length));
                lastLength = _cachedProgressOutput.Length;
            }

            double value;

            if (progress.HasValue)   // progress trumps all, even if invalid
            {
                if (progress < 0.0)
                    progress = 0.0;
                if (progress > 1.0)
                    progress = 1.0;
                value = progress.Value;
            }
            else if (taskNumber.HasValue && totalTasks.HasValue)   // calculate progress
            {
                if (taskNumber < 0 || totalTasks < 0)
                {
                    value = 0.0;
                }
                else if (taskNumber > totalTasks)
                {
                    value = 1.0;
                }
                else
                {
                    value = (double)taskNumber.Value / totalTasks.Value;
                }
            }
            else
            {
                value = 0.0;   // fallback (default)
            }

            // render the new line
            _cachedProgressOutput = value.ToString("P1").PadRight(7);
            if (description != null) 
            {
                _cachedProgressOutput += " - " + description;
            }
            Console.Write(_cachedProgressOutput);
            _lastOutputWasProgress = true;

            // overwrite the rest of the previous line, if applicable
            if (lastLength > _cachedProgressOutput.Length)
            {
                Console.Write(new string(' ', lastLength - _cachedProgressOutput.Length));
            }
        }

        /// <inheritdoc cref="RelayProgressBase.RenderMessage(string,int?)"/>
        public override void RenderMessage(string message, int? id = null)
        {
            if (_lastOutputWasProgress)
                Console.WriteLine();
            Console.WriteLine(message + (id.HasValue ? "   [msgID: " + id + "]" : ""));
            _lastOutputWasProgress = false;
        }

        ///// <summary>
        ///// Creates a new instance of <c>RelayProgressToConsole_Basic</c>.
        ///// </summary>
        //public RelayProgressToConsole_Basic()
        //{
        //}
    }
}
