using System;
using System.Threading.Tasks;

using Horseshoe.NET.Text.Internal;

namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Factory methods for object deserialization (e.g. JSON, possibly XML in the future).
    /// </summary>
    public static class Deserialize
    {
        /// <summary>
        /// Deserializes an object from JSON (either <c>System.Text.Json</c> or <c>Newtonsoft.Json</c> is required).
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <param name="objectType">A reference type.</param>
        /// <param name="onBeforeDeserialize">An action to perform pre-deserialization.</param>
        /// <returns>An object deserialized from the supplied JSON string.</returns>
        /// <exception cref="UtilityException"></exception>
        public static object Json(string json, Type objectType = null, Func<string, string> onBeforeDeserialize = null)
        {
            switch (TextSettings.JsonProvider)
            {
                case JsonProvider.NewtonsoftJson:
                    return NewtonsoftJsonImpl.Deserialize(json, objectType: objectType, onBeforeDeserialize: onBeforeDeserialize);
                case JsonProvider.SystemTextJson:
                    if (objectType == null)
                    {
                        throw new UtilityException("The System.Text.Json provider requires 'objectType' to be specified.");
                    }
                    return SystemTextJsonImpl.Deserialize(json, objectType, onBeforeDeserialize: onBeforeDeserialize);
                default:
                    throw new UtilityException("No JSON provider.  Either TextSettings.JsonProvider == JsonProvider.None or no supported NuGet packages have been installed i.e. Newtonsoft.Json or System.Text.Json");
            }
        }

        /// <summary>
        /// Deserializes an object from JSON (either <c>System.Text.Json</c> or <c>Newtonsoft.Json</c> is required).
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <param name="objectType">A reference type.</param>
        /// <param name="onBeforeDeserialize">An action to perform pre-deserialization.</param>
        /// <returns>An object deserialized from the supplied JSON string.</returns>
        /// <exception cref="UtilityException"></exception>
        public async static Task<object> JsonAsync(string json, Type objectType = null, Func<string, string> onBeforeDeserialize = null)
        {
            switch (TextSettings.JsonProvider)
            {
                case JsonProvider.NewtonsoftJson:
                    var obj = await Task.FromResult
                    (
                        NewtonsoftJsonImpl.Deserialize(json, objectType: objectType, onBeforeDeserialize: onBeforeDeserialize)
                    );
                    return obj;
                case JsonProvider.SystemTextJson:
                    if (objectType == null)
                    {
                        throw new UtilityException("The System.Text.Json provider requires 'objectType' to be specified.");
                    }
                    return await SystemTextJsonImpl.DeserializeAsync(json, objectType, onBeforeDeserialize: onBeforeDeserialize);
                default:
                    throw new UtilityException("No JSON provider.  Either TextSettings.JsonProvider == JsonProvider.None or no supported NuGet packages have been installed i.e. Newtonsoft.Json or System.Text.Json");
            }
        }

        /// <summary>
        /// Deserializes an object from JSON (either <c>System.Text.Json</c> or <c>Newtonsoft.Json</c> is required).
        /// </summary>
        /// <typeparam name="E">A reference type.</typeparam>
        /// <param name="json">A JSON string.</param>
        /// <param name="onBeforeDeserialize">An action to perform pre-deserialization.</param>
        /// <returns>An object deserialized from the supplied JSON string.</returns>
        /// <exception cref="UtilityException"></exception>
        public static E Json<E>(string json, Func<string, string> onBeforeDeserialize = null)
        {
            switch (TextSettings.JsonProvider)
            {
                case JsonProvider.NewtonsoftJson:
                    return NewtonsoftJsonImpl.Deserialize<E>(json, onBeforeDeserialize: onBeforeDeserialize);
                case JsonProvider.SystemTextJson:
                    return SystemTextJsonImpl.Deserialize<E>(json, onBeforeDeserialize: onBeforeDeserialize);
                default:
                    throw new UtilityException("No JSON provider.  Either TextSettings.JsonProvider == JsonProvider.None or no supported NuGet packages have been installed i.e. Newtonsoft.Json or System.Text.Json");
            }
        }

        /// <summary>
        /// Deserializes an object from JSON (either <c>System.Text.Json</c> or <c>Newtonsoft.Json</c> is required).
        /// </summary>
        /// <typeparam name="E">A reference type.</typeparam>
        /// <param name="json">A JSON string.</param>
        /// <param name="onBeforeDeserialize">An action to perform pre-deserialization.</param>
        /// <returns>An object deserialized from the supplied JSON string.</returns>
        /// <exception cref="UtilityException"></exception>
        public async static Task<E> JsonAsync<E>(string json, Func<string, string> onBeforeDeserialize = null)
        {
            switch (TextSettings.JsonProvider)
            {
                case JsonProvider.NewtonsoftJson:
                    var obj = await Task.FromResult
                    (
                        NewtonsoftJsonImpl.Deserialize<E>(json, onBeforeDeserialize: onBeforeDeserialize)
                    );
                    return obj;
                case JsonProvider.SystemTextJson:
                    return await SystemTextJsonImpl.DeserializeAsync<E>(json, onBeforeDeserialize: onBeforeDeserialize);
                default:
                    throw new UtilityException("No JSON provider.  Either TextSettings.JsonProvider == JsonProvider.None or no supported NuGet packages have been installed i.e. Newtonsoft.Json or System.Text.Json");
            }
        }
    }
}
