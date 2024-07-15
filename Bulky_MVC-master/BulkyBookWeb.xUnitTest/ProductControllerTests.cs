using Xunit;
using Moq;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBookWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class ProductControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
    private readonly Mock<ILogger<ProductController>> _mockLogger;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        _mockLogger = new Mock<ILogger<ProductController>>();
        _controller = new ProductController(_mockUnitOfWork.Object, _mockWebHostEnvironment.Object, _mockLogger.Object);
    }

    [Fact]
    public void GetAll_ReturnsOkResult_WithListOfProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Title = "Product1" },
            new Product { Id = 2, Title = "Product2" }
        };

        // _mockUnitOfWork.Setup(repo => repo.Product.GetAll(It.IsAny<string>())).Returns(products.AsQueryable());
        _mockUnitOfWork.Setup(repo => repo.Product.GetAll(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<string>()
        )).Returns(products.AsQueryable());

        // Act
        var result = _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnProducts = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Equal(2, returnProducts.Count);
    }

    [Fact]
    public void Get_ReturnsNotFound_WhenProductNotFound()
    {
        // Arrange

        // _mockUnitOfWork.Setup(repo => repo.Product.Get(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string>())).Returns((Product)null);
        var product = new Product { Id = 1, Title = "Product1" };
        _mockUnitOfWork.Setup(repo => repo.Product.Get(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<string>(),
            It.IsAny<bool>()
        )).Returns(product);

        // Act
        var result = _controller.Get(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Get_ReturnsOkResult_WithProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Title = "Product1" };
        _mockUnitOfWork.Setup(repo => repo.Product.Get(
            It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string>(),
            It.IsAny<bool>()
            )).Returns(product);

        // Act
        var result = _controller.Get(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(1, returnProduct.Id);
    }

    // Additional tests for other methods like Upsert, Delete, etc.
}
