using System;
using System.Security.Cryptography;
using System.Text;

namespace Core.Helpers
{
    public static class SeguridadHelper
    {
        // Convierte la contraseña en texto plano a una cadena encriptada irreconocible
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Computar el Hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convertir el arreglo de bytes a un string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Compara la contraseña que digita el usuario en el Login con la que está en la base de datos
        public static bool VerificarPassword(string passwordIngresado, string hashGuardado)
        {
            string hashDelIngreso = HashPassword(passwordIngresado);

            // Compara ignorando mayúsculas/minúsculas para evitar errores técnicos
            return StringComparer.OrdinalIgnoreCase.Compare(hashDelIngreso, hashGuardado) == 0;
        }
    }
}