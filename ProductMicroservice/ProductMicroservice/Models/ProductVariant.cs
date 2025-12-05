using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ProductMicroservice.Models.Enums;

namespace ProductMicroservice.Models
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Stock { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; }

        // Enums obligatorios
        public Color Color { get; set; }
        public Size Size { get; set; }
        public Fabric Fabric { get; set; }

        // Enums opcionales
        public NeckType? NeckType { get; set; }
        public Fit? Fit { get; set; }
    }
}
