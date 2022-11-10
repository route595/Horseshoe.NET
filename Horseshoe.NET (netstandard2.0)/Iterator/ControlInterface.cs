namespace Horseshoe.NET.Iterator
{
    public class ControlInterface
    {
        public int Count { get; internal set; }
        public int Index { get; internal set; }

        /// <summary>
        /// Exit iteration
        /// </summary>
        public void Exit()
        {
            throw new ExitIterationException();
        }

        /// <summary>
        /// Jump to next iteration
        /// </summary>
        public void Next()
        {
            throw new ContinueNextException();
        }
    }
}
