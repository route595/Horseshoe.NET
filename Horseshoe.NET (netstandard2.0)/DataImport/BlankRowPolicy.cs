namespace Horseshoe.NET.DataImport
{
    public enum BlankRowPolicy
    {
        Allow,
        Drop,
        DropLeading,
        DropTrailing,
        DropLeadingAndTrailing,
        StopImporting,
        Error
    }
}
