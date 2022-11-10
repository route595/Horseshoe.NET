namespace Horseshoe.NET.IO
{
    public static class FileSize
    {
        public const double KB = 1000D;
        public const double MB = KB * 1000D;
        public const double GB = MB * 1000D;
        public const double TB = GB * 1000D;
        public const double PB = TB * 1000D;
        public const double EB = PB * 1000D;
        public const double KiB = 1024D;
        public const double MiB = KiB * 1024D;
        public const double GiB = MiB * 1024D;
        public const double TiB = GiB * 1024D;
        public const double PiB = TiB * 1024D;
        public const double EiB = PiB * 1024D;

        public enum Unit
        {
            B,
            KB,
            MB,
            GB,
            TB,
            PB,
            EB,
            KiB,
            MiB,
            GiB,
            TiB,
            PiB,
            EiB
        }

        public static Unit DeriveUnit(long size, bool bi)
        {
            if (size < 1000L) return Unit.B;
            if (bi)
            {
                if (size >= EiB) return Unit.EiB;
                else if (size >= PiB) return Unit.PiB;
                else if (size >= TiB) return Unit.TiB;
                else if (size >= GiB) return Unit.GiB;
                else if (size >= MiB) return Unit.MiB;
                return Unit.KiB;
            }
            else
            {
                if (size >= EB) return Unit.EB;
                else if (size >= PB) return Unit.PB;
                else if (size >= TB) return Unit.TB;
                else if (size >= GB) return Unit.GB;
                else if (size >= MB) return Unit.MB;
                return Unit.KB;
            }
        }

        public static bool IsBI(this Unit unit)
        {
            switch (unit)
            {
                case Unit.KiB:
                case Unit.MiB:
                case Unit.GiB:
                case Unit.TiB:
                case Unit.PiB:
                case Unit.EiB:
                    return true;
                default:
                    return false;
            }
        }

        public static double ToMultiplier(this Unit unit)
        {
            switch (unit)
            {
                case Unit.KB: return KB;
                case Unit.MB: return MB;
                case Unit.GB: return GB;
                case Unit.TB: return TB;
                case Unit.PB: return PB;
                case Unit.EB: return EB;
                case Unit.KiB: return KiB;
                case Unit.MiB: return MiB;
                case Unit.GiB: return GiB;
                case Unit.TiB: return TiB;
                case Unit.PiB: return PiB;
                case Unit.EiB: return EiB;
                default: return 1D; // byte
            }
        }
    }
}
