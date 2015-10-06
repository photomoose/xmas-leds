using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

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
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            var td = new TopicDescription("Commands");

            if (!namespaceManager.TopicExists(td.Path))
            {
                namespaceManager.CreateTopic(td);
            }
        }
    }
}
