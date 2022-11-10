using System;

using Horseshoe.NET.DataImport;

namespace Horseshoe.NET.Excel
{
    public static class ExcelColumn
    {
        public static Column ExcelDate(string name, int fixedWidth = 0) => new Column(name)
        {
            FixedWidth = fixedWidth,
            DataType = typeof(DateTime),
            Converter = (obj) => ExcelImportUtil.ConvertToDateTime(obj, suppressErrors: false),
            Formatter = Column.DateFormatNoMilliseconds
        };
    }
}
