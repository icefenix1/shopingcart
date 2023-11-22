using ShopingCart.Models.DB;

namespace ShopingCart.Models.API
{
    public class CartDetails
    {
        public Guid Id { get; set; }
        public List<ProductsDetails> Products { get; set; }
        public string Customer { get; set; }
        public decimal TotalPrice { get => Products.Any() ? Products.Sum(price => price.Price) : 0; }

        public CartDetails(Cart cart, List<ProductsDetails> products)
        {
            Id = cart.Id;
            Products = products;
            Customer = cart.Customer;
        }


    }
}
