using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using ShopingCart.Contracts.Repositories;
using ShopingCart.Contracts.Workers;
using ShopingCart.Models.API;
using ShopingCart.Models.DB;
using System.Security.AccessControl;

namespace ShopingCart.Wokers
{
    public class ProductLogic(IImages images, IProducts products, IMinioClient minioClient, IConfiguration configuration) : IProductLogic
    {
        readonly IImages _images = images;
        readonly IProducts _products = products;
        private readonly IMinioClient _minioClient = minioClient;
        readonly IConfiguration _configuration = configuration;
        readonly string _bucketName = configuration["Minio:BucketName"] ?? "images";


        public Product GetDBProduct(Guid id) => _products.GetProduct(id);
        public List<Product> GetDBProducts() => _products.GetProducts();
        public int AddDBProduct(Product product) => _products.AddProduct(product);
        public int DeleteDBProduct(Guid id) => _products.DeleteProduct(id);
        public int UpdateDBProduct(Product product) => _products.UpdateProduct(product);
        public Image GetDBImage(Guid id) => _images.GetImage(id);
        public List<Image> GetDBImages() => _images.GetImages();
        public int AddDBImage(Image image) => _images.AddImage(image);
        public int DeleteDBImage(Guid id) => _images.DeleteImage(id);
        public int UpdateDBImage(Image image) => _images.UpdateImage(image);

        public ProductsDetails GetProductsDetails(Guid id)
        {
            var product = GetDBProduct(id);
            var images = GetDBImages().Where(i => i.ProductId == id).ToList();
            return new ProductsDetails(product, images.Select(image => image.Url).ToList());
        }

        public List<ProductsDetails> GetProductsDetails()
        {
            var products = GetDBProducts();
            var images = GetDBImages();
            var result = products.Select(product => new ProductsDetails(product, 
                product.HasImages ? 
                    images.Where(i => i.ProductId == product.Id).Select(image => image.Url).ToList() :
                    new List<string>())).ToList();
            return result;
        }

        public async Task<string> UploadImage(IFormFile file, Guid productid)
        {
            //await CreateIfDoesntBucketExist();
            try
            {
                var product = GetDBProduct(productid);
                await _minioClient.PutObjectAsync(new PutObjectArgs()
                        .WithBucket(_bucketName)
                        .WithObject(file.FileName)
                        .WithObjectSize(file.Length)
                        .WithStreamData(file.OpenReadStream())
                        .WithContentType(file.ContentType));

                var image = new Image
                {
                    ProductId = productid,
                    Url = $"{_configuration["Minio:Scheme"]}://{_configuration["Minio:Endpoint"]}/{_bucketName}/{file.FileName}"
                };

                if (AddDBImage(image) > 0)
                {
                    if(!product.HasImages)
                    {
                        product.HasImages = true;
                        UpdateDBProduct(product);
                    }
                    return "File uploaded successfully.";
                }
                return $"File Upload error: DB Write";

            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload error: {0}", e.Message);
                return $"File Upload error: {e.Message}";
            }
        }

        //TODO: Fix runtime bucket creation there is some kind of access issue

        private async Task CreateIfDoesntBucketExist()
        {
            try
            {
                // Make a bucket on the server, if not already present.                
                bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs()
                    .WithBucket(_bucketName)).ConfigureAwait(false);
                if (!found)
                {
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs()
                        .WithBucket(_bucketName)).ConfigureAwait(false);
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Bucket Exist or Creat error: {0}", e.Message);
            }
        }

    }
}
