using MyBooks.Models;
using MyBooks.ViewModels;
using System.Threading.Tasks;


namespace MyBooks.Core.Interfaces
{
    public interface IBook
    {
        public Task<PaginationModelView<BookModel>> Search(string search, int page = 0);
        public Task<PaginationModelView<BookModel>> Search(BookModel book, int page = 0);
        public Task<BookModel> Get(string id);
        public Task<BookModel> Add(BookModel book);
        public Task<BookModel> Edit(BookModel book);
        public Task<bool> Delete(string id);
        public Task<BookListModel> UserBookList(int id);

    }
}
