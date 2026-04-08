using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace SistemaGomas.Messages
{
    public class ProcesarPedidoCommand : ICommand
    {
        public decimal Total { get; set; }
        public int CantidadItems { get; set; }
        public List<ArticuloWeb> Articulos { get; set; }
    }
}
