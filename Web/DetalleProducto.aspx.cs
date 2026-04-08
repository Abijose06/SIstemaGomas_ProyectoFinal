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
    public partial class DetalleProducto : System.Web.UI.Page
    {
        // -------------------------------------------------------
        // DATOS DESDE LA BASE DE DATOS CENTRAL
        // -------------------------------------------------------
        private List<Producto> ObtenerProductos()
        {
            List<Producto> listaBD = new List<Producto>();
            string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=GomasDB;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Leemos los productos y su imagen oficial de la BD
                string query = "SELECT IdProducto, Marca, Modelo, PrecioVenta, ImagenUrl FROM tblProducto WHERE Estado = 1";
                SqlCommand cmd = new SqlCommand(query, con);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        listaBD.Add(new Producto
                        {
                            Id = Convert.ToInt32(reader["IdProducto"]),
                            Nombre = reader["Modelo"].ToString(),
                            Marca = reader["Marca"].ToString(),
                            Precio = Convert.ToDecimal(reader["PrecioVenta"]),
                            ImagenUrl = reader["ImagenUrl"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores silencioso
                }
            }

            return listaBD;
        }

        // -------------------------------------------------------
        // Carga inicial de la página
        // -------------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDetalle();
            }

            if (Session["usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // tu código existente...
            }
        }

        // -------------------------------------------------------
        // Lee el QueryString, busca el producto y llena la vista
        // -------------------------------------------------------
        private void CargarDetalle()
        {
            // 1. Leer y validar el parámetro "id" del QueryString
            string parametroId = Request.QueryString["id"];

            int id;
            bool esNumero = int.TryParse(parametroId, out id);

            if (!esNumero)
            {
                MostrarError();
                return;
            }

            // 2. Buscar el producto en la lista simulada
            List<Producto> productos = ObtenerProductos();
            Producto producto = productos.FirstOrDefault(p => p.Id == id);

            if (producto == null)
            {
                MostrarError();
                return;
            }

            // 3. Mostrar los datos del producto en pantalla
            lblNombre.Text = producto.Nombre;
            lblMarca.Text = producto.Marca;
            lblPrecio.Text = producto.Precio.ToString("C2");
            imgProducto.ImageUrl = producto.ImagenUrl;

            pnlDetalle.Visible = true;
        }

        // -------------------------------------------------------
        // Muestra el panel de error y oculta el de detalle
        // -------------------------------------------------------
        private void MostrarError()
        {
            pnlDetalle.Visible = false;
            pnlError.Visible = true;
        }

        // -------------------------------------------------------
        // Clic en "Agregar al carrito" → redirige a Carrito.aspx
        // (lógica del carrito se implementará después)
        // -------------------------------------------------------
        protected void btnAgregarCarrito_Click(object sender, EventArgs e)
        {
            // 1. Leer el id del producto desde la URL
            int id = Convert.ToInt32(Request.QueryString["id"]);

            // 2. Buscar el producto en la lista simulada
            List<Producto> productos = ObtenerProductos();
            Producto producto = productos.FirstOrDefault(p => p.Id == id);

            if (producto == null)
                return;

            // 3. Leer la cantidad del TextBox (si no es número válido, usar 1)
            int cantidad;
            if (!int.TryParse(txtCantidad.Text, out cantidad) || cantidad < 1)
                cantidad = 1;

            // 4. Obtener el carrito desde Session (o crear uno nuevo si no existe)
            List<CarritoItem> carrito = Session["carrito"] as List<CarritoItem>;

            if (carrito == null)
                carrito = new List<CarritoItem>();

            // 5. Verificar si el producto ya está en el carrito
            CarritoItem itemExistente = carrito.FirstOrDefault(c => c.ProductoId == producto.Id);

            if (itemExistente != null)
            {
                // Si ya existe, solo sumar la cantidad
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                // Si no existe, agregar como nuevo item
                carrito.Add(new CarritoItem
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad
                });
            }

            // 6. Guardar el carrito actualizado en Session
            Session["carrito"] = carrito;

            // 7. Redirigir al carrito
            Response.Redirect("Carrito.aspx");
        }
    }
}