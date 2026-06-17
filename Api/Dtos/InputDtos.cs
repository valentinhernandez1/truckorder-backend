using System.ComponentModel.DataAnnotations;
using TruckOrder.Api.Domain.Enums;

namespace TruckOrder.Api.Api.Dtos;

public class CrearPedidoIn
{
    [Range(1, int.MaxValue)]
    public int TruckId { get; set; }
}

public class AgregarItemIn
{
    [Range(1, int.MaxValue)]
    public int ProductoId { get; set; }

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }
}

public class CobrarIn
{
    [Required]
    public MetodoPago Metodo { get; set; }

    /// <summary>Requerido si el método es Efectivo.</summary>
    public decimal? MontoRecibido { get; set; }
}

public class CambiarEstadoIn
{
    [Required]
    public EstadoPedido Estado { get; set; }
}
