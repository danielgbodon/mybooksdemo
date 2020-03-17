using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyBooks.Core;
using MyBooks.Core.Interfaces;
using MyBooks.Models;
using MyBooks.ViewModels;

namespace MyBooks.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookListController : ControllerBase
    {
        private readonly IBookList _bookList;
        private readonly IAntiforgery _antiforgery;
        private readonly IStringLocalizer<BookList> _localizer;

        public BookListController(IBookList bookList, IAntiforgery antiforgery, IStringLocalizer<BookList> localizer)
        {
            _bookList = bookList;
            _antiforgery = antiforgery;
            _localizer = localizer;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ResponseViewModel> Create([FromForm, Bind("Name,Color")]  BookListModel bookList, [FromForm] string BookId)
        {
            bookList.Removable = true;
            bookList.User = AppEnvironment.CurrentUser;
            BookListModel result = await _bookList.Add(bookList);

            if (!string.IsNullOrEmpty(BookId))
            {
                result = await _bookList.AddBookToList(BookId, result.Id);
            }

            if (result == null)
                return new ResponseViewModel()
                {
                    Message = _localizer["CreacionKO"],
                    Code = (int)HttpStatusCode.BadRequest,
                    VerificationToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken
                };

            return new ResponseViewModel()
            {
                Message = _localizer["CreacionOK"],
                Result = result.getViewModel(),
                VerificationToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken
            };
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ResponseViewModel> Edit([FromForm, Bind("Id, Name,Color")]  BookListModel bookList)
        {
            bookList.Removable = true;
            bookList.User = null;
            BookListModel result = await _bookList.Edit(bookList);
            if(result == null)
                return new ResponseViewModel()
                {
                    Message = _localizer["EdicionKO"],
                    Code = (int)HttpStatusCode.BadRequest,
                    VerificationToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken
                };
            

            return new ResponseViewModel()
            {
                Message = _localizer["EdicionOK"],
                Result = result.getViewModel(),
                VerificationToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken
            };
        }

        [HttpPut("reorder")]
        public async Task<ResponseViewModel> Reorder([FromForm] List<int> OrderedIds)
        {
            bool result = await _bookList.Reorder(OrderedIds);
            if (result == false)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await Response.WriteAsync(_localizer["OrdenacionKO"]);
                return null;
            }

            return new ResponseViewModel()
            {
                Message = _localizer["OrdenacionOK"]
            };
        }

        [HttpDelete]
        public async Task<ResponseViewModel> Delete([FromForm]  int id)
        {
            bool result = await _bookList.Delete(id);
            
            if(!result)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await Response.WriteAsync(_localizer["BorradoKO"]);
                return null;
            }

            return new ResponseViewModel()
            {
                Message = _localizer["BorradoOK"]
            };
        }

        [HttpPost("book")]
        [AllowAnonymous]
        public async Task<ResponseViewModel> AddBookToList([FromForm] string BookId, [FromForm] int BookListId)
        {
            BookListModel bookList = await _bookList.AddBookToList(BookId, BookListId);
            if (bookList == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await Response.WriteAsync(_localizer["LibroAnadidoKO"]);
                return null;
            }

            return new ResponseViewModel()
            {
                Message = _localizer["LibroAnadidoOK"],
                Result = bookList.getViewModel()
            };
        }

        [HttpDelete("book")]
        [AllowAnonymous]
        public async Task<ResponseViewModel> DeleteBookToList([FromForm] string BookId, [FromForm] int BookListId)
        {
            BookListModel bookList = await _bookList.DeleteBookToList(BookId, BookListId);
            
            if (bookList == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await Response.WriteAsync(_localizer["LibroBorradoKO"]);
                return null;
            }

            return new ResponseViewModel()
            {
                Message = _localizer["LibroBorradoOK"],
                Result = bookList.getViewModel()
            };
        }
    }
}