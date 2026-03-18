namespace PuntoDeVenta.Application.Queries.Ventas.GetTicket;

public class ProductoTicketRequest
{
    public string Producto { get; set; } = string.Empty;
    public string? Presentacion { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Importe { get; set; }
}
