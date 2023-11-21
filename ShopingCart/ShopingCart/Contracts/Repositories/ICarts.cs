using ShopingCart.Models.DB;
using ShopingCart.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ShopingCart.Contracts.Repositories
{
    public interface ICarts
    {
        int AddCart(Cart cart);
        int DeleteCart(Guid id);
        Cart GetCart(Guid id);
        List<Cart> GetCarts();
        int UpdateCart(Cart cart);
    }
}