using Horseshoe.NET.Configuration;

namespace Horseshoe.NET.Email.Sms
{
    /// <summary>
    /// Configuration settings for Horseshoe.NET.Email.Sms
    /// </summary>
    public static class SmsSettings
    {
        static string _defaultFrom;

        /// <summary>
        /// Gets or sets the default sender address used by SMS.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sms:From and OrganizationalDefaultSettings: key = Sms.From)
        /// </summary>
        public static string DefaultFrom
        {
            get
            {
                return _defaultFrom
                    ?? Config.Get("Horseshoe.NET:Sms:From");
            }
            set
            {
                _defaultFrom = value;
            }
        }
    }
}
