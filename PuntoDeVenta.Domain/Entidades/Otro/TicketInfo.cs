namespace PuntoDeVenta.Domain.Entidades.Otro;

public class TicketInfo
{
    public DateTime Fecha { get; set; }
    public string Cajero { get; set; } = "";
    public string Cliente { get; set; } = "";
    public string Telefono { get; set; } = "";
    public Guid IdVenta { get; set; }
    public string MetodoPago { get; set; } = "";
    public decimal MontoRecibido { get; set; }
}
