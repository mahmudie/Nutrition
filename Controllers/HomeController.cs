using DataSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataSystem.Controllers
{
    public class HomeController : Controller
    {
        public readonly WebNutContext _context;

        public HomeController(WebNutContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            string sql = "delete from [dbo].[NMR] where NMRId in (select NMRID from[dbo].[NMR_checkcompleteness] " +
                        " where(IPDSAM_submission+OPDSAM_submission+OPDMAM_submission+MNS_submission+OPDMAM_stock_submission+IPDSAM_stock_submission+OPDSAM_stock_submission)=0)";
            _context.Database.SetCommandTimeout(500);
            if (User.Identity.IsAuthenticated)
            {
                _context.Database.ExecuteSqlCommand(sql);
            }

            if (User.IsInRole("dataentry") && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Nmr");
            }
            else if (User.IsInRole("administrator") && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("adminNmr", "Nmr");
            }
            else if (User.IsInRole("unicef") || User.IsInRole("pnd") && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "scmdash");
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
