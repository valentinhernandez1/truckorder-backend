using Microsoft.EntityFrameworkCore;
using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Repositories;

namespace TruckOrder.Api.Infrastructure.Repositories;

public class RepoProductos : IRepoProductos
{
    private readonly TruckOrderDbContext _db;

    public RepoProductos(TruckOrderDbContext db) => _db = db;

    public Task<Producto?> BuscarPorIdAsync(int id) =>
        _db.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Producto>> BuscarPorTruckAsync(int truckId) =>
        await _db.Productos
            .Include(p => p.Categoria)
            .Where(p => p.TruckId == truckId)
            .OrderBy(p => p.CategoriaId)
            .ThenBy(p => p.Nombre)
            .ToListAsync();

    public async Task<Producto> GuardarAsync(Producto producto)
    {
        if (producto.Id == 0)
            _db.Productos.Add(producto);

        await _db.SaveChangesAsync();
        return producto;
    }
}
