using Microsoft.AspNetCore.Mvc;

namespace PlanMateWebApp.Controllers
{
    public class BordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}