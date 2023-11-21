using Microsoft.EntityFrameworkCore;
using ShopingCart.Contracts.Repositories;
using ShopingCart.Models.DB;

namespace ShopingCart.Repositories
{
    public class Carts(ShopingCartContext context) : ICarts
    {
        readonly ShopingCartContext _context = context;

        public List<Cart> GetCarts() => _context.Carts.ToList();

        public Cart GetCart(Guid id) => _context.Carts.FirstOrDefault(p => p.Id == id);

        public int AddCart(Cart cart)
        {
            _context.Carts.Add(cart);
            return _context.SaveChanges();
        }
        public int DeleteCart(Guid id)
        {
            var cart = _context.Carts.FirstOrDefault(p => p.Id == id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                return _context.SaveChanges();
            }
            return 0;
        }

        public int UpdateCart(Cart cart)
        {
            _context.Carts.Update(cart);
            return _context.SaveChanges();
        }
    }
}
