using ShopingCart.Models.DB;

namespace ShopingCart.Models.API
{
    public class ProductsDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<string> Images { get; set; }


        public ProductsDetails(Product product, List<string> images) 
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
            Images = images;
        }

    }
}
