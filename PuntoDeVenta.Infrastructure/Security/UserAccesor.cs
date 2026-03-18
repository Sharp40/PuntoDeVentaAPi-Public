using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PuntoDeVenta.Domain.Interfaces;

namespace PuntoDeVenta.Infrastructure.Security;

public class UserAccesor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccesor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId()
    {
        return _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    public string GetUserName()
    {
        return _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name)!;
    }

}
