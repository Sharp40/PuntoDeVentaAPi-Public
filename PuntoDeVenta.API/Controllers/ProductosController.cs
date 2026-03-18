using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Commands.Productos.Create;
using PuntoDeVenta.Application.Commands.Productos.Update;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Application.Queries.Productos;
using PuntoDeVenta.Application.Queries.Productos.GeReport;
using PuntoDeVenta.Application.Queries.Productos.GetProductos;
using PuntoDeVenta.Application.Queries.Productos.GetProductosBajosEnStock;
using PuntoDeVenta.Domain.Constantes;

//using PuntoDeVenta.Domain.Constantes;
using PuntoDeVenta.Infrastructure.Persistence;
using static PuntoDeVenta.Application.Commands.Productos.Delete.DesactivarProductoCommand;
using static PuntoDeVenta.Application.Queries.Productos.GetProductos.GetAllProductoQuery;
using static PuntoDeVenta.Application.Queries.Productos.GetProductos.GetProductoQuery;
using static PuntoDeVenta.Application.Queries.Productos.GetProductos.GetProductosQuery;
using static PuntoDeVenta.Application.Queries.Productos.GetProductosBajosEnStock.GetProductosBajosEnStockQuery;

namespace PuntoDeVenta.API.Controllers;

[Route("api/productos")]
[ApiController]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly ISender _sender;
    private readonly PuntoDeVentaDbContext _context;
    public ProductosController(ISender sender, 
                               PuntoDeVentaDbContext context)
    {
        _sender = sender;
        _context = context;
    }


    [HttpGet]
    //[OutputCache(Tags = [CustomTag.CACHE_PRODUCTOS])]
    public async Task<ActionResult<Result<PagedList<ProductoResponse>>>> Paginar(
        [FromQuery] GetProductosRequest request, CancellationToken cancellationToken)
    {
        var query = new GetProductosQueryRequest { GetProductosRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("productos-bajos-en-stock")]
    [Authorize(Roles = CustomRoles.ADMINISTRADOR)]
    //[OutputCache(Tags = [CustomTag.CACHE_PRODUCTOS])]
    public async Task<ActionResult<Result<PagedList<ProductosBajosEnStockResponse>>>> PaginarProductosBajosEnStock(
        [FromQuery] GetProductosBajosEnStockRequest request, CancellationToken cancellationToken)
    {
        var query = new GetProductosBajosEnStockQueryRequest { GetProductosBajosEnStockRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("get-all")]
    //[OutputCache(Tags = [CustomTag.CACHE_PRODUCTOS])]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllProductoQueryRequest();
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("get-all-bajo-stock")]
    [Authorize(Roles = CustomRoles.ADMINISTRADOR)]
    public async Task<IActionResult> GetAllBajoStock(CancellationToken cancellationToken)
    {
        var productos = await _context.VistaProductosBajoStockRecientes
            .AsNoTracking().ToListAsync(cancellationToken);

        return Ok(productos);
    }

    [HttpGet("{id}")]
    //[OutputCache(Tags = [CustomTag.CACHE_PRODUCTOS])]
    public async Task<ActionResult<ProductoResponse>> Get(
        Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductoQueryRequest { Id = id };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = CustomRoles.ADMINISTRADOR)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] ProductoCreateRequest request, CancellationToken cancellationToken)
    {
        var command = new ProductoCreateCommand.ProductoCreateCommandRequest(request);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = CustomRoles.ADMINISTRADOR)]
    public async Task<ActionResult<Guid>> Update(
       Guid id, [FromBody] ProductoUpdateRequest request, CancellationToken cancellationToken)
    {
        var command = new ProductoUpdateCommand.ProductoUpdateCommandRequest(id, request);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest();
    }

    [HttpPost("desactivar/{id}")]
    [Authorize(Roles = CustomRoles.ADMINISTRADOR)]
    public async Task<ActionResult<int>> Deactivate(Guid id, string? razon, CancellationToken cancellationToken)
    {
        var command = new DesactivarProductoCommandRequest(id, razon);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest();
    }

    [HttpGet("reporte-pdf")]
    [Authorize(Roles = CustomRoles.ADMINISTRADOR)]
    public async Task<IActionResult> GetReporteProductosBajosPdf()
    {
        var pdfStream = await _sender.Send(new ProductoReportPdf.ProductoReportPdfQueryRequest());
        pdfStream.Position = 0; // reset stream position

        var currentDate = DateTime.Now.ToString("dd-MM-yyyy");
        var fileName = $"ProductosBajosEnStock_{currentDate}.pdf";

        return File(pdfStream, "application/pdf", fileName);
    }

}

