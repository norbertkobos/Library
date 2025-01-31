using Microsoft.EntityFrameworkCore;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace Library.Infrastructure.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }

        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var path = System.IO.Path.Combine(Environment.CurrentDirectory, "library.db");
                object value = optionsBuilder.UseSqlite($"Data Source={path}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "J.K. Rowling" },
                new Author { Id = 2, Name = "George R.R. Martin" },
                new Author { Id = 3, Name = "J.R.R. Tolkien" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fantasy" },
                new Category { Id = 2, Name = "Science Fiction" },
                new Category { Id = 3, Name = "Mystery" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Harry Potter", AuthorId = 1, CategoryId = 1 },
                new Book { Id = 2, Title = "Game of Thrones", AuthorId = 2, CategoryId = 1 },
                new Book { Id = 3, Title = "The Hobbit", AuthorId = 3, CategoryId = 1 }
            );
        }
    }
}
