using System.Text.Json;

namespace Silk.Helpers
{
    public static class JSON
    {
        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            AllowTrailingCommas = false,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        public static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, JsonSerializerOptions);
        }

        public static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
        }
    }
}
