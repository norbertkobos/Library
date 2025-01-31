using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Library.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Library.Tests
{
    public class CategoriesControllerTests
    {
        private readonly DbContextOptions<LibraryContext> _dbContextOptions;

        public CategoriesControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "LibraryDb")
                .Options;
        }

        private LibraryContext CreateContext()
        {
            var context = new LibraryContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task GetCategories_ReturnsListOfCategories()
        {
            // Arrange
            using var context = CreateContext();
            context.Categories.AddRange(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            });
            context.SaveChanges();

            var controller = new CategoriesController(context);

            // Act
            var result = await controller.GetCategories();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Category>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var categories = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);
            Assert.Equal(2, categories.Count());
        }

        

        

        [Fact]
        public async Task PostCategory_CreatesCategory()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new CategoriesController(context);
            var category = new Category { Id = 1, Name = "New Category" };

            // Act
            var result = await controller.PostCategory(category);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Category>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdCategory = Assert.IsType<Category>(createdAtActionResult.Value);
            Assert.Equal(category.Id, createdCategory.Id);
        }

        [Fact]
        public async Task PutCategory_UpdatesCategory()
        {
            // Arrange
            using var context = CreateContext();
            var category = new Category { Id = 1, Name = "Category 1" };
            context.Categories.Add(category);
            context.SaveChanges();

            var controller = new CategoriesController(context);
            category.Name = "Updated Category";

            // Act
            var result = await controller.PutCategory(1, category);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedCategory = context.Categories.First(c => c.Id == 1);
            Assert.Equal("Updated Category", updatedCategory.Name);
        }

        

        

        [Fact]
        public async Task DeleteCategory_RemovesCategory()
        {
            // Arrange
            using var context = CreateContext();
            var category = new Category { Id = 1, Name = "Category 1" };
            context.Categories.Add(category);
            context.SaveChanges();

            var controller = new CategoriesController(context);

            // Act
            var result = await controller.DeleteCategory(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.False(context.Categories.Any(c => c.Id == 1));
        }

        
    }
}
