using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Horseshoe.NET.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace TestMVC
{
    public static class Extensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, Serialize.Json(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null 
                ? default 
                : Deserialize.Json<T>(value);
        }
    }
}
