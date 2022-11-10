namespace Horseshoe.NET.ConsoleX
{
    public class RenderMessages
    {
        public bool IsNotSelectable { get; private set; }

        public void SetNotSelectable()
        {
            IsNotSelectable = true;
        }
    }
}
