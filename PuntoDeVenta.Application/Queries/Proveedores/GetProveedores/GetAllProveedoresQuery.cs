using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Queries.Proveedores.GetProveedores;

public class GetAllProveedoresQuery
{
    public record GetAllProveedoresQueryRequest : IRequest<Result<List<ProveedorResponse>>>
    {
        public GetProveedoresRequest? GetProveedoresRequest { get; set; }
    }

    internal class GetAllProveedoresQueryHandler : IRequestHandler<GetAllProveedoresQueryRequest, Result<List<ProveedorResponse>>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly IMapper _mapper;

        public GetAllProveedoresQueryHandler(PuntoDeVentaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<ProveedorResponse>>> Handle(GetAllProveedoresQueryRequest request, CancellationToken cancellationToken)
        {
           var proveedores = await _context.Proveedores
                .ProjectTo<ProveedorResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            return Result<List<ProveedorResponse>>.Success(proveedores);
        }
    }
}
