using ProductMicroservice.Models.Enums;

namespace ProductMicroservice.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public ProductType Type { get; set; }
        public decimal Price { get; set; }
        public List<CreateVariantDto> Variants { get; set; }
    }

    public class CreateVariantDto
    {
        public Color Color { get; set; }
        public Size Size { get; set; }
        public Fabric Fabric { get; set; }
        public NeckType? NeckType { get; set; }
        public Fit? Fit { get; set; }
        public int Stock { get; set; }
    }
}
