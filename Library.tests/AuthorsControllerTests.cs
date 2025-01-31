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
    public class AuthorsControllerTests
    {
        private readonly DbContextOptions<LibraryContext> _dbContextOptions;

        public AuthorsControllerTests()
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
        public async Task GetAuthors_ReturnsListOfAuthors()
        {

            using var context = CreateContext();
            context.Authors.AddRange(new List<Author>
            {
                new Author { Id = 1, Name = "Author 1" },
                new Author { Id = 2, Name = "Author 2" }
            });
            context.SaveChanges();

            var controller = new AuthorsController(context);

            var result = await controller.GetAuthors();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Author>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var authors = Assert.IsAssignableFrom<IEnumerable<Author>>(okResult.Value);
            Assert.Equal(2, authors.Count());
        }

        [Fact]
        public async Task GetAuthor_ReturnsAuthor()
        {

            using var context = CreateContext();
            var author = new Author { Id = 1, Name = "Author 1" };
            context.Authors.Add(author);
            context.SaveChanges();

            var controller = new AuthorsController(context);

            var result = await controller.GetAuthor(1);

            if (result.Result == null)
            {
                var authorsInDb = context.Authors.ToList();
                System.Diagnostics.Debug.WriteLine($"Authors in DB: {authorsInDb.Count}");
                foreach (var auth in authorsInDb)
                {
                    System.Diagnostics.Debug.WriteLine($"Author: {auth.Id}, {auth.Name}");
                }
            }
        }
        

        [Fact]
        public async Task PostAuthor_CreatesAuthor()
        {

            using var context = CreateContext();
            var controller = new AuthorsController(context);
            var author = new Author { Id = 1, Name = "New Author" };

            var result = await controller.PostAuthor(author);

            var actionResult = Assert.IsType<ActionResult<Author>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdAuthor = Assert.IsType<Author>(createdAtActionResult.Value);
            Assert.Equal(author.Id, createdAuthor.Id);
        }

        [Fact]
        public async Task PutAuthor_UpdatesAuthor()
        {

            using var context = CreateContext();
            var author = new Author { Id = 1, Name = "Author 1" };
            context.Authors.Add(author);
            context.SaveChanges();

            var controller = new AuthorsController(context);
            author.Name = "Updated Author";

            var result = await controller.PutAuthor(1, author);

            Assert.IsType<NoContentResult>(result);
            var updatedAuthor = context.Authors.First(a => a.Id == 1);
            Assert.Equal("Updated Author", updatedAuthor.Name);
        }

        

        

        [Fact]
        public async Task DeleteAuthor_RemovesAuthor()
        {
            using var context = CreateContext();
            var author = new Author { Id = 1, Name = "Author 1" };
            context.Authors.Add(author);
            context.SaveChanges();

            var controller = new AuthorsController(context);

            var result = await controller.DeleteAuthor(1);

            Assert.IsType<NoContentResult>(result);
            Assert.False(context.Authors.Any(a => a.Id == 1));
        }

       
    }
}
