using Microsoft.EntityFrameworkCore;

namespace ProductMicroservice.Models
{
    /// <summary>
    /// Representa el contexto de datos (sesión con la base de datos) para el microservicio de Productos.
    /// </summary>
    /// <remarks>
    /// Esta clase actúa como un puente entre tus objetos C# y las tablas de SQL Server.
    /// Hereda de <see cref="DbContext"/>, que es la clase base fundamental de Entity Framework Core.
    /// </remarks>
    public class ProductDbContext : DbContext
    {
        /// <summary>
        /// Representa la tabla "Products" en la base de datos.
        /// </summary>
        /// <remarks>
        /// DbSet permite realizar consultas LINQ y operaciones CRUD (Create, Read, Update, Delete).
        /// Si la tabla no existe, EF Core intentará crearla con este nombre al ejecutar las migraciones.
        /// </remarks>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Constructor que configura el contexto mediante Inyección de Dependencias.
        /// </summary>
        /// <param name="options">
        /// Opciones de configuración (como la cadena de conexión 'ConnectionStrings:ProductDB') 
        /// que vienen inyectadas desde el archivo Program.cs.
        /// </param>
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
            // Se llama al constructor base (base(options)) para pasarle la configuración a EF Core.
            // El cuerpo del constructor se deja vacío porque no necesitamos inicialización extra por ahora.
        }
    }
}