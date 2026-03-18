using Newtonsoft.Json;
using PuntoDeVenta.Application.Core;

namespace PuntoDeVenta.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";

            if (ex is ValidationException validationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errorResponse = new
                {
                    title = "Error de validación",
                    status = 400,
                    errors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        )
                };

                var json = JsonConvert.SerializeObject(errorResponse);
                await context.Response.WriteAsync(json);
            }
            else if (ex is System.Text.Json.JsonException jsonEx)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errorResponse = new
                {
                    title = "Error de deserialización JSON",
                    status = 400,
                    message = "La estructura del JSON enviado es inválida. Asegúrate de que todos los campos tengan el tipo de dato correcto.",
                    detalle = _environment.IsDevelopment() ? jsonEx.Message : null
                };

                var json = JsonConvert.SerializeObject(errorResponse);
                await context.Response.WriteAsync(json);
            }
            else if (ex is OverflowException overflowEx)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errorResponse = new
                {
                    title = "Error de valor numérico",
                    status = 400,
                    message = "Uno o más valores numéricos enviados son demasiado grandes o pequeños para el tipo esperado.",
                    detalle = _environment.IsDevelopment() ? overflowEx.Message : null
                };

                var json = JsonConvert.SerializeObject(errorResponse);
                await context.Response.WriteAsync(json);
            }
            else
            {
                var response = new AppException(
                    StatusCodes.Status500InternalServerError,
                    ex.Message,
                    _environment.IsDevelopment() ? ex.StackTrace : null
                );

                context.Response.StatusCode = response.StatusCode;
                context.Response.ContentType = "application/json";

                var json = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
