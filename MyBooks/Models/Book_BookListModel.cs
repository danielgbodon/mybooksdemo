
namespace MyBooks.Models
{
    public class Book_BookListModel
    {
        public int Id { get; set; }
        public BookModel Book { get; set; }
        public BookListModel BookList { get; set; }

        public int BookId {get; set;}

        public int BookListId {get; set;}

    }
}
