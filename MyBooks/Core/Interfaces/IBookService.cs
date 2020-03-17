using MyBooks.Models;
using MyBooks.ViewModels;

namespace MyBooks.Core.Interfaces
{
    public interface IBookService
    {
        public PaginationModelView<BookModel> Search(string search, int page = 0);
        public PaginationModelView<BookModel> Search(BookModel book, int page = 0);
        public BookModel Get(string id);

    }
}
