using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Horseshoe.NET.Collections;
using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET
{
    /// <summary>
    /// You may wish to compile Horseshoe.NET and "bake in" organizational defaults
    /// (for example, a corporate SMPT server) precluding the need for adding such
    /// values via confguration file. This class exists for that purpose.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Alternatively, you may set or override these defaults at any time via 
    /// configuration file, using settings' setters or by supplying the desired values as 
    /// optional parameters to the utility methods they apply to.
    /// </para>
    /// <para>
    /// Instructions:
    /// </para>
    /// <para>
    /// Uncomment the organizational defaults below that you would like to employ.
    /// Modify the values accordingly, however, do NOT modify the keys and please
    /// ensure the data type of the modified values still matches that of the sample value.
    /// </para>
    /// <para>
    /// Hint:
    /// </para>
    /// <para>
    /// Before downloading a new version of this source code and re-compiling 
    /// make a backup copy of this file. Then, restore your customizations to the new
    /// version of this file one at a time ensuring that each one has neither
    /// been renamed nor removed. Finally, check to see if new organizational defaults
    /// were added in the new version and if your organization can use them.
    /// </para>
    /// <para>
    /// Then, copy your customizations from the backup file and paste them here.
    /// </para>
    /// </remarks>
    public static class OrganizationalDefaultSettings
    {
        private static readonly IDictionary<string, object> _settings = new Dictionary<string, object>();

        static OrganizationalDefaultSettings()
        {
            //Set("ActiveDirectory.Domain", "mydomain.com");                                                                                         // source: Horseshoe.NET.ActiveDirectory > ADSettings.cs

            //Set("Bootstrap.ExceptionRendering", Bootstrap.ExceptionRenderingPolicy.Dynamic);                                                       // source: Bootstrap/BootstrapSettings.cs
            //Set("Bootstrap.AutoCloseableAlerts", true);                                                                                            // source: Bootstrap/BootstrapSettings.cs

            //Set("Caching.CacheDuration", 120);                                                                                                     // source: Horseshoe.NET.Caching > CacheSettings.cs

            //Set("Crypto.SymmetricAlgorithm", typeof(System.Security.Cryptography.AesCryptoServiceProvider));                                         // source: Crypto/CryptoSettings.cs
            //Set("Crypto.Encoding", typeof(System.TextUTF8Encoding));                                                                                            // source: Crypto/CryptoSettings.cs
            //Set("Crypto.HashAlgorithm", typeof(System.Security.Cryptography.SHA256CryptoServiceProvider));                                           // source: Crypto/CryptoSettings.cs
            //Set("Crypto.HashSalt", (byte)24);                                                                                                      // source: Crypto/CryptoSettings.cs

            //Set("Email.SmtpServer", "smtp.myorganization.com");                                                                                    // source: Email/EmailSettings.cs
            //Set("Email.SmtpPort", 25);                                                                                                             // source: Email/EmailSettings.cs
            //Set("Email.EnableSsl", false);                                                                                                         // source: Email/EmailSettings.cs
            //Set("Email.Credentials", new Credential("SmtpUser", new Password("gyHt82bNmre457sjd709Aq1==", isEncrypted: true)));                    // source: Email/EmailSettings.cs
            //Set("Email.From", "noreply@myorganization.com");                                                                                       // source: Email/EmailSettings.cs
            //Set("Email.Footer", "This is an automated email message.  If you have questions please call customer service at 1-800-000-0000.");     // source: Email/EmailSettings.cs

            //Set("Ftp.Server", "ftp.myorganization.com");  // or "11.22.33.44"                                                                      // source: Ftp/FtpSettings.cs
            //Set("Ftp.Port", 21);                                                                                                                   // source: Ftp/FtpSettings.cs
            //Set("Ftp.EnableSsl", false);                                                                                                           // source: Ftp/FtpSettings.cs
            //Set("Ftp.Credentials", new Credential("FtpUser", new Password("gyHt82bNmre457sjd709Aq1==", isEncrypted: true)));                       // source: Ftp/FtpSettings.cs
            //Set("Ftp.ServerPath", "/dir/subdir" -or- "//rootdir/subdir");                                                                          // source: Ftp/FtpSettings.cs

            //Set("Http.Domain", "mydomain.com");                                                                                                    // source: Horseshoe.NET.Http > HttpSettings.cs
            //Set("Http.Credentials", new Credential("WebServiceUser", new Password("gyHt82bNmre457sjd709Aq1==", isEncrypted: true)));               // source: Horseshoe.NET.Http > HttpSettings.cs

            //Set("Odbc.ConnectionString", "Driver={Oracle in OraClient11g_home1};Server=DBSVR01;Uid=OdbcUser;Pwd=gyHt82bNmre457sjd709Aq1==;");      // source: Horseshoe.NET.Odbc > OdbcSettings.cs
            //Set("Odbc.IsEncryptedPassword", true);                                                                                                 // source: Horseshoe.NET.Odbc > OdbcSettings.cs
            //Set("Odbc.DataSource", "DBSVR01");                                                                                                     // source: Horseshoe.NET.Odbc > OdbcSettings.cs
            //Set("Odbc.Credentials", new Credential("OdbcUser", new Password("gyHt82bNmre457sjd709Aq1==", isEncrypted: true)));                     // source: Horseshoe.NET.Odbc > OdbcSettings.cs
            //Set("Odbc.AdditionalConnectionAttributes", "Integrated Security=SSQI|Attribute1=Value1");                                              // source: Horseshoe.NET.Odbc > OdbcSettings.cs
            //Set("Odbc.ConnectionTimeout", 30);                                                                                                     // source: Horseshoe.NET.Odbc > OdbcSettings.cs

            //Set("OleDb.ConnectionString", "Provider=OraOLEDB.Oracle;Server=DBSVR01;User ID=OleDbUser;Password=gyHt82bNmre457sjd709Aq1==;");        // source: OleDb/OleDbSettings.cs
            //Set("OleDb.IsEncryptedPassword", true);                                                                                                // source: OleDb/OleDbSettings.cs
            //Set("OleDb.DataSource", "DBSVR01");                                                                                                    // source: OleDb/OleDbSettings.cs
            //Set("OleDb.Credentials", new Credential("OleDbUser", new Password("gyHt82bNmre457sjd709Aq1==", isEncrypted: true)));                   // source: OleDb/OleDbSettings.cs
            //Set("OleDb.AdditionalConnectionAttributes", "Integrated Security=SSQI|Attribute1=Value1");                                             // source: OleDb/OleDbSettings.cs
            //Set("OleDb.ConnectionTimeout", 30);                                                                                                    // source: OleDb/OleDbSettings.cs

            //                                  EZConnect
            //Set("OracleDb.ConnectionString", "Data Source=//11.22.33.44:9999/SERVICE1;User ID=OracleUser;Password=gyHt82bNmre457sjd709Aq1==;");    // source: Horseshoe.NET.OracleDb > OracleDbSettings.cs
            //Set("OracleDb.ConnectionString", "Data Source=ORADBSVR01;User ID=OracleUser;Password=gyHt82bNmre457sjd709Aq1==;");                     // source: Horseshoe.NET.OracleDb > OracleDbSettings.cs
            //Set("OracleDb.IsEncryptedPassword", true);                                                                                             // source: Horseshoe.NET.OracleDb > OracleDbSettings.cs
            //Set("OracleDb.Server", "ORADBSVR01");  // or 'NAME'11.22.33.44:9999;SERVICE1 or ORADBSVR02:9999;SERVICE1;INSTANCE1                     // source: Horseshoe.NET.OracleDb > OracleDbSettings.cs
            //Set("OracleDb.DataSource", "ORADBSVR01");                                                                                              // source: Horseshoe.NET.OracleDb > OracleDbSettings.cs
            //Set("OracleDb.Credentials", new Credential("OracleUser", new Password("gyHt82bNmre457sjd709Aq1==", isEncrypted: true)));               // source: Horseshoe.NET.OracleDb > OracleDbSettings.cs
            //Set("OracleDb.AdditionalConnectionAttributes", "Integrated Security=SSQI|Attribute1=Value1");                                          // source: Horseshoe.NET.OracleDb > OracleDbSettings.cs
            //Set("OracleDb.ConnectionTimeout", 30);                                                                                                 // source: Horseshoe.NET.OracleDb > OracleDbSettings.cs
            //Set("OracleDb.AutoClearConnectionPool", true);                                                                                         // source: Horseshoe.NET.OracleDb > OracleDbSettings.cs
            //Set("OracleDb.ServerList", "ORADBSVR01|'NAME'11.22.33.44:9999;SERVICE1|ORADBSVR02:9999;SERVICE1;INSTANCE1");                           // source: Horseshoe.NET.OracleDb > OracleDbSettings.cs

            //Set("Sms.From", "sms@myorganization.com");                                                                                             // source: Email/Sms/SmsSettings.cs

            //Set("SqlDb.ConnectionString", "Data Source;Server=DBSVR01;Initial Catalog=MyDatabase;User ID=SqlUser;Password=gyHt82bNmre457sjd709Aq1==;"); // source: Horseshoe.NET.SqlDb > SqlDbSettings.cs
            //Set("SqlDb.IsEncryptedPassword", true);                                                                                                // source: Horseshoe.NET.SqlDb > SqlDbSettings.cs
            //Set("SqlDb.Server", "DBSVR01;2008R2");  // or 'NAME'11.22.33.44:9999;2012 or DBSVR01 (lookup / versionless)                            // source: Horseshoe.NET.SqlDb > SqlDbSettings.cs
            //Set("SqlDb.DataSource", "DBSVR01");                                                                                                    // source: Horseshoe.NET.SqlDb > SqlDbSettings.cs
            //Set("SqlDb.InitialCatalog", "MyDatabase");                                                                                             // source: Horseshoe.NET.SqlDb > SqlDbSettings.cs
            //Set("SqlDb.Credentials", new Credential("SqlUser", new Password("gyHt82bNmre457sjd709Aq1==", isEncrypted: true)));                     // source: Horseshoe.NET.SqlDb > SqlDbSettings.cs
            //Set("SqlDb.AdditionalConnectionAttributes", "Integrated Security=SSQI|Attribute1=Value1");                                             // source: Horseshoe.NET.SqlDb > SqlDbSettings.cs
            //Set("SqlDb.ConnectionTimeout", 30);                                                                                                    // source: Horseshoe.NET.SqlDb > SqlDbSettings.cs
            //Set("SqlDb.ServerList", "DBSVR01|'NAME'11.22.33.44:9999;2012|DBSVR02;2008R2");                                                         // source: Horseshoe.NET.SqlDb > SqlDbSettings.cs

            //Set("Text.JsonProvider", "NewtonsoftJson" or Horseshoe.NET.Text.JsonProvicer.NewtonsoftJson);                                          // source: Text/TextSettings.cs

            //Set("ReportingServices.ReportServer", "http://reports.mycompany.com/ReportServer");                                                    // source: Horseshoe.NET.Http > ReportingServices/ReportSettings.cs
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "May be used by organizations who customize Horseshoe.NET via this class.")]
        private static void Set<T>(string key, T value)
        {
            if (value != null)
            {
                _settings.AddOrReplace(key, value);
            }
            else
            {
                _settings.Remove(key);
            }
        }

        /// <summary>
        /// Gets an organizational default setting.
        /// </summary>
        /// <param name="key">The lookup key.</param>
        /// <returns>The value corresponding to <c>key</c>.</returns>
        public static T Get<T>(string key)
        {
            if (_settings.ContainsKey(key))
            {
                return (T)_settings[key];
            }
            return default;
        }

        /// <summary>
        /// Gets an organizational default setting.
        /// </summary>
        /// <param name="key">The lookup key.</param>
        /// <param name="nonPublic">If <c>true</c>, a public or nonpublic default constructor can be used, default is <c>false</c> (public default constructor only).</param>
        /// <returns>An instance of the type specified by <c>key</c>.</returns>
        public static T GetInstance<T>(string key, bool nonPublic = false) where T : class
        {
            var type = Get<Type>(key);
            if (type == null) 
                return default;
            type = Zap.Type(type, inheritedType: typeof(T));
            return (T)TypeUtil.GetDefaultInstance(type, nonPublic: nonPublic);
        }
    }
}
