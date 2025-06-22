using Microsoft.EntityFrameworkCore;

namespace ExamenBanco.Models
{
    public class Data : DbContext
    {
        // Constructor que recibe las opciones de configuración para el contexto de base de datos
        public Data(DbContextOptions<Data> options) : base(options)
        {
        }

        // Representa la tabla de Usuarios en la base de datos
        public DbSet<Usuario> Usuarios { get; set; }

        // Representa la tabla de Transacciones en la base de datos
        public DbSet<Transaccion> Transacciones { get; set; }
    }
}
