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
    public class BooksControllerTests
    {
        private readonly DbContextOptions<LibraryContext> _dbContextOptions;

        public BooksControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "LibraryDb")
                .Options;

           
            using var context = new LibraryContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }


        [Fact]
        public async Task GetBooks_ReturnsListOfBooks()
        {

            using var context = new LibraryContext(_dbContextOptions);
            context.Books.AddRange(new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = new Author { Name = "Author 1" }, Category = new Category { Name = "Category 1" } },
                new Book { Id = 2, Title = "Book 2", Author = new Author { Name = "Author 2" }, Category = new Category { Name = "Category 2" } }
            });
            context.SaveChanges();

            var controller = new BooksController(context);

            var result = await controller.GetBooks();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
            Assert.Equal(2, books.Count());
        }

        [Fact]
        public async Task GetBook_ReturnsBook()
        {

            using var context = new LibraryContext(_dbContextOptions);
            var book = new Book { Id = 1, Title = "Book 3", Author = new Author { Name = "Author 3" }, Category = new Category { Name = "Category 3" } };
            context.Books.Add(book);
            context.SaveChanges();

            var controller = new BooksController(context);

            var result = await controller.GetBook(1);

            var actionResult = Assert.IsType<ActionResult<Book>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var retrievedBook = Assert.IsType<Book>(okResult.Value);
            Assert.Equal(book.Id, retrievedBook.Id);
        }


        [Fact]
        public async Task PostBook_CreatesBook()
        {

            using var context = new LibraryContext(_dbContextOptions);
            var controller = new BooksController(context);
            var book = new Book { Id = 5, Title = "New Book", Author = new Author { Name = "New Author" }, Category = new Category { Name = "New Category" } };

            var result = await controller.PostBook(book);

            var actionResult = Assert.IsType<ActionResult<Book>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdBook = Assert.IsType<Book>(createdAtActionResult.Value);
            Assert.Equal(book.Id, createdBook.Id);
        }



        [Fact]
        public async Task DeleteBook_RemovesBook()
        {

            using var context = new LibraryContext(_dbContextOptions);
            var book = new Book { Id = 4, Title = "Book 4", Author = new Author { Name = "Author 4" }, Category = new Category { Name = "Category 4" } };
            context.Books.Add(book);
            context.SaveChanges();

            var controller = new BooksController(context);

            var result = await controller.DeleteBook(4);

            Assert.IsType<NoContentResult>(result);
            Assert.False(context.Books.Any(b => b.Id == 4));
        }

        
    }
}
