using TruckOrder.Api.Application.PasarelaPago;
using TruckOrder.Api.Application.PaymentStrategies;
using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Enums;
using TruckOrder.Api.Domain.Repositories;

namespace TruckOrder.Api.Application.Services;

/// <summary>
/// Procesa el cobro del pedido eligiendo la <see cref="IEstrategiaCobro"/>
/// que corresponda al método. Si el pago queda aprobado, el pedido pasa a PAGADO.
/// </summary>
public class CobrosService
{
    private readonly IRepoPedidos _pedidos;
    private readonly IRepoPagos _pagos;
    private readonly IPasarelaPago _pasarela;
    private readonly Dictionary<MetodoPago, IEstrategiaCobro> _estrategias;

    public CobrosService(IRepoPedidos pedidos, IRepoPagos pagos, IPasarelaPago pasarela)
    {
        _pedidos = pedidos;
        _pagos = pagos;
        _pasarela = pasarela;

        _estrategias = new Dictionary<MetodoPago, IEstrategiaCobro>
        {
            [MetodoPago.Efectivo] = new EstrategiaCobroEfectivo(),
            [MetodoPago.Tarjeta] = new EstrategiaCobroTarjeta(),
            [MetodoPago.MercadoPago] = new EstrategiaCobroMercadoPago()
        };
    }

    public async Task<Pago> CobrarAsync(int pedidoId, MetodoPago metodo, DatosCobro datos)
    {
        var pedido = await _pedidos.BuscarPorIdAsync(pedidoId)
            ?? throw new KeyNotFoundException($"Pedido {pedidoId} no encontrado.");

        if (pedido.Estado != EstadoPedido.Nuevo)
            throw new InvalidOperationException("Solo se pueden cobrar pedidos en estado Nuevo.");

        if (pedido.Items.Count == 0)
            throw new InvalidOperationException("No se puede cobrar un pedido vacío.");

        if (!_estrategias.TryGetValue(metodo, out var estrategia))
            throw new ArgumentException($"Método de pago no soportado: {metodo}");

        var pago = estrategia.Procesar(pedido, datos, _pasarela);
        pago = await _pagos.GuardarAsync(pago);

        if (pago.Estado == EstadoPago.Aprobado)
        {
            pedido.CambiarEstado(EstadoPedido.Pagado);
            await _pedidos.GuardarAsync(pedido);
        }

        return pago;
    }
}
