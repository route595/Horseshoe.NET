using System;

namespace Horseshoe.NET.RelayMessage
{
    /// <summary>
    /// Base utility class for bundling <c>RelayMessage</c> and <c>RelayException</c> instances
    /// with indentation that writes to the console.  Exceptions are not indented by default.
    /// Supports perpetual indentation everywhere.
    /// <example>
    /// <para>
    /// Library Code Example
    /// </para>
    /// <code>
    /// using Horseshoe.NET.RelayMessage;
    /// 
    /// public class FooLib
    /// {
    ///     // For relaying messages to the user of this library
    ///     public (static) IMessageRelay Relay { get; set; }
    ///     
    ///     public void LibMethod()
    ///     {
    ///         Relay?.Message("Starting LibMethod()...");
    ///         try 
    ///         {
    ///             Relay?.Message("initiating action 1...", indentHint: IndentHint.Increment);  
    ///             // do action 1
    ///             Relay?.Message("action 1 result: bla bla bla");  // indentation same as previous
    ///             Relay?.Message("initiating action 2...");
    ///             // do action 2
    ///             Relay?.Message("action 2 result: bla bla bla");
    ///         }
    ///         catch(Exception ex)
    ///         {
    ///             Relay?.Exception(ex);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// <para>
    /// Client Code Example 1 - Instance-Level Relay
    /// </para>
    /// <code>
    /// using Horseshoe.NET.RelayMessage;
    /// 
    /// public class MyCode
    /// {
    ///     private IMessageRelay Relay { get; } = new RelayToConsole(indentInterval: 3, indentExceptionsInlineWithMessages: true);
    ///     private FooLib Foo { get; } = new FooLib { Relay = this.Relay };
    ///     
    ///     public void MyMethod()
    ///     {
    ///         Relay.Message("Start time: " + DateTime.Now, indent: 0);
    ///         Foo.LibMethod();   // library messages are also being relayed
    ///         Relay.Message("End time: " + DateTime.Now, indent: 0);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// <para>
    /// Client Code Example 2 - Class-Level Relay
    /// </para>
    /// <code>
    /// using Horseshoe.NET.RelayMessage;
    /// 
    /// public class MyCode
    /// {
    ///     static MyCode()
    ///     {
    ///         // setting the RelayToConsole instance is optional, it is only needed if setting new values via the args
    ///         RelayToConsole.SetInstance(indentInterval: 3, indentExceptionsInlineWithMessages: true);
    ///         FooLib.Relay = RelayToConsole.Instance;
    ///         OtherLib.Relay = RelayToConsole.Instance;  // contiguous indentation everywhere
    ///     }
    ///     
    ///     private IMessageRelay Relay => RelayToConsole.Instance;
    ///     private FooLib Foo { get; } = new FooLib();
    ///     
    ///     public void MyMethod()
    ///     {
    ///         Relay.Message("Start time: " + DateTime.Now, indent: 0);
    ///         Foo.LibMethod();
    ///         Relay.Message("End time: " + DateTime.Now, indent: 0);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class RelayToConsole : RelayIndentedMessagesBase
    {
        /// <summary>
        /// How far right to align message id's, if applicable.  Default is 20 less than console width.
        /// </summary>
        public int MsgIdPad { get; } = Console.WindowWidth - 20;

        /// <inheritdoc cref="RelayIndentedMessagesBase.RenderToOutput(string,int?)" />
        public override void RenderToOutput(string message, int? id = null)
        {
            message = IndentLevel > 0
                ? new string(' ', IndentLevel) + message
                : message;
            if (id.HasValue)
            {
                message = message.PadRight(MsgIdPad - 1) + " [msgID: " + id + "]";
            }
            Console.WriteLine(message);
        }

        /// <summary>
        /// Creates a new instance of <c>RelayToConsole</c>.
        /// </summary>
        /// <param name="indentInterval">Indicates how many spaces to increment or decrement, default is <c>2</c>.</param>
        /// <param name="msgIdPad">How far right to align message id's, if appblicable.  Default is 20 less than console width.</param>
        /// <param name="indentExceptionsInlineWithMessages">
        /// If <c>true</c>, indents exceptions at the same level as the last relayed message.
        /// Default is <c>false</c>.
        /// </param>
        public RelayToConsole(int indentInterval = 2, int msgIdPad = 0, bool indentExceptionsInlineWithMessages = false)
        {
            IndentInterval = indentInterval;
            if (msgIdPad > 0)
                MsgIdPad = msgIdPad;
            IndentExceptionsInlineWithMessages = indentExceptionsInlineWithMessages;
        }

        private static RelayToConsole _instance;

        /// <summary>
        /// Use this instance to support perpetual indentation across multiple sources (e.g. libraries, classes, etc.)
        /// </summary>
        public static RelayToConsole Instance 
        {
            get
            {
                if (_instance == null)
                    SetInstance();
                return _instance;
            } 
        }

        /// <summary>
        /// Create a reusable <c>RelayToConsole</c> instance to support perpetual indentation across multiple sources (e.g. libraries, classes, etc.)
        /// </summary>
        /// <param name="indentInterval">Indicates how many spaces to increment or decrement, default is <c>2</c>.</param>
        /// <param name="indentExceptionsInlineWithMessages">
        /// If <c>true</c>, indents exceptions at the same level as the last relayed message.
        /// Default is <c>false</c>.
        /// </param>
        public static void SetInstance(int indentInterval = 2, bool indentExceptionsInlineWithMessages = false)
        {
            _instance = new RelayToConsole(indentInterval: indentInterval, indentExceptionsInlineWithMessages: indentExceptionsInlineWithMessages);
        }
    }
}
