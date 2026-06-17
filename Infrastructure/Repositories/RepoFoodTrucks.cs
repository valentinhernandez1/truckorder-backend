using Microsoft.EntityFrameworkCore;
using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Repositories;

namespace TruckOrder.Api.Infrastructure.Repositories;

public class RepoFoodTrucks : IRepoFoodTrucks
{
    private readonly TruckOrderDbContext _db;

    public RepoFoodTrucks(TruckOrderDbContext db) => _db = db;

    public Task<FoodTruck?> BuscarPorIdAsync(int id) =>
        _db.FoodTrucks.FirstOrDefaultAsync(t => t.Id == id);

    public Task<FoodTruck?> BuscarPorQrAsync(string codigo) =>
        _db.FoodTrucks.FirstOrDefaultAsync(t => t.CodigoQrMenu == codigo);
}
