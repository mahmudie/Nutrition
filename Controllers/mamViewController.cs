using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DataSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace DataSystem.Controllers
{
    [Authorize(Roles="dataentry")]
    public class mamViewController : Controller
    {
        private readonly WebNutContext _context;

        public mamViewController(WebNutContext context)
        {
            _context = context;
        }
        public IActionResult Create(string nmrid)
        {
            if (nmrid == null)
            {
                return NotFound();
            }
            var nmr = _context.Nmr.SingleOrDefault(m => m.Nmrid == nmrid);
            if (nmr == null)
            {
                return NotFound();
            }
            var user = User.Identity.Name;
            if (nmr.UserName != user)
            {
                return Unauthorized();
            }

            int[] selected = _context.TblMam.Where(m => m.Nmrid == nmrid).Select(m => m.Mamid).ToArray();
            int[] valid = _context.TlkpSfp.Where(m => m.Active.Equals(true) && !m.AgeGroup.ToLower().Contains("total") && !selected.Contains(m.Sfpid)).Select(m => m.Sfpid).ToArray();
            foreach (var id in valid)
            {
                TblMam item = new TblMam();
                item.Nmrid = nmrid;
                item.Mamid = id;
                item.UserName=user;
                _context.TblMam.Add(item);
            }
            int[] query = _context.TblFstock.Where(m => m.Nmrid == nmrid).Select(m => m.StockId).ToArray();
            int[] model = _context.TlkpFstock.Where(m => m.Active.Equals(true) && !query.Contains(m.StockId)).Select(m => m.StockId).ToArray();
            foreach (var id in model)
            {
                TblFstock item = new TblFstock();
                item.Nmrid = nmrid;
                item.StockId = id;
                item.UserName=user;
                _context.TblFstock.Add(item);
            }
            _context.SaveChanges();
            return Ok();
        }

        public IActionResult Edit(int? id, string nmrid)
        {
            if (id == null || nmrid == null)
            {
                return NotFound();
            }
            ViewData["AgeGroup"] = new SelectList(_context.TlkpSfp.Where(m => m.Sfpid == id), "Sfpid", "AgeGroup");
            return View();
        }


        public IActionResult EditStock(int? id, string nmrid)
        {
            if (id == null || nmrid == null)
            {
                return NotFound();
            }
            return View();
        }



    }
}
