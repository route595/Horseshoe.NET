namespace Horseshoe.NET.ConsoleX
{
    public class ExceptionRendering
    {
        public ExceptionTypeRenderingPolicy TypeRendering { get; set; }
        public bool IncludeDateTime { get; set; }
        public bool IncludeMachineName { get; set; }
        public bool IncludeStackTrace { get; set; }
        public int Indent { get; set; } = 2;
        public bool Recursive { get; set; }
    }
}
