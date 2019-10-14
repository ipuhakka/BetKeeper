using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using Betkeeper;

namespace Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Set database connection 
            Settings.DatabasePath = ConfigurationManager.AppSettings["databasePath"];

            // Web API configuration and services
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
