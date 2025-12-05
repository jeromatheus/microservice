using ProductMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using ProductMicroservice.DTOs;

namespace ProductMicroservice.Repository
{
    /// <summary>
    /// Repositorio encargado de gestionar las operaciones CRUD para la entidad Product.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _dbContext;

        /// <summary>
        /// Constructor del repositorio.
        /// </summary>
        /// <param name="context">
        /// El contenedor de servicios (DI) "inyecta" aquí una instancia de ProductDbContext 
        /// ya configurada y lista para usar. No hacemos 'new ProductDbContext()'.
        /// </param>
        public ProductRepository(ProductDbContext context)
        {
            _dbContext = context;
        }

        public async Task DeleteProduct(int id)
        {
            var product = await GetProductById(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await Save();
            }
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _dbContext.Products
                .Include(p => p.Variants)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _dbContext.Products
                .Include(p => p.Variants)
                .ToListAsync();
        }

        public async Task InsertProduct(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await Save();
        }

        public async Task UpdateProduct(Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified; // Update() ya marca la entidad como modificada, no hace falta manipularla manualmente
            _dbContext.Products.Update(product);
            await Save();
        }

        //public async Task InsertRange(List<Product> products)
        //{
        //    await _dbContext.Products.AddRangeAsync(products);
        //    // Hace un solo viaje a la base de datos para guardar todo (Product + Variantes)
        //    await _dbContext.SaveChangesAsync();
        //}

        public async Task<IEnumerable<CatalogProductDto>> GetCatalog()
        {
            // 1. Traemos todas las variantes con sus datos de producto
            // Nota: Traemos todo a memoria (.ToListAsync) porque los GroupBy complejos 
            // a veces dan problemas de traducción en SQL Server si no están muy optimizados.
            // Para un catálogo de e-commerce normal, esto es rápido.
            var allVariants = await _dbContext.ProductVariants
                .Include(v => v.Product)
                .ToListAsync();

            var catalogGrouped = allVariants
                .GroupBy(v => new
                {
                    // Si dos variantes tienen el mismo ProdID, Tela, Cuello y Calce, se juntan.
                    v.ProductId,
                    ProductName = v.Product.Name,
                    ProductPrice = v.Product.Price,
                    ProductType = v.Product.Type,
                    v.Fabric,
                    v.NeckType,
                    v.Fit
                })
                .Select(group => new CatalogProductDto
                {
                    ProductId = group.Key.ProductId,
                    Name = group.Key.ProductName,
                    Price = group.Key.ProductPrice,
                    Type = group.Key.ProductType,
                    // Features Fijas del grupo
                    Fabric = group.Key.Fabric,
                    NeckType = group.Key.NeckType,
                    Fit = group.Key.Fit,
                    // Agregamos lo que varía (Colores y Talles)
                    AvailableColors = group.Select(v => v.Color).Distinct().ToList(), // .Distinct() es vital para que no salga [Rojo, Rojo, Rojo]
                    AvailableSizes = group.Select(v => v.Size).Distinct().ToList(),
                    // Calculamos si hay stock en general
                    TotalStock = group.Sum(v => v.Stock)
                })
                .ToList();

            return catalogGrouped;
        }
        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

    }
}
