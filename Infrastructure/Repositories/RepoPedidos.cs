using Microsoft.EntityFrameworkCore;
using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Enums;
using TruckOrder.Api.Domain.Repositories;

namespace TruckOrder.Api.Infrastructure.Repositories;

public class RepoPedidos : IRepoPedidos
{
    private readonly TruckOrderDbContext _db;

    public RepoPedidos(TruckOrderDbContext db) => _db = db;

    public Task<Pedido?> BuscarPorIdAsync(int id) =>
        _db.Pedidos
            .Include(p => p.Items)
            .Include(p => p.Pago)
            .FirstOrDefaultAsync(p => p.Id == id);

    public Task<Pedido?> BuscarPorNumeroAsync(int truckId, int numero) =>
        _db.Pedidos
            .Include(p => p.Items)
            .Include(p => p.Pago)
            .FirstOrDefaultAsync(p => p.TruckId == truckId && p.Numero == numero);

    public async Task<List<Pedido>> BuscarPorTruckAsync(int truckId) =>
        await _db.Pedidos
            .Include(p => p.Items)
            .Include(p => p.Pago)
            .Where(p => p.TruckId == truckId)
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();

    public async Task<List<Pedido>> BuscarPorEstadoAsync(int truckId, EstadoPedido estado) =>
        await _db.Pedidos
            .Include(p => p.Items)
            .Include(p => p.Pago)
            .Where(p => p.TruckId == truckId && p.Estado == estado)
            .OrderBy(p => p.Fecha)
            .ToListAsync();

    public async Task<int> ProximoNumeroAsync(int truckId)
    {
        var max = await _db.Pedidos
            .Where(p => p.TruckId == truckId)
            .MaxAsync(p => (int?)p.Numero) ?? 0;
        return max + 1;
    }

    public async Task<Pedido> GuardarAsync(Pedido pedido)
    {
        if (pedido.Id == 0)
            _db.Pedidos.Add(pedido);

        await _db.SaveChangesAsync();
        return pedido;
    }
}
