using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CajaGomasPOS
{
    public partial class Form1 : Form
    {
        // --- NUEVO: Variable para que NServiceBus encuentre esta pantalla ---
        public static Form1 InstanciaActual;
        public Form1()
        {
            InitializeComponent();
            InstanciaActual = this; // <-- AGREGAR ESTA LÍNEA
        }

        string reciboMetodo = "";
        decimal reciboEfectivo = 0;
        decimal reciboDevuelta = 0;

        public decimal FondoCaja = 0;
        public decimal TotalEfectivoDelDia = 0;
        // --- MEMORIA TEMPORAL PARA IMPRESIÓN WEB ---
        private bool imprimiendoOrdenWeb = false;
        private List<SistemaGomas.Messages.ArticuloWeb> reciboWebArticulos;
        private decimal reciboWebSubtotal = 0;
        private decimal reciboWebImpuesto = 0;
        private decimal reciboWebTotal = 0;

        // --- CONEXIÓN A LA BASE DE DATOS CENTRAL ---
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=GomasDB;Trusted_Connection=True;";

        List<string> inventarioGomas = new List<string>();

        private void CargarInventarioDesdeBD()
        {
            inventarioGomas.Clear();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Marca + ' ' + Modelo + ' - ' + Medida AS Descripcion FROM tblProducto WHERE Estado = 1";
                SqlCommand cmd = new SqlCommand(query, con);
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        inventarioGomas.Add(reader["Descripcion"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar a la Base de Datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // =========================================================================
        // MOTOR DE PRECIOS CENTRAL (NUEVO)
        // =========================================================================
        private decimal ObtenerPrecioArticulo(string descripcion)
        {
            decimal precio = 0m;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    // 1. Intentamos buscar si es un Producto (Goma)
                    string queryProducto = "SELECT PrecioVenta FROM tblProducto WHERE (Marca + ' ' + Modelo + ' - ' + Medida) = @desc AND Estado = 1";
                    using (SqlCommand cmd = new SqlCommand(queryProducto, con))
                    {
                        cmd.Parameters.AddWithValue("@desc", descripcion);
                        object result = cmd.ExecuteScalar();
                        if (result != null) return Convert.ToDecimal(result);
                    }

                    // 2. Si no lo encuentra como goma, buscamos si es un Servicio (Taller)
                    string queryServicio = "SELECT Precio FROM tblServicio WHERE NombreServicio = @desc AND Estado = 1";
                    using (SqlCommand cmdServicio = new SqlCommand(queryServicio, con))
                    {
                        cmdServicio.Parameters.AddWithValue("@desc", descripcion);
                        object result = cmdServicio.ExecuteScalar();
                        if (result != null) return Convert.ToDecimal(result);
                    }
                }
                catch
                {
                    // Si ocurre un error, simplemente retornará 0 para que no explote la caja
                }
            }
            return precio;
        }
        public void ActualizarGaveta()
        {
            decimal totalEnGaveta = FondoCaja + TotalEfectivoDelDia;
            lblDineroCaja.Text = "Dinero en Gaveta: " + totalEnGaveta.ToString("C");
        }
        private void GuardarVentaOffline()
        {
            // 👉 PARA EL EQUIPO DE INTEGRACIÓN (3/3): 
            // Actualmente este método guarda un archivo .txt como backup local.
            // Aquí es donde deben poner el código de conexión a SQL para hacer 
            // el INSERT INTO Factura y el INSERT INTO DetalleFactura.
            // Mantengan el código del .txt dentro de un bloque 'catch' por si la BD se cae.

            try
            {
                string fechaHora = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
                string textoFactura = $"FECHA: {fechaHora}\n";
                textoFactura += $"TOTAL PAGADO: {lblTotal.Text} (Pago en {reciboMetodo})\n";
                textoFactura += "ARTÍCULOS VENDIDOS:\n";

                foreach (DataGridViewRow fila in dgvCarrito.Rows)
                {
                    string cantidad = fila.Cells[2].Value.ToString();
                    string descripcion = fila.Cells[1].Value.ToString();
                    string subtotal = fila.Cells[4].Value.ToString();
                    textoFactura += $"  - {cantidad}x {descripcion} (${subtotal})\n";
                }

                textoFactura += "--------------------------------------------------\n";
                System.IO.File.AppendAllText("Backup_VentasOffline.txt", textoFactura);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Advertencia: No se pudo guardar el respaldo offline. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ActualizarGaveta();
            // --- NUEVA LÍNEA AGREGADA ---
            CargarInventarioDesdeBD();
            // ---------------------------
            cmbCliente.Items.Add("Cliente Genérico (Al Contado)");
            cmbCliente.Items.Add("Juan Pérez");
            cmbCliente.Items.Add("María Gómez");
            cmbCliente.SelectedIndex = 0;

            cmbSucursal.Items.Add("Principal - Santo Domingo");
            cmbSucursal.Items.Add("Sucursal Santiago");
            cmbSucursal.SelectedIndex = 0;

            cmbEmpleado.Items.Add("Carlos Cajero");
            cmbEmpleado.Items.Add("Ana Ventas");
            cmbEmpleado.SelectedIndex = 0;

            cmbTipoItem.Items.Add("Producto (Goma)");
            cmbTipoItem.Items.Add("Servicio (Taller)");
            cmbTipoItem.SelectedIndex = 0;
        }
        private void cmbTipoItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbBuscarItem.Items.Clear();

            if (cmbTipoItem.Text == "Producto (Goma)")
            {
                txtBuscarMedida.Enabled = true;
                foreach (string goma in inventarioGomas)
                {
                    cmbBuscarItem.Items.Add(goma);
                }
            }
            else if (cmbTipoItem.Text == "Servicio (Taller)")
            {
                txtBuscarMedida.Enabled = false;
                txtBuscarMedida.Clear();
                cmbBuscarItem.Items.Add("Alineación Computarizada");
                cmbBuscarItem.Items.Add("Balanceo por Goma");
            }

            if (cmbBuscarItem.Items.Count > 0) cmbBuscarItem.SelectedIndex = 0;
        }
        // =========================================================================
        // MOSTRAR PRECIO EN VIVO (NUEVO)
        // =========================================================================
        private void txtBuscarMedida_TextChanged(object sender, EventArgs e)
        {
            // EL GUARDIÁN REPARADO
            if (cmbTipoItem.Text != "Producto (Goma)") return;

            cmbBuscarItem.Items.Clear();
            string filtro = txtBuscarMedida.Text.ToLower();

            foreach (string goma in inventarioGomas)
            {
                if (goma.ToLower().Contains(filtro))
                {
                    cmbBuscarItem.Items.Add(goma);
                }
            }

            if (cmbBuscarItem.Items.Count > 0) cmbBuscarItem.SelectedIndex = 0;
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbBuscarItem.Text))
            {
                MessageBox.Show("Por favor, seleccione un artículo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tipo = cmbTipoItem.Text;
            string descripcion = cmbBuscarItem.Text;
            int cantidad = (int)nudCantidad.Value;

            // Usamos el Motor de Precios que creamos arriba
            decimal precioUnitario = ObtenerPrecioArticulo(descripcion);

            if (precioUnitario == 0m)
            {
                MessageBox.Show("Este artículo no tiene un precio válido o no hay stock.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal subTotalLinea = cantidad * precioUnitario;
            dgvCarrito.Rows.Add(tipo, descripcion, cantidad, precioUnitario, subTotalLinea);

            decimal sumaSubTotal = 0;
            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                sumaSubTotal += Convert.ToDecimal(fila.Cells[4].Value);
            }

            decimal impuesto = sumaSubTotal * 0.18m;
            decimal totalGeneral = sumaSubTotal + impuesto;

            lblSubTotal.Text = sumaSubTotal.ToString("C");
            lblImpuesto.Text = impuesto.ToString("C");
            lblTotal.Text = totalGeneral.ToString("C");
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.CurrentRow != null)
            {
                dgvCarrito.Rows.Remove(dgvCarrito.CurrentRow);
                decimal sumaSubTotal = 0;
                foreach (DataGridViewRow fila in dgvCarrito.Rows)
                {
                    sumaSubTotal += Convert.ToDecimal(fila.Cells[4].Value);
                }
                decimal impuesto = sumaSubTotal * 0.18m;
                decimal totalGeneral = sumaSubTotal + impuesto;

                lblSubTotal.Text = sumaSubTotal.ToString("C");
                lblImpuesto.Text = impuesto.ToString("C");
                lblTotal.Text = totalGeneral.ToString("C");
            }
            else
            {
                MessageBox.Show("Seleccione el artículo que desea eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnFacturar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.Rows.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal sumaSubTotal = 0;
            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                sumaSubTotal += Convert.ToDecimal(fila.Cells[4].Value);
            }
            decimal totalFactura = sumaSubTotal + (sumaSubTotal * 0.18m);

            FormCobro pantallaCobro = new FormCobro();
            pantallaCobro.TotalPagar = totalFactura;
            pantallaCobro.EfectivoEnCaja = FondoCaja + TotalEfectivoDelDia;

            if (pantallaCobro.ShowDialog() == DialogResult.OK)
            {
                reciboMetodo = pantallaCobro.MetodoPago;
                reciboEfectivo = pantallaCobro.EfectivoEntregado;
                reciboDevuelta = pantallaCobro.CambioDevuelto;

                if (reciboMetodo == "Efectivo")
                {
                    TotalEfectivoDelDia += (reciboEfectivo - reciboDevuelta);
                    ActualizarGaveta();
                }

                MessageBox.Show("¡El pago fue recibido con éxito!", "Venta Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GuardarVentaOffline();
                previewImprimir.ShowDialog();

                dgvCarrito.Rows.Clear();
                lblSubTotal.Text = "$0.00";
                lblImpuesto.Text = "$0.00";
                lblTotal.Text = "$0.00";
                nudCantidad.Value = 1;
                cmbBuscarItem.Text = "";
                lblPrecioVista.Text = "Precio: $0.00"; // Limpiamos la etiqueta del precio también
            }
        }
        private void docImprimir_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font fuenteTitulo = new Font("Arial", 14, FontStyle.Bold);
            Font fuenteNormal = new Font("Arial", 10);
            int y = 20;

            e.Graphics.DrawString("PRECISION TIRE", fuenteTitulo, Brushes.Black, new PointF(80, y));
            y += 30;

            // Título dinámico
            string tituloRecibo = imprimiendoOrdenWeb ? "Recibo de Venta (WEB)" : "Recibo de Venta";
            e.Graphics.DrawString(tituloRecibo, fuenteNormal, Brushes.Black, new PointF(80, y));
            y += 30;

            // =========================================================
            // LÓGICA SI ES UNA ORDEN WEB
            // =========================================================
            if (imprimiendoOrdenWeb)
            {
                e.Graphics.DrawString("Cliente: Cliente Online", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;
                e.Graphics.DrawString("Atendido por: Plataforma Web", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;
                e.Graphics.DrawString($"Sucursal: {cmbSucursal.Text}", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 25;

                e.Graphics.DrawString("-------------------------------------------------", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;

                if (reciboWebArticulos != null)
                {
                    foreach (var item in reciboWebArticulos)
                    {
                        e.Graphics.DrawString($"{item.Cantidad}x {item.Descripcion}", fuenteNormal, Brushes.Black, new PointF(10, y));
                        e.Graphics.DrawString($"${item.Subtotal}", fuenteNormal, Brushes.Black, new PointF(250, y));
                        y += 25;
                    }
                }

                e.Graphics.DrawString("-------------------------------------------------", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;
                e.Graphics.DrawString($"SubTotal: {reciboWebSubtotal:C}", fuenteNormal, Brushes.Black, new PointF(150, y));
                y += 20;
                e.Graphics.DrawString($"Impuesto: {reciboWebImpuesto:C}", fuenteNormal, Brushes.Black, new PointF(150, y));
                y += 25;
                e.Graphics.DrawString($"TOTAL: {reciboWebTotal:C}", fuenteTitulo, Brushes.Black, new PointF(120, y));
                y += 30;
                e.Graphics.DrawString("-------------------------------------------------", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;
                e.Graphics.DrawString("Pago en: Tarjeta (Pasarela de Pago Online)", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 40;
            }
            // =========================================================
            // LÓGICA SI ES UNA VENTA EN CAJA FÍSICA (Tú código original)
            // =========================================================
            else
            {
                e.Graphics.DrawString($"Cliente: {cmbCliente.Text}", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;
                e.Graphics.DrawString($"Atendido por: {cmbEmpleado.Text}", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;
                e.Graphics.DrawString($"Sucursal: {cmbSucursal.Text}", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 25;

                e.Graphics.DrawString("-------------------------------------------------", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;

                foreach (DataGridViewRow fila in dgvCarrito.Rows)
                {
                    string descripcion = fila.Cells[1].Value.ToString();
                    string cantidad = fila.Cells[2].Value.ToString();
                    string subTotalFila = fila.Cells[4].Value.ToString();

                    e.Graphics.DrawString($"{cantidad}x {descripcion}", fuenteNormal, Brushes.Black, new PointF(10, y));
                    e.Graphics.DrawString($"${subTotalFila}", fuenteNormal, Brushes.Black, new PointF(250, y));
                    y += 25;
                }

                e.Graphics.DrawString("-------------------------------------------------", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;
                e.Graphics.DrawString($"SubTotal: {lblSubTotal.Text}", fuenteNormal, Brushes.Black, new PointF(150, y));
                y += 20;
                e.Graphics.DrawString($"Impuesto: {lblImpuesto.Text}", fuenteNormal, Brushes.Black, new PointF(150, y));
                y += 25;
                e.Graphics.DrawString($"TOTAL: {lblTotal.Text}", fuenteTitulo, Brushes.Black, new PointF(120, y));
                y += 30;
                e.Graphics.DrawString("-------------------------------------------------", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;
                e.Graphics.DrawString($"Pago en: {reciboMetodo}", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;

                if (reciboMetodo == "Tarjeta")
                {
                    e.Graphics.DrawString($"Monto Cobrado: {reciboEfectivo:C}", fuenteNormal, Brushes.Black, new PointF(10, y));
                    y += 20;
                }
                else
                {
                    e.Graphics.DrawString($"Recibido: {reciboEfectivo:C}", fuenteNormal, Brushes.Black, new PointF(10, y));
                    y += 20;
                    e.Graphics.DrawString($"Devuelta: {reciboDevuelta:C}", fuenteNormal, Brushes.Black, new PointF(10, y));
                    y += 20;
                }
                e.Graphics.DrawString("-------------------------------------------------", fuenteNormal, Brushes.Black, new PointF(10, y));
                y += 20;
            }

            // Pie de página común para ambos
            e.Graphics.DrawString("¡Gracias por preferir Precision Tire!", fuenteNormal, Brushes.Black, new PointF(50, y));
        }
        private void btnCierreCaja_Click(object sender, EventArgs e)
        {
            decimal totalEnGaveta = FondoCaja + TotalEfectivoDelDia;
            string reporte = "=== REPORTE DE CIERRE DE CAJA ===\n\n" +
                             $"Fondo Inicial (Mañana): {FondoCaja.ToString("C")}\n" +
                             $"Ventas en Efectivo: {TotalEfectivoDelDia.ToString("C")}\n" +
                             "--------------------------------------------------\n" +
                             $"DINERO TOTAL EN GAVETA: {totalEnGaveta.ToString("C")}\n\n" +
                             "¿Desea cerrar la caja y salir del sistema?";

            DialogResult respuesta = MessageBox.Show(reporte, "Cuadre de Turno", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (respuesta == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void cmbBuscarItem_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Cada vez que el cajero toca un artículo, buscamos su precio y lo mostramos
            if (!string.IsNullOrWhiteSpace(cmbBuscarItem.Text))
            {
                decimal precioVista = ObtenerPrecioArticulo(cmbBuscarItem.Text);
                lblPrecioVista.Text = "Precio: " + precioVista.ToString("C");
            }
        }
        private void btnAnularVenta_Click(object sender, EventArgs e)
        {
            // Confirmamos por si el cajero le dio por accidente
            DialogResult respuesta = MessageBox.Show("¿Está seguro que desea cancelar toda la venta actual?", "Anular Venta", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (respuesta == DialogResult.Yes)
            {
                dgvCarrito.Rows.Clear();
                lblSubTotal.Text = "$0.00";
                lblImpuesto.Text = "$0.00";
                lblTotal.Text = "$0.00";
                nudCantidad.Value = 1;
                cmbBuscarItem.SelectedIndex = -1; // Deselecciona el artículo
                lblPrecioVista.Text = "Precio: $0.00";
            }
        }
        // =========================================================================
        // FACTURACIÓN WEB SILENCIOSA
        // =========================================================================
        public void ProcesarOrdenWebEnSilencio(decimal subtotalWeb, List<SistemaGomas.Messages.ArticuloWeb> articulos)
        {
            // Pedimos permiso para actualizar la pantalla desde el hilo de fondo
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ProcesarOrdenWebEnSilencio(subtotalWeb, articulos)));
                return;
            }

            // 1. Cálculos matemáticos (La Web manda el subtotal, la Caja le suma el 18% de ITBIS)
            decimal impuesto = subtotalWeb * 0.18m;
            decimal totalGeneral = subtotalWeb + impuesto;

            // 2. Armar los textos detallados para la factura y la alerta
            string detalleArticulosMessageBox = "";
            string textoFacturaBackup = $"FECHA: {DateTime.Now:dd/MM/yyyy hh:mm tt}\n";
            textoFacturaBackup += $"TOTAL PAGADO WEB: {totalGeneral:C} (Pago Online)\n";
            textoFacturaBackup += "ARTÍCULOS VENDIDOS (WEB):\n";

            // Recorremos la lista de artículos que nos llegó por NServiceBus
            if (articulos != null)
            {
                foreach (var item in articulos)
                {
                    detalleArticulosMessageBox += $"- {item.Cantidad}x {item.Descripcion}\n";
                    textoFacturaBackup += $"  - {item.Cantidad}x {item.Descripcion} (${item.Subtotal})\n";
                }
            }
            textoFacturaBackup += "--------------------------------------------------\n";

            // 3. Facturar en silencio (Lo guardamos en el TXT de backup de ventas sin molestar al cajero)
            try
            {
                System.IO.File.AppendAllText("Backup_VentasOffline.txt", textoFacturaBackup);
            }
            catch { }

            // 4. Mostrar la alerta visual con todo el desglose exacto que pediste
            string mensajeAlerta = $"¡Nueva orden web recibida y facturada automáticamente!\n\n" +
                                   $"Artículos Comprados:\n{detalleArticulosMessageBox}\n" +
                                   $"Subtotal: {subtotalWeb:C}\n" +
                                   $"ITBIS (18%): {impuesto:C}\n" +
                                   $"TOTAL FACTURADO: {totalGeneral:C}";

            MessageBox.Show(mensajeAlerta, "Venta Web Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 5. Preparar la memoria y lanzar la vista previa del recibo
            imprimiendoOrdenWeb = true;
            reciboWebArticulos = articulos;
            reciboWebSubtotal = subtotalWeb;
            reciboWebImpuesto = impuesto;
            reciboWebTotal = totalGeneral;

            // Mostrar el recibo en pantalla
            previewImprimir.ShowDialog();

            // Limpiar la memoria al terminar
            imprimiendoOrdenWeb = false;
        }
    }
}