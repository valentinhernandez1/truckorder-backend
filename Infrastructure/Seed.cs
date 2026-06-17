using TruckOrder.Api.Domain.Entities;

namespace TruckOrder.Api.Infrastructure;

/// <summary>
/// Carga datos de prueba al iniciar la API si la base está vacía.
/// </summary>
public static class Seed
{
    public static void Run(TruckOrderDbContext db)
    {
        if (db.FoodTrucks.Any())
            return;

        var cHamb = new Categoria { Nombre = "Hamburguesas" };
        var cPapas = new Categoria { Nombre = "Papas" };
        var cBebidas = new Categoria { Nombre = "Bebidas" };
        var cPostres = new Categoria { Nombre = "Postres" };

        db.Categorias.AddRange(cHamb, cPapas, cBebidas, cPostres);

        var truck = new FoodTruck
        {
            Nombre = "Truck & Roll #1",
            CodigoQrMenu = "TR-001",
            UbicacionActual = "Parque Rodó"
        };
        db.FoodTrucks.Add(truck);
        db.SaveChanges();

        var productos = new[]
        {
            new Producto { TruckId = truck.Id, CategoriaId = cHamb.Id, Nombre = "Doble cheese", Descripcion = "Doble medallón con queso cheddar", Precio = 320m, Disponible = true },
            new Producto { TruckId = truck.Id, CategoriaId = cHamb.Id, Nombre = "Clásica", Descripcion = "Hamburguesa simple con lechuga y tomate", Precio = 280m, Disponible = true },
            new Producto { TruckId = truck.Id, CategoriaId = cHamb.Id, Nombre = "Veggie", Descripcion = "Hamburguesa vegetariana de garbanzos", Precio = 290m, Disponible = true },
            new Producto { TruckId = truck.Id, CategoriaId = cHamb.Id, Nombre = "BBQ bacon", Descripcion = "Con bacon y salsa BBQ", Precio = 340m, Disponible = true },
            new Producto { TruckId = truck.Id, CategoriaId = cHamb.Id, Nombre = "Pollo crispy", Descripcion = "Hamburguesa de pollo rebozado", Precio = 310m, Disponible = false },
            new Producto { TruckId = truck.Id, CategoriaId = cPapas.Id, Nombre = "Papas chicas", Descripcion = "Porción individual", Precio = 120m, Disponible = true },
            new Producto { TruckId = truck.Id, CategoriaId = cPapas.Id, Nombre = "Papas grandes", Descripcion = "Para compartir", Precio = 200m, Disponible = true },
            new Producto { TruckId = truck.Id, CategoriaId = cBebidas.Id, Nombre = "Bebida cola", Descripcion = "500ml", Precio = 80m, Disponible = true },
            new Producto { TruckId = truck.Id, CategoriaId = cBebidas.Id, Nombre = "Bebida agua", Descripcion = "500ml sin gas", Precio = 70m, Disponible = true },
            new Producto { TruckId = truck.Id, CategoriaId = cPostres.Id, Nombre = "Brownie", Descripcion = "Con chips de chocolate", Precio = 150m, Disponible = true },
        };
        db.Productos.AddRange(productos);
        db.SaveChanges();
    }
}
