namespace Horseshoe.NET.DataImport
{
    public class DataImportRow
    {
        public int RowNumber { get; set; }

        public string[] RawValues { get; set; }

        public object[] Values { get; set; }
    }
}
