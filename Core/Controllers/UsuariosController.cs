using System;
using System.Linq;
using System.Web.Http;
using Core.Models;
using Core.Helpers;

namespace Core.Controllers
{
    // Esto define la ruta base. Para llamar a este controlador usarán: http://localhost:puerto/api/usuarios
    [RoutePrefix("api/usuarios")]
    public class UsuariosController : ApiController
    {
        // Instanciamos el motor de la base de datos
        private GomasContext db = new GomasContext();

        // 1. ENDPOINT DE REGISTRO
        // Ruta: POST api/usuarios/registro
        [HttpPost]
        [Route("registro")]
        public IHttpActionResult RegistrarUsuario(Usuario nuevoUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos incompletos o incorrectos.");
            }

            // Validar que el documento (Cédula/RNC) no exista ya en la base de datos
            bool existe = db.Usuarios.Any(u => u.Documento == nuevoUsuario.Documento);
            if (existe)
            {
                return BadRequest("Ya existe un usuario registrado con este documento.");
            }

            try
            {
                // Encriptamos la clave que llegó en texto plano
                nuevoUsuario.ClaveHash = SeguridadHelper.HashPassword(nuevoUsuario.ClaveHash);
                nuevoUsuario.Estado = true; // Activo por defecto

                // Guardamos en la base de datos
                db.Usuarios.Add(nuevoUsuario);
                db.SaveChanges();

                return Ok(new { Mensaje = "Usuario registrado exitosamente.", Id = nuevoUsuario.IdUsuario });
            }
            catch (Exception ex)
            {
                // En un futuro aquí usaremos Log4Net para guardar el ex.Message
                return InternalServerError(ex);
            }
        }

        // 2. ENDPOINT DE LOGIN
        // Ruta: POST api/usuarios/login
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Documento) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Debe enviar el documento y la contraseña.");
            }

            // Buscamos al usuario por su documento
            var usuarioDb = db.Usuarios.FirstOrDefault(u => u.Documento == request.Documento);

            if (usuarioDb == null)
            {
                return NotFound(); // No existe el usuario
            }

            if (!usuarioDb.Estado)
            {
                return BadRequest("Este usuario está inactivo.");
            }

            // Validamos la contraseña usando nuestro Helper
            bool claveCorrecta = SeguridadHelper.VerificarPassword(request.Password, usuarioDb.ClaveHash);

            if (!claveCorrecta)
            {
                return Unauthorized(); // Error 401: Credenciales inválidas
            }

            // Si todo está bien, devolvemos los datos del usuario (¡NUNCA devolvemos el ClaveHash!)
            return Ok(new
            {
                IdUsuario = usuarioDb.IdUsuario,
                NombreCompleto = usuarioDb.Nombres + " " + usuarioDb.Apellidos,
                Rol = usuarioDb.Rol,
                Token = Guid.NewGuid().ToString() // Simulamos un token de sesión
            });
        }

        // Se ejecuta cuando el controlador se destruye para liberar la base de datos
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Clase auxiliar (DTO) para recibir solo los datos necesarios en el Login
    public class LoginRequest
    {
        public string Documento { get; set; }
        public string Password { get; set; }
    }
}