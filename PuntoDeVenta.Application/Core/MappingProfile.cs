using AutoMapper;
using PuntoDeVenta.Application.Queries.Productos;
using PuntoDeVenta.Application.Queries.Productos.GeReport;
using PuntoDeVenta.Application.Queries.Productos.GetProductosBajosEnStock;
using PuntoDeVenta.Application.Queries.Proveedores;
using PuntoDeVenta.Application.Queries.Usuarios;
using PuntoDeVenta.Domain.Entidades;

namespace PuntoDeVenta.Application.Core;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Usuario, UsuarioResponse>()
            .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.NombreDeUsuario));

        CreateMap<Producto, ProductoResponse>()
            .ForMember(dest => dest.Proveedor, 
                        opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Nombre : null));

        CreateMap<Proveedor, ProveedorResponse>();

        CreateMap<ProductosBajosEnStock, ProductosBajosEnStockResponse>();
        CreateMap<ProductosBajosEnStock, ProductoReportPdfResponse>();
    }
}
