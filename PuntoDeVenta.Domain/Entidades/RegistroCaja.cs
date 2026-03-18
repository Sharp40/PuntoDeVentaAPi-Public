using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Domain.Entidades;

public class RegistroCaja
{
    [Column("id_registroCaja")]
    public Guid Id { get; set; }
    public DateTime FechaHora { get; set; }
    public string? MetodoPago { get; set; }
    public decimal MontoPagado { get; set; }
    public decimal CambioEntregado { get; set; }
    public decimal TotalVenta { get; set; }
    public string? Usuario { get; set; }

    public Guid IdVenta { get; set; }
    public Venta? Venta { get; set; }
}
