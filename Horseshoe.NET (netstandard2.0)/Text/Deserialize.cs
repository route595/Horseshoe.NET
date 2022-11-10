using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text.Internal;

namespace Horseshoe.NET.Text
{
    public static class Deserialize
    {
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
