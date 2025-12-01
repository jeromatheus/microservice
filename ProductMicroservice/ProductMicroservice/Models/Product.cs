using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductMicroservice.Models
{
    /// <summary>
    /// El Id como campo necesario para hacer un POST en Swagger es porque usar la entidad como modelo de 
    /// entrada es una mala práctica llamada Overposting. Debes crear una clase "molde" (DTO) con solo los 
    /// datos que el usuario tiene permiso de enviar.
    /// </summary>
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
