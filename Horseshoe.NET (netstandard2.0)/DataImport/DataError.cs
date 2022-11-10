namespace Horseshoe.NET.DataImport
{
    public class DataError
    {
        public string Message { get; }

        public DataError(string message)
        {
            Message = message;
        }

        public DataError(string message, int col, int sourceRow) : this("Col " + col + ", Source Row " + sourceRow + ": " + message)
        {
        }

        public string Print()
        {
            return Message;
        }

        public override string ToString()
        {
            return "(err)";
        }
    }
}
