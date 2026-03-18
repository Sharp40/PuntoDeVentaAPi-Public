using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Queries.Usuarios.GetUsuarios;

public class GetUsuariosQuery
{
    public record GetUsuariosQueryRequest : IRequest<Result<PagedList<UsuarioResponse>>>
    {
        public GetUsuariosRequest? UsuariosRequest { get; set; }
    }

    internal class GetUsuariosQueryHandler : IRequestHandler<GetUsuariosQueryRequest, Result<PagedList<UsuarioResponse>>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly IMapper _mapper;

        public GetUsuariosQueryHandler(PuntoDeVentaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<UsuarioResponse>>> Handle(GetUsuariosQueryRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Usuario> queryable = _context.Usuarios;

            var predicate = ExpressionBuilder.New<Usuario>();

            if (!string.IsNullOrEmpty(request.UsuariosRequest!.Usuario))
            {
                predicate = predicate
                    .And(u => u.NombreDeUsuario!.ToLower().Contains(request.UsuariosRequest.Usuario.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.UsuariosRequest!.Rol))
            {
                predicate = predicate
                    .And(u => u.Rol!.ToLower().Contains(request.UsuariosRequest.Rol));
            }

            if (!string.IsNullOrEmpty(request.UsuariosRequest.OrderBy))
            {
                Expression<Func<Usuario, object>>? orderBySelector =
                    request.UsuariosRequest.OrderBy.ToLower() switch
                    {
                        "usuario" => u => u.NombreDeUsuario!,
                        "rol" => u => u.Rol!,
                        _ => u => u.NombreDeUsuario!
                    };

                bool orderBy = request.UsuariosRequest.OrderAscending.HasValue
                    ? request.UsuariosRequest.OrderAscending.Value
                    : true;

                queryable = orderBy
                    ? queryable.OrderBy(orderBySelector)
                    : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable.Where(predicate);

            var usuariosQuery = queryable
                .ProjectTo<UsuarioResponse>(_mapper.ConfigurationProvider)
                .AsQueryable();

            var pagination = await PagedList<UsuarioResponse>.CreateAsync(
                 usuariosQuery,
                 request.UsuariosRequest.PageNumber,
                 request.UsuariosRequest.PageSize
            );


            return Result<PagedList<UsuarioResponse>>.Success(pagination);
        }
    }
}
