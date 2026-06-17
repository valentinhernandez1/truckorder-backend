namespace TruckOrder.Api.Domain.Entities;

public class Producto
{
    public int Id { get; set; }
    public int TruckId { get; set; }
    public int? CategoriaId { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public bool Disponible { get; set; } = true;

    public FoodTruck? Truck { get; set; }
    public Categoria? Categoria { get; set; }

    public void MarcarDisponible(bool disponible) => Disponible = disponible;
}
