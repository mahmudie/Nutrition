using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "dataentry")]
    public class TblMnsController : Controller
    {
        private readonly WebNutContext _context;

        public TblMnsController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TblMns
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.TblMn.Include(t => t.Mn).Include(t => t.Nmr);
            return View(await myDbContext.ToListAsync());
        }
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Create(string nmrid)
        {
            int[] query = _context.TblMn.Where(m => m.Nmrid == nmrid).Select(m => m.Mnid).ToArray();
            int [] mns = _context.TlkpMn.Where(m => m.Active.Equals(true) && !query.Contains(m.Mnid)).Select(m=>m.Mnid).ToArray();
            foreach (int a in mns)
            {
                TblMn item = new TblMn();
                item.UserName=User.Identity.Name;
                item.Nmrid = nmrid;
                item.Mnid = a;
                _context.TblMn.Add(item);
            }
            _context.SaveChanges();

            return Ok();
        }

    }
}
