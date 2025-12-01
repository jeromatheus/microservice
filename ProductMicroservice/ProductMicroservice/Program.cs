using Microsoft.EntityFrameworkCore;
using ProductMicroservice.Models;
using ProductMicroservice.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// REGISTRO DE BD E INYECCIÓN DE DEPENDENCIAS
// 1. Registra 'ProductDbContext' en el contenedor de servicios (DI). Lo inyecta automáticamente en los constructores
// de los controladores, evitando tener que crearlo manualmente (new ProductDbContext()).
// 2. Se crea una instancia por cada petición HTTP y se destruye automáticamente al terminar, gestionando mejor la memoria.

builder.Services.AddDbContext<ProductDbContext>(options =>
{
    // Configura SQL Server leyendo la cadena de conexión "ProductDB" desde appsettings.json
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDB"));
});

// Se registra la interfaz IProductRepository y su implementación ProductRepository en el contenedor de dependencias. 
// Cada vez que se solicite IProductRepository, se creará una nueva instancia de ProductRepository.
builder.Services.AddTransient<IProductRepository, ProductRepository>();

// Servicio para poder usar Swagger como documentación Open API alternativa a Postman
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
