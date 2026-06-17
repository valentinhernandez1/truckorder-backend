using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TruckOrder.Api.Application.PasarelaPago;
using TruckOrder.Api.Application.Services;
using TruckOrder.Api.Domain.Repositories;
using TruckOrder.Api.Infrastructure;
using TruckOrder.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ---------- Base de datos (EF Core + SQLite) ----------
builder.Services.AddDbContext<TruckOrderDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// ---------- Repositorios (Repository pattern + Dependency Inversion) ----------
// Cada servicio depende de la INTERFAZ, no de SQLAlchemy / EF Core.
builder.Services.AddScoped<IRepoPedidos, RepoPedidos>();
builder.Services.AddScoped<IRepoProductos, RepoProductos>();
builder.Services.AddScoped<IRepoFoodTrucks, RepoFoodTrucks>();
builder.Services.AddScoped<IRepoPagos, RepoPagos>();

// ---------- Servicios de aplicación (DDD Services) ----------
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<PedidosService>();
builder.Services.AddScoped<CobrosService>();

// ---------- Pasarela de pagos ----------
// Mock que aprueba el 90% de los pagos. En producción se reemplaza por el cliente real.
builder.Services.AddSingleton<IPasarelaPago, PasarelaPagoMock>();

// ---------- API ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TruckOrder API",
        Version = "1.0.0",
        Description =
            "Backend de TruckOrder, la app para los food trucks de Truck & Roll. " +
            "Cubre los PUC-001 (registrar pedido y cobrar) y PUC-003 (cocina y pantalla pública)."
    });
});

// CORS abierto para el front (que está en outsourcing).
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// Crear DB si no existe y cargar datos de prueba.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TruckOrderDbContext>();
    db.Database.EnsureCreated();
    Seed.Run(db);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TruckOrder API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    name = "TruckOrder API",
    version = "1.0.0",
    docs = "/swagger"
}));

app.Run();
