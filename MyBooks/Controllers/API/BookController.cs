using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyBooks.Core.Interfaces;
using MyBooks.Models;
using MyBooks.ViewModels;

namespace MyBooks.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBook _book;

        public BookController(ILogger<BookController> logger, IBook book, ICategory category)
        {
            _book = book;
        }

        [HttpGet]
        public async Task<PaginationModelView<BookViewModel>> Get(string search, int page)
        {
            if (page < 0) page = 0;
            else if (page > 0) page--;

            PaginationModelView<BookModel> result = await _book.Search(search, page);

            return new PaginationModelView<BookViewModel>()
            {
                currentPage = result.currentPage,
                elementsPerPage = result.elementsPerPage,
                totalItems = result.totalItems,
                items = result.items.Select(b => b.getViewModel()).ToList()
            };
        }
    }
}