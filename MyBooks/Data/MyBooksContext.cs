using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBooks.Models;

namespace MyBooks.Data
{
    public class MyBooksContext : IdentityDbContext<UserModel>
    {
        public MyBooksContext(DbContextOptions<MyBooksContext> options)
            : base(options)
        {
        }

        public DbSet<BookListModel> BookList { get; set; }
        public DbSet<BookCategoryModel> Category { get; set; }
        public DbSet<AuthorModel> Author { get; set; }
        public DbSet<PublisherModel> Publisher { get; set; }
        public DbSet<BookModel> Book { get; set; }
        public DbSet<Book_BookListModel> BookList_Book { get; set; }
        public DbSet<Author_BookModel> Author_Book { get; set; }
        public DbSet<Book_CategoryModel> Book_Category { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookListModel>()
                .HasMany(bl => bl.Book_BookLists)
                .WithOne(bl => bl.BookList)
                .HasForeignKey(bl => bl.BookListId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
