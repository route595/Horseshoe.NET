using System;

using Horseshoe.NET.DataImport;

namespace Horseshoe.NET.Excel
{
    /// <summary>
    /// Represents Excel data column metadata, responsible for file-to-object parsing as well as object-to-text formatting
    /// </summary>
    public static class ExcelColumn
    {
        /// <summary>
        /// Creates a basic <c>DateTime</c> <c>Column</c> for dates
        /// </summary>
        /// <param name="name">column name</param>
        /// <param name="fixedWidth">optional column width</param>
        /// <param name="displayFormat">custom date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayLocale">a locale (e.g. "en-US")</param>
        /// <returns></returns>
        public static Column ExcelDate(string name, int fixedWidth = 0, string displayFormat = null, string displayLocale = null) => new Column(name)
        {
            FixedWidth = fixedWidth,
            DataType = typeof(DateTime),
            Converter = (obj) => ExcelImportUtil.ConvertToDateTime(obj, suppressErrors: false),
            Formatter = (o) => Column.DateFormat(o, format: displayFormat, locale: displayLocale)
        };
    }
}
