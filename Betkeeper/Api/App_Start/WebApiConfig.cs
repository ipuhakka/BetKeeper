using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Betkeeper;

namespace Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // TODO: Turha kun kanta vaihtuu
            Settings.DatabasePath = ConfigurationManager.AppSettings["databasePath"];

            // Set database connection
            Settings.ConnectionString = ConfigurationManager
                .ConnectionStrings["sql"]?.ConnectionString 
                ?? File.ReadAllText(
                    ConfigurationManager
                    .AppSettings.Get("devSecretsPath"));

            if (string.IsNullOrEmpty(Settings.ConnectionString))
            {
                throw new Betkeeper.Exceptions.ConfigurationException(
                    "Connection string was not given");
            }

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
