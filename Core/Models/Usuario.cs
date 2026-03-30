using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("tblUsuario")] // El nombre EXACTO de tu tabla en SQL
    public class Usuario
    {
        [Key] // Le dice a EF que este es tu Primary Key
        public int IdUsuario { get; set; }

        public int TipoDocumento { get; set; }

        [Required]
        [StringLength(15)]
        public string Documento { get; set; }

        [Required]
        [StringLength(80)]
        public string Nombres { get; set; }

        [Required]
        [StringLength(80)]
        public string Apellidos { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(255)]
        public string Correo { get; set; }

        public bool Estado { get; set; }

        [Required]
        [StringLength(255)]
        public string ClaveHash { get; set; }

        [Required]
        [StringLength(50)]
        public string Rol { get; set; }
    }
}