using System.Globalization;

namespace Horseshoe.NET.Finance
{
    public static class Settings
    {
        public static bool IncludeInterestPaymentsInSnowballOutput { get; set; }
        public static bool IncludePrincipalPaymentsInSnowballOutput { get; set; }
        public static int NumberDecimalDigits { get; set; } = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
        public static int CurrencyDecimalDigits { get; set; } = NumberFormatInfo.CurrentInfo.CurrencyDecimalDigits;
    }
}
