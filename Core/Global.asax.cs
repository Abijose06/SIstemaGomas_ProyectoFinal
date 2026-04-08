using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NServiceBus; // ← ¡Nuevo!

namespace Core
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static IEndpointInstance EndpointInstance { get; private set; }

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // --- CONFIGURACIÓN DE NSERVICEBUS ---
            var endpointConfiguration = new EndpointConfiguration("SistemaGomas.Core");

            // Agregamos "var transport =" aquí:
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(@"C:\TransporteGomas");

            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.EnableInstallers();

            // Encendemos el motor
            EndpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        }
    }
}