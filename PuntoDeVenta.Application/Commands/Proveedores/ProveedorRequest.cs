namespace PuntoDeVenta.Application.Commands.Proveedores;

public abstract class ProveedorRequest
{
    public string? RazonSocial { get; set; }
    public string? RFC { get; set; }
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    //public DateTime? FechaDeRegistro { get; set; } = DateTime.Now; // Verificar con el team si obtener por defecto
}
