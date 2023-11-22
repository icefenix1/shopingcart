using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minio;
using Moq;
using ShopingCart.Contracts.Repositories;
using ShopingCart.Contracts.Workers;
using ShopingCart.Controllers;
using ShopingCart.Models.API;
using ShopingCart.Models.DB;
using ShopingCart.Repositories;
using ShopingCart.Wokers;
using System;
using System.Collections.Generic;

namespace ShoppingCart.Tests
{
    [TestClass]
    public class CartLogicTests
    {
        private readonly Mock<ICarts> _cartsMock = new Mock<ICarts>();
        private readonly Mock<IProductLogic> _productLogicMock = new Mock<IProductLogic>();



        [TestMethod]
        public void GetDBCart_ReturnsCorrectCart()
        {
            // Arrange
            var logic = new CartLogic(_cartsMock.Object, _productLogicMock.Object);
            var cartId = Guid.NewGuid();
            var expectedCart = new Cart { Id = cartId, ProductIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }, Customer = "test@test.com" };

            _cartsMock.Setup(c => c.GetCart(cartId)).Returns(expectedCart);

            // Act
            var result = logic.GetDBCart(cartId);

            // Assert
            Assert.AreEqual(expectedCart, result);
        }

        [TestMethod]
        public void GetDBCarts_ReturnsCorrectCarts()
        {
            // Arrange
            var logic = new CartLogic(_cartsMock.Object, _productLogicMock.Object);
            var expectedCarts = new List<Cart>
            {
                new Cart { Id = Guid.NewGuid(), ProductIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }, Customer = "test@test.com" },
                new Cart { Id = Guid.NewGuid(), ProductIds = new List<Guid> { Guid.NewGuid() }, Customer = "test2@test.com" }
            };

            _cartsMock.Setup(c => c.GetCarts()).Returns(expectedCarts);

            // Act
            var result = logic.GetDBCarts();

