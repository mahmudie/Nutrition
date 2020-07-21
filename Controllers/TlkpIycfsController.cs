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
    
    public class TlkpIycfsController : Controller
    {
        private readonly WebNutContext _context;

        public TlkpIycfsController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TlkpIycfs
        public async Task<IActionResult> Index()
        {
            return View(await _context.TlkpIycf.ToListAsync());
        }


        // GET: TlkpIycfs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TlkpIycfs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Iycfid,Active,CauseConsultation,CauseShortName")] TlkpIycf tlkpIycf)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tlkpIycf);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tlkpIycf);
        }

        // GET: TlkpIycfs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpIycf = await _context.TlkpIycf.SingleOrDefaultAsync(m => m.Iycfid == id);
            if (tlkpIycf == null)
            {
                return NotFound();
            }
            return View(tlkpIycf);
        }

        // POST: TlkpIycfs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Iycfid,Active,CauseConsultation,CauseShortName")] TlkpIycf tlkpIycf)
        {
            if (id != tlkpIycf.Iycfid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tlkpIycf);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TlkpIycfExists(tlkpIycf.Iycfid))
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
            return View(tlkpIycf);
        }

        // GET: TlkpIycfs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpIycf = await _context.TlkpIycf.SingleOrDefaultAsync(m => m.Iycfid == id);
            if (tlkpIycf == null)
            {
                return NotFound();
            }

            return View(tlkpIycf);
        }

        // POST: TlkpIycfs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tlkpIycf = await _context.TlkpIycf.SingleOrDefaultAsync(m => m.Iycfid == id);
            _context.TlkpIycf.Remove(tlkpIycf);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TlkpIycfExists(int id)
        {
            return _context.TlkpIycf.Any(e => e.Iycfid == id);
        }
    }
}
