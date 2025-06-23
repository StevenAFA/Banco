using Microsoft.EntityFrameworkCore;
namespace ExamenBanco.Models
{
    public class Data : DbContext
    {
        public Data(DbContextOptions<Data> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
    }
}
