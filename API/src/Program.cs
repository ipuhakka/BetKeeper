using System;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace API
{
    class Program
    {
        static void Main(string[] args)
        {

            var config = new HttpSelfHostConfiguration("http://localhost:3000");

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            config.EnableCors();

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                try
                {
                    server.OpenAsync().Wait();
                }
                catch (AggregateException)
                {
                    Console.WriteLine("Access was denied, please run as admin. \n Press enter to quit");
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine("Listening on localhost:3000, press Enter to quit.");
                Console.ReadLine();
            }

        }
    }
}
