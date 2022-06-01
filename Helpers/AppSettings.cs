using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace SwaggerServiceParser.Helpers
{
    public static class AppSettings
    {
        public async static Task<Settings> GetAppSettings(string appsettingsFileName)
        {
            using var settingsStream = new MemoryStream(File.ReadAllBytes(appsettingsFileName));
            var settings = await JsonSerializer.DeserializeAsync<Settings>(settingsStream);
            return settings;
        }
    }

    public class Settings
    {
        public string ApiServiceLocation { get; set; }
        public List<Service> ServiceList { get; set; }
    }

    public class Service
    {
        [JsonPropertyName("swagger")]
        public string Swagger { get; set; }
        [JsonPropertyName("api-path")]
        public string ApiPath { get; set; }
        [JsonPropertyName("folder-path")]
        public string FolderPath { get; set; }
    }
}