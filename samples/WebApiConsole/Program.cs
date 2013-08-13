using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.SelfHost;

namespace WebApiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = "http://localhost:8080";
            var config = new HttpSelfHostConfiguration(baseAddress);

            config.Routes.MapHttpRoute("default", "api/{controller}");
            
            var server = new HttpSelfHostServer(config);



            IApiExplorer apiExplorer = config.Services.GetApiExplorer();

            foreach (ApiDescription api in apiExplorer.ApiDescriptions)
    {
        Console.WriteLine("Uri path: {0}", api.RelativePath);
        Console.WriteLine("HTTP method: {0}", api.HttpMethod);
        foreach (ApiParameterDescription parameter in api.ParameterDescriptions)
        {
            Console.WriteLine("Parameter: {0} - {1}", parameter.Name, parameter.Source);
        }
        Console.WriteLine();
   }

            Console.WriteLine("Opening server at " + baseAddress);
            server.OpenAsync().Wait();

            Console.WriteLine("Press enter to shutdown server");
            Console.ReadLine();
            server.CloseAsync().Wait();


        }
    }
}
