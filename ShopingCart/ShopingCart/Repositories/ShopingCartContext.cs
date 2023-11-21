using Microsoft.EntityFrameworkCore;
using ShopingCart.Contracts.Repositories;
using ShopingCart.Models.DB;


namespace ShopingCart.Repositories;

public class ShopingCartContext : DbContext
{
    public ShopingCartContext(DbContextOptions<ShopingCartContext> options)
        : base(options)
    {
    }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Image> Images { get; set; }
}