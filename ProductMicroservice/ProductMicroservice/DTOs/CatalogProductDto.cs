using ProductMicroservice.Models.Enums;

namespace ProductMicroservice.DTOs
{
    public class CatalogProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public ProductType Type { get; set; }

        // --- Características que hacen única a la prenda (El Grupo) ---
        public Fabric Fabric { get; set; }
        public NeckType? NeckType { get; set; }
        public Fit? Fit { get; set; }

        // --- Listas Agrupadas (Para que el usuario sepa qué hay) ---
        public List<Color> AvailableColors { get; set; }
        public List<Size> AvailableSizes { get; set; }

        // Suma total del stock de todas las variantes de este grupo
        public int TotalStock { get; set; }
    }
}
