using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Domain.Interfaces;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.Productos.Delete;

public class DesactivarProductoCommand
{
    public record DesactivarProductoCommandRequest(Guid Id, string? Razon) : IRequest<Result<int>>, ICommandBase;

    internal class DesactivarProductoCommandHandler
        : IRequestHandler<DesactivarProductoCommandRequest, Result<int>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public DesactivarProductoCommandHandler(PuntoDeVentaDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<Result<int>> Handle(DesactivarProductoCommandRequest request, CancellationToken cancellationToken)
        {
            var usuarioId = Guid.Parse(_userAccessor.GetUserId());

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId, cancellationToken);

            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == request.Id);

            if (producto is null)
                return Result<int>.Failure("El producto que deseas desactivar no existe.");

            var idParam = new SqlParameter("@Id", request.Id);
            var outputParam = new SqlParameter
            {
                ParameterName = "@FilasAfectadas",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output,
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC SP_DesactivarProducto @Id, @FilasAfectadas OUTPUT", idParam, outputParam
            );

            var filasAfectadas = (int)(outputParam.Value ?? 0);

            if(filasAfectadas > 0)
            {
                var productoElminado = new ProductoEliminado
                {
                    Id = Guid.NewGuid(),
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre ?? "SIN NOMBRE",
                    PrecioUnitario = producto.PrecioUnitario,
                    PrecioVenta = producto.PrecioVenta,
                    Stock = 0,
                    CodigoBarras = producto.CodigoBarras ?? "N/A",
                    ProveedorId = producto.ProveedorId,
                    FechaEliminacion = DateTime.UtcNow,
                    IdUsuarioElimino = usuario?.Id ?? Guid.Empty,
                    MotivoEliminacion = request.Razon ?? "No especificado",
                    Descripcion = "Desactivación de producto",
                    Fecha = DateTime.UtcNow
                };



                _context.Productos_Eliminados.Add(productoElminado);
                await _context.SaveChangesAsync(cancellationToken);

            }

            return filasAfectadas > 0
                ? Result<int>.Success(filasAfectadas)
                : Result<int>.Failure("No se desactivó ningún producto. Verifique el estado actual.");

        }
    }

    public class DesactivarProductoCommandRequestValidator : AbstractValidator<DesactivarProductoCommandRequest>
    {
        public DesactivarProductoCommandRequestValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("El id del producto no debe estar vacio.")
                .NotNull().WithMessage("El id del producto no debe ser nulo.");
        }
    }
}
