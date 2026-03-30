using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("tblProducto")]
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required]
        [StringLength(100)]
        public string Marca { get; set; }

        [Required]
        [StringLength(100)]
        public string Modelo { get; set; }

        [Required]
        [StringLength(15)]
        public string Medida { get; set; }

        public decimal PrecioVenta { get; set; }

        public decimal Costo { get; set; }

        public bool Estado { get; set; }
    }
}