using ProductMicroservice.Models;
using Microsoft.EntityFrameworkCore; 

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
            return await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _dbContext.Products.ToListAsync();
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

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

    }
}
