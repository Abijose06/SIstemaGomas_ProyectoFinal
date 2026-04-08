using System.Threading.Tasks;
using NServiceBus;
using SistemaGomas.Messages;

namespace CajaGomasPOS
{
    public class PedidoCompletadoHandler : IHandleMessages<PedidoCompletadoEvent>
    {
        public Task Handle(PedidoCompletadoEvent message, IMessageHandlerContext context)
        {
            // Verificamos que la pantalla de la caja principal esté abierta
            if (Form1.InstanciaActual != null)
            {
                // Llamamos a la facturación silenciosa pasándole el Subtotal y la Lista de Artículos
                Form1.InstanciaActual.ProcesarOrdenWebEnSilencio(message.TotalFacturado, message.Articulos);
            }

            return Task.CompletedTask;
        }
    }
}