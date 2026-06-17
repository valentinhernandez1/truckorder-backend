using Microsoft.AspNetCore.Mvc;
using TruckOrder.Api.Api.Dtos;
using TruckOrder.Api.Application.Services;

namespace TruckOrder.Api.Api.Controllers;

[ApiController]
[Route("api")]
[Tags("Menú y Productos")]
public class ProductosController : ControllerBase
{
    private readonly MenuService _menu;

    public ProductosController(MenuService menu) => _menu = menu;

    /// <summary>Productos del menú de un food truck. Lo usa la pantalla del cajero.</summary>
    [HttpGet("trucks/{truckId:int}/productos")]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> Listar(int truckId)
    {
        try
        {
            var productos = await _menu.ObtenerMenuAsync(truckId);
            return Ok(productos.Select(ProductoDto.From));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    /// <summary>Menú público para el cliente que escanea el QR del food truck.</summary>
    [HttpGet("menu-qr/{codigo}")]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> MenuPorQr(string codigo)
    {
        try
        {
            var productos = await _menu.ObtenerMenuPorQrAsync(codigo);
            return Ok(productos.Select(ProductoDto.From));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }
}
