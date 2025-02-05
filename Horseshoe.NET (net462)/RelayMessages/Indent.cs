using System.IO;

namespace Horseshoe.NET.RelayMessages
{
    /// <summary>
    /// Instructions for indenting relayed messages.
    /// </summary>
    public class Indent
    {
        /// <summary>
        /// A fixed length indent that implementations may use when rendering messages. This trumps <c>Hint</c> except when <c>IncrementNext</c> or <c>DecrementNext</c>.
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// An indentation strategy that implementations may use when rendering messages.
        /// </summary>
        public IndentHint Hint { get; set; }

        /// <summary>
        /// This helper instance increases the indent level (i.e. <c>new Indent { Hint = IndentHint.Increment }</c>)
        /// </summary>
        public static Indent Increment { get; } = new Indent { Hint = IndentHint.Increment };

        /// <summary>
        /// This helper instance increases the next indent level (i.e. <c>new Indent { Hint = IndentHint.IncrementNext }</c>)
        /// </summary>
        public static Indent IncrementNext { get; } = new Indent { Hint = IndentHint.IncrementNext };

        /// <summary>
        /// This helper instance decreases the indent level (i.e. <c>new Indent { Hint = IndentHint.Decrement }</c>)
        /// </summary>
        public static Indent Decrement { get; } = new Indent { Hint = IndentHint.Decrement };

        /// <summary>
        /// This helper instance decreases the next indent level (i.e. <c>new Indent { Hint = IndentHint.DecrementNext }</c>)
        /// </summary>
        public static Indent DecrementNext { get; } = new Indent { Hint = IndentHint.DecrementNext };

        /// <summary>
        /// This helper instance restores the indent to the last explicitly set level (i.e. <c>new Indent { Hint = IndentHint.Restore }</c>)
        /// </summary>
        public static Indent Restore { get; } = new Indent { Hint = IndentHint.Restore };

        /// <summary>
        /// This helper instance resets the indent level to zero (i.e. <c>new Indent { Hint = IndentHint.Reset }</c>)
        /// </summary>
        public static Indent Reset { get; } = new Indent { Hint = IndentHint.Reset };

        ///// <summary>
        ///// Auto-converts 
        ///// </summary>
        ///// <param name="indentHint"></param>
        //public static implicit operator Indent(IndentHint indentHint)
        //{
        //    switch (indentHint)
        //    {
        //        case IndentHint.Increment:
        //            return Increment;
        //        case IndentHint.IncrementNext:
        //            return IncrementNext;
        //        case IndentHint.Decrement:
        //            return Decrement;
        //        case IndentHint.DecrementNext:
        //            return DecrementNext;
        //        case IndentHint.Restore:
        //            return Restore;
        //        case IndentHint.Reset:
        //            return Reset;
        //    }
        //    return new Indent { Hint = indentHint };
        //}
    }
}
