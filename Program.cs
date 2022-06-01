using System;
using System.Linq;
using System.Threading.Tasks;
using SwaggerServiceParser.Helpers;

namespace SwaggerServiceParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Writing Services...");

                var appSettings = await AppSettings.GetAppSettings("appsettings.json");

                foreach (var service in appSettings.ServiceList)
                {
                    var json = await Swagger.GetSwaggerJson(service.Swagger);
                    var endpoints = await EndPointList.GetEndPoints(json);
                    var componentList = await ComponentList.GetComponentList(json);
                    var controllerNames = endpoints.Select(e => e.Controller).Distinct().ToList();

                    FileWriter.CleanFiles(service.FolderPath, controllerNames);
                    endpoints.ForEach(async ep => await FileWriter.WriteServiceToFile(service, ep, componentList, appSettings.ApiServiceLocation));
                    FileWriter.FinishFiles(service.FolderPath, controllerNames);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
