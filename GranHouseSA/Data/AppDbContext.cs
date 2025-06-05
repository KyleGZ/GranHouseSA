using GranHouseSA.Models;
using Microsoft.EntityFrameworkCore;

namespace GranHouseSA.Data
{
    public class AppDbContext : DbContext
    {
        //Contructor con parámetros
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //Muy importante crear el DbSet
        public DbSet<Clientes> clientes { get; set; }

        public DbSet<Paquete> paquete { get; set; }

        public DbSet<Usuarios> usuarios { get; set; }

        public DbSet<Compras> compras { get; set; }

        public DbSet<Reservacion> reservaciones { get; set; }

    }
}
