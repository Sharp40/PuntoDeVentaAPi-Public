using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PuntoDeVenta.Application.Queries.Usuarios.GetCurrentUser;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Domain.Interfaces;
using static PuntoDeVenta.Application.Queries.Usuarios.GetCurrentUser.GetCurrentUserQuery;

public class CrearVentaCommand
{
    private readonly IConfiguration _config;
    private readonly IUserAccessor _userAccessor;

    public CrearVentaCommand(IConfiguration config, IUserAccessor userAccessor)
    {
        _config = config;
        _userAccessor = userAccessor;
    }

    public async Task<VentaResponse> EjecutarAsync(
        Cliente cliente,
        string metodoPago,
        decimal totalVenta,
        decimal montoRecibido,
        decimal cambio,
        List<DetalleVentaInput> detalles)
    {
        var connStr = _config.GetConnectionString("ConexionDb");

        using var connection = new SqlConnection(connStr);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Obtener el usuario actual
            Guid userId = Guid.Parse(_userAccessor.GetUserId());

            var usuarioCmd = new SqlCommand(@"
                SELECT TOP 1 id_usuario, nombre, Apellido_Paterno, Apellido_Materno, usuario
                FROM Usuarios 
                WHERE id_usuario = @id_usuario", connection, transaction);
            usuarioCmd.Parameters.AddWithValue("@id_usuario", userId);

            UsuarioDTO usuario;
            string username;

            using (var reader = await usuarioCmd.ExecuteReaderAsync())
            {
                if (!await reader.ReadAsync())
                    throw new Exception("Usuario actual no encontrado");

                usuario = new UsuarioDTO
                {
                    Id = reader.IsDBNull(0) ? Guid.Empty : reader.GetGuid(0),
                    Nombre = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    ApellidoPaterno = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    ApellidoMaterno = reader.IsDBNull(3) ? "" : reader.GetString(3),
                };

                username = reader.IsDBNull(4) ? "" : reader.GetString(4);

               
            }

            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("No se pudo obtener el nombre de usuario.");

            // Si cliente tiene Id vacío, asignar nuevo Guid
            if (cliente.Id == Guid.Empty)
                cliente.Id = Guid.NewGuid();

            var clienteId = cliente.Id;

            // Verificar si cliente existe
            var checkClienteCmd = new SqlCommand("SELECT COUNT(*) FROM Clientes WHERE id_cliente = @Id", connection, transaction);
            checkClienteCmd.Parameters.AddWithValue("@Id", clienteId);
            var exists = (int )await checkClienteCmd.ExecuteScalarAsync() > 0;

            if (!exists)
            {
                // Insertar nuevo cliente
                var insertClienteCmd = new SqlCommand(
                    "INSERT INTO Clientes (id_cliente, nombre, telefono, email, direccion) VALUES (@Id, @Nombre, @Telefono, @Email, @Direccion)",
                    connection, transaction);
                insertClienteCmd.Parameters.AddWithValue("@Id", clienteId);
                insertClienteCmd.Parameters.AddWithValue("@Nombre", cliente.Nombre ?? "");
                insertClienteCmd.Parameters.AddWithValue("@Telefono", cliente.Telefono ?? "");
                insertClienteCmd.Parameters.AddWithValue("@Email", cliente.Email ?? "");
                insertClienteCmd.Parameters.AddWithValue("@Direccion", cliente.Direccion ?? "");
                await insertClienteCmd.ExecuteNonQueryAsync();
            }

            // Crear venta
            var ventaId = Guid.NewGuid();
            var insertVentaCmd = new SqlCommand(
                "INSERT INTO Ventas (id_venta, fecha, total, id_usuario, id_cliente) VALUES (@Id, @Fecha, @Total, @UsuarioId, @ClienteId)",
                connection, transaction);
            insertVentaCmd.Parameters.AddWithValue("@Id", ventaId);
            insertVentaCmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
            insertVentaCmd.Parameters.AddWithValue("@Total", totalVenta);
            insertVentaCmd.Parameters.AddWithValue("@UsuarioId", usuario.Id);
            insertVentaCmd.Parameters.AddWithValue("@ClienteId", clienteId);
            await insertVentaCmd.ExecuteNonQueryAsync();

            var detallesResponse = new List<DetalleVentaResponse>();

            // Insertar detalles y actualizar stock
            foreach (var d in detalles)
            {
                var getProductoCmd = new SqlCommand(
                    "SELECT nombre, stock FROM Productos WHERE id_producto = @Id",
                    connection, transaction);
                getProductoCmd.Parameters.AddWithValue("@Id", d.ProductoId);

                string nombreProducto;
                int stockActual;

                using (var reader = await getProductoCmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        throw new Exception($"Producto {d.ProductoId} no encontrado");

                    nombreProducto = reader.IsDBNull(0) ? "Nombre no disponible" : reader.GetString(0);
                    stockActual = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                }

                if (stockActual < d.Cantidad)
                    throw new Exception($"Stock insuficiente para el producto {nombreProducto}");

                var detalleId = Guid.NewGuid();
                var insertDetalleCmd = new SqlCommand(
                    "INSERT INTO Detalle_Ventas (id_detalle_venta, id_venta, id_producto, cantidad, precio_unitario) VALUES (@Id, @VentaId, @ProductoId, @Cantidad, @PrecioUnitario)",
                    connection, transaction);
                insertDetalleCmd.Parameters.AddWithValue("@Id", detalleId);
                insertDetalleCmd.Parameters.AddWithValue("@VentaId", ventaId);
                insertDetalleCmd.Parameters.AddWithValue("@ProductoId", d.ProductoId);
                insertDetalleCmd.Parameters.AddWithValue("@Cantidad", d.Cantidad);
                insertDetalleCmd.Parameters.AddWithValue("@PrecioUnitario", d.PrecioUnitario);
                await insertDetalleCmd.ExecuteNonQueryAsync();

                // Actualizar stock
                var updateStockCmd = new SqlCommand(
                    "UPDATE Productos SET stock = stock - @Cantidad WHERE id_producto = @id_producto",
                    connection, transaction);
                updateStockCmd.Parameters.AddWithValue("@Cantidad", d.Cantidad);
                updateStockCmd.Parameters.AddWithValue("@id_producto", d.ProductoId);
                await updateStockCmd.ExecuteNonQueryAsync();

                detallesResponse.Add(new DetalleVentaResponse
                {
                    ProductoNombre = nombreProducto,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Importe = d.Cantidad * d.PrecioUnitario
                });
            }

            // Registrar movimiento en caja
            var regCajaId = Guid.NewGuid();
            var insertCajaCmd = new SqlCommand(
                "INSERT INTO RegistroCaja (id_registroCaja, FechaHora, MetodoPago, MontoPagado, CambioEntregado, TotalVenta, Usuario, IdVenta) " +
                "VALUES (@Id, @FechaHora, @MetodoPago, @MontoPagado, @Cambio, @TotalVenta, @Usuario, @IdVenta)",
                connection, transaction);
            insertCajaCmd.Parameters.AddWithValue("@Id", regCajaId);
            insertCajaCmd.Parameters.AddWithValue("@FechaHora", DateTime.Now);
            insertCajaCmd.Parameters.AddWithValue("@MetodoPago", metodoPago);
            insertCajaCmd.Parameters.AddWithValue("@MontoPagado", montoRecibido);
            insertCajaCmd.Parameters.AddWithValue("@Cambio", cambio);
            insertCajaCmd.Parameters.AddWithValue("@TotalVenta", totalVenta);
            insertCajaCmd.Parameters.AddWithValue("@Usuario", username);
            insertCajaCmd.Parameters.AddWithValue("@IdVenta", ventaId);
            await insertCajaCmd.ExecuteNonQueryAsync();

            // Confirmar transacción
            await transaction.CommitAsync();

            return new VentaResponse
            {
                IdVenta = ventaId,
                Fecha = DateTime.Now,
                UsuarioNombreCompleto = $"{usuario.Nombre} {usuario.ApellidoPaterno} {usuario.ApellidoMaterno}".Trim(),
                ClienteNombre = cliente.Nombre ?? "Cliente General",
                ClienteTelefono = cliente.Telefono ?? "",
                MetodoPago = metodoPago,
                TotalVenta = totalVenta,
                MontoRecibido = montoRecibido,
                Cambio = cambio,
                Detalles = detallesResponse
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error al crear la venta: " + ex.Message, ex);
        }
    }
}

// DTO para mapear usuario
public class UsuarioDTO
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string ApellidoPaterno { get; set; } = "";
    public string ApellidoMaterno { get; set; } = "";
}

// Modelos para entrada y salida

public class DetalleVentaInput
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class VentaResponse
{
    public Guid IdVenta { get; set; }
    public DateTime Fecha { get; set; }
    public string UsuarioNombreCompleto { get; set; } = "";
    public string ClienteNombre { get; set; } = "";
    public string ClienteTelefono { get; set; } = "";
    public string MetodoPago { get; set; } = "";
    public decimal TotalVenta { get; set; }
    public decimal MontoRecibido { get; set; }
    public decimal Cambio { get; set; }
    public List<DetalleVentaResponse> Detalles { get; set; } = new();
}

public class DetalleVentaResponse
{
    public string ProductoNombre { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Importe { get; set; }
}

public class VentaRequestDto
{
    public Cliente Cliente { get; set; } = new();
    public string MetodoPago { get; set; } = "";
    public decimal TotalVenta { get; set; }
    public decimal MontoRecibido { get; set; }
    public decimal Cambio { get; set; }
    public List<DetalleVentaInput> Detalles { get; set; } = new();
}
