using System.Security.Cryptography;
using System.Text;

using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// Settings for crypto operations - sources are client-supplied values, configuration and organizational default settings
    /// </summary>
    public static class CryptoSettings
    {
        private static SymmetricAlgorithm _defaultSymmetricAlgorithm;

        /// <summary>
        /// Gets or sets the default symmetric algorithm used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Crypto:SymmetricAlgorithm and OrganizationalDefaultSettings: key = Crypto.SymmetricAlgorithm)
        /// </summary>
        public static SymmetricAlgorithm DefaultSymmetricAlgorithm
        {
            get
            {
                if (_defaultSymmetricAlgorithm == null)
                {
                    _defaultSymmetricAlgorithm = CryptoUtil.BuildNSymmetricAlgorithm
                    (
                        _Config.GetInstance<SymmetricAlgorithm>("Horseshoe.NET:Crypto:SymmetricAlgorithm"),   // e.g. "System.Security.Cryptography.AesCryptoServiceProvider"
                        _Config.Get<byte[]>("Horseshoe.NET:Crypto:SymmetricKey", encoding: DefaultEncoding),
                        false,
                        _Config.Get<byte[]>("Horseshoe.NET:Crypto:SymmetricIV", encoding: DefaultEncoding),
                        true,
                        _Config.Get<int?>("Horseshoe.NET:Crypto:SymmetricBlockSize"),
                        _Config.Get<CipherMode>("Horseshoe.NET:Crypto:SymmetricCipherMode"),
                        _Config.Get<PaddingMode>("Horseshoe.NET:Crypto:SymmetricPadding")
                    );
                }
                return _defaultSymmetricAlgorithm 
                    ?? OrganizationalDefaultSettings.GetInstance<SymmetricAlgorithm>("Crypto.SymmetricAlgorithm")
                    ?? CryptoUtil.BuildSymmetricAlgorithm(new AesManaged(), DefaultEncoding.GetBytes("k+ (&tw!tBv~$6u7"), false, null, true, null, null, null);
            }
            set
            {
                _defaultSymmetricAlgorithm = value;
            }
        }

        private static HashAlgorithm _defaultHashAlgorithm;

        /// <summary>
        /// Gets or sets the default hash algorithm used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Crypto:HashAlgorithm and OrganizationalDefaultSettings: key = Crypto.HashAlgorithm)
        /// </summary>
        public static HashAlgorithm DefaultHashAlgorithm
        {
            get
            {
                return _defaultHashAlgorithm  // example "System.Security.Cryptography.SHA256CryptoServiceProvider"
                    ?? _Config.GetInstance<HashAlgorithm>("Horseshoe.NET:Crypto:HashAlgorithm")
                    ?? OrganizationalDefaultSettings.GetInstance<HashAlgorithm>("Crypto.HashAlgorithm")
                    ?? new SHA1CryptoServiceProvider();
            }
            set
            {
                _defaultHashAlgorithm = value;
            }
        }

        static byte? _defaultHashSalt;

        /// <summary>
        /// Gets or sets the default hash salt used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Crypto:HashSalt and OrganizationalDefaultSettings: key = Crypto.HashSalt)
        /// </summary>
        public static byte? DefaultHashSalt
        {
            get
            {
                return _defaultHashSalt
                    ?? _Config.Get<byte?>("Horseshoe.NET:Crypto:HashSalt")    // example: 240 or HashSalt[hex] F0
                    ?? OrganizationalDefaultSettings.Get<byte?>("Crypto.HashSalt");
            }
            set
            {
                _defaultHashSalt = value;
            }
        }

        static Encoding _defaultEncoding;

        /// <summary>
        /// Gets or sets the text encoding used by Cryptography. Defaults to UTF8Encoding. Note: Override by passing directly to a Cryptography function or via config file: key = "Horseshoe.NET:Crypto:Encoding" or OrganizationalDefaultSettings: key = Crypto.Encoding
        /// </summary>
        public static Encoding DefaultEncoding
        {
            get
            {
                var encodingClassNameFromConfig = _Config.Get("Horseshoe.NET:Crypto:Encoding");   // example: "System.Text.UTF8Encoding"
                return _defaultEncoding
                    ?? (encodingClassNameFromConfig != null ? (Encoding)TypeUtil.GetInstance(encodingClassNameFromConfig) : null)
                    ?? OrganizationalDefaultSettings.Get<Encoding>("Crypto.Encoding")
                    ?? Encoding.Default;
            }
            set
            {
                _defaultEncoding = value;
            }
        }
    }
}
