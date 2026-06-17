using TruckOrder.Api.Application.PasarelaPago;
using TruckOrder.Api.Domain.Entities;

namespace TruckOrder.Api.Application.PaymentStrategies;

public record DatosCobro(decimal? MontoRecibido = null);

/// <summary>
/// Strategy: cada método de pago implementa su forma de procesar.
/// Agregar uno nuevo (transferencia, criptomonedas) solo requiere una nueva clase,
/// no se toca CobrosService (Open/Closed Principle).
/// </summary>
public interface IEstrategiaCobro
{
    Pago Procesar(Pedido pedido, DatosCobro datos, IPasarelaPago pasarela);
}
