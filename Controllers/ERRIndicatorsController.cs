using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "dataentry,administrator,unicef,pnd")]
    public class ERRIndicatorsController : Controller
    {
        private readonly WebNutContext _context;

        public ERRIndicatorsController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TblMns
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.EmrIndicators.Include(t => t.lkpEmrIndicators).Include(t => t.Ernmr);
            return View(await myDbContext.ToListAsync());
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Create(int ernmrid)
        {
            int[] query = _context.EmrIndicators.Where(m => m.ErnmrId == ernmrid).Select(m => m.IndicatorId).ToArray();
            int [] mns = _context.TlkpEmrIndicators.Where(m => m.Type.Equals(1) && !query.Contains(m.IndicatorId)).Select(m=>m.IndicatorId).ToArray();
            foreach (int a in mns)
            {
                EmrIndicators item = new EmrIndicators();
                item.UserName=User.Identity.Name;
                item.ErnmrId = ernmrid;
                item.UpdateDate = DateTime.Now;
                item.IndicatorId = a;
                
                _context.EmrIndicators.Add(item);
            }
            _context.SaveChanges();

            return Ok();
        }

    }
}
