using PuntoDeVenta.Domain.Entidades;

namespace PuntoDeVenta.Application.Commands.Ventas.Create;

public class VentaCreateResponse
{
    public Guid VentaId { get; set; }
    public string? Usuario { get; set; }
    public string? NombreCliente { get; set; }
    public string? MetodoPago { get; set; }
    public string? TelefonoCliente { get; set; }
    public decimal Total { get; set; }
    public List<ProductoVentaResponse> Productos { get; set; } = new();
    public DateTime Fecha { get; set; }

}

public class ProductoVentaResponse
{
    public string Producto { get; set; } = string.Empty;
    public string? Presentacion { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Importe => Cantidad * PrecioUnitario;
}

