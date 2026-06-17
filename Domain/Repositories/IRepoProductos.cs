using TruckOrder.Api.Domain.Entities;

namespace TruckOrder.Api.Domain.Repositories;

public interface IRepoProductos
{
    Task<Producto?> BuscarPorIdAsync(int id);
    Task<List<Producto>> BuscarPorTruckAsync(int truckId);
    Task<Producto> GuardarAsync(Producto producto);
}
