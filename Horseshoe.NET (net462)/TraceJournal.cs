using System;
using System.Collections.Generic;

namespace Horseshoe.NET
{
    public class TraceJournal
    {
        /// <summary>
        /// Used to indicate depth of nested calls
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Write the journal entry to a file, REST service, memory, etc.
        /// </summary>
        public Action<string> WriteEntry { get; }

        /// <summary>
        /// Creates a new <c>TraceJournal</c> instance with the supplied write action
        /// </summary>
        /// <param name="writeEntry">write the journal entry to a file, REST service, memory, etc. (if <c>null</c>, the default action is to add to <c>DefaultEntries</c>)</param>
        public TraceJournal(Action<string> writeEntry)
        {
            WriteEntry = writeEntry ?? DefaultWriteEntry;
        }

        private void DefaultWriteEntry(string entry) => 
            DefaultEntries.Add((Level >= 0 ? new string(' ', Level * 2) : new string('<', Math.Abs(Level))) + entry);

        public static List<string> DefaultEntries;
        public static TraceJournal Default;

        static TraceJournal()
        {
            DefaultEntries = new List<string>();
            Default = new TraceJournal(null);  // init with default action
        }

        public static TraceJournal ResetDefault()
        {
            DefaultEntries.Clear();
            Default.Level = 0;
            return Default;
        }
    }
}
