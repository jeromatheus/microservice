var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. ACTIVAR CONTROLADORES
// Esto es obligatorio porque creamos un AuthController.cs
builder.Services.AddControllers();

// Configuración de Swagger (Útil para probar directo sin Gateway)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. DESACTIVAR HTTPS REDIRECTION
// Comentado para evitar el warning "Failed to determine the https port" 
// y facilitar la conexión con Ocelot en http://localhost:5005
// app.UseHttpsRedirection();

app.UseAuthorization();

// 3. MAPEAR CONTROLADORES
// Sin esto, tu AuthController no recibirá ninguna petición.
app.MapControllers();

app.Run();