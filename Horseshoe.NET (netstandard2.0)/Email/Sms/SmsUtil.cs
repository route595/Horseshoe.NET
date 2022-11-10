using System;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.Email.Sms
{
    public static class SmsUtil
    {
        public static string GetDescription(Carrier carrier)
        {
            switch (carrier)
            {
                case Carrier.ATT:
                    return "AT&T";
                case Carrier.SprintPCS:
                    return "Sprint PCS";
                case Carrier.TMobile:
                    return "T-Mobile";
                case Carrier.USCellular:
                    return "US Cellular";
                case Carrier.VirginMobile:
                    return "Virgin Mobile";
                default:
                    return carrier.ToString();
            }
        }

        public static Carrier ResolveCarrier(string description)
        {
            switch (description)
            {
                case "AT&T":
                    return Carrier.ATT;
                case "Sprint PCS":
                    return Carrier.SprintPCS;
                case "T-Mobile":
                    return Carrier.TMobile;
                case "US Cellular":
                    return Carrier.USCellular;
                case "Virgin Mobile":
                    return Carrier.VirginMobile;
                default:
                    return (Carrier)Enum.Parse(typeof(Carrier), description);
            }
        }

        public static string BuildTextRecipientAddress(string mobileNumber, Carrier carrier)
        {
            mobileNumber = ValidateMobileNumber(mobileNumber);
            if (mobileNumber.Length == 11)
            {
                mobileNumber = mobileNumber.Substring(1);
            }
            return mobileNumber + "@" + GetEmailDomain(carrier);
        }

        private static Regex TenOrElevenDigitUsMobileNumberPattern => new Regex(@"^((\d{10,11})|((1-)?\d{3}-\d{3}-\d{4})|((1\-?)?\(\d{3}\)\d{3}-\d{4}))$");

        public static string ValidateMobileNumber(string mobileNumber)
        {
            mobileNumber = mobileNumber.Trim();
            var match = TenOrElevenDigitUsMobileNumberPattern.Match(mobileNumber);
            if (match == null || !match.Success)
            {
                throw new UtilityException("Not a valid 10-digit US phone number: " + mobileNumber);
            }
            mobileNumber = mobileNumber.Replace("(", "").Replace(")", "").Replace("-", "");
            return mobileNumber;
        }

        private static string GetEmailDomain(Carrier carrier)
        {
            switch (carrier)
            {
                case Carrier.Alltel:
                    return "message.alltel.com";
                case Carrier.ATT:
                    return "txt.att.net";
                case Carrier.Boost:
                    return "myboostmobile.com";
                case Carrier.Nextel:
                    return "messaging.nextel.com";
                case Carrier.SprintPCS:
                    return "messaging.sprintpcs.com";
                case Carrier.TMobile:
                    return "tmomail.net";
                case Carrier.USCellular:
                    return "email.uscc.net";
                case Carrier.Verizon:
                    return "vtext.com";
                case Carrier.VirginMobile:
                    return "vmobl.com";
            }
            throw new UtilityException("This should never happen...");
        }
    }
}
