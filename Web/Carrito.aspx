<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Carrito.aspx.cs" Inherits="WebGomas.Models.Carrito" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Tu Carrito — PrecisionTire</title>
    <style>

        /* =====================================================
           VARIABLES GLOBALES
           Mantienen consistencia con Productos.aspx,
           DetalleProducto.aspx y Confirmacion.aspx
        ===================================================== */
        :root {
            --azul:           #007BFF;
            --azul-oscuro:    #0062cc;
            --azul-claro:     #e8f0fe;
            --azul-hover:     #dbeafe;
            --fondo:          #F8F9FA;
            --blanco:         #ffffff;
            --gris-suave:     #f1f3f5;
            --gris-borde:     #e9ecef;
            --gris-texto:     #6c757d;
            --negro:          #1E293B;
            --rojo-suave:     #fff0f0;
            --rojo:           #e53e3e;
            --rojo-hover:     #c53030;
            --verde:          #1e8449;
            --verde-claro:    #d5f5e3;
            --sombra-card:    0 4px 32px rgba(0,0,0,0.08), 0 1px 4px rgba(0,0,0,0.04);
            --sombra-hover:   0 8px 28px rgba(0,123,255,0.13);
            --radio:          16px;
            --radio-btn:      12px;
            --radio-img:      12px;
        }

        /* =====================================================
           RESET BASE
        ===================================================== */
        *, *::before, *::after {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: var(--fondo);
            color: var(--negro);
            min-height: 100vh;
            display: flex;
            flex-direction: column;
        }

        /* =====================================================
   HEADER
===================================================== */
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

.header-logo {
    font-size: 20px;
    font-weight: 800;
    color: var(--negro);
    letter-spacing: -0.5px;
    text-decoration: none;
    flex-shrink: 0;
}

.header-logo span { color: var(--azul); }

.header-nav {
    display: flex;
    align-items: center;
    gap: 4px;
}

.header-nav a {
    color: var(--gris-texto);
    text-decoration: none;
    font-size: 14px;
    font-weight: 500;
    padding: 6px 14px;
    border-radius: 8px;
    transition: color 0.2s, background 0.2s;
    white-space: nowrap;
}

.header-nav a:hover  { color: var(--azul); background: var(--azul-claro); }
.header-nav a.activo { color: var(--azul); font-weight: 700; background: var(--azul-claro); }

.header-user {
    display: flex;
    align-items: center;
    flex-shrink: 0;
}

.btn-login-header {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 8px 16px;
    background: var(--azul);
    color: var(--blanco);
    text-decoration: none;
    font-size: 13px;
    font-weight: 600;
    border-radius: 10px;
    transition: background 0.2s, transform 0.15s;
    box-shadow: 0 2px 8px rgba(0,123,255,0.25);
}

.btn-login-header:hover {
    background: var(--azul-oscuro);
    transform: translateY(-1px);
}

.user-card {
    display: flex;
    align-items: center;
    gap: 10px;
    background: var(--gris-suave);
    border: 1px solid var(--gris-borde);
    border-radius: 12px;
    padding: 6px 6px 6px 12px;
}

.user-avatar {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    background: var(--azul);
    color: var(--blanco);
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 14px;
    font-weight: 700;
    flex-shrink: 0;
}

.user-datos {
    display: flex;
    flex-direction: column;
    line-height: 1.3;
}

