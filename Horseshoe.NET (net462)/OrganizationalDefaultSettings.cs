using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Objects;

namespace Horseshoe.NET
{
    /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *  
     * Description:
     * 
     * You may wish to compile these toolkit libraries so you can "bake in" organizational 
     * defaults and add other customizations.  Here, in this file, exists the fastest and
     * easiest way to add organizational defaults (for example, SMPT server).
     * 
     * With these defaults set you will no longer have to modify a configuration file,
     * change any settings or supply the values as optional parameters to the appropriate 
     * utility methods.
     * 
     * You may, of course, set or override these defaults at any time via configuration file,
     * using settings' setters or by supplying the desired values as optional parameters to 
     * the utility methods they apply to.
     * 
     * Instructions: 
     * 
     * Uncomment the organizational defaults below that you would like to employ.
     * Modify the values accordingly, however, do NOT modify the keys and please
     * ensure the data type of the modified values matches that of the sample value.
     * 
     * Hint:
     * 
     * Before downloading a new version of the source code and compiling locally 
     * make a backup copy of this file.  Then, restore your customizations to the new,
     * unedited version of this file one at a time ensuring that each one has neither 
     * been renamed nor removed. Finally, check to see if new organizational defaults 
     * were added in the new version and if your organization can use them.
     * 
     * Then, copy your customizations from the backup file and paste them here. 
     * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

    public static class OrganizationalDefaultSettings
    {
        private static readonly IDictionary<string, object> _settings = new Dictionary<string, object>();

        static OrganizationalDefaultSettings()
        {
            //Set("ActiveDirectory.Domain", "mydomain.com");

            //Set("Bootstrap.ExceptionRendering", Bootstrap.ExceptionRenderingPolicy.Dynamic);
            //Set("Bootstrap.AutoCloseable", true);

            //Set("Caching.CacheDuration", 120);

            //Set("Cryptography.SymmetricAlgorithm", new System.Security.Cryptography.AesCryptoServiceProvider());
            //Set("Cryptography.HashAlgorithm", new System.Security.Cryptography.SHA256CryptoServiceProvider());
            //Set("Cryptography.Encoding", new UTF8Encoding());

            //Set("Email.SmtpServer", "smtp.myorganization.com");
            //Set("Email.SmtpPort", 25);
            //Set("Email.SmtpEnableSSL", false);
            //Set("Email.SmtpCredentials", new Credential("SmtpUser", "gyHt82bNmre457sjd709Aq1==", isEncryptedPassword: true));
            //Set("Email.From", "noreply@myorganization.com");
            //Set("Email.Footer", "This is an automated email message.  If you have questions please call customer service at 1-800-000-0000.");

            //Set("Ftp.Server", "ftp.myorganization.com");  // or "11.22.33.44"
            //Set("Ftp.Port", 21);
            //Set("Ftp.Credentials", new Credential("FtpUser", "gyHt82bNmre457sjd709Aq1==", isEncryptedPassword: true));
            //Set("Ftp.ServerPath", "/dir/subdir");  // or "//rootdir/subdir"

            //Set("Odbc.ConnectionString", "Driver={Oracle in OraClient11g_home1};Server=DBSVR01;Uid=OdbcUser;Pwd=gyHt82bNmre457sjd709Aq1==;");
            //Set("Odbc.IsEncryptedPassword", true);
            //Set("Odbc.DataSource", "DBSVR01");
            //Set("Odbc.Credentials", new Credential("OdbcUser", "gyHt82bNmre457sjd709Aq1==", isEncryptedPassword: true));
            //Set("Odbc.AdditionalConnectionAttributes", "Integrated Security=SSQI|Attribute1=Value1");
            //Set("Odbc.Timeout", 30);

            //Set("OleDb.ConnectionString", "Provider=OraOLEDB.Oracle;Server=DBSVR01;User ID=OleDbUser;Password=gyHt82bNmre457sjd709Aq1==;");
            //Set("OleDb.IsEncryptedPassword", true);
            //Set("OleDb.DataSource", "DBSVR01");
            //Set("OleDb.Credentials", new Credential("OleDbUser", "gyHt82bNmre457sjd709Aq1==", isEncryptedPassword: true));
            //Set("OleDb.AdditionalConnectionAttributes", "Integrated Security=SSQI|Attribute1=Value1");
            //Set("OleDb.Timeout", 30);

            //Set("OracleDb.ConnectionString", "Data Source=//11.22.33.44:9999/SERVICE1;User ID=OracleUser;Password=gyHt82bNmre457sjd709Aq1==;");  // EZConnect
            //Set("OracleDb.ConnectionString", "Data Source=ORADBSVR01;User ID=OracleUser;Password=gyHt82bNmre457sjd709Aq1==;");
            //Set("OracleDb.IsEncryptedPassword", true);
            //Set("OracleDb.Server", "ORADBSVR01");  // or 'NAME'11.22.33.44:9999;SERVICE1 or ORADBSVR02:9999;SERVICE1;INSTANCE1
            //Set("OracleDb.DataSource", "ORADBSVR01");
            //Set("OracleDb.Credentials", new Credential("OracleUser", "gyHt82bNmre457sjd709Aq1==", isEncryptedPassword: true));
            //Set("OracleDb.AdditionalConnectionAttributes", "Integrated Security=SSQI|Attribute1=Value1");
            //Set("OracleDb.Timeout", 30);
            //Set("OracleDb.AutoClearConnectionPool", true);
            //Set("OracleDb.ServerList", "ORADBSVR01|'NAME'11.22.33.44:9999;SERVICE1|ORADBSVR02:9999;SERVICE1;INSTANCE1");

            //Set("Sms.From", "sms@myorganization.com");

            //Set("SqlDb.ConnectionString", "Data Source;Server=DBSVR01;Initial Catalog=MyDatabase;User ID=SqlUser;Password=gyHt82bNmre457sjd709Aq1==;");
            //Set("SqlDb.IsEncryptedPassword", true);
            //Set("SqlDb.Server", "DBSVR01;2008R2");  // or 'NAME'11.22.33.44:9999;2012 or DBSVR01 (lookup / versionless)
            //Set("SqlDb.DataSource", "DBSVR01");
            //Set("SqlDb.InitialCatalog", "MyDatabase");
            //Set("SqlDb.Credentials", new Credential("SqlUser", "gyHt82bNmre457sjd709Aq1==", isEncryptedPassword: true));
            //Set("SqlDb.AdditionalConnectionAttributes", "Integrated Security=SSQI|Attribute1=Value1");
            //Set("SqlDb.Timeout", 30);
            //Set("SqlDb.ServerList", "DBSVR01|'NAME'11.22.33.44:9999;2012|DBSVR02;2008R2");

            //Set("Text.JsonProvider", "NewtonsoftJson");

            //Set("ReportingServices.ReportServer", "http://reports.mycompany.com/ReportServer");

            //Set("WebService.Credentials", new Credential("WebServiceUser", "gyHt82bNmre457sjd709Aq1==", isEncryptedPassword: true));
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "May be used by organizations who customize Horseshoe.NET via this class.")]
        static void Set(string key, object value)
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

