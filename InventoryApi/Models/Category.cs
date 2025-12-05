using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace InventoryApi.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
