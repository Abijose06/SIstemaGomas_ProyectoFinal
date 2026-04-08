using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGomas.Models;
using System.Data.SqlClient;

namespace WebGomas
{
    public partial class Historial : System.Web.UI.Page
    {
        // -------------------------------------------------------
        // Carga inicial (Corregida)
        // -------------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. PRIMERO protegemos la página (si no hay sesión, lo sacamos)
            if (Session["usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // 2. LUEGO cargamos los datos si es la primera vez que entra a la página
            if (!IsPostBack)
            {
                ActualizarHeader();
                CargarHistorial();
            }
        }
            // -------------------------------------------------------
            // HISTORIAL DESDE LA BASE DE DATOS CENTRAL
            // -------------------------------------------------------
        private List<Pedido> ObtenerPedidos()
        {
            // 1. Declaramos la lista y las variables que Visual Studio no encontraba
            List<Pedido> listaBD = new List<Pedido>();
            string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=GomasDB;Trusted_Connection=True;";
            string idUsuario = Session["usuario"].ToString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // 2. Buscamos en tblFactura con las columnas correctas
                string query = "SELECT IdFactura, Fecha, TotalGeneral AS Total, EstadoFactura AS Estado FROM tblFactura WHERE IdCliente = 1 ORDER BY Fecha DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                // Buscamos las facturas del cliente 1, que es el que usamos para guardar en el carrito
                cmd.Parameters.AddWithValue("@IdCliente", 1);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listaBD.Add(new Pedido
                        {
                            Id = Convert.ToInt32(reader["IdFactura"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("dd/MM/yyyy"),
                            Total = reader["Total"] != DBNull.Value ? Convert.ToDecimal(reader["Total"]) : 0m,
                            Estado = reader["Estado"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    // Falla en silencio si hay error de conexión
                }
            }

            // 3. Devolvemos la lista llena (esto quita el error de "not all code paths return a value")
            return listaBD;
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

        // -------------------------------------------------------
        // Llena la tabla y calcula las estadísticas
        // -------------------------------------------------------
        private void CargarHistorial()
        {
            List<Pedido> pedidos = ObtenerPedidos();

            // Binding al GridView
            gvHistorial.DataSource = pedidos;
            gvHistorial.DataBind();

            // ── Estadísticas ──

            // Gasto total
            decimal gastoTotal = pedidos.Sum(p => p.Total);
            lblGastoTotal.Text = gastoTotal.ToString("C2");

            // Pedidos completados
            int completados = pedidos.Count(p => p.Estado == "Completado");
            lblPedidosCompletados.Text = completados + " pedidos";

            // Próximo servicio (cada 3 pedidos completados)
            int proximoServicio = (completados / 3 + 1) * 3;
            lblProximoServicio.Text = "Al pedido #" + proximoServicio;

            // Badge con total de pedidos
            lblTotalPedidos.Text = pedidos.Count + " pedidos";
        }

        // -------------------------------------------------------
        // Colorea el badge de estado fila por fila
        // -------------------------------------------------------
        protected void gvHistorial_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            Label lblEstado = e.Row.FindControl("lblEstado") as Label;

            if (lblEstado == null)
                return;

            switch (lblEstado.Text)
            {
                case "Completado":
                    lblEstado.CssClass = "badge-estado estado-completado";
                    break;
                case "Procesando":
                    lblEstado.CssClass = "badge-estado estado-procesando";
                    break;
                case "Pendiente":
                    lblEstado.CssClass = "badge-estado estado-pendiente";
                    break;
            }
        }
    }
}