using MyBooks.Models;
using System;
using MyBooks.Core.Interfaces;
using MyBooks.ViewModels;
using MyBooks.Core;
using System.Threading.Tasks;
using MyBooks.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace MyBooks.Controllers
{
    public class Book : IBook
    {
        private readonly IBookService _bookService;
        private readonly MyBooksContext _context;
        private readonly ICategory _category;
        private readonly IAuthor _author;
        private readonly IPublisher _publisher;


        public Book(IBookService bookService, MyBooksContext context, ICategory category, IAuthor author, IPublisher publisher)
        {
            _bookService = bookService;
            _context = context;
            _category = category;
            _author = author;
            _publisher = publisher;
        }

        public async Task<BookModel> Add(BookModel book)
        {
            if (book == null) return null;

            if(book.Id > 0) return book;

            try
            {
                if (book.Categories != null)
                    foreach (BookCategoryModel category in book.Categories)
                        await _category.Add(category);

                if (book.Authors != null)
                    foreach (AuthorModel author in book.Authors)
                        await _author.Add(author);

                if (book.Publisher != null)
                    await _publisher.Add(book.Publisher);

                _context.Add(book);
                await _context.SaveChangesAsync();

                return book;
            }
            catch(Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        public async Task<bool> Delete(string id)
        {
            BookModel item = await Get(id);
            if (item == null)
                return false;

            try
            {
                _context.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                item = await Get(id);
                return (item == null);
            }
        }

        public Task<BookModel> Edit(BookModel book)
        {
            throw new NotImplementedException();
        }

        public async Task<BookModel> Get(string id)
        {
            if (id == null) return null;
            string[] idSplitted = id.Split(AppEnvironment.BOOK_SEPARATOR_ID);
            if (idSplitted.Length < 2) return null;
            string bdId = "", googleId = "";
            switch (idSplitted[0])
            {
                case AppEnvironment.BOOK_PREFIX_BD: bdId = idSplitted[1]; break;
                case AppEnvironment.BOOK_PREFIX_GOOGLE: googleId = idSplitted[1]; break;
            }

            return await Get(bdId, googleId);
        }

        private async Task<BookModel> Get(string BdId = "", string googleId = "", bool searchInServices = true)
        {
            var query = _context.Book
                .Include(b => b.Authors_Book).ThenInclude(ab => ab.Author)
                .Include(b => b.Book_Categories).ThenInclude(bc => bc.Category)
                .Include(b => b.Book_BookLists).ThenInclude(bbl => bbl.BookList)
                .Include(b => b.Publisher);

            BookModel book = null;
            if (!string.IsNullOrEmpty(BdId))
            {
                int id;
                if (!int.TryParse(BdId, out id)) return null;
                book = await query.Where(b => b.Id == id).FirstOrDefaultAsync();
            }

            if (!string.IsNullOrEmpty(googleId))
            {
                book = await query.Where(b => b.IdGoogleService == googleId).FirstOrDefaultAsync();
                if (book == null && searchInServices) return _bookService.Get(googleId);
            }

            if (book == null) return null;

            BookListModel bl = (await UserBookList(book.Id));
            book.UserBookListId = (bl == null) ? 0 : bl.Id;

            return book;
        }

        public async Task<BookListModel> UserBookList(int id)
        {
            if (id == 0) return null;
            UserModel u = AppEnvironment.CurrentUser;
            Book_BookListModel book_bookList = await _context.BookList_Book
                .Include(bbl => bbl.Book)
                .Include(bbl => bbl.BookList)
                .Where(b => b.Book.Id == id && b.BookList.UserId == AppEnvironment.CurrentUser.Id).FirstOrDefaultAsync();

            return (book_bookList == null) ? null : book_bookList.BookList;
        }

        public async Task<PaginationModelView<BookModel>> Search(string search, int page = 0)
        {
            PaginationModelView<BookModel> result = _bookService.Search(search, page);
            result.items = await prepareSearchResults(result.items);
            return result;
            
        }

        public async Task<PaginationModelView<BookModel>> Search(BookModel book, int page = 0)
        {
            PaginationModelView<BookModel> result = _bookService.Search(book, page);
            result.items = await prepareSearchResults(result.items);
            return result;
        }

        private async Task<IList<BookModel>> prepareSearchResults(IList<BookModel> results)
        {
            if (results.Count == 0) return results;

            List<BookModel> finalResults = new List<BookModel>();

            foreach (BookModel book in results)
            {
                BookModel finalBook = await Get("", book.IdGoogleService, false);
                if (finalBook == null) finalBook = book;
                else
                {
                    BookListModel bl = (await UserBookList(book.Id));
                    book.UserBookListId = (bl == null) ? 0 : bl.Id;
                }
                finalResults.Add(finalBook);
            }

            return finalResults;
        } 

    }
}
