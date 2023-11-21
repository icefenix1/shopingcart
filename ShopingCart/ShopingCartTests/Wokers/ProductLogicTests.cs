using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Moq;
using ShopingCart.Contracts.Repositories;
using ShopingCart.Models.DB;
using ShopingCart.Wokers;

namespace ShoppingCart.Tests
{
    [TestClass]
    public class ProductLogicTests
    {
        private readonly Mock<IImages> _imagesMock = new Mock<IImages>();
        private readonly Mock<IProducts> _productsMock = new Mock<IProducts>();
        private readonly Mock<IMinioClient> _minioClientMock = new Mock<IMinioClient>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();

        [TestMethod]
        public void GetDBProduct_ReturnsCorrectProduct()
        {
            // Arrange
            var logic = new ProductLogic(_imagesMock.Object, _productsMock.Object, _minioClientMock.Object, _configurationMock.Object);
            var productId = Guid.NewGuid();
            var expectedProduct = new Product { Id = productId, Name = "TestProduct", Description = "TestDescription", Price = 19.99m, HasImages = true };


            _productsMock.Setup(p => p.GetProduct(productId)).Returns(expectedProduct);

            // Act
            var result = logic.GetDBProduct(productId);

            // Assert
            Assert.AreEqual(expectedProduct, result);
        }

        // TODO: Fix this test
        // Need to workout how to correct the return type of PutObjectAsync when being MOQed

        //[TestMethod]
        //public async Task UploadImage_UploadsFileAndUpdatesDatabase()
        //{
        //    // Arrange
        //    var logic = new ProductLogic(_imagesMock.Object, _productsMock.Object, _minioClientMock.Object, _configurationMock.Object);
        //    var productId = Guid.NewGuid();
        //    var file = new Mock<IFormFile>();
        //    file.Setup(f => f.FileName).Returns("test.jpg");
        //    file.Setup(f => f.ContentType).Returns("image/jpeg");

        //    var product = new Product { Id = productId, Name = "TestProduct", Description = "TestDescription", Price = 19.99m, HasImages = false };

        //    _productsMock.Setup(p => p.GetProduct(productId)).Returns(product);
        //    _minioClientMock.Setup(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), new CancellationToken()))
        //        .Returns((Task<Minio.DataModel.Response.PutObjectResponse>)Task.CompletedTask);
        //    _imagesMock.Setup(i => i.AddImage(It.IsAny<Image>())).Returns(1);

        //    // Act
        //    var result = await logic.UploadImage(file.Object, productId);

        //    // Assert
        //    Assert.AreEqual("File uploaded successfully.", result);
        //    _minioClientMock.Verify(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), new CancellationToken()), Times.Once);
        //    _imagesMock.Verify(i => i.AddImage(It.IsAny<Image>()), Times.Once);
        //    _productsMock.Verify(p => p.UpdateProduct(It.IsAny<Product>()), Times.Once);
        //}

        [TestMethod]
        public void GetProductsDetails_ReturnsCorrectDetails()
        {
            // Arrange
            var logic = new ProductLogic(_imagesMock.Object, _productsMock.Object, _minioClientMock.Object, _configurationMock.Object);

            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "TestProduct", Description = "TestDescription", Price = 19.99m, HasImages = true };
            var image = new Image { ProductId = productId, Url = "https://example.com/image.jpg" };

            _productsMock.Setup(p => p.GetProduct(productId)).Returns(product);
            _imagesMock.Setup(i => i.GetImages()).Returns(new List<Image> { image });

            // Act
            var result = logic.GetProductsDetails(productId);

            // Assert
            Assert.AreEqual(product.Id, result.Id);
            Assert.AreEqual(product.Name, result.Name);
            Assert.AreEqual(product.Description, result.Description);
            Assert.AreEqual(product.Price, result.Price);
            CollectionAssert.AreEqual(new List<string> { image.Url }, result.Images);
        }

        [TestMethod]
        public void GetProductsDetails_ReturnsCorrectDetails_MultipleProducts()
        {
            // Arrange
            var logic = new ProductLogic(_imagesMock.Object, _productsMock.Object, _minioClientMock.Object, _configurationMock.Object);
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            var product1 = new Product { Id = productId1, Name = "TestProduct1", Description = "TestDescription1", Price = 19.99m, HasImages = true };
            var product2 = new Product { Id = productId2, Name = "TestProduct2", Description = "TestDescription2", Price = 29.99m, HasImages = true };



            var image1 = new Image { ProductId = productId1, Url = "https://example.com/image1.jpg" };
            var image2 = new Image { ProductId = productId2, Url = "https://example.com/image2.jpg" };

            _productsMock.Setup(p => p.GetProducts()).Returns(new List<Product> { product1, product2 });
            _imagesMock.Setup(i => i.GetImages()).Returns(new List<Image> { image1, image2 });

            // Act
            var result = logic.GetProductsDetails();

            // Assert
            Assert.AreEqual(2, result.Count);

            var resultProduct1 = result.FirstOrDefault(p => p.Id == productId1);
            Assert.IsNotNull(resultProduct1);
            Assert.AreEqual(product1.Name, resultProduct1.Name);
            CollectionAssert.AreEqual(new List<string> { image1.Url }, resultProduct1.Images);

            var resultProduct2 = result.FirstOrDefault(p => p.Id == productId2);
            Assert.IsNotNull(resultProduct2);
            Assert.AreEqual(product2.Name, resultProduct2.Name);
            CollectionAssert.AreEqual(new List<string> { image2.Url }, resultProduct2.Images);
        }

        [TestMethod]
        public void GetDBProduct_ThrowsException()
        {
            // Arrange
            var logic = new ProductLogic(_imagesMock.Object, _productsMock.Object, _minioClientMock.Object, _configurationMock.Object);
            var productId = Guid.NewGuid();

            _productsMock.Setup(p => p.GetProduct(productId)).Throws<Exception>();

            // Act & Assert
            Assert.ThrowsException<Exception>(() => logic.GetDBProduct(productId));
        }

        [TestMethod]
        public async Task UploadImage_ThrowsMinioException()
        {
            // Arrange
            var logic = new ProductLogic(_imagesMock.Object, _productsMock.Object, _minioClientMock.Object, _configurationMock.Object);

            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns("test.jpg");
            file.Setup(f => f.ContentType).Returns("image/jpeg");

            var product = new Product { Name = "TestProduct", Description = "TestDescription", Price = 19.99m, HasImages = false };
            var productId = product.Id;

            _productsMock.Setup(p => p.GetProduct(productId)).Returns(product);
            _minioClientMock.Setup(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), new CancellationToken())).Throws<MinioException>(); 

            // Act & Assert
            var result = await logic.UploadImage(file.Object, productId);
            StringAssert.Contains(result, "File Upload error");
        }
    }
}
