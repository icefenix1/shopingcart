using ShopingCart.Models.DB;

namespace ShopingCart.Contracts.Repositories
{
    public interface IProducts
    {
        int AddProduct(Product product);
        int DeleteProduct(Guid id);
        Product GetProduct(Guid id);
        List<Product> GetProducts();
        int UpdateProduct(Product product);
    }
}