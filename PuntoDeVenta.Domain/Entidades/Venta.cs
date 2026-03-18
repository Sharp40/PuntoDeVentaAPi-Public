using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Domain.Entidades;

public class Venta
{
    [Column("id_venta")]
    public Guid Id { get; set; }
    [Column("fecha")]
    public DateTime? Fecha { get; set; }
    [Column("total")]
    public decimal Total { get; set; }


    [Column("id_usuario")]
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }


    [Column("id_cliente")]
    public Guid ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
}
