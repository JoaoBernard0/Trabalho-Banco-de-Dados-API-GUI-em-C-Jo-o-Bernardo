using System.ComponentModel.DataAnnotations;

namespace InventoryApi.DTOs
{
    public class UpdateProductDto
    {
        [Required, StringLength(150, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