.user-saludo {
    font-size: 10px;
    font-weight: 600;
    color: var(--gris-texto);
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.user-nombre {
    font-size: 13px;
    font-weight: 700;
    color: var(--negro);
}

.btn-logout {
    width: 32px;
    height: 32px;
    border-radius: 8px;
    background: #fff5f5;
    border: 1px solid #fecaca;
    color: #e53e3e;
    display: flex;
    align-items: center;
    justify-content: center;
    text-decoration: none;
    font-size: 16px;
    transition: background 0.2s, color 0.2s, border-color 0.2s;
}

.btn-logout:hover {
    background: #e53e3e;
    color: var(--blanco);
    border-color: #e53e3e;
}

@media (max-width: 768px) {
    .header     { padding: 0 16px; }
    .header-nav { display: none; }
    .user-datos { display: none; }
    .user-card  { padding: 6px; gap: 6px; }
}

        /* =====================================================
           BARRA DE PROGRESO — consistente con Confirmacion.aspx
        ===================================================== */
        .progreso-barra {
            background: var(--blanco);
            border-bottom: 1px solid var(--gris-borde);
            padding: 16px 40px;
        }

        .progreso-pasos {
            max-width: 860px;
            margin: 0 auto;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .paso {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 13px;
            font-weight: 600;
            color: var(--gris-texto);
        }

        .paso.activo { color: var(--azul); }
        .paso.hecho  { color: var(--verde); }

        .paso-num {
            width: 26px;
            height: 26px;
            border-radius: 50%;
            border: 2px solid currentColor;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
            font-weight: 700;
            flex-shrink: 0;
        }

        .paso.hecho  .paso-num { background: var(--verde); border-color: var(--verde); color: var(--blanco); }
        .paso.activo .paso-num { background: var(--azul);  border-color: var(--azul);  color: var(--blanco); }

        .paso-linea {
            flex: 1;
            height: 2px;
            background: var(--gris-borde);
            margin: 0 12px;
            max-width: 80px;
        }

        .paso-linea.hecha { background: var(--verde); }

        /* =====================================================
           PÁGINA PRINCIPAL
        ===================================================== */
        .pagina {
            flex: 1;
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 52px 20px 72px;
        }

        /* Encabezado de sección */
        .pagina-header {
            text-align: center;
            margin-bottom: 40px;
            animation: fadeDown 0.4s ease both;
        }

        @keyframes fadeDown {
            from { opacity: 0; transform: translateY(-14px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        .pagina-titulo {
            font-size: 32px;
            font-weight: 800;
            color: var(--negro);
            letter-spacing: -0.5px;
            margin-bottom: 8px;
        }

        .pagina-subtitulo {
            font-size: 15px;
            color: var(--gris-texto);
        }

        /* =====================================================
           LAYOUT DE DOS COLUMNAS
           Izquierda: lista de productos  |  Derecha: resumen
        ===================================================== */
        .carrito-layout {
            width: 100%;
            max-width: 1000px;
            display: grid;
            grid-template-columns: 1fr 320px;
            gap: 24px;
            align-items: start;
            animation: fadeUp 0.5s ease 0.1s both;
        }

        @keyframes fadeUp {
            from { opacity: 0; transform: translateY(20px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        /* =====================================================
           CARD IZQUIERDA — lista de productos
        ===================================================== */
        .card-productos {
            background: var(--blanco);
            border-radius: var(--radio);
            box-shadow: var(--sombra-card);
            overflow: hidden;
        }

        /* Cabecera de la card */
        .card-productos-header {
            padding: 22px 28px 18px;
            border-bottom: 1px solid var(--gris-borde);
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .card-productos-titulo {
            font-size: 13px;
            font-weight: 700;
            letter-spacing: 1.2px;
            text-transform: uppercase;
            color: var(--gris-texto);
        }

        /* Cantidad de items badge */
        .badge-items {
            background: var(--azul-claro);
            color: var(--azul);
            font-size: 12px;
            font-weight: 700;
            padding: 3px 10px;
            border-radius: 20px;
        }

        /* =====================================================
           FILA DE PRODUCTO — generada por el Repeater
        ===================================================== */
        .fila-producto {
            display: grid;

            /* imagen | info | cantidad | precio | subtotal | eliminar */
            grid-template-columns: 72px 1fr 110px 90px 90px 36px;

            align-items: center;
            gap: 16px;
            padding: 20px 28px;
            border-bottom: 1px solid var(--gris-suave);
            transition: background 0.15s;
        }

        .fila-producto:last-child { border-bottom: none; }

        .fila-producto:hover { background: #fafbff; }

      /* ── Imagen del producto ── */
.prod-imagen-wrapper {
    width: 68px;
    height: 68px;
    border-radius: var(--radio-img);
    background: linear-gradient(145deg, #f0f4ff, #e6eeff);
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    overflow: hidden;
    padding: 6px;
}

.prod-imagen-wrapper img {
    width: 100%;
    height: 100%;
    object-fit: contain;
    display: block;
}

        /* ── Info: nombre + detalle ── */
        .prod-info { min-width: 0; }

        .prod-nombre {
            font-size: 15px;
            font-weight: 700;
            color: var(--negro);
            margin-bottom: 4px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .prod-detalle {
            font-size: 12px;
            color: var(--gris-texto);
            background: var(--gris-suave);
            display: inline-block;
            padding: 2px 8px;
            border-radius: 6px;
        }

        /* ── Controles de cantidad ── */
        .prod-cantidad {
            display: flex;
            align-items: center;
            gap: 0;
            border: 2px solid var(--gris-borde);
            border-radius: 10px;
            overflow: hidden;
            width: fit-content;
        }

        .btn-cantidad {
            width: 32px;
            height: 32px;
            background: var(--gris-suave);
            border: none;
            cursor: pointer;
            font-size: 16px;
            font-weight: 700;
            color: var(--negro);
            display: flex;
            align-items: center;
            justify-content: center;
            transition: background 0.15s, color 0.15s;
            line-height: 1;
        }

        .btn-cantidad:hover {
            background: var(--azul-claro);
            color: var(--azul);
        }

        .cantidad-valor {
            width: 36px;
            text-align: center;
            font-size: 14px;
            font-weight: 700;
            color: var(--negro);
            border: none;
            border-left: 1px solid var(--gris-borde);
            border-right: 1px solid var(--gris-borde);
            background: var(--blanco);
            padding: 0;
            line-height: 32px;
            height: 32px;
        }

        /* ── Precio unitario ── */
        .prod-precio {
            font-size: 14px;
            color: var(--gris-texto);
            text-align: right;
        }

        /* ── Subtotal ── */
        .prod-subtotal {
            font-size: 16px;
            font-weight: 800;
            color: var(--negro);
            text-align: right;
        }

        /* ── Botón eliminar ── */
        .btn-eliminar {
            width: 34px;
            height: 34px;
            border-radius: 8px;
            background: var(--rojo-suave);
            border: none;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 15px;
            color: var(--rojo);
            transition: background 0.15s, transform 0.15s;
            text-decoration: none;
        }

        .btn-eliminar:hover {
            background: var(--rojo);
            color: var(--blanco);
            transform: scale(1.1);
        }

        /* =====================================================
           CARD DERECHA — resumen del pedido
        ===================================================== */
        .card-resumen {
            background: var(--blanco);
            border-radius: var(--radio);
            box-shadow: var(--sombra-card);
            overflow: hidden;
            position: sticky;
            top: 88px;           /* se queda fija al hacer scroll */
        }

        /* Cabecera del resumen */
        .resumen-header {
            background: linear-gradient(135deg, #0f2545 0%, #1a3a6e 60%, #1565c0 100%);
            padding: 22px 24px;
        }

        .resumen-header h3 {
            color: var(--blanco);
            font-size: 16px;
            font-weight: 700;
            margin-bottom: 2px;
        }

        .resumen-header p {
            color: rgba(255,255,255,0.6);
            font-size: 12px;
        }

        /* Cuerpo del resumen */
        .resumen-cuerpo { padding: 24px; }

        /* Fila de subtotal/envío */
        .resumen-fila {
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 14px;
            margin-bottom: 12px;
        }

        .resumen-fila-label { color: var(--gris-texto); }

        .resumen-fila-valor {
            font-weight: 600;
            color: var(--negro);
        }

        .resumen-fila-valor.gratis {
            color: var(--verde);
            font-weight: 700;
        }

        /* Separador */
        .resumen-separador {
            height: 1px;
            background: var(--gris-borde);
            margin: 16px 0;
        }

        /* Sección total general */
        .resumen-total {
            background: var(--gris-suave);
            border-radius: 12px;
            padding: 16px 18px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

        .resumen-total-label {
            font-size: 13px;
            font-weight: 700;
            color: var(--gris-texto);
            letter-spacing: 0.3px;
        }

        .resumen-total-label small {
            display: block;
            font-size: 11px;
            font-weight: 400;
            color: #adb5bd;
            margin-top: 2px;
        }

        /* Label del total de ASP.NET */
        .label-total {
            font-size: 28px;
            font-weight: 800;
            color: var(--azul);
            letter-spacing: -1px;
            line-height: 1;
        }

        /* ── Botón confirmar ── */
        .btn-confirmar {
            width: 100%;
            padding: 15px 20px;
            font-size: 15px;
            font-weight: 700;
            letter-spacing: 0.4px;
            color: var(--blanco);
            background: var(--azul);
            border: none;
            border-radius: var(--radio-btn);
            cursor: pointer;
            margin-bottom: 10px;
            transition: background 0.2s, transform 0.15s, box-shadow 0.2s;
            box-shadow: 0 4px 16px rgba(0,123,255,0.32);
        }

        .btn-confirmar:hover {
            background: var(--azul-oscuro);
            transform: translateY(-2px);
            box-shadow: 0 8px 24px rgba(0,123,255,0.42);
        }

        .btn-confirmar:active {
            transform: translateY(0);
            box-shadow: 0 2px 8px rgba(0,123,255,0.22);
        }

        /* ── Botón eliminar todo ── */
        .btn-eliminar-todo {
            width: 100%;
            padding: 12px 20px;
            font-size: 14px;
            font-weight: 600;
            color: var(--rojo);
            background: var(--blanco);
            border: 2px solid #fecaca;
            border-radius: var(--radio-btn);
            cursor: pointer;
            transition: background 0.2s, border-color 0.2s, color 0.2s, transform 0.15s;
        }

        .btn-eliminar-todo:hover {
            background: var(--rojo-suave);
            border-color: var(--rojo);
            transform: translateY(-1px);
        }

        /* Nota de seguridad */
        .resumen-nota {
            text-align: center;
            font-size: 12px;
            color: var(--gris-texto);
            margin-top: 14px;
        }

        .resumen-nota span { color: var(--verde); font-weight: 600; }

        /* Enlace seguir comprando */
        .link-seguir {
            display: block;
            text-align: center;
            margin-top: 16px;
            font-size: 13px;
            color: var(--azul);
            text-decoration: none;
            font-weight: 500;
            transition: color 0.2s;
        }

        .link-seguir:hover { color: var(--azul-oscuro); text-decoration: underline; }

        /* =====================================================
           PANEL CARRITO VACÍO
        ===================================================== */
        .panel-vacio {
            width: 100%;
            max-width: 520px;
            background: var(--blanco);
            border-radius: var(--radio);
            box-shadow: var(--sombra-card);
            text-align: center;
            padding: 72px 40px;
            animation: fadeUp 0.5s ease both;
        }

        .vacio-icono {
            font-size: 64px;
            display: block;
            margin-bottom: 20px;
            opacity: 0.3;
        }

        .vacio-titulo {
            font-size: 22px;
            font-weight: 800;
            color: var(--negro);
            margin-bottom: 8px;
        }

        .vacio-subtitulo {
            font-size: 14px;
            color: var(--gris-texto);
            margin-bottom: 28px;
            line-height: 1.6;
        }

        .btn-ir-catalogo {
            display: inline-block;
            padding: 13px 28px;
            background: var(--azul);
            color: var(--blanco);
            text-decoration: none;
            font-size: 15px;
            font-weight: 700;
            border-radius: var(--radio-btn);
            transition: background 0.2s, transform 0.15s;
            box-shadow: 0 4px 14px rgba(0,123,255,0.30);
        }

        .btn-ir-catalogo:hover {
            background: var(--azul-oscuro);
            transform: translateY(-2px);
        }

        /* =====================================================
           FOOTER
        ===================================================== */
        .page-footer {
            text-align: center;
            padding: 28px;
            color: var(--gris-texto);
            font-size: 13px;
            border-top: 1px solid var(--gris-borde);
            margin-top: auto;
        }

        /* =====================================================
           RESPONSIVE
        ===================================================== */
        @media (max-width: 820px) {
            .carrito-layout {
                grid-template-columns: 1fr;
            }

            .card-resumen { position: static; }
        }

        @media (max-width: 640px) {
            .fila-producto {
                grid-template-columns: 56px 1fr 36px;
                grid-template-rows: auto auto auto;
                gap: 10px;
                padding: 16px 18px;
            }

            /* En móvil: imagen ocupa 1 col, info 1 col, eliminar 1 col */
            .prod-imagen-wrapper { grid-row: 1; grid-column: 1; }
            .prod-info            { grid-row: 1; grid-column: 2; }
            .btn-eliminar         { grid-row: 1; grid-column: 3; }

            /* Segunda fila: cantidad + precio + subtotal */
            .prod-cantidad  { grid-row: 2; grid-column: 1 / 3; }
            .prod-precio    { display: none; }           
            .prod-subtotal  { grid-row: 2; grid-column: 3; font-size: 14px; }

            .header         { padding: 16px 20px; }
            .header-nav     { display: none; }
            .pagina-titulo  { font-size: 24px; }
            .progreso-barra { padding: 12px 20px; }
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- ================================================
             HEADER
        ================================================= -->
        <header class="header">

    <!-- IZQUIERDA: Logo -->
    <a href="Productos.aspx" class="header-logo">Precision<span>Tire</span></a>

    <!-- CENTRO: Navegación -->
    <nav class="header-nav">
        <a href="Productos.aspx">Catálogo</a>
        <a href="Carrito.aspx">Carrito</a>
        <a href="Historial.aspx">Mis pedidos</a>
    </nav>

    <!-- DERECHA: Usuario -->
    <div class="header-user">

        <%-- No logueado --%>
        <asp:PlaceHolder ID="phNoLogueado" runat="server">
            <a href="Login.aspx" class="btn-login-header">
                🔐 Iniciar sesión
            </a>
        </asp:PlaceHolder>

        <%-- Logueado --%>
        <asp:PlaceHolder ID="phLogueado" runat="server" Visible="false">
            <div class="user-card">
                <div class="user-avatar">
                    <asp:Label ID="lblAvatar" runat="server" />
                </div>
                <div class="user-datos">
                    <span class="user-saludo">Bienvenido</span>
                    <asp:Label ID="lblUsuario" runat="server" CssClass="user-nombre" />
                </div>
                <a href="Logout.aspx" class="btn-logout" title="Cerrar sesión">⏻</a>
            </div>
        </asp:PlaceHolder>

    </div>

</header>

        <!-- ================================================
             BARRA DE PROGRESO
        ================================================= -->
        <div class="progreso-barra">
            <div class="progreso-pasos">
                <div class="paso hecho">
                    <div class="paso-num">✓</div>
                    <span>Catálogo</span>
                </div>
                <div class="paso-linea hecha"></div>
                <div class="paso activo">
                    <div class="paso-num">2</div>
                    <span>Carrito</span>
                </div>
                <div class="paso-linea"></div>
                <div class="paso">
                    <div class="paso-num">3</div>
                    <span>Confirmación</span>
                </div>
            </div>
        </div>

        <!-- ================================================
             CONTENIDO PRINCIPAL
        ================================================= -->
        <main class="pagina">

            <!-- Encabezado de página -->
            <div class="pagina-header">
                <h1 class="pagina-titulo">Tu Carrito</h1>
                <p class="pagina-subtitulo">Revisa los detalles de tu pedido antes de finalizar</p>
            </div>

            <!-- ============================================
                 PANEL: carrito con productos
            ============================================= -->
            <asp:Panel ID="pnlCarrito" runat="server" Visible="false"
                       CssClass="carrito-layout">

                <!-- ── Columna izquierda: lista de items ── -->
                <div class="card-productos">

                    <!-- Cabecera con contador -->
                    <div class="card-productos-header">
                        <span class="card-productos-titulo">Productos seleccionados</span>
                        <asp:Label ID="lblCantidadItems" runat="server"
                                   CssClass="badge-items" />
                    </div>

                    <!-- Repeater: una fila por producto -->
                    <asp:Repeater ID="rptCarrito" runat="server" OnItemCommand="rptCarrito_ItemCommand">
                        <ItemTemplate>
                            <div class="fila-producto">

                                <!-- Imagen -->
                                <div class="prod-imagen-wrapper">
                                      <img
        src='<%# ResolveUrl((Eval("ImagenUrl") ?? "~/images/GomaPilotSport4.png").ToString()) %>'
        alt='<%# Eval("Nombre") %>' />
                                </div>

                                <!-- Nombre + detalle -->
                                <div class="prod-info">
                                    <div class="prod-nombre"><%# Eval("Nombre") %></div>
                                    <span class="prod-detalle">205/55 R16 · Unidad</span>
                                </div>

                                <!-- Controles de cantidad (solo visual, sin postback) -->
                               <div class="prod-cantidad">
                <asp:Button
                    runat="server"
                    Text="−"
                    CssClass="btn-cantidad"
                    CommandName="Restar"
                    CommandArgument='<%# Eval("ProductoId") %>'
                    CausesValidation="false" />

                <div class="cantidad-valor"><%# Eval("Cantidad") %></div>

                <asp:Button
                    runat="server"
                    Text="+"
                    CssClass="btn-cantidad"
                    CommandName="Sumar"
                    CommandArgument='<%# Eval("ProductoId") %>'
                    CausesValidation="false" />
            </div>

                                <!-- Precio unitario -->
                                <div class="prod-precio">
                                    <%# string.Format("{0:C2}", Eval("Precio")) %>
                                </div>

                                <!-- Subtotal -->
                                <div class="prod-subtotal">
                                    <%# string.Format("{0:C2}", Eval("Subtotal")) %>
                                </div>

                                <!-- Botón eliminar -->
                                 <asp:Button
                runat="server"
                Text="✕"
                CssClass="btn-eliminar"
                CommandName="Eliminar"
                CommandArgument='<%# Eval("ProductoId") %>'
                CausesValidation="false" />

        </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>

                <!-- ── Columna derecha: resumen y acciones ── -->
                <div class="card-resumen">

                    <!-- Cabecera oscura -->
                    <div class="resumen-header">
                        <h3>Resumen del pedido</h3>
                        <p>Antes de confirmar tu compra</p>
                    </div>

                    <!-- Cuerpo del resumen -->
                    <div class="resumen-cuerpo">

                        <!-- Desglose -->
                        <div class="resumen-fila">
                            <span class="resumen-fila-label">Subtotal productos</span>
                            <asp:Label ID="lblSubtotal" runat="server"
                                       CssClass="resumen-fila-valor" />
                        </div>
                        <div class="resumen-fila">
                            <span class="resumen-fila-label">Envío</span>
                            <span class="resumen-fila-valor gratis">Gratis</span>
                        </div>

                        <div class="resumen-separador"></div>

                        <!-- Total general -->
                        <div class="resumen-total">
                            <div class="resumen-total-label">
                                Total general
                                <small>IVA incluido</small>
                            </div>
                            <asp:Label ID="lblTotal" runat="server"
                                       CssClass="label-total" />
                        </div>

                        <!-- Botón confirmar -->
                        <asp:Button
                            ID="btnConfirmar"
                            runat="server"
                            Text="✓  Confirmar compra"
                            CssClass="btn-confirmar"
                            OnClick="btnConfirmar_Click" />

                        <!-- Botón eliminar todo -->
                        <asp:Button
                            ID="btnEliminarTodo"
                            runat="server"
                            Text="🗑  Eliminar todo"
                            CssClass="btn-eliminar-todo"
                            OnClick="btnEliminarTodo_Click"
                            OnClientClick="return confirm('¿Deseas vaciar el carrito?');" />

                        <!-- Nota de seguridad -->
                        <p class="resumen-nota">
                            <span>🔒 Pago seguro</span> · Tu información está protegida
                        </p>

                        <!-- Seguir comprando -->
                        <a href="Productos.aspx" class="link-seguir">
                            ← Seguir comprando
                        </a>

                    </div>
                </div>

            </asp:Panel>

            <!-- ============================================
                 PANEL: carrito vacío
            ============================================= -->
            <asp:Panel ID="pnlVacio" runat="server" Visible="false"
                       CssClass="panel-vacio">
                <span class="vacio-icono">🛒</span>
                <p class="vacio-titulo">Tu carrito está vacío</p>
                <p class="vacio-subtitulo">
                    Aún no has agregado ningún producto.<br />
                    Explora el catálogo y encuentra tu neumático ideal.
                </p>
                <a href="Productos.aspx" class="btn-ir-catalogo">
                    Ver catálogo
                </a>
            </asp:Panel>

        </main>

        <!-- ================================================
             FOOTER
        ================================================= -->
        <footer class="page-footer">
            © 2026 PrecisionTire · Todos los derechos reservados
        </footer>

    </form>
</body>
</html>