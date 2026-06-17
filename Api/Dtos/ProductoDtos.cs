using TruckOrder.Api.Domain.Entities;

namespace TruckOrder.Api.Api.Dtos;

public record CategoriaDto(int Id, string Nombre);

public record ProductoDto(
    int Id,
    string Nombre,
    string? Descripcion,
    decimal Precio,
    bool Disponible,
    CategoriaDto? Categoria
)
{
    public static ProductoDto From(Producto p) => new(
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.Precio,
        p.Disponible,
        p.Categoria is null ? null : new CategoriaDto(p.Categoria.Id, p.Categoria.Nombre)
    );
}
