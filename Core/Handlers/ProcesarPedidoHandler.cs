using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus;
using SistemaGomas.Messages;

namespace Core.Handlers
{
    public class ProcesarPedidoHandler : IHandleMessages<ProcesarPedidoCommand>
    {
        public async Task Handle(ProcesarPedidoCommand message, IMessageHandlerContext context)
        {
            // 1. Preparamos la conexión a la base de datos
            string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=GomasDB;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 2. Buscamos la ÚLTIMA factura que acaba de crear tu página web
                int idFactura = 0;
                string queryFactura = "SELECT TOP 1 IdFactura FROM tblFactura ORDER BY IdFactura DESC";
                using (SqlCommand cmdF = new SqlCommand(queryFactura, con))
                {
                    object result = cmdF.ExecuteScalar();
                    if (result != null) idFactura = Convert.ToInt32(result);
                }

                // 3. Si encontramos la factura, insertamos los artículos uno por uno
                if (idFactura > 0)
                {
                    foreach (var articulo in message.Articulos)
                    {
                        // TipoItem 'P' (Producto), IdProducto 1 (prueba), IdVehiculo 1 (prueba)
                        // No insertamos "SubTotal" porque también es Computed y se calcula solo.
                        string queryDetalle = @"INSERT INTO tblDetalle_Factura 
                                              (TipoItem, IdProducto, Cantidad, PrecioUnitario, IdFactura, IdVehiculo, Estado) 
                                              VALUES ('P', 1, @Cantidad, @Precio, @IdFactura, 1, 1)";

                        using (SqlCommand cmdD = new SqlCommand(queryDetalle, con))
                        {
                            cmdD.Parameters.AddWithValue("@Cantidad", articulo.Cantidad);
                            cmdD.Parameters.AddWithValue("@Precio", articulo.PrecioUnitario);
                            cmdD.Parameters.AddWithValue("@IdFactura", idFactura);

                            cmdD.ExecuteNonQuery(); // Guardamos el artículo
                        }
                    }
                }
            }

            // 4. Silenciamos la publicación del evento para que la Caja no abra la ventana emergente
            // con las compras que vienen desde la página web.

            // var evento = new PedidoCompletadoEvent
            // {
            //     TotalFacturado = message.Total,
            //     Articulos = message.Articulos
            // };
            // await context.Publish(evento);
        }
    }
}