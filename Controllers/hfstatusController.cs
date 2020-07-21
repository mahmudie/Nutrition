using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator")]
    [Authorize(Policy = "admin")]
    
    public class hfstatusController : Controller
    {
        private readonly WebNutContext _context;

        public hfstatusController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: hfstatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.LkpHfstatus.ToListAsync());
        }

        // GET: hfstatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lkpHfstatus = await _context.LkpHfstatus.SingleOrDefaultAsync(m => m.HfactiveStatusId == id);
            if (lkpHfstatus == null)
            {
                return NotFound();
            }

            return View(lkpHfstatus);
        }

        // GET: hfstatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: hfstatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HfactiveStatusId,HfstatusDescription")] LkpHfstatus lkpHfstatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lkpHfstatus);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(lkpHfstatus);
        }

        // GET: hfstatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lkpHfstatus = await _context.LkpHfstatus.SingleOrDefaultAsync(m => m.HfactiveStatusId == id);
            if (lkpHfstatus == null)
            {
                return NotFound();
            }
            return View(lkpHfstatus);
        }

        // POST: hfstatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HfactiveStatusId,HfstatusDescription")] LkpHfstatus lkpHfstatus)
        {
            if (id != lkpHfstatus.HfactiveStatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lkpHfstatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LkpHfstatusExists(lkpHfstatus.HfactiveStatusId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(lkpHfstatus);
        }

        // GET: hfstatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lkpHfstatus = await _context.LkpHfstatus.SingleOrDefaultAsync(m => m.HfactiveStatusId == id);
            if (lkpHfstatus == null)
            {
                return NotFound();
            }

            return View(lkpHfstatus);
        }

        // POST: hfstatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lkpHfstatus = await _context.LkpHfstatus.SingleOrDefaultAsync(m => m.HfactiveStatusId == id);
            _context.LkpHfstatus.Remove(lkpHfstatus);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool LkpHfstatusExists(int id)
        {
            return _context.LkpHfstatus.Any(e => e.HfactiveStatusId == id);
        }
    }
}
