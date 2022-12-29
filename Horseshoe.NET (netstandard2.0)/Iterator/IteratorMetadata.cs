namespace Horseshoe.NET.Iterator
{
    /// <summary>
    /// The interactive <c>Iterator</c> control interface and information provider.
    /// </summary>
    public class IteratorMetadata
    {
        /// <summary>
        /// Provides the total count of the collection or span being iterated.
        /// </summary>
        public int Count { get; internal set; }

        /// <summary>
        /// Provides the index of the current iterated item.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Terminate the iteration.
        /// </summary>
        public void Exit()
        {
            throw new ExitIterationException();
        }

        /// <summary>
        /// Jump to next item in the collection or span.
        /// </summary>
        public void Next()
        {
            throw new ContinueNextException();
        }
    }
}
