using MyBooks.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBooks.Core.Interfaces
{
    public interface IBookList
    {
        public Task<BookListModel> Add(BookListModel bookList);
        public Task<BookListModel> Get(int bookListId, bool includeBooks = false);
        public Task<List<BookListModel>> GetAll(bool includeBooks = false);
        public Task<BookListModel> Edit(BookListModel bookList);
        public Task<bool> Delete(int bookListId);
        public Task<BookListModel> AddBookToList(string bookId, int bookListId);
        public Task<BookListModel> DeleteBookToList(string bookId, int bookListId);
        public Task<bool> Reorder(IList<int> orderedIds);
        public Task<BookListModel> GetByName(string name);
    }
}
