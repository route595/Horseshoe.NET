using System.Threading.Tasks;

using Horseshoe.NET.Text.Internal;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Factory methods for object serialization (e.g. JSON, possibly XML in the future).
    /// </summary>
    public static class Serialize
    {
        /// <summary>
        /// Serializes an object to JSON (either <c>System.Text.Json</c> or <c>Newtonsoft.Json</c> is required).
        /// </summary>
        /// <param name="obj">An object to serialize.</param>
        /// <param name="indented">Whether to render serialized result in human-friendly indented style.</param>
        /// <returns>A JSON <c>string</c> serialized from <c>obj</c>.</returns>
        /// <exception cref="UtilityException"></exception>
        public static string Json(object obj, bool indented = true)
        {
            string json;
            switch (TextSettings.JsonProvider)
            {
                case JsonProvider.NewtonsoftJson:
                    json = NewtonsoftJsonImpl.Serialize(obj, indented: indented);
                    return json;
                case JsonProvider.SystemTextJson:
                    json = SystemTextJsonImpl.Serialize(obj, indented: indented);
                    return json;
                default:
                    throw new UtilityException("No JSON provider.  Either TextSettings.JsonProvider == JsonProvider.None or no supported NuGet packages have been installed i.e. Newtonsoft.Json or System.Text.Json");
            }
        }

        /// <summary>
        /// Serializes an object to JSON (either <c>System.Text.Json</c> or <c>Newtonsoft.Json</c> is required).
        /// </summary>
        /// <param name="obj">An object to serialize.</param>
        /// <param name="indented">Whether to render serialized result in human-friendly indented style.</param>
        /// <returns>A JSON <c>string</c> serialized from <c>obj</c>.</returns>
        /// <exception cref="UtilityException"></exception>
        public async static Task<string> JsonAsync(object obj, bool indented = true)
        {
            string json;
            switch (TextSettings.JsonProvider)
            {
                case JsonProvider.NewtonsoftJson:
                    json = await Task.FromResult
                    ( 
                        NewtonsoftJsonImpl.Serialize(obj, indented: indented)
                    );
                    return json;
                case JsonProvider.SystemTextJson:
                    json = await SystemTextJsonImpl.SerializeAsync(obj, indented: indented);
                    return json;
                default:
                    throw new UtilityException("No JSON provider.  Either TextSettings.JsonProvider == JsonProvider.None or no supported NuGet packages have been installed i.e. Newtonsoft.Json or System.Text.Json");
            }
        }
    }
}
