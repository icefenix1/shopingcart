using ShopingCart.Models.DB;

namespace ShopingCart.Contracts.Repositories
{
    public interface IImages
    {
        int AddImage(Image image);
        int DeleteImage(Guid id);
        Image GetImage(Guid id);
        List<Image> GetImages();
        int UpdateImage(Image image);
    }
}