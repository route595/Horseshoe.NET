using System.IO;

using Horseshoe.NET.IO;

namespace Horseshoe.NET.RelayMessage
{
    /// <summary>
    /// Base utility class for bundling <c>RelayMessage</c> and <c>RelayException</c> instances
    /// with indentation that writes to the filesystem.  Exceptions are not indented by default.
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
    ///     private IMessageRelay Relay { get; } = new RelayToFile("relayed-msgs.txt", indentInterval: 3, indentExceptionsInlineWithMessages: true);
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
    ///         // setting the RelayToFile instance is required if is going to be used
    ///         RelayToFile.SetInstance("relayed-msgs.txt");
    ///         FooLib.Relay = RelayToFile.Instance;
    ///         OtherLib.Relay = RelayToFile.Instance;  // contiguous indentation everywhere
    ///     }
    ///     
    ///     private IMessageRelay Relay => RelayToFile.Instance;
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
    /// <example>
    /// <para>
    /// Client Code Example 3 - Relay with optimized file write
    /// </para>
    /// <code>
    /// using Horseshoe.NET.RelayMessage;
    /// 
    /// public class MyCode
    /// {
    ///     private RelayToFile Relay { get; } = new RelayToFile("relayed-msgs.txt");
    ///     private FooLib Foo { get; } = new FooLib { Relay = this.Relay };
    ///     
    ///     public void MyMethod()
    ///     {
    ///         Relay.BeginOptimizedSession();
    ///         Relay.Message("Start time: " + DateTime.Now, indent: 0);
    ///         Foo.LibMethod();   // library messages are also being relayed
    ///         Relay.Message("End time: " + DateTime.Now, indent: 0);
    ///         Relay.EndOptimizedSession();
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class RelayToFile : RelayIndentedMessagesBase
    {
        /// <summary>
        /// The file to which relayed messages should be written.
        /// </summary>
        public FilePath FileOrPath { get; }

        /// <summary>
        /// Used to keep the file connection open until manually closed, see <c>StartOptimizedSession()</c> and <c>EndOptimizedSession()</c>.
        /// </summary>
        private StreamWriter Writer { get; set; }

        /// <summary>
        /// How far right to align message id's, if appblicable.  Default is 100.
        /// </summary>
        public int MsgIdPad { get; }

        /// <summary>
        /// If <c>true</c>, silently ignores IO exceptions.  Default is <c>false</c>.
        /// </summary>
        public bool SuppressExceptions { get; set; }

        private bool Optimized { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileOrPath">The file to which relayed messages should be written.</param>
        /// <param name="append">If <c>true</c> appends data to the file, <c>false</c> to overwrite the file.  Default is <c>false</c>.</param>
        /// <param name="indentInterval">Indicates how many spaces to increment or decrement, default is 2.</param>
        /// <param name="msgIdPad">How far right to align message id's, if appblicable.  Default is 100.</param>
        /// <param name="indentExceptionsInlineWithMessages">
        /// If <c>true</c>, indents exceptions at the same level as the last relayed message.
        /// Default is <c>false</c>.
        /// </param>
        /// <param name="suppressExceptions">If <c>true</c>, silently ignores IO exceptions.  Default is <c>false</c>.</param>
        public RelayToFile(FilePath fileOrPath, bool append = false, int indentInterval = 2, int msgIdPad = 100, bool indentExceptionsInlineWithMessages = false, bool suppressExceptions = false)
        {
            FileOrPath = fileOrPath;
            if (!append)
            {
                try
                {
                    if (fileOrPath.Exists)
                        fileOrPath.Delete();
                }
                catch (IOException)
                {
                    if (!suppressExceptions)
                        throw;
                }
            }
            IndentInterval = indentInterval;
            MsgIdPad = msgIdPad;
            IndentExceptionsInlineWithMessages = indentExceptionsInlineWithMessages;
            SuppressExceptions = suppressExceptions;
        }

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
            try
            {
                if (Optimized)  // Writer is not null
                {
                    Writer.WriteLine(message);
                }
                else
                {
                    using (Writer = new StreamWriter(FileOrPath, true))
                    {
                        Writer.WriteLine(message);
                    }
                }
            }
            catch(IOException)
            {
                if (!SuppressExceptions)
                    throw;
            }
        }

        /// <summary>
        /// Opens a file writer which will write all incoming messages using a single file connection.  
        /// Must be closed manually (see <c>EndOptimizedSession()</c>).
        /// </summary>
        /// <param name="append">If <c>true</c> appends data to the file, <c>false</c> to overwrite the file.  Default is <c>false</c>.</param>
        public void BeginOptimizedSession(bool append = false)
        {
            Writer = new StreamWriter(FileOrPath, append);
            Writer.WriteLine("*** begin optimized session ***");
            Optimized = true;
        }

        /// <summary>
        /// Closes the file writer that was opened using <c>BeginOptimizedSession()</c>.
        /// </summary>
        public void EndOptimizedSession()
        {
            if (Writer != null)
            {
                Writer.WriteLine("*** end optimized session ***");
                try
                {
                    Writer.Flush();
                    Writer.Close();
                }
                catch { }
                try
                {
                    Writer.Dispose();
                }
                catch { }
            }
            Optimized = false;
        }

        /// <summary>
        /// Use this instance to support perpetual indentation across multiple sources (e.g. libraries, classes, etc.)
        /// </summary>
        public static RelayToFile Instance { get; private set; }

        /// <summary>
        /// Create an instance to support perpetual indentation across multiple sources (e.g. libraries, classes, etc.)
        /// </summary>
        /// <param name="fileOrPath">The file to which relayed messages should be written.</param>
        /// <param name="indentInterval">Indicates how many spaces to increment or decrement, default is <c>2</c>.</param>
        /// <param name="indentExceptionsInlineWithMessages">
        /// If <c>true</c>, indents exceptions at the same level as the last relayed message.
        /// Default is <c>false</c>.
        /// </param>
        public static void SetInstance(FilePath fileOrPath, int indentInterval = 2, bool indentExceptionsInlineWithMessages = false)
        {
            Instance = new RelayToFile(fileOrPath, indentInterval: indentInterval, indentExceptionsInlineWithMessages: indentExceptionsInlineWithMessages);
        }
    }
}
