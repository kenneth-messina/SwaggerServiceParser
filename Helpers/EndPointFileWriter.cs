using System.Text;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace SwaggerServiceParser.Helpers
{
    public class EndPointFileWriter
    {
        private string[] _parameters { get; set; }
        private EndPoint _endPoint { get; set; }
        private string HttpCommand() { return EnumHelper.GetAttribute<EndPointAttribute>(_endPoint.EndPointType).Description; }

        private string GetParametersString(bool noBraketOverride = false)
        {
            var addBrakets = _endPoint.EndPointType == EndPointType.Get || _endPoint.EndPointType == EndPointType.Delete;
            var paramString = string.Join(", ", _parameters);
            return addBrakets && !noBraketOverride ? $"{{ {paramString} }}" : paramString;
        }

        private class HttpGetParameterContainer
        {
            [JsonPropertyName("parameters")]
            public Parameter[] Parameters { get; set; }
        }

        private class Parameter
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public async Task Parse(string apiPath, EndPoint endpoint, List<Component> componentList)
        {
            var lines = endpoint.Json.Split('\n');

            if ((endpoint.EndPointType == EndPointType.Put || endpoint.EndPointType == EndPointType.Post) && endpoint.Json.Contains("\"requestBody\":"))
            {
                var objName = lines.FirstOrDefault(l => l.Trim().StartsWith("\"$ref\":"));
                if (objName != null && objName.Length > 0) _parameters = new string[] { objName = objName.Substring(objName.LastIndexOf('/') + 1).Trim().Trim('\"').ToCamelCase() };
                if (lines.Any(l => l.Contains("multipart/form-data"))) _parameters = new string[] { "formData" };
            }

            if ((endpoint.EndPointType == EndPointType.Get || endpoint.EndPointType == EndPointType.Delete) && endpoint.Json.Contains("\"parameters\":"))
            {
                var parameterLines = new StringBuilder();
                var adding = false;
                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("\"parameters\": [")) adding = true;
                    if (adding) parameterLines.AppendLine(line);
                    if (adding && line.Trim().StartsWith("],")) break;
                }

                using var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes($"{{{parameterLines.ToString().Trim().Trim(',')}}}"));
                HttpGetParameterContainer parameterContainer = await JsonSerializer.DeserializeAsync<HttpGetParameterContainer>(jsonStream);

                _parameters = parameterContainer.Parameters.Select(p => p.Name.ToCamelCase()).ToArray();
            }

            _endPoint = endpoint;
        }

        public string GetFunctionCall()
        {
            var sb = new StringBuilder();

            if (_parameters == null || _parameters.Length == 0)
            {
                sb.AppendLine($"  async {_endPoint.Method}() {{");
                sb.AppendLine($"    return await apiSerivce.{HttpCommand().ToCamelCase()}(prefix + '{_endPoint.Method}')");
            }
            else
            {
                sb.AppendLine($"  async {_endPoint.Method}({GetParametersString(true)}) {{");
                sb.AppendLine($"    return await apiSerivce.{HttpCommand().ToCamelCase()}(prefix + '{_endPoint.Method}', {GetParametersString()})");
            }
            sb.AppendLine($"  }},");
            return sb.ToString();
        }
    }
}