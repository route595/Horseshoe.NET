using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TraceJournal : List<TraceJournal._Entry>
    {
        /// <summary>
        /// Used to define a hierarchical postion in the journal where zero (0) is the main entry and 1 - n are subentries (e.g. level of nested method call).
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Adds ability to forward journal entries to a file, REST service, etc.
        /// </summary>
        public readonly Action<_Entry> WriteEntryAction;

        /// <summary>
        /// Optional subclass of <c>TraceJournal._Entry</c> from which to build journal entries.
        /// </summary>
        public Type EntryType { get; }

        /// <summary>
        /// An additional storage module for desired data capture.
        /// </summary>
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Creates a new <c>TraceJournal</c> instance with optional <c>writeEntryAction</c> and <c>entryType</c>.
        /// </summary>
        /// <param name="writeEntryAction">Adds ability to forward journal entries to a file, REST service, etc.</param>
        /// <param name="entryType">Optional subclass of <c>TraceJournal._Entry</c> from which to build journal entries.</param>
        public TraceJournal(Action<_Entry> writeEntryAction = null, Type entryType = null)
        {
            WriteEntryAction = writeEntryAction;
            if (entryType != null)
            {
                if (typeof(_Entry).IsAssignableFrom(entryType))
                {
                    EntryType = entryType;
                }
                else throw new ValidationException(entryType.FullName + " does not derive from " + typeof(_Entry).FullName);
            }
            LastJournal = this;
        }

        /// <summary>
        /// Adds an entry to the journal and invokes <c></c>writeEntryAction' (if applicable).
        /// </summary>
        /// <param name="message">A journal entry.</param>
        public void WriteEntry(string message)
        {
            var entry = EntryType != null
                ? Activator.CreateInstance(EntryType) as _Entry
                : new _Entry();
            entry.Level = Level;
            entry.Entry = message ?? TextConstants.Null;
            Add(entry);
            WriteEntryAction?.Invoke(entry);
        }

        /// <summary>
        /// Adds an entry to the journal and invokes 'writeEntryAction' (if applicable).
        /// </summary>
        /// <param name="obj">An object or message.</param>
        public void Write(object obj)
        {
            WriteEntry(obj?.ToString());
        }

        /// <summary>
        /// Adds or replaces a key / value and then invokes the 'write' action on it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddAndWriteEntry(string key, object value) 
        {
            DictionaryUtilAbstractions.AddOrReplace(Data, key, value);
            WriteEntry(key + " = " + value);
        }

        /// <summary>
        /// Tags <c>partialEntry</c> onto the end of the last entry.  If this journal contains no entries a new one is created.
        /// </summary>
        /// <param name="partialEntry">Some text.</param>
        public void AppendLastEntry(string partialEntry)
        {
            if (Count == 0)
            {
                WriteEntry(partialEntry);
            }
            else
            {
                this[Count - 1].Entry += partialEntry;
            }
        }

        /// <summary>
        /// Throws an exception but not before adding an entry to the journal.
        /// </summary>
        /// <param name="ex">An exception.</param>
        public void LogException(Exception ex)
        {
            WriteEntry(ex.GetType().FullName + ": " + ex.Message);
        }

        /// <summary>
        /// Creates a <c>string</c> rendering of the current journal.
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            return string.Join(Environment.NewLine, RenderLines());
        }

        /// <summary>
        /// Creates a line-by-line <c>string</c> rendering of the current journal.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> RenderLines()
        {
            var list = this
                .Select(e => e.ToString())
                .ToList();
            if (list.Any())
            {
                list.Insert(0, "Journal Entries");
                list.Insert(1, "---------------");
            }

            if (Data.Any())
            {
                var targetWidth = Data.Keys.Max(s => s.Length);
                list.Insert(0, "");
                foreach (var key in Data.Keys.OrderByDescending(s => s)) 
                {
                    list.Insert(0, Data[key]?.ToString() ?? TextConstants.Empty);
                }
                list.Insert(0, "Data");
                list.Insert(1, "----");
            }
            return list;
        }

        /// <summary>
        /// The base class for entries written to <c>TraceJournal</c> (features 2-space indentation).
        /// </summary>
        public class _Entry
        {
            /// <summary>
            /// Used to define a hierarchical postion in the journal where zero (0) is the main entry and 1 - n are subentries (e.g. level of nested method call).
            /// </summary>
            public int Level { get; set; }

            /// <summary>
            /// The journal entry text.
            /// </summary>
            public string Entry { get; set; }

            /// <summary>
            /// Renders a journal entry to a <c>string</c> taking level into account.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return (Level >= 0 ? new string(' ', Level * 2) : "<") + Entry;
            }
        }

        /// <summary>
        /// Recalls the last created journal for easy instant rendering.
        /// </summary>
        public static TraceJournal LastJournal { get; set; }
    }
}
