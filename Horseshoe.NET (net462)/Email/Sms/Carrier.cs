namespace Horseshoe.NET.Email.Sms
{
    /// <summary>
    /// Known mobile carriers with email-to-SMS text messaging
    /// </summary>
    /// <remarks>
    /// Sources: 
    /// <para>
    /// <a href="http://acme.highpoint.edu/~msetzler/IntroPSC/introReads/hacknmod.com%20-%20email-to-text-messages-for-att-verizon.pdf">http://acme.highpoint.edu/~msetzler/IntroPSC/introReads/hacknmod.com%20-%20email-to-text-messages-for-att-verizon.pdf</a>
    /// </para>
    /// <para>
    /// <a href="https://www.digitaltrends.com/mobile/how-to-send-a-text-from-your-email-account/">https://www.digitaltrends.com/mobile/how-to-send-a-text-from-your-email-account/</a>
    /// </para>
    /// <para>
    /// <a href="https://www.reddit.com/r/mintmobile/comments/ps74h0/what_is_mint_mobiles_emailtotext_domain/">https://www.reddit.com/r/mintmobile/comments/ps74h0/what_is_mint_mobiles_emailtotext_domain/</a>
    /// </para>
    /// </remarks>
    public enum Carrier
    {
        /// <summary>
        /// Alltel
        /// </summary>
        Alltel,

        /// <summary>
        /// AT&amp;T
        /// </summary>
        ATT,

        /// <summary>
        /// Boost
        /// </summary>
        Boost,

        /// <summary>
        /// Cricket Wireless
        /// </summary>
        CricketWireless,

        /// <summary>
        /// Google Project FI
        /// </summary>
        GoogleProjectFI,

        /// <summary>
        /// Mint Mobile
        /// </summary>
        MintMobile,

        /// <summary>
        /// Nextel
        /// </summary>
        Nextel,

        /// <summary>
        /// Republic Wireless
        /// </summary>
        RepublicWireless,

        /// <summary>
        /// Spectrum Mobile
        /// </summary>
        SpectrumMobile,

        /// <summary>
        /// Sprint PCS
        /// </summary>
        SprintPCS,

        /// <summary>
        /// StraightTalk
        /// </summary>
        StraightTalk,

        /// <summary>
        /// Ting
        /// </summary>
        Ting,

        /// <summary>
        /// T-Mobile
        /// </summary>
        TMobile,

        /// <summary>
        /// US Cellular
        /// </summary>
        USCellular,

        /// <summary>
        /// Verizon
        /// </summary>
        Verizon,

        /// <summary>
        /// Virgin Mobile
        /// </summary>
        VirginMobile
    }
}
