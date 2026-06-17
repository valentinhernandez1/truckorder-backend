namespace TruckOrder.Api.Domain.Entities;

public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProductoId { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public Pedido? Pedido { get; set; }
    public Producto? Producto { get; set; }

    public void Recalcular()
    {
        Subtotal = PrecioUnitario * Cantidad;
    }
}
