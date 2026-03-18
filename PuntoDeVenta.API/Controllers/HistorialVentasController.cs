using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoDeVenta.Domain.Constantes;
using PuntoDeVenta.Infrastructure.Reports;

namespace PuntoDeVenta.API.Controllers;

[Route("api/historial-ventas")]
[ApiController]
[Authorize(Roles = CustomRoles.ADMINISTRADOR)]
public class HistorialVentasController : ControllerBase
{
    private readonly HistorialDeVentas _reportService;

    public HistorialVentasController(IConfiguration config)
    {
        _reportService = new HistorialDeVentas(config.GetConnectionString("ConexionDb")!);
    }

    // 📄 Reporte Diario (hoy)
    [HttpGet("ReporteDiario")]
    public IActionResult GetReporteDiario()
    {
        var fecha = DateTime.Today;
        var pdfBytes = _reportService.GenerarReporteDiario(fecha);
        return File(pdfBytes, "application/pdf", $"Reporte_Diario_{fecha:yyyyMMdd}.pdf");
    }

    // 📄 Reporte Semanal (semana actual)
    [HttpGet("ReporteSemanal")]
    public IActionResult GetReporteSemanal()
    {
        var fecha = DateTime.Today;
        var pdfBytes = _reportService.GenerarReporteSemanal(fecha);
        return File(pdfBytes, "application/pdf", $"Reporte_Semanal_{fecha:yyyyMMdd}.pdf");
    }

    // 📄 Reporte Mensual (mes actual)
    [HttpGet("ReporteMensual")]
    public IActionResult GetReporteMensual()
    {
        var fecha = DateTime.Today;
        var pdfBytes = _reportService.GenerarReporteMensual(fecha.Year, fecha.Month);
        return File(pdfBytes, "application/pdf", $"Reporte_Mensual_{fecha:yyyyMM}.pdf");
    }

    // 📄 Reporte Anual (año actual)
    [HttpGet("ReporteAnual")]
    public IActionResult GetReporteAnual()
    {
        var anio = DateTime.Today.Year;
        var pdfBytes = _reportService.GenerarReporteAnual(anio);
        return File(pdfBytes, "application/pdf", $"Reporte_Anual_{anio}.pdf");
    }

    // 📄 Reporte Stock
    [HttpGet("ReporteStock")]
    public IActionResult GetReporteStock()
    {
        var pdfBytes = _reportService.GenerarReporteStock();
        return File(pdfBytes, "application/pdf", $"Reporte_Stock_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("ReporteProductosMasVendidos")]
    public IActionResult GetReporteProductosMasVendidos()
    {
        var pdfBytes = _reportService.GenerarReporteProductosMasVendidos();
        return File(pdfBytes, "application/pdf", $"Reporte_ProductosMasVendidos_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("ReporteProductosMenosVendidos")]
    public IActionResult GetReporteProductosMenosVendidos()
    {
        var pdfBytes = _reportService.GenerarReporteProductosMenosVendidos();
        return File(pdfBytes, "application/pdf", $"Reporte_ProductosMenosVendidos_{DateTime.Now:yyyyMMdd}.pdf");
    }


    // LOS SIGIENTES ENPOINTS SERAN PARA ENVIARLE LOS DATOS AL FORNT PARA QUE CREE LAS GRAFICAS

    [HttpGet("historial-datos")]
    public IActionResult GetHistorialDatos()
    {
        var datos = _reportService.ObtenerHistorialDatos();
        return Ok(datos);
    }

    [HttpGet("mas-vendido-datos")]
    public IActionResult GetMasVendidoDatos()
    {
        var datos = _reportService.ObtenerMasVendidoDatos();
        return Ok(datos);
    }

    [HttpGet("menos-vendido-datos")]
    public IActionResult GetMenosVendidoDatos()
    {
        var datos = _reportService.ObtenerMenosVendidoDatos();
        return Ok(datos);
    }

}
