namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Connection string element pseudo-names
    /// </summary>
    public enum ConnectionStringPart
    {
        /// <summary>
        /// Data Source, Server or DSN
        /// </summary>
        DataSource,

        /// <summary>
        /// User ID or UID
        /// </summary>
        UserId,

        /// <summary>
        /// Password or PWD
        /// </summary>
        Password,

        /// <summary>
        /// Driver
        /// </summary>
        Driver,

        /// <summary>
        /// Provider
        /// </summary>
        Provider,

        /// <summary>
        /// Initial Catalog or Database
        /// </summary>
        InitialCatalog
    }
}
