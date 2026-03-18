namespace PuntoDeVenta.Domain.Entidades.Otro;

public class TicketDetalle
{
    public string Producto { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Importe { get; set; }
    public string? Presentacion { get; set; }
}
