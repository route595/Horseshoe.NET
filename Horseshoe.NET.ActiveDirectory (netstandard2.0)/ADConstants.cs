namespace Horseshoe.NET.ActiveDirectory
{
    public static class ADConstants
    {
        public static class MetaProperties
        {
            /// <summary>
            /// Key name of the fully qualified url
            /// </summary>
            public const string adspath = "adspath";  // always included in search results
            /// <summary>
            /// Key name of the name of the object
            /// </summary>
            public const string name = "name";
            /// <summary>
            /// Key name of the common name of the object
            /// </summary>
            public const string cn = "cn";
            /// <summary>
            /// Key name of the fully qualified path
            /// </summary>
            public const string distinguishedname = "distinguishedname";
            /// <summary>
            /// Key name of the SAM account name
            /// </summary>
            public const string samaccountname = "samaccountname";
            /// <summary>
            /// Key name of the created date/time
            /// </summary>
            public const string whencreated = "whencreated";
            /// <summary>
            /// Key name of the modified date/time
            /// </summary>
            public const string whenchanged = "whenchanged";
        }
        
        public static class UserProperties
        {
            public const string Default =
                "name|cn|distinguishedname|samaccountname|whencreated|whenchanged" +  // <- meta
                "|givenname|initial|sn|displayname|userprincipalname|mail";           // <- user
            public const string DefaultIncludingGroups =
                "name|cn|distinguishedname|samaccountname|whencreated|whenchanged" +  // <- meta
                "|givenname|initial|sn|displayname|userprincipalname|mail|memberof";  // <- user
            
            public const string givenname = "givenname";
            public const string initial = "initial";
            public const string sn = "sn";
            public const string displayname = "displayname";
            public const string userprincipalname = "userprincipalname";
            public const string mail = "mail";
            
            /// <summary>
            /// User groups (roles)
            /// </summary>
            public const string memberof = "memberof";
        }

        public static class UserSearchProperties
        {
            public const string GetDefault = "sAMAccountName|mail|userPrincipalName";
            public const string SearchDefault = "sAMAccountName|mail|userPrincipalName|sn|givenName";
        }

        public static class GroupProperties
        {
            public const string GetDefault =
                "name|cn|distinguishedname|samaccountname|whencreated|whenchanged" +  // <- meta
                "|member";                                                            // <- group
            public const string SearchDefault =
                "name|cn|distinguishedname|samaccountname|whencreated|whenchanged";   // <- meta

            /// <summary>
            /// Group members
            /// </summary>
            public const string member = "member";
        }

        // public static class GroupSearchProperties
        // {
        //     public const string cn = "cn";
        // }
    }
}