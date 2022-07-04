
using Newtonsoft.Json;

namespace Capgemini.Xrm.DataMigration.Config
{
    public static class JsonSerializerConfig
    {
        private static JsonSerializerSettings serializerSettings;

        public static JsonSerializerSettings SerializerSettings
        {
            get
            {
                if (serializerSettings == null)
                {
                    serializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.None,
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    };
                }

                return serializerSettings;
            }
        }
    }
}