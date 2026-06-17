using Microsoft.EntityFrameworkCore;
using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Repositories;

namespace TruckOrder.Api.Infrastructure.Repositories;

public class RepoPagos : IRepoPagos
{
    private readonly TruckOrderDbContext _db;

    public RepoPagos(TruckOrderDbContext db) => _db = db;

    public Task<Pago?> BuscarPorPedidoAsync(int pedidoId) =>
        _db.Pagos.FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

    public async Task<Pago> GuardarAsync(Pago pago)
    {
        if (pago.Id == 0)
            _db.Pagos.Add(pago);

        await _db.SaveChangesAsync();
        return pago;
    }
}
