using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Enums;
using TruckOrder.Api.Domain.Repositories;

namespace TruckOrder.Api.Application.Services;

/// <summary>
/// Orquesta el ciclo de vida del pedido. Cubre el PUC-001 (armar) y el
/// PUC-003 (cambios de estado para cocina y entrega).
/// </summary>
public class PedidosService
{
    private readonly IRepoPedidos _pedidos;
    private readonly IRepoProductos _productos;
    private readonly IRepoFoodTrucks _trucks;

    public PedidosService(IRepoPedidos pedidos, IRepoProductos productos, IRepoFoodTrucks trucks)
    {
        _pedidos = pedidos;
        _productos = productos;
        _trucks = trucks;
    }

    // ---------- PUC-001 ----------

    public async Task<Pedido> CrearPedidoAsync(int truckId)
    {
        var truck = await _trucks.BuscarPorIdAsync(truckId)
            ?? throw new KeyNotFoundException($"Food truck {truckId} no encontrado.");

        var pedido = new Pedido
        {
            TruckId = truck.Id,
            Numero = await _pedidos.ProximoNumeroAsync(truck.Id),
            Estado = EstadoPedido.Nuevo,
            Total = 0m
        };

        return await _pedidos.GuardarAsync(pedido);
    }

    public async Task<Pedido> AgregarItemAsync(int pedidoId, int productoId, int cantidad)
    {
        var pedido = await ObtenerOFallarAsync(pedidoId);
        if (pedido.Estado != EstadoPedido.Nuevo)
            throw new InvalidOperationException("Solo se pueden modificar pedidos en estado Nuevo.");

        var producto = await _productos.BuscarPorIdAsync(productoId)
            ?? throw new KeyNotFoundException($"Producto {productoId} no encontrado.");

        pedido.AgregarItem(producto, cantidad);
        return await _pedidos.GuardarAsync(pedido);
    }

    public async Task<Pedido> QuitarItemAsync(int pedidoId, int itemId)
    {
        var pedido = await ObtenerOFallarAsync(pedidoId);
        if (pedido.Estado != EstadoPedido.Nuevo)
            throw new InvalidOperationException("Solo se pueden modificar pedidos en estado Nuevo.");

        pedido.QuitarItem(itemId);
        return await _pedidos.GuardarAsync(pedido);
    }

    public async Task<Pedido> CancelarAsync(int pedidoId)
    {
        var pedido = await ObtenerOFallarAsync(pedidoId);
        pedido.CambiarEstado(EstadoPedido.Cancelado);
        return await _pedidos.GuardarAsync(pedido);
    }

    // ---------- PUC-003 ----------

    public async Task<List<Pedido>> ListarParaCocinaAsync(int truckId)
    {
        var pagados = await _pedidos.BuscarPorEstadoAsync(truckId, EstadoPedido.Pagado);
        var enPrep = await _pedidos.BuscarPorEstadoAsync(truckId, EstadoPedido.EnPreparacion);
        return pagados.Concat(enPrep).OrderBy(p => p.Fecha).ToList();
    }

    public Task<List<Pedido>> ListarListosAsync(int truckId) =>
        _pedidos.BuscarPorEstadoAsync(truckId, EstadoPedido.Listo);

    public async Task<Pedido> CambiarEstadoAsync(int pedidoId, EstadoPedido nuevo)
    {
        var pedido = await ObtenerOFallarAsync(pedidoId);
        pedido.CambiarEstado(nuevo);
        return await _pedidos.GuardarAsync(pedido);
    }

    public Task<Pedido> ObtenerAsync(int pedidoId) => ObtenerOFallarAsync(pedidoId);

    private async Task<Pedido> ObtenerOFallarAsync(int pedidoId) =>
        await _pedidos.BuscarPorIdAsync(pedidoId)
            ?? throw new KeyNotFoundException($"Pedido {pedidoId} no encontrado.");
}
