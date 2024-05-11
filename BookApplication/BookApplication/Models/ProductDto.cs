using System.ComponentModel.DataAnnotations;

namespace BookApplication.Models
{
    public class ProductDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";
        [Required, MaxLength(100)]
        public string Brand { get; set; } = "";
        [Required, MaxLength(100)]
        public string Category { get; set; } = "";
        [Required]
        public decimal Price { get; set; } = 0;
        [Required]
        public string Descrption { get; set; } = "";
        public IFormFile? ImageFile { get; set; }
    }
}
