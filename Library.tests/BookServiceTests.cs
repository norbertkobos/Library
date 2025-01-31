using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Library.Api.SoapServices;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Library.Tests
{
    public class BookServiceTests
    {
        private readonly DbContextOptions<LibraryContext> _dbContextOptions;

        public BookServiceTests()
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

            var service = new BookService(context);

            var result = await service.GetBooks();

            Assert.Equal(2, result.Count());
        }

       

        

        [Fact]
        public async Task AddBook_AddsBookToDatabase()
        {

            using var context = new LibraryContext(_dbContextOptions);
            var service = new BookService(context);
            var book = new Book { Id = 3, Title = "New Book", Author = new Author { Name = "New Author" }, Category = new Category { Name = "New Category" } };

            await service.AddBook(book);

            Assert.Equal(1, context.Books.Count());
            Assert.Equal("New Book", context.Books.Single().Title);
        }

        [Fact]
        public async Task UpdateBook_UpdatesBookInDatabase()
        {
            using var context = new LibraryContext(_dbContextOptions);
            var book = new Book { Id = 4, Title = "Book 4", Author = new Author { Name = "Author 4" }, Category = new Category { Name = "Category 4" } };
            context.Books.Add(book);
            context.SaveChanges();

            var service = new BookService(context);
            book.Title = "Updated Book";

            await service.UpdateBook(book);

            var updatedBook = context.Books.Single();
            Assert.Equal("Updated Book", updatedBook.Title);
        }

        [Fact]
        public async Task DeleteBook_RemovesBookFromDatabase()
        {
            using var context = new LibraryContext(_dbContextOptions);
            var book = new Book
            {
                Id = 5,
                Title = "Book 5",
                Author = new Author { Name = "Author 5" },
                Category = new Category
                {
                    Name = "Category 5"
                }
            };
            context.Books.Add(book);
            context.SaveChanges();

            var service = new BookService(context);

            await service.DeleteBook(5);

            Assert.Equal(0, context.Books.Count());
        }
    }
}
