using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Queries.Proveedores.GetProveedores;

public class GetProveedorQuery
{
    public record GetProveedorQueryRequest : IRequest<Result<ProveedorResponse>>
    {
        public Guid Id { get; set; }
    }

    internal class GetProveedorQueryHandler : IRequestHandler<GetProveedorQueryRequest, Result<ProveedorResponse>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly IMapper _mapper;

        public GetProveedorQueryHandler(PuntoDeVentaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<ProveedorResponse>> Handle(GetProveedorQueryRequest request, CancellationToken cancellationToken)
        {
            var proveedor = await _context.Proveedores
                .Where(p => p.Id == request.Id)
                .ProjectTo<ProveedorResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (proveedor is null) return Result<ProveedorResponse>.Failure("No se encontro el proveedor con ese id.");

            return Result<ProveedorResponse>.Success(proveedor);
        }
    }
}
