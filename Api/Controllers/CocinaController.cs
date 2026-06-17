using Microsoft.AspNetCore.Mvc;
using TruckOrder.Api.Api.Dtos;
using TruckOrder.Api.Application.Services;

namespace TruckOrder.Api.Api.Controllers;

/// <summary>
/// Endpoints del PUC-003: pantallas de cocina y pantalla pública del food truck.
/// </summary>
[ApiController]
[Route("api/trucks/{truckId:int}")]
[Tags("Cocina y Pantalla Pública (PUC-003)")]
public class CocinaController : ControllerBase
{
    private readonly PedidosService _pedidos;

    public CocinaController(PedidosService pedidos) => _pedidos = pedidos;

    /// <summary>Pedidos PAGADOS o ENPREPARACION para mostrar en la pantalla de cocina.</summary>
    [HttpGet("cocina/pedidos")]
    public async Task<ActionResult<IEnumerable<PedidoDto>>> ListarParaCocina(int truckId)
    {
        var pedidos = await _pedidos.ListarParaCocinaAsync(truckId);
        return Ok(pedidos.Select(PedidoDto.From));
    }

    /// <summary>Pedidos LISTOS para retirar. Los muestra la pantalla pública.</summary>
    [HttpGet("publica/listos")]
    public async Task<ActionResult<IEnumerable<PedidoDto>>> ListarListos(int truckId)
    {
        var pedidos = await _pedidos.ListarListosAsync(truckId);
        return Ok(pedidos.Select(PedidoDto.From));
    }
}
