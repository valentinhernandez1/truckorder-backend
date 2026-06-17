using TruckOrder.Api.Application.PasarelaPago;
using TruckOrder.Api.Domain.Entities;
using TruckOrder.Api.Domain.Enums;

namespace TruckOrder.Api.Application.PaymentStrategies;

public class EstrategiaCobroEfectivo : IEstrategiaCobro
{
    public Pago Procesar(Pedido pedido, DatosCobro datos, IPasarelaPago pasarela)
    {
        if (datos.MontoRecibido is null)
            throw new ArgumentException("Para cobrar en efectivo hay que informar el monto recibido.");

        var recibido = datos.MontoRecibido.Value;
        if (recibido < pedido.Total)
            throw new ArgumentException(
                $"El monto recibido (${recibido:F2}) es menor al total (${pedido.Total:F2}).");

        var cambio = Math.Round(recibido - pedido.Total, 2);

        return new Pago
        {
            PedidoId = pedido.Id,
            Monto = pedido.Total,
            Metodo = MetodoPago.Efectivo,
            Estado = EstadoPago.Aprobado,
            MontoRecibido = recibido,
            Cambio = cambio
        };
    }
}
