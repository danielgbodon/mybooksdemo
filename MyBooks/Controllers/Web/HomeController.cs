using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBooks.Models;
using MyBooks.Core;
using MyBooks.Core.Interfaces;
using System.Globalization;

namespace MyBooks.Controllers.Web
{
    public class HomeController : Controller
    { 
        private readonly IBookList _bookList;        
        private readonly IUser _user;

        public HomeController(IBookList bookList, IUser user)
        {
            _bookList = bookList;
            _user = user;
        }

        public async Task<IActionResult> Index()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            if (AppEnvironment.CurrentUser.Welcomed && !AppEnvironment.CurrentUser.WebTrained)
            {
                ViewData["LoadTraining"] = true;
                AppEnvironment.CurrentUser.WebTrained = true;
                await _user.Edit(AppEnvironment.CurrentUser);
            }

            ViewData["UserBookLists"] = (await _bookList.GetAll(true)).Select(bl => bl.getViewModel()).ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
