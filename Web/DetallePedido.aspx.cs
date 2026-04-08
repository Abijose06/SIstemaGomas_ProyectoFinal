using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebGomas
{
    // Clase local para transportar los datos desde SQL sin romper tus modelos
    public class DatosFacturaWeb
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public decimal Total { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Subtotal { get; set; }
        public string Estado { get; set; }
    }

    public class TicketItem
    {
        public int Cantidad { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
    }

    public partial class DetallePedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ActualizarHeader();
                CargarDetalleYTicket();
            }
        }

        private void ActualizarHeader()
        {
            if (Session["usuario"] != null)
            {
                phLogueado.Visible = true;
                phNoLogueado.Visible = false;

                string nombre = Session["nombre"].ToString();
                lblUsuario.Text = nombre;
                lblAvatar.Text = nombre.Substring(0, 1).ToUpper();
            }
            else
            {
                phLogueado.Visible = false;
                phNoLogueado.Visible = true;
            }
        }

        private void CargarDetalleYTicket()
        {
            string parametro = Request.QueryString["id"];
            int idFactura;

            if (!int.TryParse(parametro, out idFactura))
            {
                MostrarError();
                return;
            }

            // 1. Obtener la Cabecera (Total, Impuesto, Estado)
            DatosFacturaWeb pedido = ObtenerDatosCabecera(idFactura);

            if (pedido == null)
            {
                MostrarError();
                return;
            }

            // 2. Llenar la vista web normal (Azul)
            lblId.Text = "#" + pedido.Id;
            lblIdDetalle.Text = "#" + pedido.Id;
            lblFecha.Text = pedido.Fecha;
            lblTotal.Text = pedido.Total.ToString("C2");

            lblEstado.Text = pedido.Estado;
            switch (pedido.Estado)
            {
                case "Completado": lblEstado.CssClass = "badge-estado estado-completado"; break;
                case "Procesando": lblEstado.CssClass = "badge-estado estado-procesando"; break;
                case "Pendiente": lblEstado.CssClass = "badge-estado estado-pendiente"; break;
            }

            // 3. Llenar la vista del Ticket de Impresión
            lblTicketCliente.Text = Session["nombre"] != null ? Session["nombre"].ToString() : "Cliente Web";
            lblTicketPedido.Text = pedido.Id.ToString();
            lblTicketFecha.Text = pedido.Fecha;

            lblTicketSubtotal.Text = pedido.Subtotal.ToString("C2");
            lblTicketImpuesto.Text = pedido.Impuesto.ToString("C2");
            lblTicketTotal.Text = pedido.Total.ToString("C2");
            lblTicketCobrado.Text = pedido.Total.ToString("C2");

            // 4. Buscar y llenar los artículos de esta factura en el Ticket
            CargarArticulosTicket(idFactura);

            pnlDetalle.Visible = true;
        }

        private DatosFacturaWeb ObtenerDatosCabecera(int idFactura)
        {
            DatosFacturaWeb pedidoEncontrado = null;
            string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=GomasDB;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Traemos el Impuesto además del TotalGeneral
                string query = "SELECT IdFactura, Fecha, TotalGeneral AS Total, Impuesto, EstadoFactura AS Estado FROM tblFactura WHERE IdFactura = @IdFactura";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdFactura", idFactura);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        decimal total = reader["Total"] != DBNull.Value ? Convert.ToDecimal(reader["Total"]) : 0m;
                        decimal impuesto = reader["Impuesto"] != DBNull.Value ? Convert.ToDecimal(reader["Impuesto"]) : 0m;

                        pedidoEncontrado = new DatosFacturaWeb
                        {
                            Id = Convert.ToInt32(reader["IdFactura"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("dd/MM/yyyy"),
                            Total = total,
                            Impuesto = impuesto,
                            Subtotal = total - impuesto, // Calculamos la base imponible
                            Estado = reader["Estado"].ToString()
                        };
                    }
                }
                catch (Exception)
                {
                    // Falla silenciada
                }
            }

            return pedidoEncontrado;
        }

        private void CargarArticulosTicket(int idFactura)
        {
            List<TicketItem> listaItems = new List<TicketItem>();
            string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=GomasDB;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Cambiamos INNER JOIN por LEFT JOIN y usamos ISNULL para evitar que la página se quede en blanco
                string query = @"
                    SELECT d.Cantidad, d.PrecioUnitario, 
                           ISNULL((p.Marca + ' ' + p.Modelo), 'Neumático (Dato perdido en BD)') AS Nombre 
                    FROM tblDetalle_Factura d
                    LEFT JOIN tblProducto p ON d.IdProducto = p.IdProducto
                    WHERE d.IdFactura = @IdFactura";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdFactura", idFactura);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listaItems.Add(new TicketItem
                        {
                            Cantidad = Convert.ToInt32(reader["Cantidad"]),
                            Nombre = reader["Nombre"].ToString(), // Ahora trae "Michelin Pilot Sport 4"
                            Precio = Convert.ToDecimal(reader["PrecioUnitario"])
                        });
                    }
                }
                catch (Exception) { }
            }

            rptTicketItems.DataSource = listaItems;
            rptTicketItems.DataBind();
        }

        private void MostrarError()
        {
            pnlDetalle.Visible = false;
            pnlError.Visible = true;
        }
    }
}