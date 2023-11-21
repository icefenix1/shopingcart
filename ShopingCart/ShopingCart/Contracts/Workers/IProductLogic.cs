using ShopingCart.Models.API;
using ShopingCart.Models.DB;

namespace ShopingCart.Contracts.Workers
{
    public interface IProductLogic
    {
        int AddDBImage(Image image);
        int AddDBProduct(Product product);
        int DeleteDBImage(Guid id);
        int DeleteDBProduct(Guid id);
        Image GetDBImage(Guid id);
        List<Image> GetDBImages();
        Product GetDBProduct(Guid id);
        List<Product> GetDBProducts();
        List<ProductsDetails> GetProductsDetails();
        ProductsDetails GetProductsDetails(Guid id);
        int UpdateDBImage(Image image);
        int UpdateDBProduct(Product product);
        Task<string> UploadImage(IFormFile file, Guid productid);
    }
}