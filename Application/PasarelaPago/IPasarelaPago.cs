namespace TruckOrder.Api.Application.PasarelaPago;

public record DatosQr(string QrData, int ExpiraEnSegundos);

public record ResultadoPago(bool Aprobado, string ReferenciaExterna, string? MotivoRechazo = null);

/// <summary>
/// Abstracción de la pasarela de pago. En producción se implementa con los
/// SDKs reales de Mercado Pago y del procesador de tarjetas.
/// </summary>
public interface IPasarelaPago
{
    DatosQr GenerarQrMercadoPago(decimal monto);
    ResultadoPago ProcesarTarjeta(decimal monto);
    ResultadoPago ConsultarEstadoQr(string referencia);
}
