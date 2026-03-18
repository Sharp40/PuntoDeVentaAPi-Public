
namespace PuntoDeVenta.Application.Commands.RegistroCajas.Create;

public class RegistroCajaCreateRequest
{
    public DateTime FechaHora { get; set; }
    public string? MetodoPago { get; set; }
    public decimal MontoPagado { get; set; }
    public decimal CambioEntregado { get; set; }
    public decimal TotalVenta { get; set; }
    public string? Usuario { get; set; } // Obtener el nombre del usuario logeado
    public Guid IdVenta { get; set; }
}
