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

    public partial class Productos : System.Web.UI.Page
    {
        // -------------------------------------------------------
        // Datos simulados (hardcoded) — sin base de datos
        // -------------------------------------------------------
        // -------------------------------------------------------
        // CATÁLOGO DESDE LA BASE DE DATOS CENTRAL
        // -------------------------------------------------------
        private List<Producto> ObtenerProductos()
        {
            List<Producto> listaBD = new List<Producto>();
            string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=GomasDB;Trusted_Connection=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // NUEVO: Agregamos ImagenUrl a la consulta
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

                            // NUEVO: Leemos la ruta exacta desde la base de datos sin adivinar nada
                            ImagenUrl = reader["ImagenUrl"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores
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
                CargarProductos();
            }

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
        // Vincula la lista al Repeater y actualiza el contador
        // -------------------------------------------------------
        private void CargarProductos()
        {
            List<Producto> productos = ObtenerProductos();

            // Mostrar cantidad de productos en el badge del header
            lblCantidad.Text = productos.Count + " productos";

            // Binding al Repeater
            rptProductos.DataSource = productos;
            rptProductos.DataBind();
        }

    }
}
