using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Enums;

namespace TruckOrder.Api.Domain.Repositories;

public interface IRepoPedidos
{
    Task<Pedido?> BuscarPorIdAsync(int id);
    Task<Pedido?> BuscarPorNumeroAsync(int truckId, int numero);
    Task<List<Pedido>> BuscarPorTruckAsync(int truckId);
    Task<List<Pedido>> BuscarPorEstadoAsync(int truckId, EstadoPedido estado);
    Task<int> ProximoNumeroAsync(int truckId);
    Task<Pedido> GuardarAsync(Pedido pedido);
}
