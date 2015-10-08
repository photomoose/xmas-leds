using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Rumr.DurryLights.ServiceBus;

namespace XmasLeds.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var busPublisher = new BusPublisher(connectionString);

            busPublisher.InitializeAsync().Wait();
        }
    }
}
