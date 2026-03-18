using PuntoDeVenta.Application.Core;

namespace PuntoDeVenta.Application.Queries.Proveedores.GetProveedores;

public class GetProveedoresRequest : PagingParams
{
    public string? RazonSocial { get; set; }
    public string? RFC { get; set; }
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public DateTime? FechaDeRegistro { get; set; }
}
