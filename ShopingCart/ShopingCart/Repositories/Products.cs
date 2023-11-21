using Microsoft.EntityFrameworkCore;
using ShopingCart.Contracts.Repositories;
using ShopingCart.Models.DB;

namespace ShopingCart.Repositories
{
    public class Products(ShopingCartContext context) : IProducts
    {
        readonly ShopingCartContext _context = context;

        public List<Product> GetProducts() => _context.Products.ToList();

        public Product GetProduct(Guid id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public int AddProduct(Product product)
        {
            _context.Products.Add(product);
            return _context.SaveChanges();
        }
        public int DeleteProduct(Guid id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                return _context.SaveChanges();
            }
            return 0;
        }

        public int UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            return _context.SaveChanges();
        }
    }
}
