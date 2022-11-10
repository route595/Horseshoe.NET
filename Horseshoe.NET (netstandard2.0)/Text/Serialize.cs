using System.Threading.Tasks;

using Horseshoe.NET.Text.Internal;

namespace Horseshoe.NET.Text
{
    public static class Serialize
    {
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
