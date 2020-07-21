using Microsoft.AspNetCore.Mvc;

namespace DataSystem.Controllers
{
    public class ManualController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult adminman()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }
    }
}