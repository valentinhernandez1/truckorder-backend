using TruckOrder.Api.Domain.Entities;

namespace TruckOrder.Api.Domain.Repositories;

public interface IRepoPagos
{
    Task<Pago?> BuscarPorPedidoAsync(int pedidoId);
    Task<Pago> GuardarAsync(Pago pago);
}
