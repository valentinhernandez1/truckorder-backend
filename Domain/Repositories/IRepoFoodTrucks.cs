using TruckOrder.Api.Domain.Entities;

namespace TruckOrder.Api.Domain.Repositories;

public interface IRepoFoodTrucks
{
    Task<FoodTruck?> BuscarPorIdAsync(int id);
    Task<FoodTruck?> BuscarPorQrAsync(string codigo);
}
