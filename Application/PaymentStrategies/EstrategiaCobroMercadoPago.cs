using TruckOrder.Api.Application.PasarelaPago;
using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Enums;

namespace TruckOrder.Api.Application.PaymentStrategies;

public class EstrategiaCobroMercadoPago : IEstrategiaCobro
{
    public Pago Procesar(Pedido pedido, DatosCobro datos, IPasarelaPago pasarela)
    {
        var qr = pasarela.GenerarQrMercadoPago(pedido.Total);

        // Para esta entrega el mock confirma sincrónicamente.
        // En producción habría un webhook desde MP que llega después.
        var resultado = pasarela.ConsultarEstadoQr(qr.QrData);

        return new Pago
        {
            PedidoId = pedido.Id,
            Monto = pedido.Total,
            Metodo = MetodoPago.MercadoPago,
            Estado = resultado.Aprobado ? EstadoPago.Aprobado : EstadoPago.Rechazado,
            ReferenciaExterna = resultado.ReferenciaExterna
        };
    }
}
