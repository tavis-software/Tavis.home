using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.SelfHost;
using Tavis;

namespace WebApiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
           

            var baseAddress = "http://localhost:8080";
            var config = new HttpSelfHostConfiguration(baseAddress);

            config.Routes.MapHttpRoute("defaultWithId", "api/{controller}/{id}");
            config.Routes.MapHttpRoute("default", "api/{controller}");
            //config.Routes.MapHttpRoute("default", "api/{controller}/{id}", new {id= RouteParameter.Optional});


            var server = new HttpSelfHostServer(config);


            Console.WriteLine("Opening server at " + baseAddress);
            server.OpenAsync().Wait();

            Console.WriteLine("Press enter to shutdown server");
            Console.ReadLine();
            server.CloseAsync().Wait();


        }
    }
}
