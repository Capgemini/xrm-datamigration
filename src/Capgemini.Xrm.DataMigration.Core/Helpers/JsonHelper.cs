using System.IO;
using Capgemini.Xrm.DataMigration.Config;
using Newtonsoft.Json;

namespace Capgemini.Xrm.DataMigration.Helpers
{
    public static class JsonHelper
    {
        public static void Serialize(object value, Stream s)
        {
            using (StreamWriter writer = new StreamWriter(s))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = JsonSerializerConfig.SerializerSettings.Formatting;

                JsonSerializer ser = new JsonSerializer
                {
                    Formatting = JsonSerializerConfig.SerializerSettings.Formatting,
                    NullValueHandling = JsonSerializerConfig.SerializerSettings.NullValueHandling,
                    TypeNameHandling = JsonSerializerConfig.SerializerSettings.TypeNameHandling
                };

                ser.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }

        public static T Deserialize<T>(Stream s)
        {
            using (StreamReader reader = new StreamReader(s))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer ser = new JsonSerializer
                {
                    Formatting = JsonSerializerConfig.SerializerSettings.Formatting,
                    NullValueHandling = JsonSerializerConfig.SerializerSettings.NullValueHandling,
                    TypeNameHandling = JsonSerializerConfig.SerializerSettings.TypeNameHandling
                };

                return ser.Deserialize<T>(jsonReader);
            }
        }
    }
}