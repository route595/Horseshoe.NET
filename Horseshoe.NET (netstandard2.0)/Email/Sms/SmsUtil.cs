using System;
using System.Text.RegularExpressions;

using Horseshoe.NET.Text.TextClean;

namespace Horseshoe.NET.Email.Sms
{
    /// <summary>
    /// Utility methods for email-to-sms messaging
    /// </summary>
    public static class SmsUtil
    {
        /// <summary>
        /// Gets a carrier's name based off its <c>enum</c> value
        /// </summary>
        /// <param name="carrier">A carrier <c>enum</c> value</param>
        /// <returns></returns>
        public static string GetDescription(Carrier carrier)
        {
            switch (carrier)
            {
                case Carrier.ATT:
                    return "AT&T";
                case Carrier.MintMobile:
                    return "Mint Mobile";
                case Carrier.RepublicWireless:
                    return "Republic Wireless";
                case Carrier.SpectrumMobile:
                    return "Spectrum Mobile";
                case Carrier.SprintPCS:
                    return "Sprint PCS";
                case Carrier.StraightTalk:
                    return "Straight Talk";
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

        /// <summary>
        /// Gets a carrier's <c>enum</c> value based off its name
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static Carrier ResolveCarrier(string description)
        {
            switch (description)
            {
                case "AT&T":
                    return Carrier.ATT;
                case "T-Mobile":
                    return Carrier.TMobile;
                default:
                    return (Carrier)Enum.Parse(typeof(Carrier), TextClean.RemoveWhitespace(description));
            }
        }

        /// <summary>
        /// Builds the email address from the mobile number and carrier's SMS gateway
        /// </summary>
        /// <param name="mobileNumber">A mobile number</param>
        /// <param name="carrier">A carrier</param>
        /// <returns></returns>
        public static string BuildTextRecipientAddress(string mobileNumber, Carrier carrier)
        {
            mobileNumber = ValidateMobileNumber(mobileNumber);
            if (mobileNumber.Length == 11)
            {
                mobileNumber = mobileNumber.Substring(1);
            }
            return mobileNumber + "@" + GetSmsGateway(carrier);
        }

        private static Regex _usMobileNumberPattern = new Regex("(?<=^[+]?1[ .]?)?(([0-9]{10}$)|((((\\([0-9]{3}\\))|[0-9]{3})[ .-]?[0-9]{3}[-.][0-9]{4}$)))");

        /// <summary>
        /// Accepts a US 10-digit phone number in a variety of formats and extracts just the digits (excluding +1) for 
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException">If not a valid 10-digit US phone number</exception>
        public static string ValidateMobileNumber(string mobileNumber)
        {
            mobileNumber = mobileNumber.Trim();
            var match = _usMobileNumberPattern.Match(mobileNumber.Trim());
            if (match == null || !match.Success)
            {
                throw new ValidationException("Not a valid 10-digit US phone number: " + mobileNumber);
            }
            mobileNumber = mobileNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
            return mobileNumber;
        }

        /// <summary>
        /// Gets the SMS gateway for the specified carrier (e.g. the 'example.com' in '5554443333@example.com')
        /// </summary>
        /// <param name="carrier">A carrier</param>
        /// <returns></returns>
        /// <exception cref="ValidationException">If the supplied carrier is not associated with an SMS gateway</exception>
        public static string GetSmsGateway(Carrier carrier)
        {
            switch (carrier)
            {
                case Carrier.Alltel:
                    return "message.alltel.com";
                case Carrier.ATT:
                    return "txt.att.net";
                case Carrier.Boost:
                    return "sms.myboostmobile.com";
                case Carrier.CricketWireless:
                    return "sms.cricketwireless.net";
                case Carrier.GoogleProjectFI:
                    return "msg.fi.google.com";
                case Carrier.MintMobile:
                    return "mailmymobile.net";
                case Carrier.Nextel:
                    return "messaging.nextel.com";
                case Carrier.RepublicWireless:
                    return "text.republicwireless.com";
                case Carrier.SprintPCS:
                    return "messaging.sprintpcs.com";
                case Carrier.Ting:
                    return "message.ting.com";
                case Carrier.TMobile:
                    return "tmomail.net";
                case Carrier.USCellular:
                    return "email.uscc.net";
                case Carrier.SpectrumMobile:
                case Carrier.StraightTalk:
                case Carrier.Verizon:
                    return "vtext.com";
                case Carrier.VirginMobile:
                    return "vmobl.com";
            }
            throw new ValidationException("The supplied carrier is not associated with an SMS gateway: " + carrier);
        }
    }
}
