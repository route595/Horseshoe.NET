using System;

namespace Horseshoe.NET.IO.FileFilter
{
    public class FileDateModifiedFilter : IFileFilter
    {
        public DateTime? MinDate { get; }

        public DateTime? MaxDate { get; }

        public FilterMode FilterMode { get; }  // implements interface, otherwise serves no purpose

        public bool CaseSensitive => false;

        public FileDateModifiedFilter(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue && !maxDate.HasValue)
                throw new ValidationException("Invalid filter: a min date or max date (or both) must be supplied");
            //if (minDate > maxDate)
            //    throw new ValidationException("Invalid filter: min date cannot exceed max date");
            MinDate = minDate;
            MaxDate = maxDate;
        }

        public bool IsMatch(FilePath file)
        {
            if (MinDate.HasValue)
            {
                if (MaxDate < MinDate)  // account for switched min/max values
                    return file.DateModified <= MaxDate || file.DateModified >= MinDate;
                if (file.DateModified < MinDate)
                    return false;
            }
            if (MaxDate.HasValue && file.DateModified > MaxDate)
                return false;
            return true;
        }
    }
}
