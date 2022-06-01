using System.Threading.Tasks;
using System.Net.Http;

namespace SwaggerServiceParser.Helpers
{
    public static class Swagger
    {
        public static Task<string> GetSwaggerJson(string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            return client.GetStringAsync(url);
        }
    }
}