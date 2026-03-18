using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoDeVenta.Application.Commands.Proveedores.Create;
using PuntoDeVenta.Application.Commands.Proveedores.Update;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Application.Queries.Proveedores;
using PuntoDeVenta.Application.Queries.Proveedores.GetProveedores;
using PuntoDeVenta.Domain.Constantes;
using static PuntoDeVenta.Application.Queries.Proveedores.GetProveedores.GetProveedoresQuery;
using static PuntoDeVenta.Application.Queries.Proveedores.GetProveedores.GetProveedorQuery;

namespace PuntoDeVenta.API.Controllers;

[Route("api/proveedores")]
[ApiController]
[Authorize(Roles = CustomRoles.ADMINISTRADOR)]
public class ProveedoresController : ControllerBase
{
    private readonly ISender _sender;

    public ProveedoresController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<Result<PagedList<ProveedorResponse>>>> Paginar(
        [FromQuery] GetProveedoresRequest request, CancellationToken cancellationToken)
    {
        var query = new GetProveedoresQueryRequest { GetProveedoresRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<Result<List<ProveedorResponse>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var query = new GetAllProveedoresQuery.GetAllProveedoresQueryRequest();
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProveedorResponse>> Get(
        Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProveedorQueryRequest { Id = id };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }


    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] ProveedorCreateRequest request, CancellationToken cancellationToken)
    {
        var command = new ProveedorCreateCommand.ProveedorCreateCommmandRequest(request);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest();
    }

    [HttpPut]
    public async Task<ActionResult<Guid>> Update(
        Guid id,
        [FromBody] ProveedorUpdateRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new ProveedorUpdateCommand.ProveedorUpdateCommandRequest(id, request);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest();
    }
}
