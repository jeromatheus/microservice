using ProductMicroservice.DTOs;
using ProductMicroservice.Models;

namespace ProductMicroservice.Repository
{
    // Métodos asíncronos para evitar bloqueo esperándo la respuesta
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product?> GetProductById(int id); 
        Task InsertProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(int id);
        //Task InsertRange(List<Product> products);
        Task<IEnumerable<CatalogProductDto>> GetCatalog();
        Task Save();
    }
}
