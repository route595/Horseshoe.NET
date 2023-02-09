using System;
using System.Collections.Generic;
using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

namespace Horseshoe.NET
{
    /// <summary>
    /// <para>
    /// A basic logging utility aimed at giving developers and testers the ability to view details of each in a chain 
    /// of nested method calls.  For example, a variable's value, a switch statement output, which method is being called next and why.
    /// </para>
    /// <para>
    /// By convention, please use concise language and always hide passwords.
    /// </para>
    /// <para>
    /// Each method in the chain that includes a <c>TraceJournal</c> parameter can receive the logging mechanism and 
    /// potentially pass it farther down the chain.  
    /// </para>
    /// <para>
    /// Write trace journaling into your code starting today with easy, out-of-the-box functionality.
    /// </para>
    /// </summary>
    public class TraceJournal : Dictionary<string, object>
    {
        /// <summary>
        /// Used to indicate depth of nested calls
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Write the journal entry to a file, REST service, memory, etc.
        /// </summary>
        private readonly Action<int, string> writeEntryAction;

        /// <summary>
        /// Creates a new <c>TraceJournal</c> instance with the supplied write action
        /// </summary>
        /// <param name="writeEntryAction">write the journal entry to a file, REST service, memory, etc. (if <c>null</c>, the default action is to add to <c>DefaultEntries</c>)</param>
        public TraceJournal(Action<int, string> writeEntryAction)
        {
            this.writeEntryAction = writeEntryAction ?? DefaultWriteEntry;
        }

        /// <summary>
        /// Invokes the 'write' action on <c>message</c>, the default action adds indented messages to the entry list.
        /// </summary>
        /// <param name="message">A journal entry</param>
        public void WriteEntry(string message)
        {
            writeEntryAction.Invoke(Level, message ?? TextConstants.Null);
        }

        /// <summary>
        /// Invokes the 'write' action on <c>obj</c>, the default action converts objects to indented messages and adds them to the entry list.
        /// </summary>
        /// <param name="obj">An object or message</param>
        public void Write(object obj)
        {
            writeEntryAction.Invoke(Level, obj?.ToString() ?? TextConstants.Null);
        }

        /// <summary>
        /// Adds or replaces a key / value and then invokes the 'write' action on it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddAndWriteEntry(string key, object value) 
        {
            DictionaryUtilAbstractions.AddOrReplace(this, key, value);
            WriteEntry(key + " = " + value);
        }

        /// <summary>
        /// Throws an exception but not before invoking the 'write' action on it.
        /// </summary>
        /// <param name="ex">An exception.</param>
        /// <param name="levelDown">Whether to decrement the level by one.</param>
        public void WriteEntryAndThrow(Exception ex, bool levelDown = false)
        {
            WriteEntry(ex.GetType().FullName + ": " + ex.Message);
            if (levelDown)
                Level--;
            throw ex;
        }

        private static void DefaultWriteEntry(int level, string entry) => 
            DefaultEntries.Add((level >= 0 ? new string(' ', level * 2) : new string('<', Math.Abs(level))) + entry);

        /// <summary>
        /// The default functionality is to write journal entries here for later retrieval. Caution: <c>ResetDefault()</c> clears this list.
        /// </summary>
        public static List<string> DefaultEntries { get; }

        /// <summary>
        /// Use this for easy journaling with zero setup using the default functionality
        /// </summary>
        /// <remarks><see cref="ResetDefault"/></remarks>
        public static TraceJournal Default;

        static TraceJournal()
        {
            DefaultEntries = new List<string>();
            Default = new TraceJournal(null);  // init with default action
        }

        /// <summary>
        /// Easily set up a journaling session with the default functionaliy
        /// </summary>
        /// <returns></returns>
        public static TraceJournal ResetDefault()
        {
            DefaultEntries.Clear();
            Default.Level = 0;
            Default.Clear();
            return Default;
        }
    }
}
