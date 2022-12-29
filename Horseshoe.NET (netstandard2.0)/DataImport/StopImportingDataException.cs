namespace Horseshoe.NET.DataImport
{
    /// <summary>
    /// Certain conditions may trigger this exception to gracefully end the data import process. It is caught by the system, the definition of a benign exception.
    /// </summary>
    public class StopImportingDataException : BenignException 
    {
    }
}
