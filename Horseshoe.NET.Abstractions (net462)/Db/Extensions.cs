using System;
using System.Data;
using System.Globalization;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Extension methods for Horsehoe.NET.Db and consumers
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the string value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnName">A column name.</param>
        /// <returns>The string value of the column.</returns>
        public static string ZapStringColumn(this IDataReader reader, string columnName)
        {
            return Zap.String(reader[columnName]);
        }

        /// <summary>
        /// Returns the string value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="index">A column index.</param>
        /// <returns>The string value of the column.</returns>
        public static string ZapStringColumn(this IDataReader reader, int index)
        {
            return Zap.String(reader[index]);
        }

        /// <summary>
        /// Returns the string value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="columnName">A column name.</param>
        /// <returns>The string value of the column.</returns>
        public static string ZapStringColumn(this DataRow row, string columnName)
        {
            return Zap.String(row[columnName]);
        }

        /// <summary>
        /// Returns the string value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="index">A column index.</param>
        /// <returns>The string value of the column.</returns>
        public static string ZapStringColumn(this DataRow row, int index)
        {
            return Zap.String(row[index]);
        }

        /// <summary>
        /// Returns the int value of the specified column.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The int value of the column.</returns>
        public static int ZapIntColumn(this IDataReader reader, string columnName, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.Int(reader[columnName], force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the int value of the specified column.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="index">A column index.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The int value of the column.</returns>
        public static int ZapIntColumn(this IDataReader reader, int index, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.Int(reader[index], force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the int value of the specified column.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The int value of the column.</returns>
        public static int ZapIntColumn(this DataRow row, string columnName, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.Int(row[columnName], force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the int value of the specified column.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="index">A column index.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The int value of the column.</returns>
        public static int ZapIntColumn(this DataRow row, int index, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.Int(row[index], force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the nullable int value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The nullable int value of the column.</returns>
        public static int? ZapNIntColumn(this IDataReader reader, string columnName, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.NInt(reader[columnName], force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the nullable int value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="index">A column index.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The nullable int value of the column.</returns>
        public static int? ZapNIntColumn(this IDataReader reader, int index, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.NInt(reader[index], force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the nullable int value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The nullable int value of the column.</returns>
        public static int? ZapNIntColumn(this DataRow row, string columnName, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.NInt(row[columnName], force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the nullable int value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="index">A column index.</param>
        /// <param name="force">If <c>true</c>, casts units larger than <c>int</c> to <c>int</c> regardless if the value is greater than the max value of <c>int</c> or less than the min value, the default is <c>false</c>.</param>
        /// <param name="numberStyle">If supplied, indicates the expected number format.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The nullable int value of the column.</returns>
        public static int? ZapNIntColumn(this DataRow row, int index, bool force = false, NumberStyles? numberStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.NInt(row[index], force: force, numberStyle: numberStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the date/time value of the specified column.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The date/time value of the column.</returns>
        public static DateTime ZapDateTimeColumn(this IDataReader reader, string columnName, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.DateTime(reader[columnName], dateTimeStyle: dateTimeStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the date/time value of the specified column.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="index">A column index.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The date/time value of the column.</returns>
        public static DateTime ZapDateTimeColumn(this IDataReader reader, int index, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.DateTime(reader[index], dateTimeStyle: dateTimeStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the date/time value of the specified column.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The date/time value of the column.</returns>
        public static DateTime ZapDateTimeColumn(this DataRow row, string columnName, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.DateTime(row[columnName], dateTimeStyle: dateTimeStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the date/time value of the specified column.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="index">A column index.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The date/time value of the column.</returns>
        public static DateTime ZapDateTimeColumn(this DataRow row, int index, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.DateTime(row[index], dateTimeStyle: dateTimeStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the nullable date/time value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The nullable date/time value of the column.</returns>
        public static DateTime? ZapNDateTimeColumn(this IDataReader reader, string columnName, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.NDateTime(reader[columnName], dateTimeStyle: dateTimeStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the nullable date/time value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="index">A column index.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The nullable date/time value of the column.</returns>
        public static DateTime? ZapNDateTimeColumn(this IDataReader reader, int index, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.NDateTime(reader[index], dateTimeStyle: dateTimeStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the nullable date/time value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The nullable date/time value of the column.</returns>
        public static DateTime? ZapNDateTimeColumn(this DataRow row, string columnName, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.NDateTime(row[columnName], dateTimeStyle: dateTimeStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the nullable date/time value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="index">A column index.</param>
        /// <param name="dateTimeStyle">Defines the formatting options that customize string parsing for some date and time parsing methods.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is used to set a value for <c>provider</c> if not supplied.</param>
        /// <returns>The nullable date/time value of the column.</returns>
        public static DateTime? ZapNDateTimeColumn(this DataRow row, int index, DateTimeStyles? dateTimeStyle = null, IFormatProvider provider = null, string locale = null)
        {
            return Zap.NDateTime(row[index], dateTimeStyle: dateTimeStyle, provider: provider, locale: locale);
        }

        /// <summary>
        /// Returns the bool value of the specified column.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="delimiter">The <c>char</c> used to delimit <c>trueValues</c> and <c>falseValues</c>.</param>
        /// <param name="treatArbitraryAsFalse">If <c>true</c>, allows any value not in <c>trueValues</c> to return <c>false</c>, default is <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns>The bool value of the column.</returns>
        public static bool ZapBoolColumn(this IDataReader reader, string columnName, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", char delimiter = '|', bool treatArbitraryAsFalse = false, bool ignoreCase = true)
        {
            return Zap.Bool(reader[columnName], trueValues: trueValues, falseValues: falseValues, delimiter: delimiter, treatArbitraryAsFalse: treatArbitraryAsFalse, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Returns the bool value of the specified column.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="index">A column index.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="delimiter">The <c>char</c> used to delimit <c>trueValues</c> and <c>falseValues</c>.</param>
        /// <param name="treatArbitraryAsFalse">If <c>true</c>, allows any value not in <c>trueValues</c> to return <c>false</c>, default is <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns>The bool value of the column.</returns>
        public static bool ZapBoolColumn(this IDataReader reader, int index, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", char delimiter = '|', bool treatArbitraryAsFalse = false, bool ignoreCase = true)
        {
            return Zap.Bool(reader[index], trueValues: trueValues, falseValues: falseValues, delimiter: delimiter, treatArbitraryAsFalse: treatArbitraryAsFalse, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Returns the bool value of the specified column.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="delimiter">The <c>char</c> used to delimit <c>trueValues</c> and <c>falseValues</c>.</param>
        /// <param name="treatArbitraryAsFalse">If <c>true</c>, allows any value not in <c>trueValues</c> to return <c>false</c>, default is <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns>The bool value of the column.</returns>
        public static bool ZapBoolColumn(this DataRow row, string columnName, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", char delimiter = '|', bool treatArbitraryAsFalse = false, bool ignoreCase = true)
        {
            return Zap.Bool(row[columnName], trueValues: trueValues, falseValues: falseValues, delimiter: delimiter, treatArbitraryAsFalse: treatArbitraryAsFalse, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Returns the bool value of the specified column.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="index">A column index.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="delimiter">The <c>char</c> used to delimit <c>trueValues</c> and <c>falseValues</c>.</param>
        /// <param name="treatArbitraryAsFalse">If <c>true</c>, allows any value not in <c>trueValues</c> to return <c>false</c>, default is <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns>The bool value of the column.</returns>
        public static bool ZapBoolColumn(this DataRow row, int index, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", char delimiter = '|', bool treatArbitraryAsFalse = false, bool ignoreCase = true)
        {
            return Zap.Bool(row[index], trueValues: trueValues, falseValues: falseValues, delimiter: delimiter, treatArbitraryAsFalse: treatArbitraryAsFalse, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Returns the nullable bool value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="delimiter">The <c>char</c> used to delimit <c>trueValues</c> and <c>falseValues</c>.</param>
        /// <param name="treatArbitraryAsFalse">If <c>true</c>, allows any value not in <c>trueValues</c> to return <c>false</c>, default is <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns>The nullable bool value of the column.</returns>
        public static bool? ZapNBoolColumn(this IDataReader reader, string columnName, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", char delimiter = '|', bool treatArbitraryAsFalse = false, bool ignoreCase = true)
        {
            return Zap.NBool(reader[columnName], trueValues: trueValues, falseValues: falseValues, delimiter: delimiter, treatArbitraryAsFalse: treatArbitraryAsFalse, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Returns the nullable bool value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="reader">A data reader.</param>
        /// <param name="index">A column index.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="delimiter">The <c>char</c> used to delimit <c>trueValues</c> and <c>falseValues</c>.</param>
        /// <param name="treatArbitraryAsFalse">If <c>true</c>, allows any value not in <c>trueValues</c> to return <c>false</c>, default is <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns>The nullable bool value of the column.</returns>
        public static bool? ZapNBoolColumn(this IDataReader reader, int index, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", char delimiter = '|', bool treatArbitraryAsFalse = false, bool ignoreCase = true)
        {
            return Zap.NBool(reader[index], trueValues: trueValues, falseValues: falseValues, delimiter: delimiter, treatArbitraryAsFalse: treatArbitraryAsFalse, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Returns the nullable bool value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="columnName">A column name.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="delimiter">The <c>char</c> used to delimit <c>trueValues</c> and <c>falseValues</c>.</param>
        /// <param name="treatArbitraryAsFalse">If <c>true</c>, allows any value not in <c>trueValues</c> to return <c>false</c>, default is <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns>The nullable bool value of the column.</returns>
        public static bool? ZapNBoolColumn(this DataRow row, string columnName, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", char delimiter = '|', bool treatArbitraryAsFalse = false, bool ignoreCase = true)
        {
            return Zap.NBool(row[columnName], trueValues: trueValues, falseValues: falseValues, delimiter: delimiter, treatArbitraryAsFalse: treatArbitraryAsFalse, ignoreCase: ignoreCase);
        }

        /// <summary>
        /// Returns the nullable bool value of the specified column.  <c>DBNull</c> and blank values are returned as <c>null</c>.
        /// </summary>
        /// <param name="row">A data row.</param>
        /// <param name="index">A column index.</param>
        /// <param name="trueValues">A delimited list of <c>string</c> values that evaluate to <c>true</c>.</param>
        /// <param name="falseValues">A delimited list of <c>string</c> values that evaluate to <c>false</c>.</param>
        /// <param name="delimiter">The <c>char</c> used to delimit <c>trueValues</c> and <c>falseValues</c>.</param>
        /// <param name="treatArbitraryAsFalse">If <c>true</c>, allows any value not in <c>trueValues</c> to return <c>false</c>, default is <c>false</c>.</param>
        /// <param name="ignoreCase">Whether to ignore the letter case of <c>trueValues</c> and <c>falseValues</c>, default is <c>false</c>.</param>
        /// <returns>The nullable bool value of the column.</returns>
        public static bool? ZapNBoolColumn(this DataRow row, int index, string trueValues = "y|yes|t|true|1", string falseValues = "n|no|f|false|0", char delimiter = '|', bool treatArbitraryAsFalse = false, bool ignoreCase = true)
        {
            return Zap.NBool(row[index], trueValues: trueValues, falseValues: falseValues, delimiter: delimiter, treatArbitraryAsFalse: treatArbitraryAsFalse, ignoreCase: ignoreCase);
        }
    }
}
