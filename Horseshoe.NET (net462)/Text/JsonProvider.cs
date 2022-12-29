namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Which JSON library is installed or which one to use if both are installed.
    /// </summary>
    public enum JsonProvider
    {
        /// <summary>
        /// No JSON library was installed
        /// </summary>
        None,

        /// <summary>
        /// Uses Newtonsoft.Json library
        /// </summary>
        NewtonsoftJson,

        /// <summary>
        /// Uses System.Text.Json library
        /// </summary>
        SystemTextJson
    }
}
