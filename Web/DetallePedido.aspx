<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetallePedido.aspx.cs" Inherits="WebGomas.DetallePedido" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Detalle del Pedido — Precision Tire</title>
    <style>
        :root {
            --azul:        #007BFF;
            --azul-claro:  #e8f0fe;
            --fondo:       #F8F9FA;
            --blanco:      #ffffff;
            --gris-borde:  #e9ecef;
            --gris-texto:  #6c757d;
            --negro:       #1E293B;
            --verde:       #1e8449;
            --verde-claro: #d5f5e3;
            --naranja:     #d35400;
            --sombra:      0 4px 24px rgba(0,0,0,0.07);
            --radio:       16px;
        }

        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: var(--fondo);
            color: var(--negro);
            min-height: 100vh;
            display: flex;
            flex-direction: column;
        }

        /* HEADER */
        .header {
            background: var(--blanco);
            border-bottom: 1px solid var(--gris-borde);
            padding: 0 40px;
            height: 64px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            position: sticky;
            top: 0;
            z-index: 100;
            box-shadow: 0 1px 8px rgba(0,0,0,0.05);
        }

        .header-logo { font-size: 20px; font-weight: 800; color: var(--negro); text-decoration: none; }
        .header-logo span { color: var(--azul); }

        .header-nav { display: flex; gap: 4px; }
        .header-nav a { color: var(--gris-texto); text-decoration: none; font-size: 14px; font-weight: 500; padding: 6px 14px; border-radius: 8px; transition: color 0.2s, background 0.2s; }
        .header-nav a:hover { color: var(--azul); background: var(--azul-claro); }

        .header-user { display: flex; align-items: center; }
        .btn-login-header { padding: 8px 16px; background: var(--azul); color: var(--blanco); text-decoration: none; font-size: 13px; font-weight: 600; border-radius: 10px; }
        .user-card { display: flex; align-items: center; gap: 10px; background: var(--gris-suave); border: 1px solid var(--gris-borde); border-radius: 12px; padding: 6px 6px 6px 12px; }
        .user-avatar { width: 32px; height: 32px; border-radius: 50%; background: var(--azul); color: var(--blanco); display: flex; align-items: center; justify-content: center; font-size: 14px; font-weight: 700; }
        .user-datos { display: flex; flex-direction: column; line-height: 1.3; }
        .user-saludo { font-size: 10px; font-weight: 600; color: var(--gris-texto); text-transform: uppercase; }
        .user-nombre { font-size: 13px; font-weight: 700; color: var(--negro); }
        .btn-logout { width: 32px; height: 32px; border-radius: 8px; background: #fff5f5; border: 1px solid #fecaca; color: #e53e3e; display: flex; align-items: center; justify-content: center; text-decoration: none; font-size: 16px; }

        @media (max-width: 768px) { .header { padding: 0 16px; } .header-nav, .user-datos { display: none; } }

        /* CONTENIDO WEB */
        .pagina { flex: 1; max-width: 760px; width: 100%; margin: 0 auto; padding: 44px 24px 64px; }
        .link-volver { display: inline-flex; align-items: center; gap: 6px; color: var(--gris-texto); text-decoration: none; font-size: 14px; font-weight: 500; margin-bottom: 28px; }
        .link-volver:hover { color: var(--azul); }

        .card { background: var(--blanco); border-radius: var(--radio); box-shadow: var(--sombra); overflow: hidden; margin-bottom: 24px; }
        .card-cabecera { background: linear-gradient(135deg, #0f2545 0%, #1a3a6e 60%, #1565c0 100%); padding: 24px 32px; display: flex; align-items: center; justify-content: space-between; }
        .card-cabecera h2 { color: var(--blanco); font-size: 18px; font-weight: 700; margin-bottom: 3px; }
        .card-cabecera p { color: rgba(255,255,255,0.6); font-size: 13px; }
        .cabecera-icono { width: 44px; height: 44px; border-radius: 50%; background: rgba(255,255,255,0.12); display: flex; align-items: center; justify-content: center; font-size: 20px; }
        .card-cuerpo { padding: 28px 32px; }
        .dato-fila { display: flex; justify-content: space-between; align-items: center; padding: 14px 0; border-bottom: 1px solid var(--gris-borde); font-size: 14px; }
        .dato-fila:last-child { border-bottom: none; }
        .dato-label { color: var(--gris-texto); font-weight: 500; }
        .dato-valor { font-weight: 700; color: var(--negro); }
        .dato-valor.azul { color: var(--azul); font-size: 20px; }

        .badge-estado { display: inline-flex; align-items: center; gap: 5px; padding: 5px 14px; border-radius: 20px; font-size: 12px; font-weight: 700; }
        .estado-completado { background: var(--verde-claro); color: var(--verde); border: 1px solid #a9dfbf; }
        .estado-procesando { background: var(--azul-claro); color: var(--azul); border: 1px solid #b3d4ff; }
        .estado-pendiente { background: #fef3e2; color: var(--naranja); border: 1px solid #f8c471; }

        .btn-imprimir { display: block; width: 100%; padding: 15px; text-align: center; background: var(--blanco); color: var(--azul); border: 2px solid var(--azul); font-size: 15px; font-weight: 700; border-radius: 12px; margin-bottom: 12px; cursor: pointer; transition: background 0.2s; }
        .btn-imprimir:hover { background: var(--azul-claro); }
        .btn-volver { display: block; width: 100%; padding: 15px; text-align: center; background: var(--azul); color: var(--blanco); text-decoration: none; font-size: 15px; font-weight: 700; border-radius: 12px; }
        .panel-error { background: #fff5f5; border: 1px solid #fed7d7; color: #c53030; padding: 20px 24px; border-radius: var(--radio); font-size: 15px; font-weight: 500; }
        .page-footer { text-align: center; padding: 28px; color: var(--gris-texto); font-size: 13px; border-top: 1px solid var(--gris-borde); }

        /* =====================================================
           TICKET DE IMPRESIÓN (Oculto en la web)
        ===================================================== */
        #ticket-impresion { display: none; } 

        @media print {
            /* 1. Ocultar la web bonita */
            .header, .pagina, .page-footer { display: none !important; }
            body { background: white; margin: 0; padding: 0; }

            /* 2. Mostrar y formatear el recibo estilo Caja POS */
            #ticket-impresion {
                display: block !important;
                width: 80mm; /* Ancho estándar de impresora térmica */
                margin: 0 auto;
                font-family: 'Courier New', Courier, monospace;
                font-size: 14px;
                color: black;
                line-height: 1.2;
            }

            .ticket-header { text-align: center; font-weight: bold; margin-bottom: 15px; }
            .ticket-header h2 { font-size: 18px; margin: 0; }
            .ticket-header p { margin: 2px 0; font-size: 14px; }

            .ticket-info p { margin: 4px 0; }
            .ticket-separador { text-align: center; margin: 8px 0; font-weight: bold; letter-spacing: 2px; }

            .ticket-items { width: 100%; border-collapse: collapse; margin: 5px 0; }
            .ticket-items td { padding: 4px 0; vertical-align: top; }
            .col-cant { width: 15%; }
            .col-desc { width: 55%; }
            .col-precio { width: 30%; text-align: right; }

            .ticket-totales { text-align: right; margin: 10px 0; }
            .ticket-totales p { margin: 4px 0; }
            .ticket-totales h3 { margin: 8px 0; font-size: 16px; }

            .ticket-footer { text-align: center; margin-top: 15px; font-style: italic; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <header class="header">
            <a href="Productos.aspx" class="header-logo">Precision<span>Tire</span></a>
            <nav class="header-nav">
                <a href="Productos.aspx">Catálogo</a>
                <a href="Carrito.aspx">Carrito</a>
                <a href="Historial.aspx">Mis pedidos</a>
            </nav>
            <div class="header-user">
                <asp:PlaceHolder ID="phNoLogueado" runat="server">
                    <a href="Login.aspx" class="btn-login-header">🔐 Iniciar sesión</a>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phLogueado" runat="server" Visible="false">
                    <div class="user-card">
                        <div class="user-avatar"><asp:Label ID="lblAvatar" runat="server" /></div>
                        <div class="user-datos">
                            <span class="user-saludo">Bienvenido</span>
                            <asp:Label ID="lblUsuario" runat="server" CssClass="user-nombre" />
                        </div>
                        <a href="Logout.aspx" class="btn-logout" title="Cerrar sesión">⏻</a>
                    </div>
                </asp:PlaceHolder>
            </div>
        </header>

        <main class="pagina">
            <a href="Historial.aspx" class="link-volver">Volver al historial</a>

            <asp:Panel ID="pnlDetalle" runat="server" Visible="false">
                
                <div class="card">
                    <div class="card-cabecera">
                        <div>
                            <h2>Pedido <asp:Label ID="lblId" runat="server" /></h2>
                            <p>Información detallada de tu orden</p>
                        </div>
                        <div class="cabecera-icono">📦</div>
                    </div>
                    <div class="card-cuerpo">
                        <div class="dato-fila">
                            <span class="dato-label">ID del pedido</span>
                            <span class="dato-valor"><asp:Label ID="lblIdDetalle" runat="server" /></span>
                        </div>
                        <div class="dato-fila">
                            <span class="dato-label">Fecha</span>
                            <span class="dato-valor"><asp:Label ID="lblFecha" runat="server" /></span>
                        </div>
                        <div class="dato-fila">
                            <span class="dato-label">Total pagado</span>
                            <span class="dato-valor azul"><asp:Label ID="lblTotal" runat="server" /></span>
                        </div>
                        <div class="dato-fila">
                            <span class="dato-label">Estado</span>
                            <asp:Label ID="lblEstado" runat="server" />
                        </div>
                    </div>
                </div>

                <button type="button" class="btn-imprimir" onclick="window.print()">🖨️ Imprimir Recibo</button>
                <a href="Historial.aspx" class="btn-volver">← Volver al historial</a>
            </asp:Panel>

            <asp:Panel ID="pnlError" runat="server" Visible="false">
                <div class="panel-error">⚠ Pedido no encontrado. El ID indicado no existe.</div>
            </asp:Panel>

        </main>

        <footer class="page-footer">
            © 2026 Precision Tire · Todos los derechos reservados
        </footer>

        <div id="ticket-impresion">
            <div class="ticket-header">
                <h2>PRECISION TIRE</h2>
                <p>Recibo de Venta</p>
            </div>
            
            <div class="ticket-info">
                <p>Cliente: <asp:Label ID="lblTicketCliente" runat="server" /></p>
                <p>Pedido: #<asp:Label ID="lblTicketPedido" runat="server" /></p>
                <p>Fecha: <asp:Label ID="lblTicketFecha" runat="server" /></p>
            </div>
            
            <div class="ticket-separador">--------------------------------</div>
            
            <table class="ticket-items">
                <asp:Repeater ID="rptTicketItems" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td class="col-cant"><%# Eval("Cantidad") %>x</td>
                            <td class="col-desc"><%# Eval("Nombre") %></td>
                            <td class="col-precio"><%# string.Format("{0:C2}", Eval("Precio")) %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            
            <div class="ticket-separador">--------------------------------</div>
            
            <div class="ticket-totales">
                <p>SubTotal: <asp:Label ID="lblTicketSubtotal" runat="server" /></p>
                <p>Impuesto: <asp:Label ID="lblTicketImpuesto" runat="server" /></p>
                <h3>TOTAL: <asp:Label ID="lblTicketTotal" runat="server" /></h3>
            </div>
            
            <div class="ticket-separador">--------------------------------</div>
            
            <div class="ticket-info">
                <p>Pago: Online</p>
                <p>Monto Cobrado: <asp:Label ID="lblTicketCobrado" runat="server" /></p>
            </div>
            
            <div class="ticket-separador">--------------------------------</div>
            
            <div class="ticket-footer">
                <p>¡Gracias por preferir Precision Tire!</p>
            </div>
        </div>

    </form>
</body>
</html>