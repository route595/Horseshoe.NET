using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Horseshoe.NET.Text.Internal
{
    internal static class SystemTextJsonImpl
    {
        internal static string Serialize(object obj, bool indented = false)
        {
            var options = new JsonSerializerOptions { WriteIndented = indented };
            var json = JsonSerializer.Serialize(obj, options: options);
            return json;
        }

        internal async static Task<string> SerializeAsync(object obj, bool indented = false)
        {
            var options = new JsonSerializerOptions { WriteIndented = indented };
            var stream = new MemoryStream(1048000);
            await JsonSerializer.SerializeAsync(stream, obj, options: options);
            var json = Encoding.UTF8.GetString(stream.ToArray());
            return json;
        }

        internal static object Deserialize(string json, Type objectType, Func<string, string> onBeforeDeserialize = null)
        {
            if (json == null) return default;
            if (onBeforeDeserialize != null)
            {
                json = onBeforeDeserialize.Invoke(json);
            }
            var obj = JsonSerializer.Deserialize(json, objectType);
            return obj;
        }

        internal async static Task<object> DeserializeAsync(string json, Type objectType, Func<string, string> onBeforeDeserialize = null)
        {
            if (json == null) return default;
            if (onBeforeDeserialize != null)
            {
                json = onBeforeDeserialize.Invoke(json);
            }
            var bytes = Encoding.UTF8.GetBytes(json);
            var stream = new MemoryStream(bytes.Length);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            var obj = await JsonSerializer.DeserializeAsync(stream, objectType);
            return obj;
        }

        internal static E Deserialize<E>(string json, Func<string, string> onBeforeDeserialize = null)
        {
            if (json == null) return default;
            if (onBeforeDeserialize != null)
            {
                json = onBeforeDeserialize.Invoke(json);
            }
            var e = JsonSerializer.Deserialize<E>(json);
            return e;
        }

        internal async static Task<E> DeserializeAsync<E>(string json, Func<string, string> onBeforeDeserialize = null)
        {
            if (json == null) return default;
            if (onBeforeDeserialize != null)
            {
                json = onBeforeDeserialize.Invoke(json);
            }
            var bytes = Encoding.UTF8.GetBytes(json);
            var stream = new MemoryStream(bytes.Length);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            var e = await JsonSerializer.DeserializeAsync<E>(stream);
            return e;
        }
    }
}
