using System;

namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// A <c>FileFilter</c> implementation for filtering based on modified date.
    /// </summary>
    public class FileDateModifiedFilter : FileFilter
    {
        /// <summary>
        /// Minimum date.  If omitted, all modified dates than or equal the maximum modified date are a match.
        /// </summary>
        public DateTime? MinDate { get; }

        /// <summary>
        /// Maximum date.  If omitted, all modified dates greater than or equal the minimum modified date are a match.
        /// </summary>
        public DateTime? MaxDate { get; }

        /// <summary>
        /// Creates a new <c>FileSizeFilter</c>
        /// </summary>
        /// <param name="minDate">Minimum date.  If omitted, all modified dates than or equal the maximum modified date are a match.</param>
        /// <param name="maxDate">Maximum date.  If omitted, all modified dates greater than or equal the minimum modified date are a match.</param>
        /// <param name="filterMode">Optional, dictates which items to include based on criteria matching.</param>
        /// <exception cref="ValidationException"></exception>
        public FileDateModifiedFilter(DateTime? minDate, DateTime? maxDate, FilterMode filterMode = default)
        {
            // validation
            if (minDate.HasValue)
            {
                if (maxDate.HasValue)
                {
                    if (minDate > maxDate)
                        throw new ValidationException("Invalid filter: min date cannot exceed max date");
                }
            }
            else if (!maxDate.HasValue)
                throw new ValidationException("Invalid filter: a min date or max date (or both) must be supplied");

            MinDate = minDate;
            MaxDate = maxDate;
            FilterMode = filterMode;
        }

        /// <summary>
        /// Indicates whether the supplied file constitutes a critea match.
        /// </summary>
        /// <param name="file">a file path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public override bool IsMatch(FilePath file)
        {
            return (!MinDate.HasValue || file.DateModified >= MinDate) && (!MaxDate.HasValue || file.DateModified <= MaxDate);
        }
    }
}
