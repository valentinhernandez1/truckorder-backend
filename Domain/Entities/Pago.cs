using TruckOrder.Api.Domain.Enums;

namespace TruckOrder.Api.Domain.Entities;

public class Pago
{
    public int Id { get; set; }
    public int PedidoId { get; set; }

    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public MetodoPago Metodo { get; set; }
    public EstadoPago Estado { get; set; } = EstadoPago.Pendiente;

    public string? ReferenciaExterna { get; set; }  // id devuelto por la pasarela
    public decimal? MontoRecibido { get; set; }     // solo para efectivo
    public decimal? Cambio { get; set; }            // solo para efectivo

    public Pedido? Pedido { get; set; }
}
