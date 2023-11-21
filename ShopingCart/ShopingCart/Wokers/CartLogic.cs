using ShopingCart.Contracts.Repositories;
using ShopingCart.Contracts.Workers;
using ShopingCart.Models.API;
using ShopingCart.Models.DB;

namespace ShopingCart.Wokers
{
    public class CartLogic(ICarts carts, IProductLogic productLogic) : ICartLogic
    {
        readonly IProductLogic _productLogic = productLogic;
        readonly ICarts _carts = carts;

        public Cart GetDBCart(Guid id) => _carts.GetCart(id);
        public List<Cart> GetDBCarts() => _carts.GetCarts();
        public int AddDBCart(Cart cart) => _carts.AddCart(cart);
        public int DeleteDBCart(Guid id) => _carts.DeleteCart(id);
        public int UpdateDBCart(Cart cart) => _carts.UpdateCart(cart);

        public CartDetails GetCartDetails(Guid id)
        {
            var cart = GetDBCart(id);
            var products = new List<ProductsDetails>();
            foreach (var product in cart.ProductIds)
            {
                products.Add(_productLogic.GetProductsDetails(product));
            }
            return new CartDetails(cart, products);
        }

        public List<CartDetails> GetCartDetails()
        {
            var carts = GetDBCarts();
            var cartDetails = new List<CartDetails>();
            foreach (var cart in carts)
            {
                cartDetails.Add(GetCartDetails(cart.Id));
            }
            return cartDetails;
        }

    }
}
