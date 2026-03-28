using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Domain.Entidades;

namespace PuntoDeVenta.Infrastructure.Persistence;

public class PuntoDeVentaDbContext : DbContext
{
    public PuntoDeVentaDbContext(DbContextOptions<PuntoDeVentaDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<RegistroCaja> RegistroCaja { get; set; }
    public DbSet<DetalleVenta> Detalle_Ventas { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<ProductoEliminado> Productos_Eliminados { get; set; }


    public DbSet<ProductosBajosEnStock> VistaProductosBajoStockRecientes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<ProductosBajosEnStock>()
            .ToView("VistaProductosBajoStockRecientes")
            .HasNoKey(); // ← Si la vista no tiene una PK clara

        modelBuilder.Entity<DetalleVenta>()
            .Property(d => d.PrecioUnitario)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Producto>()
            .Property(p => p.PrecioUnitario)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Producto>()
            .Property(p => p.PrecioVenta)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ProductoEliminado>()
            .Property(p => p.PrecioUnitario)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ProductoEliminado>()
            .Property(p => p.PrecioVenta)
            .HasPrecision(18, 2);

        modelBuilder.Entity<RegistroCaja>()
            .Property(r => r.CambioEntregado)
            .HasPrecision(18, 2);

        modelBuilder.Entity<RegistroCaja>()
            .Property(r => r.MontoPagado)
            .HasPrecision(18, 2);

        modelBuilder.Entity<RegistroCaja>()
            .Property(r => r.TotalVenta)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Venta>()
            .Property(v => v.Total)
            .HasPrecision(18, 2);
    }

}
