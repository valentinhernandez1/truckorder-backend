using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Repositories;

namespace TruckOrder.Api.Application.Services;

/// <summary>
/// Servicio de aplicación que expone el menú del food truck.
/// Lo consume la pantalla del cajero al armar el pedido y la consulta por QR del cliente.
/// </summary>
public class MenuService
{
    private readonly IRepoProductos _productos;
    private readonly IRepoFoodTrucks _trucks;

    public MenuService(IRepoProductos productos, IRepoFoodTrucks trucks)
    {
        _productos = productos;
        _trucks = trucks;
    }

    public async Task<List<Producto>> ObtenerMenuAsync(int truckId)
    {
        var truck = await _trucks.BuscarPorIdAsync(truckId)
            ?? throw new KeyNotFoundException($"Food truck {truckId} no encontrado.");

        return await _productos.BuscarPorTruckAsync(truck.Id);
    }

    public async Task<List<Producto>> ObtenerMenuPorQrAsync(string codigoQr)
    {
        var truck = await _trucks.BuscarPorQrAsync(codigoQr)
            ?? throw new KeyNotFoundException($"Código QR '{codigoQr}' no corresponde a ningún food truck.");

        return await _productos.BuscarPorTruckAsync(truck.Id);
    }
}
