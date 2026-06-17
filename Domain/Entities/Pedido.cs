using TruckOrder.Api.Domain.Enums;

namespace TruckOrder.Api.Domain.Entities;

/// <summary>
/// Entidad raiz del agregado Pedido. Concentra el comportamiento (DDD), no es anemica.
/// </summary>
public class Pedido
{
    public int Id { get; set; }
    public int TruckId { get; set; }
    public int Numero { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public EstadoPedido Estado { get; set; } = EstadoPedido.Nuevo;
    public decimal Total { get; set; }

    public FoodTruck? Truck { get; set; }
    public List<ItemPedido> Items { get; set; } = new();
    public Pago? Pago { get; set; }

    // ----- Comportamiento del dominio -----

    public ItemPedido AgregarItem(Producto producto, int cantidad)
    {
        if (!producto.Disponible)
            throw new InvalidOperationException($"El producto '{producto.Nombre}' no está disponible.");

        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));

        // Si ya hay una línea con ese producto, sumamos cantidad.
        var existente = Items.FirstOrDefault(i => i.ProductoId == producto.Id);
        if (existente != null)
        {
            existente.Cantidad += cantidad;
            existente.Recalcular();
            RecalcularTotal();
            return existente;
        }

        var item = new ItemPedido
        {
            ProductoId = producto.Id,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio,
            Subtotal = producto.Precio * cantidad
        };
        Items.Add(item);
        RecalcularTotal();
        return item;
    }

    public void QuitarItem(int itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new KeyNotFoundException($"Item {itemId} no pertenece al pedido.");
        Items.Remove(item);
        RecalcularTotal();
    }

    public decimal RecalcularTotal()
    {
        Total = Items.Sum(i => i.Subtotal);
        return Total;
    }

    public void CambiarEstado(EstadoPedido nuevo)
    {
        // Reglas mínimas de transición; alcanza para el MVP.
        if (Estado == EstadoPedido.Entregado)
            throw new InvalidOperationException("Un pedido entregado no puede cambiar de estado.");
        if (Estado == EstadoPedido.Cancelado)
            throw new InvalidOperationException("Un pedido cancelado no puede cambiar de estado.");

        Estado = nuevo;
    }
}
