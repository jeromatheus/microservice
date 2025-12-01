using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SEGURIDAD ---
var secretKey = builder.Configuration["JwtSettings:SecretKey"];

// Validación de seguridad: Evita que la app arranque si falta la clave
if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("La clave secreta 'JwtSettings:SecretKey' no está configurada en appsettings.json");
}

var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false, // Cambiar a true en producción
        ValidateAudience = false, // Cambiar a true en producción
        ClockSkew = TimeSpan.Zero
    };
});

// --- 2. CONFIGURACIÓN DE OCELOT ---
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// (Opcional) Si quieres Swagger para el Gateway, deja AddEndpointsApiExplorer
// builder.Services.AddEndpointsApiExplorer(); 

var app = builder.Build();

// --- 3. PIPELINE DE MIDDLEWARE (EL ORDEN ES VITAL) ---

// A. (Opcional) Redirección HTTPS - Coméntalo si trabajas en localhost puerto 9000
// app.UseHttpsRedirection(); 

// B. AUTENTICACIÓN: Primero validamos el Token (¿Quién es?)
app.UseAuthentication();

// C. AUTORIZACIÓN: Luego revisamos permisos (¿Puede pasar?)
app.UseAuthorization();

// D. OCELOT: Finalmente, si pasó los filtros anteriores, Ocelot redirige la petición
await app.UseOcelot();

app.Run();