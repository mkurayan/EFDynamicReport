using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SchoolReport
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ReportsApi",
                routeTemplate: "api/{controller}/{key}",
                defaults: new { key = RouteParameter.Optional }
            );

            RegisterSerializers(config);
        }

        private static void RegisterSerializers(HttpConfiguration config)
        {

            config.Formatters.JsonFormatter.SerializerSettings =
                new JsonSerializerSettings
                {
                    ContractResolver =
                        new CamelCasePropertyNamesContractResolver()
                };

            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}