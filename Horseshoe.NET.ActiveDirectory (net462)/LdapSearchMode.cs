namespace Horseshoe.NET.ActiveDirectory
{
    /// <summary>
    /// How to search property values in LDAP
    /// </summary>
    public enum LdapSearchMode
    {
        /// <summary>
        /// The property value contains the search <c>string</c>
        /// </summary>
        Contains,

        /// <summary>
        /// The property value contains the search <c>string</c> at the beginning
        /// </summary>
        StartsWith,

        /// <summary>
        /// The property value contains the search <c>string</c> at the end
        /// </summary>
        EndsWith
    }
}