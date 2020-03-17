
namespace MyBooks.Models
{
    public class Author_BookModel
    {
        public int Id { get; set; }
        public BookModel Book { get; set; }
        public AuthorModel Author { get; set; }

        public int BookId { get; set; }

        public int AuthorId { get; set; }
    }
}
