using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Linq;

namespace SwaggerServiceParser.Helpers
{
    public class Component
    {
        public string Name { get; set; }
        public string[] Parameters { get; set; }
    }

    public static class ComponentList
    {
        public static async Task<List<Component>> GetComponentList(string json)
        {
            using var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            ComponentJson componentJson = await JsonSerializer.DeserializeAsync<ComponentJson>(jsonStream);

            var componentList = new List<Component>();
            foreach (var comps in componentJson.SchemaJson.Components)
            {
                componentList.Add(new Component
                {
                    Name = comps.Key,
                    Parameters = comps.Value.Properties.Select(p => p.Key).ToArray()
                });
            }
            return componentList;
        }

        private class ComponentJson
        {
            [JsonPropertyName("components")]
            public SchemaJson SchemaJson { get; set; }
        }

        private class SchemaJson
        {
            [JsonPropertyName("schemas")]
            public Dictionary<string, PropertiesJson> Components { get; set; }
        }

        private class PropertiesJson
        {
            [JsonPropertyName("properties")]
            public Dictionary<string, object> Properties { get; set; }
        }
    }
}