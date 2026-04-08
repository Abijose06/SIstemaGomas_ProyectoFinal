using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using NServiceBus; // ← ¡Nuevo!
using SistemaGomas.Messages; // ← ¡Nuevo!

namespace WebGomas
{
    public class Global : HttpApplication
    {
        // Esta variable nos permitirá enviar mensajes desde el carrito
        public static IEndpointInstance EndpointInstance { get; private set; }

        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // --- CONFIGURACIÓN DE NSERVICEBUS ---
            var endpointConfiguration = new EndpointConfiguration("SistemaGomas.Web");
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(@"C:\TransporteGomas");
            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

            // Aquí le decimos que los "Comandos" se envíen al Core
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(ProcesarPedidoCommand), "SistemaGomas.Core");

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.EnableInstallers();

            // Encendemos el motor
            EndpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        }
    }
}