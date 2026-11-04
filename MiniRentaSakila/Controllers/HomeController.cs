using Microsoft.AspNetCore.Mvc;
using MiniRentaSakila.Filters;

namespace MiniRentaSakila.Controllers
{
    [Autorizar]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}