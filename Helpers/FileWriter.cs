using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SwaggerServiceParser.Helpers
{
    public static class FileWriter
    {
        public static string GetServiceFileName(string folderPath, string controller)
        {
            return Path.Combine(folderPath, $"{controller}Service.js");
        }

        public static void CleanFiles(string folderPath, List<string> controllerNames)
        {
            foreach (var controller in controllerNames)
            {
                var fileName = GetServiceFileName(folderPath, controller);
                if (File.Exists(fileName)) File.Delete(fileName);
            }
        }

        public static async Task WriteServiceToFile(Service service, EndPoint endPoint, List<Component> componentList, string apiSerivceLocation)
        {
            var fileName = GetServiceFileName(service.FolderPath, endPoint.Controller);

            if (File.Exists(fileName) == false)
            {
                var fs = File.Create(fileName);
                fs.Close();
                var sb = new StringBuilder();
                sb.AppendLine($"import apiSerivce from '{apiSerivceLocation}'");
                sb.AppendLine();
                sb.AppendLine($"const prefix = '/{service.ApiPath.Trim('/')}/{endPoint.Controller}/'");
                sb.AppendLine();
                sb.AppendLine($"const {endPoint.Controller}Service = {{");
                File.WriteAllText(fileName, sb.ToString());
            }

            var endPointFileWriter = new EndPointFileWriter();
            await endPointFileWriter.Parse(service.ApiPath, endPoint, componentList);
            File.AppendAllText(fileName, endPointFileWriter.GetFunctionCall());
        }

        public static void FinishFiles(string folderPath, List<string> controllerNames)
        {
            foreach (var controller in controllerNames)
            {
                var fileName = GetServiceFileName(folderPath, controller);
                var sb = new StringBuilder();
                sb.AppendLine("}");
                sb.AppendLine();
                sb.AppendLine($"export default {controller}Service");
                File.AppendAllText(fileName, sb.ToString());

                Console.WriteLine($"Completed: {fileName}");
            }
        }
    }
}