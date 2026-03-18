namespace PuntoDeVenta.Application.Queries.Productos.GeReport;

public class ProductoReportPdfResponse
{
    public string? Nombre { get; set; }
    public string? CodigoDeBarras { get; set; }
    public string? Descripcion { get; set; }
    public int Stock { get; set; }
    public DateTime? FechaAlerta { get; set; }
}
