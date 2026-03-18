using PuntoDeVenta.Domain.Entidades.Otro;

namespace PuntoDeVenta.Domain.Interfaces;

public interface IReportService <T> where T : class
{
    Task<MemoryStream> GetCsvReport(List<T> records);
    Task<MemoryStream> GetPdfReport(List<T> records);
    Task<MemoryStream> GetPdfReportProductosBajosEnStock(List<T> records);

}

public interface IGenerarTicketService
{
    Task<MemoryStream> GenerarTicket(List<TicketDetalle> detalles, TicketInfo info);

}
