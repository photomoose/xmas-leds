using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using DurryLights.ElasticSearch;
using Rumr.DurryLights.Domain;
using Rumr.DurryLights.Domain.Messaging;
using Rumr.DurryLights.Domain.Repositories;
using Rumr.DurryLights.Domain.Services;
using Rumr.DurryLights.Domain.Utilities;
using Rumr.DurryLights.ServiceBus;
using Rumr.DurryLights.Sql;

namespace XmasLeds.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<LightsService>().As<ILightsService>().SingleInstance();
            builder.RegisterType<BusPublisher>().As<IBusPublisher>()
                .WithParameter("connectionString", ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"]);
            builder.RegisterType<ColourRepository>().As<IColourRepository>()
                .WithParameter("connectionString", ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>();
            builder.RegisterType<ElasticMetricWriter>().As<IMetricWriter>()
                .WithParameter("elasticSearchEndpoint", ConfigurationManager.AppSettings["ElasticSearchEndpoint"]);

            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var busPublisher = new BusPublisher(connectionString);

            busPublisher.InitializeAsync().Wait();
        }
    }
}
