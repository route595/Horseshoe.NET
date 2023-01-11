using Microsoft.Office.Interop.Excel;

namespace Horseshoe.NET.Excel.Interop
{
    public enum ExcelFileTypeByExtension
    {
        Xls = XlFileFormat.xlExcel8,
        Xlt = XlFileFormat.xlTemplate,
        Xlsx = XlFileFormat.xlOpenXMLWorkbook,
        Xltx = XlFileFormat.xlOpenXMLTemplate,
        Xlsm = XlFileFormat.xlOpenXMLWorkbookMacroEnabled,
        Xlsb = XlFileFormat.xlExcel12,
        Csv = XlFileFormat.xlCSV
    }
}
