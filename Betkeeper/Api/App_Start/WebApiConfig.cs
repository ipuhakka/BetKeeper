using Betkeeper;
using System.Configuration;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json;

namespace Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var secretsObject = JsonConvert.DeserializeObject(
                File.ReadAllText(
                    ConfigurationManager
                    .AppSettings.Get("devSecretsPath"))) as dynamic;

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
