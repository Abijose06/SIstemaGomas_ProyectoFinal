using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace SistemaGomas.Messages
{
    public class PedidoCompletadoEvent : IEvent
    {
        public decimal TotalFacturado { get; set; }
        public List<ArticuloWeb> Articulos { get; set; }
    }
}
