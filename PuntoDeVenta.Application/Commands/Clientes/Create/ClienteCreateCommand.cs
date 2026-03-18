using FluentValidation;
using MediatR;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.Clientes.Create;

public class ClienteCreateCommand
{
    public record ClienteCreateCommandRequest(ClienteCreateRequest ClienteCreateRequest)
        : IRequest<Result<Guid>>;

    public record ClienteCreateCommnandHandler
        : IRequestHandler<ClienteCreateCommandRequest, Result<Guid>>
    {
        private readonly PuntoDeVentaDbContext _context;

        public ClienteCreateCommnandHandler(PuntoDeVentaDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(ClienteCreateCommandRequest request, CancellationToken cancellationToken)
        {
            var clienteId = Guid.NewGuid();
            var cliente = new Domain.Entidades.Cliente
            {
                Id = clienteId,
                Nombre = request.ClienteCreateRequest.Nombre,
                Telefono = request.ClienteCreateRequest.Telefono,
                Email = request.ClienteCreateRequest.Email,
                Direccion = request.ClienteCreateRequest.Direccion
            };

            _context.Clientes.Add(cliente);
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Guid>.Success(clienteId)
                : Result<Guid>.Failure("Error al crear el cliente.");
        }
    }

    public class ClienteCreateCommandRequestValidator : AbstractValidator<ClienteCreateCommandRequest>
    {
        public ClienteCreateCommandRequestValidator()
        {
            RuleFor(c => c.ClienteCreateRequest).SetValidator(new ClienteCreateValidator());
        }
    }
}