            // Assert
            CollectionAssert.AreEqual(expectedCarts, result);
        }

        [TestMethod]
        public void AddDBCart_AddsCartToRepository()
        {
            // Arrange
            var logic = new CartLogic(_cartsMock.Object, _productLogicMock.Object);
            var cartToAdd = new Cart { Id = Guid.NewGuid(), ProductIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }, Customer = "test@test.com" };

            _cartsMock.Setup(c => c.AddCart(It.IsAny<Cart>())).Returns(1);

            // Act
            var result = logic.AddDBCart(cartToAdd);

            // Assert
            Assert.AreEqual(1, result);
            _cartsMock.Verify(c => c.AddCart(It.IsAny<Cart>()), Times.Once);
        }

        [TestMethod]
        public void DeleteDBCart_DeletesCartFromRepository()
        {
            // Arrange
            var logic = new CartLogic(_cartsMock.Object, _productLogicMock.Object);
            var cartIdToDelete = Guid.NewGuid();

            _cartsMock.Setup(c => c.DeleteCart(cartIdToDelete)).Returns(1);

            // Act
            var result = logic.DeleteDBCart(cartIdToDelete);

            // Assert
            Assert.AreEqual(1, result);
            _cartsMock.Verify(c => c.DeleteCart(cartIdToDelete), Times.Once);
        }

        [TestMethod]
        public void UpdateDBCart_UpdatesCartInRepository()
        {
            // Arrange
            var logic = new CartLogic(_cartsMock.Object, _productLogicMock.Object);
            var cartToUpdate = new Cart { Id = Guid.NewGuid(), ProductIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }, Customer = "test@test.com"};

            _cartsMock.Setup(c => c.UpdateCart(cartToUpdate)).Returns(1);

            // Act
            var result = logic.UpdateDBCart(cartToUpdate);

            // Assert
            Assert.AreEqual(1, result);
            _cartsMock.Verify(c => c.UpdateCart(cartToUpdate), Times.Once);
        }


        //TODO: Fix this test. It's not working because of the callback
        [TestMethod]
        public void GetCartDetails_ReturnsCorrectDetails()
        {
            // Arrange
            var logic = new CartLogic(_cartsMock.Object, _productLogicMock.Object);
            var cartId = Guid.NewGuid();            

            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            var product1 = new Product { Id = productId1, Name = "TestProduct1", Description = "TestDescription1", Price = 19.99m, HasImages = true };
            var product2 = new Product { Id = productId2, Name = "TestProduct2", Description = "TestDescription2", Price = 29.99m, HasImages = false };

            var imageList = new List<Image> { 
                new Image { ProductId = productId1, Url = "https://example.com/image1.jpg" },
                new Image { ProductId = productId1, Url = "https://example.com/image2.jpg" }};

            var productDetails1 = new ProductsDetails(product1, imageList.Select(image => image.Url).ToList());
            var productDetails2 = new ProductsDetails(product2, new List<string>());
            var products = new List<ProductsDetails> { productDetails1, productDetails2};
            var carts = new Cart { Id = cartId, ProductIds = new List<Guid> { productDetails1.Id, productDetails2.Id }, Customer = "test@test.com" };


            _productLogicMock.Setup(p => p.GetProductsDetails(It.IsAny<Guid>())).Callback((Guid id) => products.FirstOrDefault(product => product.Id == id));
            _cartsMock.Setup(c => c.GetCart(cartId)).Returns(carts);

            // Act
            var result = logic.GetCartDetails(cartId);

            // Assert
            Assert.AreEqual(cartId, result.Id);
            CollectionAssert.AreEqual(new List<ProductsDetails> { productDetails1, productDetails2 }, result.Products);
        }

        //TODO: Fix this test. It's not working because of the callback
        [TestMethod]
        public void GetCartDetails_ReturnsCorrectDetails_MultipleCarts()
        {
            // Arrange
            var logic = new CartLogic(_cartsMock.Object, _productLogicMock.Object);
            var cartId1 = Guid.NewGuid();
            var cartId2 = Guid.NewGuid();

            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();
            var productId3 = Guid.NewGuid();

            var product1 = new Product { Id = productId1, Name = "TestProduct1", Description = "TestDescription1", Price = 19.99m, HasImages = true };
            var product2 = new Product { Id = productId2, Name = "TestProduct2", Description = "TestDescription2", Price = 29.99m, HasImages = false };
            var product3 = new Product { Id = productId3, Name = "TestProduct3", Description = "TestDescription3", Price = 39.99m, HasImages = true };

            var imageList = new List<Image> {
                new Image { ProductId = productId1, Url = "https://example.com/image1.jpg" },
                new Image { ProductId = productId3, Url = "https://example.com/image2.jpg" }};



            var productDetails1 = new ProductsDetails(product1, imageList.Where(image => image.ProductId == product1.Id).Select(url => url.Url).ToList());
            var productDetails2 = new ProductsDetails(product2, new List<string>());
            var productDetails3 = new ProductsDetails(product3, imageList.Where(image => image.ProductId == product2.Id).Select(url => url.Url).ToList());
            var products = new List<ProductsDetails> { productDetails1, productDetails2, productDetails3 };
            var carts = new List<Cart>
            {
                new Cart { Id = cartId1, ProductIds = new List<Guid> { productDetails1.Id }, Customer = "test@test.com"},
                new Cart { Id = cartId2, ProductIds = new List<Guid> { productDetails2.Id, productDetails3.Id }, Customer = "test2@test.com" }
            };

            _productLogicMock.Setup(p => p.GetProductsDetails(It.IsAny<Guid>())).Callback((Guid id) =>  products.FirstOrDefault(product => product.Id == id));
            _cartsMock.Setup(c => c.GetCarts()).Returns(carts);


            // Act
            var result = logic.GetCartDetails();

            // Assert
            CollectionAssert.AreEqual(new List<CartDetails>
            {
                new CartDetails(carts[0], new List<ProductsDetails> { productDetails1 }),
                new CartDetails(carts[1], new List<ProductsDetails> { productDetails2, productDetails3 })
            }, result);
        }
    }
}
