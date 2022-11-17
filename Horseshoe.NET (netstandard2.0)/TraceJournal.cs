using System;
using System.Collections.Generic;

using Horseshoe.NET.Text;

namespace Horseshoe.NET
{
    /// <summary>
    /// Write trace journaling into your code with easy, out-of-the-box functionality
    /// </summary>
    public class TraceJournal
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
        /// Writes <c>obj</c> to the write
        /// </summary>
        /// <param name="obj"></param>
        public void WriteEntry(object obj)
        {
            writeEntryAction.Invoke(Level, TextUtil.Reveal(obj));
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
            return Default;
        }
    }
}
