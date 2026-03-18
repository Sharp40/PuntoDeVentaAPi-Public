namespace PuntoDeVenta.Application.Queries.Ventas.GetTicket;

public class GenerarTicketRequest
{
    public DateTime Fecha { get; set; }
    public string? Cajero { get; set; }
    public string? Cliente { get; set; }
    public string? Telefono { get; set; }
    public Guid IdVenta { get; set; }
    public string? MetodoPago { get; set; }
    public decimal MontoRecibido { get; set; }
    public List<ProductoTicketRequest> Productos { get; set; } = new();
}
