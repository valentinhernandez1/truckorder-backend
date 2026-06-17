namespace TruckOrder.Api.Domain.Entities;

public class FoodTruck
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CodigoQrMenu { get; set; } = string.Empty;
    public string? UbicacionActual { get; set; }

    public List<Producto> Productos { get; set; } = new();
    public List<Pedido> Pedidos { get; set; } = new();
}
