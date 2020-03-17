using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBooks.Core;
using MyBooks.Core.Interfaces;
using MyBooks.Models;

namespace MyBooks.Controllers.Web
{
    public class BookController : Controller
    {
        private readonly IBook _book;
        private readonly IBookList _bookList;

        public BookController(IBook book, IBookList bookList)
        {
            _book = book;
            _bookList = bookList;
        }

        public async Task<IActionResult> Index(string Search, int Page)
        {
            ViewData["UniqueId"] = Utils.RandomString();
            ViewData["UserBookLists"] = (await _bookList.GetAll()).Select(bl => bl.getViewModel()).ToList();

            if (!string.IsNullOrEmpty(Search)) ViewData["Search"] = Search;
            ViewData["Page"] = Page;

            return View();
        }


        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            BookModel book = await _book.Get(id);
            if (book == null)
            {
                return NotFound();
            }

            ViewData["UserBookLists"] = (await _bookList.GetAll()).Select(bl => bl.getViewModel()).ToList();

            return View(book.getViewModel());
        }

        [Authorize(Roles = AppEnvironment.WRITER_PROFILE)]
        public IActionResult Write(string id)
        {
            return View();
        }
    }
}