        public static object Get(string key)
        {
            if (_settings.ContainsKey(key))
            {
                return _settings[key];
            }
            return null;
        }

        public static string GetString(string key)
        {
            return Get(key)?.ToString();
        }

        public static T Get<T>(string key, Func<object, T> parseFunc = null) where T : class
        {
            var obj = Get(key);
            if (obj == null) return null;
            if (parseFunc != null) return parseFunc.Invoke(obj);
            if (obj is T t) return t;
            if (obj is Type type) return (T)ObjectUtil.GetInstance(type);
            if (obj is string className) return ObjectUtil.GetInstance<T>(className);
            throw new UtilityException("Cannot convert " + obj.GetType().FullName + " to " + typeof(T).FullName);
        }

        public static T? GetNullable<T>(string key) where T : struct
        {
            var obj = Get(key);
            if (obj == null) return null;
            if (obj is T t) return t;
            if (typeof(T).IsEnum)
            {
                if (obj is string stringValue)
                {
                    return (T)Enum.Parse(typeof(T), stringValue);
                }
                if (obj is int)
                {
                    return (T)Enum.ToObject(typeof(T), obj);
                }
            }
            throw new UtilityException("Cannot convert " + obj.GetType().FullName + " to " + typeof(T).FullName);
        }

        public static int GetInt(string key, int defaultValue = default)
        {
            var intValue = GetNInt(key);
            return intValue ?? defaultValue;
        }

        public static int? GetNInt(string key)
        {
            if (_settings.ContainsKey(key))
            {
                var obj = _settings[key];
                if (obj == null) return null;
                return (int)obj;
            }
            return null;
        }

        public static int GetByte(string key, byte defaultValue = default)
        {
            var byteValue = GetNByte(key);
            return byteValue ?? defaultValue;
        }

        public static byte? GetNByte(string key)
        {
            if (_settings.ContainsKey(key))
            {
                var obj = _settings[key];
                if (obj == null) return null;
                return (byte)obj;
            }
            return null;
        }

        public static byte[] GetBytes(string key, Encoding encoding = null)
        {
            var obj = Get(key);
            if (obj == null) return null;
            if (obj is byte[] byteArray) return byteArray;
            if (obj is int[] intArray) return intArray.Select(i => (byte)i).ToArray();
            if (obj is string stringValue) return (encoding ?? Encoding.Default).GetBytes(stringValue);
            throw new UtilityException("Cannot convert " + obj.GetType().FullName + " to byte[]");
        }

        public static bool GetBoolean(string key, bool defaultValue = default)
        {
            var boolValue = GetNBoolean(key);
            return boolValue ?? defaultValue;
        }

        public static bool? GetNBoolean(string key)
        {
            if (_settings.ContainsKey(key))
            {
                var obj = _settings[key];
                if (obj == null) return null;
                return (bool)obj;
            }
            return null;
        }
    }
}
