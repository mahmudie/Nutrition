using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "dataentry,administrator,pnd,unicef")]
    public class ERRImamservicesController : Controller
    {
        private readonly WebNutContext _context;

        public ERRImamservicesController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TblMns
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.EmrImamServices.Include(t => t.tlkpEmrIndicators).Include(t => t.Ernmr);
            return View(await myDbContext.ToListAsync());
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Create(int ernmrid)
        {
            int[] query = _context.EmrImamServices.Where(m => m.ErnmrId == ernmrid).Select(m => m.IndicatorId).ToArray();
            int [] mns = _context.TlkpEmrIndicators.Where(m => m.Type.Equals(2) && !query.Contains(m.IndicatorId)).Select(m=>m.IndicatorId).ToArray();
            foreach (int a in mns)
            {
                EmrImamServices item = new EmrImamServices();
                item.UserName=User.Identity.Name;
                item.ErnmrId = ernmrid;
                item.UpdateDate = DateTime.Now;
                item.IndicatorId = a;
                
                _context.EmrImamServices.Add(item);
            }
            _context.SaveChanges();

            return Ok();
        }
        [Authorize(Roles = "dataentry,administrator")]
        public IActionResult Edit(int? id, int ernmrid)
        {
            if (id == null || ernmrid == 0)
            {
                return NotFound();
            }
            ViewData["OutServices"] = new SelectList(_context.TlkpEmrIndicators.Where(m => m.IndicatorId == id), "IndicatorId", "IndicatorName");
            return View();
        }
    }
}
