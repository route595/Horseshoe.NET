namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Constant values for database methods
    /// </summary>
    public static class DbConstants
    {
        internal static string MessageRelayGroup { get; } = typeof(DbConstants).Namespace;

        /// <summary>
        /// Constant values for database method message relays
        /// </summary>
        public static class MessageRelay
        {
            /// <summary>
            /// Relay message id for generated connection strings
            /// </summary>
            public const int GENERATED_CONNECTION_STRING = 1030000010;

            /// <summary>
            /// Relay message id for the source of a generated connection string
            /// </summary>
            public const int CONNECTION_STRING_SOURCE = 1030000011;

            /// <summary>
            /// Relay message id for generated INSERT statements
            /// </summary>
            public const int GENERATED_INSERT_STATEMENT = 1030000021;

            /// <summary>
            /// Relay message id for generated INSERT "get identity" statements
            /// </summary>
            public const int GENERATED_INSERT_GET_IDENTIY_STATEMENT = 1030000022;

            /// <summary>
            /// Relay message id for generated UPDATE statements
            /// </summary>
            public const int GENERATED_UPDATE_STATEMENT = 1030000023;

            /// <summary>
            /// Relay message id for generated table function statements
            /// </summary>
            public const int GENERATED_FUNCTION_STATEMENT = 1030000030;
        }
    }
}
