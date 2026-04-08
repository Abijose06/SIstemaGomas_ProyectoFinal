using NServiceBus;
using SistemaGomas.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace WebGomas.Models
{
    public partial class Carrito : System.Web.UI.Page
    {
        // -------------------------------------------------------
        // Carga inicial
        // -------------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                CargarCarrito();
            }

            // Proteger la página
            if (Session["usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ActualizarHeader();    // ← agregar esta línea
                                       // ... tu código existente que ya tenías
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

        // -------------------------------------------------------
        // Lee el carrito desde Session y llena la vista
        // -------------------------------------------------------
        private void CargarCarrito()
        {
            List<CarritoItem> carrito = Session["carrito"] as List<CarritoItem>;

            if (carrito == null || carrito.Count == 0)
            {
                pnlVacio.Visible = true;
                pnlCarrito.Visible = false;
                return;
            }

            // Binding al Repeater
            rptCarrito.DataSource = carrito;
            rptCarrito.DataBind();

            // --- NUEVO CÁLCULO MATEMÁTICO (Igual a la Caja) ---
            // Sumamos el costo de los productos
            decimal subtotal = carrito.Sum(item => item.Subtotal);

            // Calculamos el 18% de ITBIS
            decimal impuesto = subtotal * 0.18m;

            // Sumamos ambos para el total real a pagar
            decimal totalGeneral = subtotal + impuesto;

            // Mostramos los valores en la página web
            lblSubtotal.Text = subtotal.ToString("C2");
            lblTotal.Text = totalGeneral.ToString("C2");
            lblCantidadItems.Text = carrito.Count + (carrito.Count == 1 ? " producto" : " productos");

            pnlCarrito.Visible = true;
            pnlVacio.Visible = false;
        }

        // -------------------------------------------------------
        // Maneja los botones +, - y ✕ del Repeater
        // -------------------------------------------------------
        protected void rptCarrito_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // Obtener el carrito actual
            List<CarritoItem> carrito = Session["carrito"] as List<CarritoItem>;

            if (carrito == null)
                return;

            // Leer el ProductoId enviado por CommandArgument
            int productoId = Convert.ToInt32(e.CommandArgument);

            // Buscar el item en el carrito
            CarritoItem item = carrito.FirstOrDefault(c => c.ProductoId == productoId);

            if (item == null)
                return;

            switch (e.CommandName)
            {
                case "Sumar":
                    // Aumentar cantidad en 1
                    item.Cantidad += 1;
                    break;

                case "Restar":
                    // Bajar cantidad en 1 — si llega a 0, eliminar el item
                    item.Cantidad -= 1;
                    if (item.Cantidad <= 0)
                        carrito.Remove(item);
                    break;

                case "Eliminar":
                    // Eliminar el item directamente
                    carrito.Remove(item);
                    break;
            }

            // Guardar el carrito actualizado en Session
            Session["carrito"] = carrito;

            // Recargar la vista con los datos nuevos
            CargarCarrito();
        }

        // -------------------------------------------------------
        // Clic en "Confirmar compra"
        // -------------------------------------------------------
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            // 1. Obtener el carrito actual de la sesión
            List<CarritoItem> carrito = Session["carrito"] as List<CarritoItem>;

            // Verificamos si el carrito tiene productos
            if (carrito != null && carrito.Count > 0)
            {
                // --- CÁLCULO MATEMÁTICO ---
                decimal subtotal = carrito.Sum(item => item.Subtotal);
                decimal impuesto = subtotal * 0.18m;
                decimal totalGeneral = subtotal + impuesto;

                // --- INICIO DEL CÓDIGO DE BASE DE DATOS ---
                string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=GomasDB;Trusted_Connection=True;";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();

                        // 1. Insertamos la factura y LE ROBAMOS EL ID que SQL le acaba de asignar (OUTPUT INSERTED.IdFactura)
                        string queryCabecera = "INSERT INTO tblFactura (IdCliente, Fecha, EstadoFactura, Estado, Impuesto, MetodoPago, IdSucursal, IdEmpleado) OUTPUT INSERTED.IdFactura VALUES (1, GETDATE(), 'Completado', 1, @Impuesto, 'Web', 1, 1)";
                        int nuevaFacturaId = 0;

                        using (SqlCommand cmd = new SqlCommand(queryCabecera, con))
                        {
                            cmd.Parameters.AddWithValue("@Impuesto", impuesto);
                            nuevaFacturaId = (int)cmd.ExecuteScalar();
                        }

                        // 2. Ahora NOSOTROS insertamos el detalle con el IdProducto perfecto, sin depender del Core
                        string queryDetalle = "INSERT INTO tblDetalle_Factura (TipoItem, IdProducto, Cantidad, PrecioUnitario, IdFactura, IdVehiculo, Estado) VALUES ('P', @IdProducto, @Cantidad, @Precio, @IdFactura, 1, 1)";

                        using (SqlCommand cmdDet = new SqlCommand(queryDetalle, con))
                        {
                            foreach (var item in carrito)
                            {
                                cmdDet.Parameters.Clear();
                                cmdDet.Parameters.AddWithValue("@IdProducto", item.ProductoId);
                                cmdDet.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                                cmdDet.Parameters.AddWithValue("@Precio", item.Precio);
                                cmdDet.Parameters.AddWithValue("@IdFactura", nuevaFacturaId);

                                cmdDet.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Ignoramos el error de BD para no frenar la compra
                    }
                }
                // --- FIN DEL CÓDIGO DE BASE DE DATOS ---

                

                // Vaciar el carrito tras la compra exitosa
                Session["carrito"] = null;
            }

            // Redirigir a la pantalla de éxito suavemente
            Response.Redirect("Confirmacion.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // -------------------------------------------------------
        // Clic en "Eliminar todo"
        // -------------------------------------------------------
        protected void btnEliminarTodo_Click(object sender, EventArgs e)
        {
            Session["carrito"] = null;
            CargarCarrito();
        }

       
    }
}
