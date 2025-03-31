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
        /// <param name="name">Column name</param>
        /// <param name="displayFormat">Optional display date format (e.g. "G", "M/d/yyyy", etc.)</param>
        /// <param name="displayFormatProvider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="displayLocale">An optional locale in which to display the date (e.g. "en-US")</param>
        /// <returns></returns>
        public static Column ExcelDate
        (
            string name,
            string displayFormat = null,
            IFormatProvider displayFormatProvider = null,
            string displayLocale = null
        ) => 
        Column.DateTime
        (
            name: name,
            sourceParser: (obj) => ExcelImportUtil.ConvertToDateTime(obj, suppressErrors: false),
            displayFormat: displayFormat,
            displayFormatProvider: displayFormatProvider,
            displayLocale: displayLocale
        );
    }
}
