using Betkeeper;
using Api.Classes;
using System.Configuration;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json;
using System;
using System.Web.Http.ExceptionHandling;

namespace Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Services.Add(typeof(IExceptionLogger), new ErrorLogger());

            var secretsObject = File.Exists(ConfigurationManager.AppSettings.Get("devSecretsPath"))
                ? JsonConvert.DeserializeObject(File.ReadAllText(
                    ConfigurationManager.AppSettings.Get("devSecretsPath"))) as dynamic
                : null;

            // Set database connection
            Settings.ConnectionString = ConfigurationManager
                .ConnectionStrings["sql"]?.ConnectionString
                ?? secretsObject["ConnectionString"]?.ToString();

            Settings.SecretKey = ConfigurationManager
                .AppSettings.Get("secretKey")
                ?? secretsObject["SecretKey"]?.ToString();

            if (string.IsNullOrEmpty(Settings.ConnectionString))
            {
                throw new Betkeeper.Exceptions.ConfigurationException(
                    "Connection string was not given");
            }

            if (string.IsNullOrEmpty(Settings.SecretKey))
            {
                throw new Betkeeper.Exceptions.ConfigurationException(
                    "Secret key was not given");
            }

            Settings.InitializeOptionsBuilderService();

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
