using PuntoDeVenta.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
var cadenaConexion = builder.Configuration.GetConnectionString("ConexionDb");

builder.Services.AddExtensionesAPI(cadenaConexion!,builder.Configuration);

var app = builder.Build();

app.UseExtensionesAPI();

app.Run();
