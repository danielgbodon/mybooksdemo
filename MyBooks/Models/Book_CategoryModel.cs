
namespace MyBooks.Models
{
    public class Book_CategoryModel
    {
        public int Id { get; set; }
        public BookModel Book { get; set; }
        public BookCategoryModel Category { get; set; }

        public int BookId { get; set; }

        public int CategoryId { get; set; }
    }
}
