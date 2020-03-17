using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBooks.Core.Interfaces;
using MyBooks.Data;
using MyBooks.Models;
using MyBooks.Core;
using Microsoft.Extensions.Localization;

namespace MyBooks.Controllers
{
    public class BookList : IBookList
    {
        private readonly MyBooksContext _context;
        private readonly IBook _book;
        private readonly IStringLocalizer<BookList> _localizer;

        public BookList(MyBooksContext context, IBook book, IStringLocalizer<BookList> localizer)
        {
            _context = context;
            _book = book;
            _localizer = localizer;
        }

        public async Task<BookListModel> Add(BookListModel bookList)
        {
            if (bookList.User == null) bookList.User = AppEnvironment.CurrentUser;

            bookList.Order = (await GetAllRaw()).Count;
            _context.Add(bookList);
            await _context.SaveChangesAsync();
            return bookList;
        }

        public async Task<BookListModel> AddBookToList(string bookId, int bookListId)
        {
            BookListModel bookList = await GetRaw(bookListId, true);
            if (bookList == null) return null;

            BookModel book = await _book.Add(await _book.Get(bookId));
            if (book == null) return null;

            if (bookList.Books.Select(b => b.Id).Contains(book.Id)) return bookList;


            BookListModel currentBookList = await _book.UserBookList(book.Id);
            if (currentBookList != null)
            {
                List<Book_BookListModel> book_bookList_list = currentBookList.Book_BookLists.Where(b => b.Book.Id == book.Id).ToList();
                _context.Remove(book_bookList_list.First());
            }

            bookList.Book_BookLists.Add(new Book_BookListModel()
            {
                Book = book,
                BookList = bookList
            });

            _context.Update(bookList);
            await _context.SaveChangesAsync();
            return bookList;
        }

        public async Task<bool> Delete(int bookListId)
        {
            BookListModel item = await GetRaw(bookListId);
            if (item == null)
                return true;

            if(!item.Removable) return false;

            try
            {
                _context.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                item = await GetRaw(bookListId);
                return (item == null);
            }
        }

        public async Task<BookListModel> DeleteBookToList(string bookId, int bookListId)
        {
            BookListModel bookList = await GetRaw(bookListId, true);
            if (bookList == null) return null;

            BookModel book = await _book.Get(bookId);
            if (book == null || book.Id <=0) return bookList;

            List<Book_BookListModel> book_bookList_list = bookList.Book_BookLists.Where(b => b.BookId == book.Id).ToList();
            if (book_bookList_list.Count == 0) return bookList;

            _context.Remove(book_bookList_list.First());

            _context.Update(bookList);
            await _context.SaveChangesAsync();
            return bookList;
        }

        public async Task<BookListModel> Edit(BookListModel bookList)
        {
            BookListModel item = await GetRaw(bookList.Id);
            if (item == null)
                return null;

            if (!item.Removable) return item;

            item.Color = bookList.Color;
            item.Name = bookList.Name;
            _context.Entry(item).State = EntityState.Modified;

            try
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return bookList;
            }
            catch (DbUpdateConcurrencyException)
            {
                return item;
            }
        }

        public async Task<BookListModel> Get(int bookListId, bool includeBooks = false)
        {
            BookListModel bl = await GetRaw(bookListId, includeBooks);

            if (bl == null) return null;

            if (AppEnvironment.DefaultBookLists.ContainsKey(bl.Name)) bl.Name = _localizer[bl.Name];

            _context.Entry(bl).State = EntityState.Detached;
            return bl;
        }

        private async Task<BookListModel> GetRaw(int bookListId, bool includeBooks = false)
        {
            IQueryable<BookListModel> query = _context.BookList
                .Where(bl => bl.Id == bookListId && bl.User.Id == AppEnvironment.CurrentUser.Id);

            if (includeBooks)
                query = query.Include(bl => bl.Book_BookLists).ThenInclude(bbl => bbl.Book);

            BookListModel bl = await query.FirstOrDefaultAsync();
            return bl;
        }

        public async Task<List<BookListModel>> GetAll(bool includeBooks = false)
        {
            IQueryable<BookListModel> query = _context.BookList.Where(bl => bl.User.Id == AppEnvironment.CurrentUser.Id);
            
            if (includeBooks) 
                query = query.Include(bl => bl.Book_BookLists).ThenInclude(bbl => bbl.Book)
                    .ThenInclude(b => b.Authors_Book).ThenInclude(ab => ab.Author);

            List<BookListModel> results = await GetAllRaw(includeBooks);

            foreach (BookListModel bl in results)
            {
                if (AppEnvironment.DefaultBookLists.ContainsKey(bl.Name)) bl.Name = _localizer[bl.Name];
                _context.Entry(bl).State = EntityState.Detached;
            }
                

            return results;
        }

        public async Task<List<BookListModel>> GetAllRaw(bool includeBooks = false)
        {
            IQueryable<BookListModel> query = _context.BookList.Where(bl => bl.User.Id == AppEnvironment.CurrentUser.Id);

            if (includeBooks)
                query = query.Include(bl => bl.Book_BookLists).ThenInclude(bbl => bbl.Book)
                    .ThenInclude(b => b.Authors_Book).ThenInclude(ab => ab.Author);

            return await query.OrderBy(bl => bl.Order).ToListAsync();
        }

        public async Task<BookListModel> GetByName(string name)
        {
            foreach (string defaultBookListName in AppEnvironment.DefaultBookLists.Keys)
            {//IF THE NAME IS IN TRANSLATE NAME, I CATCH THE REAL NAME
                if (_localizer[defaultBookListName] == name) {
                    name = defaultBookListName;
                    break;
                }
            }

            return await _context.BookList
                .Where(bl => bl.Name.ToUpper() == name.ToUpper() && bl.UserId == AppEnvironment.CurrentUser.Id && bl.Removable == false)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Reorder(IList<int> orderedIds)
        {
            List<BookListModel> userLists = await GetAllRaw();
            List<int> userListIds = userLists.Select(bl => bl.Id).ToList();

            if(userListIds.Count != orderedIds.Count) return false;

            int position=0;
            foreach(int id in orderedIds)
            {
                if(!userListIds.Contains(id)) return false;
                BookListModel bookList= userLists[userListIds.IndexOf(id)];
                bookList.Order = position;
                _context.Update(bookList);
                position++;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}