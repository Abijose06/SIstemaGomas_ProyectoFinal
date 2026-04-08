using System;
using System.Windows.Forms;
using NServiceBus;

namespace CajaGomasPOS
{
    internal static class Program
    {
        // Variable global para que NServiceBus funcione en toda la caja
        public static IEndpointInstance EndpointInstance { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // --- CONFIGURACIÓN DE NSERVICEBUS ---
                var endpointConfiguration = new EndpointConfiguration("SistemaGomas.Caja");

                var transport = endpointConfiguration.UseTransport<LearningTransport>();
                transport.StorageDirectory(@"C:\TransporteGomas");

                endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
                endpointConfiguration.SendFailedMessagesTo("error");
                endpointConfiguration.EnableInstallers();

                // Encendemos el motor
                EndpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Si algo falla al arrancar, forzamos a que salga una ventana de error
                MessageBox.Show("Error al encender NServiceBus: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Arrancamos tu pantalla de Login
            Application.Run(new FormLogin());
        }
    }
}