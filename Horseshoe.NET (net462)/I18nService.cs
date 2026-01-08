namespace Horseshoe.NET
{
    /// <summary>
    /// Wrapper for Horseshoe.NET's service for maintaining and retrieving internationalized strings based on calling assembly, culture and key.
    /// (Internal use only.)
    /// </summary>
    internal static class I18nService
    {
        private static readonly I18nRelay _relay;

        static I18nService()
        {
            _relay = new I18nRelay();
            AddOrReplace(I18nKey.BankTransfer, null, "Bank Transfer");          // English
            AddOrReplace(I18nKey.BankTransfer, "es", "Transferencia Bancaria"); // Spanish
            AddOrReplace(I18nKey.BankTransfer, "de", "Banküberweisung");        // German
        }

        /// <inheritdoc cref="I18nRelay.Get(string, bool)"/>
        public static string Get(I18nKey key, bool strict = false) =>
            _relay.Get(key.ToString(), strict: strict);

        /// <inheritdoc cref="I18nRelay.Get(string, string, bool)"/>
        public static string Get(I18nKey key, string culture, bool strict = false) =>
            _relay.Get(key.ToString(), culture, strict: strict);

        /// <inheritdoc cref="I18nRelay.AddOrReplace(string, string)"/>
        public static void AddOrReplace(I18nKey key, string value) =>
            _relay.AddOrReplace(key.ToString(), value);

        /// <inheritdoc cref="I18nRelay.AddOrReplace(string, string, string)"/>
        public static void AddOrReplace(I18nKey key, string culture, string value) =>
            _relay.AddOrReplace(key.ToString(), culture, value);
    }
}
