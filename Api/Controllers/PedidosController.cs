using Microsoft.AspNetCore.Mvc;
using TruckOrder.Api.Api.Dtos;
using TruckOrder.Api.Application.PaymentStrategies;
using TruckOrder.Api.Application.Services;

namespace TruckOrder.Api.Api.Controllers;

/// <summary>
/// Endpoints del PUC-001: armar un pedido, modificar sus items, cobrarlo y cancelar.
/// </summary>
[ApiController]
[Route("api/pedidos")]
[Tags("Pedidos (PUC-001)")]
public class PedidosController : ControllerBase
{
    private readonly PedidosService _pedidos;
    private readonly CobrosService _cobros;

    public PedidosController(PedidosService pedidos, CobrosService cobros)
    {
        _pedidos = pedidos;
        _cobros = cobros;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PedidoDto>> Crear([FromBody] CrearPedidoIn body)
    {
        try
        {
            var pedido = await _pedidos.CrearPedidoAsync(body.TruckId);
            return CreatedAtAction(nameof(Obtener), new { pedidoId = pedido.Id }, PedidoDto.From(pedido));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    [HttpGet("{pedidoId:int}")]
    public async Task<ActionResult<PedidoDto>> Obtener(int pedidoId)
    {
        try
        {
            var pedido = await _pedidos.ObtenerAsync(pedidoId);
            return PedidoDto.From(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    [HttpPost("{pedidoId:int}/items")]
    public async Task<ActionResult<PedidoDto>> AgregarItem(int pedidoId, [FromBody] AgregarItemIn body)
    {
        try
        {
            var pedido = await _pedidos.AgregarItemAsync(pedidoId, body.ProductoId, body.Cantidad);
            return PedidoDto.From(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { detail = ex.Message });
        }
    }

    [HttpDelete("{pedidoId:int}/items/{itemId:int}")]
    public async Task<ActionResult<PedidoDto>> QuitarItem(int pedidoId, int itemId)
    {
        try
        {
            var pedido = await _pedidos.QuitarItemAsync(pedidoId, itemId);
            return PedidoDto.From(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
    }

    [HttpPost("{pedidoId:int}/cobrar")]
    public async Task<ActionResult<PagoDto>> Cobrar(int pedidoId, [FromBody] CobrarIn body)
    {
        try
        {
            var datos = new DatosCobro(body.MontoRecibido);
            var pago = await _cobros.CobrarAsync(pedidoId, body.Metodo, datos);
            return PagoDto.From(pago);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { detail = ex.Message });
        }
    }

    [HttpPost("{pedidoId:int}/cancelar")]
    public async Task<ActionResult<PedidoDto>> Cancelar(int pedidoId)
    {
        try
        {
            var pedido = await _pedidos.CancelarAsync(pedidoId);
            return PedidoDto.From(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
    }

    /// <summary>Cambia el estado del pedido. Lo usa la cocina y el cajero al entregar.</summary>
    [HttpPatch("{pedidoId:int}/estado")]
    public async Task<ActionResult<PedidoDto>> CambiarEstado(int pedidoId, [FromBody] CambiarEstadoIn body)
    {
        try
        {
            var pedido = await _pedidos.CambiarEstadoAsync(pedidoId, body.Estado);
            return PedidoDto.From(pedido);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
    }
}
