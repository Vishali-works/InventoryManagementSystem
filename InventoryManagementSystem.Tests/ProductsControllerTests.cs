using InventoryManagementSystem.Controllers;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InventoryManagementSystem.Tests;

public class ProductsControllerTests
{
    [Fact]
    public async Task PostProduct_CreatesDefaultSupplier_WhenSupplierIdIsMissingOrInvalid()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);
        var controller = new ProductsController(context);

        var product = new Product
        {
            ProductName = "Test Product",
            Price = 10.99m,
            QuantityInStock = 5,
            ReorderLevel = 2,
            SupplierId = 999
        };

        var result = await controller.PostProduct(product);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var savedProduct = Assert.IsType<Product>(created.Value);
        Assert.True(savedProduct.SupplierId > 0);
        Assert.Single(context.Suppliers);
    }
}
