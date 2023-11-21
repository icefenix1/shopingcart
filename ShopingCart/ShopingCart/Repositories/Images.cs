using Microsoft.EntityFrameworkCore;
using ShopingCart.Contracts.Repositories;
using ShopingCart.Models.DB;

namespace ShopingCart.Repositories
{
    public class Images(ShopingCartContext context) : IImages
    {
        readonly ShopingCartContext _context = context;

        public List<Image> GetImages() => _context.Images.ToList();

        public Image GetImage(Guid id)
        {
            return _context.Images.FirstOrDefault(p => p.Id == id);
        }

        public int AddImage(Image image)
        {
            _context.Images.Add(image);
            return _context.SaveChanges();
        }
        public int DeleteImage(Guid id)
        {
            var image = _context.Images.FirstOrDefault(p => p.Id == id);
            if (image != null)
            {
                _context.Images.Remove(image);
                return _context.SaveChanges();
            }
            return 0; 

        }

        public int UpdateImage(Image image)
        {
            _context.Images.Update(image);
            return _context.SaveChanges();
        }
    }
}
