using ShopingCart.Models.API;
using ShopingCart.Models.DB;

namespace ShopingCart.Contracts.Workers
{
    public interface ICartLogic
    {
        int AddDBCart(Cart cart);
        int DeleteDBCart(Guid id);
        CartDetails GetCartDetails(Guid id);
        List<CartDetails> GetCartDetails();
        Cart GetDBCart(Guid id);
        List<Cart> GetDBCarts();
        int UpdateDBCart(Cart cart);
    }
}