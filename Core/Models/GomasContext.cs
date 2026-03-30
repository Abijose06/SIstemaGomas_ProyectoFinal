using System.Data.Entity;

namespace Core.Models
{
    public class GomasContext : DbContext
    {
        // Esto le dice a EF que busque el ConnectionString "GomasContext" en tu Web.config
        public GomasContext() : base("name=GomasContext")
        {
        }

        // Aquí irán las referencias a tus tablas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
    }
}