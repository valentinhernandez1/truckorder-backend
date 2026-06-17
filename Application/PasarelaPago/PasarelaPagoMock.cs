namespace TruckOrder.Api.Application.PasarelaPago;

/// <summary>
/// Mock para desarrollo y demo. Aprueba el 90% de los pagos con tarjeta.
/// </summary>
public class PasarelaPagoMock : IPasarelaPago
{
    private readonly Random _rng = new();

    public DatosQr GenerarQrMercadoPago(decimal monto)
    {
        var refExt = $"MP-{Guid.NewGuid().ToString("N")[..10]}";
        var qr = $"mp://pagar?ref={refExt}&monto={monto:F2}";
        return new DatosQr(qr, ExpiraEnSegundos: 120);
    }

    public ResultadoPago ProcesarTarjeta(decimal monto)
    {
        if (_rng.NextDouble() < 0.9)
            return new ResultadoPago(true, $"TJ-{Guid.NewGuid().ToString("N")[..10]}");

        return new ResultadoPago(false, string.Empty, "Fondos insuficientes");
    }

    public ResultadoPago ConsultarEstadoQr(string referencia)
    {
        // Para el mock asumimos que el pago se aprueba tras generar el QR.
        return new ResultadoPago(true, referencia);
    }
}
