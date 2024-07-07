namespace Horseshoe.NET.IO
{
    /// <summary>
    /// Set of values and utility methods for interpreting and displaying file sizes.
    /// </summary>
    public static class FileSize
    {
        /// <summary>
        /// Multiplier for kilobytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double KB = 1000D;

        /// <summary>
        /// Multiplier for megabytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double MB = KB * 1000D;

        /// <summary>
        /// Multiplier for gigabytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double GB = MB * 1000D;

        /// <summary>
        /// Multiplier for terabytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double TB = GB * 1000D;

        /// <summary>
        /// Multiplier for pedabytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double PB = TB * 1000D;

        /// <summary>
        /// Multiplier for exabytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double EB = PB * 1000D;

        /// <summary>
        /// Multiplier for kibibytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double KiB = 1024D;

        /// <summary>
        /// Multiplier for mebibytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double MiB = KiB * 1024D;

        /// <summary>
        /// Multiplier for gibibytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double GiB = MiB * 1024D;

        /// <summary>
        /// Multiplier for tebibytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double TiB = GiB * 1024D;

        /// <summary>
        /// Multiplier for pebibytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double PiB = TiB * 1024D;

        /// <summary>
        /// Multiplier for exbibytes, used in calculating unit-based file sizes.
        /// </summary>
        public const double EiB = PiB * 1024D;

        /// <summary>
        /// Detects the best size unit to go with in the formatting operation.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="bi"></param>
        /// <returns></returns>
        public static FileSizeUnit DeriveUnit(long? size, bool bi = false)
        {
            if (!size.HasValue) return FileSizeUnit.B;
            if (size < 1000L) return FileSizeUnit.B;
            if (bi)
            {
                if (size >= EiB) return FileSizeUnit.EiB;
                else if (size >= PiB) return FileSizeUnit.PiB;
                else if (size >= TiB) return FileSizeUnit.TiB;
                else if (size >= GiB) return FileSizeUnit.GiB;
                else if (size >= MiB) return FileSizeUnit.MiB;
                return FileSizeUnit.KiB;
            }
            else
            {
                if (size >= EB) return FileSizeUnit.EB;
                else if (size >= PB) return FileSizeUnit.PB;
                else if (size >= TB) return FileSizeUnit.TB;
                else if (size >= GB) return FileSizeUnit.GB;
                else if (size >= MB) return FileSizeUnit.MB;
                return FileSizeUnit.KB;
            }
        }
    }
}
