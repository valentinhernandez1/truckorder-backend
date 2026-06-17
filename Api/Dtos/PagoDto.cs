using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Enums;

namespace TruckOrder.Api.Api.Dtos;

public record PagoDto(
    int Id,
    decimal Monto,
    MetodoPago Metodo,
    EstadoPago Estado,
    string? ReferenciaExterna,
    decimal? MontoRecibido,
    decimal? Cambio,
    DateTime Fecha
)
{
    public static PagoDto From(Pago p) => new(
        p.Id, p.Monto, p.Metodo, p.Estado,
        p.ReferenciaExterna, p.MontoRecibido, p.Cambio, p.Fecha
    );
}
