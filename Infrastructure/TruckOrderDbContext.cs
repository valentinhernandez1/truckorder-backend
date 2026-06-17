using Microsoft.EntityFrameworkCore;
using TruckOrder.Api.Domain.Entities;

namespace TruckOrder.Api.Infrastructure;

public class TruckOrderDbContext : DbContext
{
    public TruckOrderDbContext(DbContextOptions<TruckOrderDbContext> options) : base(options) { }

    public DbSet<FoodTruck> FoodTrucks => Set<FoodTruck>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItemsPedido => Set<ItemPedido>();
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // FoodTruck
        mb.Entity<FoodTruck>(e =>
        {
            e.HasIndex(t => t.CodigoQrMenu).IsUnique();
            e.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
            e.Property(t => t.CodigoQrMenu).IsRequired().HasMaxLength(50);
            e.HasMany(t => t.Productos)
                .WithOne(p => p.Truck!)
                .HasForeignKey(p => p.TruckId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(t => t.Pedidos)
                .WithOne(p => p.Truck!)
                .HasForeignKey(p => p.TruckId);
        });

        // Producto
        mb.Entity<Producto>(e =>
        {
            e.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            e.Property(p => p.Precio).HasColumnType("decimal(10,2)");
            e.HasOne(p => p.Categoria).WithMany().HasForeignKey(p => p.CategoriaId);
        });

        // Pedido y composición con ItemPedido
        mb.Entity<Pedido>(e =>
        {
            e.Property(p => p.Total).HasColumnType("decimal(10,2)");
            e.HasMany(p => p.Items)
                .WithOne(i => i.Pedido!)
                .HasForeignKey(i => i.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);  // composición fuerte
            e.HasOne(p => p.Pago)
                .WithOne(pg => pg.Pedido!)
                .HasForeignKey<Pago>(pg => pg.PedidoId);
            e.HasIndex(p => new { p.TruckId, p.Numero });
        });

        mb.Entity<ItemPedido>(e =>
        {
            e.Property(i => i.PrecioUnitario).HasColumnType("decimal(10,2)");
            e.Property(i => i.Subtotal).HasColumnType("decimal(10,2)");
            e.HasOne(i => i.Producto).WithMany().HasForeignKey(i => i.ProductoId);
        });

        mb.Entity<Pago>(e =>
        {
            e.Property(p => p.Monto).HasColumnType("decimal(10,2)");
            e.Property(p => p.MontoRecibido).HasColumnType("decimal(10,2)");
            e.Property(p => p.Cambio).HasColumnType("decimal(10,2)");
            e.Property(p => p.ReferenciaExterna).HasMaxLength(100);
            e.HasIndex(p => p.PedidoId).IsUnique();
        });
    }
}
