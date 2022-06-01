
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.Json;

namespace SwaggerServiceParser.Helpers
{
    public class EndPoint
    {
        public EndPointType EndPointType { get; set; }
        public string Json { get; set; }
        public string Controller { get; set; }
        public string Method { get; set; }
    }

    public static class EndPointList
    {
        public async static Task<List<EndPoint>> GetEndPoints(string json)
        {
            using var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            PathsContainer paths = await JsonSerializer.DeserializeAsync<PathsContainer>(jsonStream);
            if (paths == null) throw new Exception($"Json object is null: {json}");

            var list = new List<EndPoint>();
            foreach (var ep in paths.EndPoints)
            {
                var split = SplitEndPoint(ep.Key);

                var lines = ep.Value.ToString().Split('\n');
                var endPointTypeDescription = lines[1].Trim().Split(':')[0].Trim('"');

                list.Add(new EndPoint
                {
                    EndPointType = GetEndPointTypeByDescription(endPointTypeDescription),
                    Json = ep.Value.ToString(),
                    Controller = split.Controller,
                    Method = split.Method
                });
            }
            return list;
        }

        private static EndPointType GetEndPointTypeByDescription(string description)
        {
            foreach (EndPointType e in Enum.GetValues(typeof(EndPointType)))
            {
                if (description.ToLower() == EnumHelper.GetAttribute<EndPointAttribute>(e).Description.ToLower()) return e;
            }
            return EndPointType.Get;
        }

        private static (string Controller, string Method) SplitEndPoint(string endpoint)
        {
            var arr = endpoint.Split('/');
            if (arr.Length != 3) return ("", "");
            return (arr[1].Trim().Trim('/'), arr[2].Trim().Trim('/'));
        }

        private class PathsContainer
        {
            [JsonPropertyName("paths")]
            public Dictionary<string, object> EndPoints { get; set; }
        }
    }
}