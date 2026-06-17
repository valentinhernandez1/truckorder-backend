using TruckOrder.Api.Application.PasarelaPago;
using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Enums;

namespace TruckOrder.Api.Application.PaymentStrategies;

public class EstrategiaCobroTarjeta : IEstrategiaCobro
{
    public Pago Procesar(Pedido pedido, DatosCobro datos, IPasarelaPago pasarela)
    {
        var resultado = pasarela.ProcesarTarjeta(pedido.Total);

        return new Pago
        {
            PedidoId = pedido.Id,
            Monto = pedido.Total,
            Metodo = MetodoPago.Tarjeta,
            Estado = resultado.Aprobado ? EstadoPago.Aprobado : EstadoPago.Rechazado,
            ReferenciaExterna = resultado.ReferenciaExterna
        };
    }
}
