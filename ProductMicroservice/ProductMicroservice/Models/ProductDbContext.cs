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
        /// <remarks>
        /// DbSet permite realizar consultas LINQ y operaciones CRUD (Create, Read, Update, Delete).
        /// Si la tabla no existe, EF Core intentará crearla con este nombre al ejecutar las migraciones.
        /// </remarks>

        /// <summary>
        /// Tabla principal de Productos (Padres).
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Tabla de Variantes (Hijos/SKUs). Aquí se guarda el stock real por talle y color.
        /// </summary>
        public DbSet<ProductVariant> ProductVariants { get; set; }

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

        /// <summary>
        /// Configuración avanzada del modelo al crear la base de datos.
        /// Aquí definimos cómo se guardan los Enums.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
                .Property(p => p.Type)
                .HasConversion<string>();

            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.Color)
                .HasConversion<string>();

            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.Size)
                .HasConversion<string>();

            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.Fabric)
                .HasConversion<string>();

            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.NeckType)
                .HasConversion<string>();

            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.Fit)
                .HasConversion<string>();
        }
    }
}