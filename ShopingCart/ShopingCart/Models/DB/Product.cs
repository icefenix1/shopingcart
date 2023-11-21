using System.ComponentModel.DataAnnotations;

namespace ShopingCart.Models.DB
{
    public class Product
    {
        public Guid Id { get ; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public bool HasImages { get; set; }
    }
}
