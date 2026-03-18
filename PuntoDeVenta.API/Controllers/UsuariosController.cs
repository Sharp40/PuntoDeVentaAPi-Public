using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoDeVenta.Application.Commands.Usuarios;
using PuntoDeVenta.Application.Commands.Usuarios.Create;
using PuntoDeVenta.Application.Commands.Usuarios.Login;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Application.Queries.Usuarios;
using PuntoDeVenta.Application.Queries.Usuarios.GetCurrentUser;
using PuntoDeVenta.Application.Queries.Usuarios.GetUsuarios;
using PuntoDeVenta.Domain.Constantes;
using PuntoDeVenta.Domain.Interfaces;
using static PuntoDeVenta.Application.Commands.Usuarios.Login.LoginCommand;
using static PuntoDeVenta.Application.Queries.Usuarios.GetCurrentUser.GetCurrentUserQuery;
using static PuntoDeVenta.Application.Queries.Usuarios.GetUsuarios.GetUsuariosQuery;

namespace PuntoDeVenta.API.Controllers;

[Route("api/usuarios")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserAccessor _userAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuariosController(ISender sender, IUserAccessor userAccessor, IHttpContextAccessor httpContextAccessor)
    {
        _sender = sender;
        _userAccessor = userAccessor;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [Authorize(Roles = CustomRoles.ADMINISTRADOR)]
    public async Task<ActionResult<PagedList<UsuarioResponse>>> Paginar(
        [FromQuery] GetUsuariosRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetUsuariosQueryRequest { UsuariosRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }


    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<LoginResponse>>> Crear([FromBody] UsuarioCreateRequest request,
                                                        CancellationToken cancellationToken)
    {
        var command = new UsuarioCreateCommand.UsuarioCreateCommandRequest(request);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Unauthorized();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<LoginResponse>>> Login([FromBody] LoginRequest request,
                                                                CancellationToken cancellationToken)
    {
        var command = new LoginCommandRequest(request);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Unauthorized();
    }

    [HttpGet("yo")]
    [Authorize]
    public async Task<ActionResult<Profile>> CurrentUser(CancellationToken cancellationToken)
    {
        Guid userId = Guid.Parse(_userAccessor.GetUserId());
        
        var request = new GetCurrentUserRequest { UserId = userId };
        var query = new GetCurrentUserQueryRequest(request);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : Unauthorized();
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var isHttps = _httpContextAccessor.HttpContext?.Request.IsHttps ?? false;

        Response.Cookies.Delete("Secure_Fingerprint", new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = SameSiteMode.None,
            Path = "/",
        });

        return Ok(new { message = "Sesión cerrada exitosamente" });
    }
}
