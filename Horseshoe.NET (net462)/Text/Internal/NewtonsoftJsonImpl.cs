using System;

using Newtonsoft.Json;

namespace Horseshoe.NET.Text.Internal
{
    internal static class NewtonsoftJsonImpl
    {
        internal static string Serialize(object obj, bool indented = false)
        {
            var json = JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
            return json;
        }

        internal static object Deserialize(string json, Type objectType = null, Func<string, string> onBeforeDeserialize = null)
        {
            if (json == null) return null;
            if (onBeforeDeserialize != null)
            {
                json = onBeforeDeserialize.Invoke(json);
            }
            var obj = JsonConvert.DeserializeObject(json, objectType);
            return obj;
        }

        internal static E Deserialize<E>(string json, Func<string, string> onBeforeDeserialize = null)
        {
            if (json == null) return default;
            if (onBeforeDeserialize != null)
            {
                json = onBeforeDeserialize.Invoke(json);
            }
            var e = JsonConvert.DeserializeObject<E>(json);
            return e;
        }
    }
}
