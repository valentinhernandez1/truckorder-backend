using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Enums;

namespace TruckOrder.Api.Api.Dtos;

public record ItemPedidoDto(
    int Id,
    int ProductoId,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal
)
{
    public static ItemPedidoDto From(ItemPedido i) =>
        new(i.Id, i.ProductoId, i.Cantidad, i.PrecioUnitario, i.Subtotal);
}

public record PedidoDto(
    int Id,
    int TruckId,
    int Numero,
    DateTime Fecha,
    EstadoPedido Estado,
    decimal Total,
    List<ItemPedidoDto> Items,
    PagoDto? Pago
)
{
    public static PedidoDto From(Pedido p) => new(
        p.Id, p.TruckId, p.Numero, p.Fecha, p.Estado, p.Total,
        p.Items.Select(ItemPedidoDto.From).ToList(),
        p.Pago is null ? null : PagoDto.From(p.Pago)
    );
}
