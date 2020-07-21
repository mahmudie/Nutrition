
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

    public class TlkpMnsController : Controller
    {
        private readonly WebNutContext _context;

        public TlkpMnsController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TlkpMns
        public async Task<IActionResult> Index()
        {
            return View(await _context.TlkpMn.ToListAsync());
        }
        // GET: TlkpMns/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TlkpMns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mnid,Active,Mnitems")] TlkpMn tlkpMn)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tlkpMn);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tlkpMn);
        }

        // GET: TlkpMns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpMn = await _context.TlkpMn.SingleOrDefaultAsync(m => m.Mnid == id);
            if (tlkpMn == null)
            {
                return NotFound();
            }
            return View(tlkpMn);
        }

        // POST: TlkpMns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Mnid,Active,Mnitems")] TlkpMn tlkpMn)
        {
            if (id != tlkpMn.Mnid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tlkpMn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TlkpMnExists(tlkpMn.Mnid))
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
            return View(tlkpMn);
        }

        // GET: TlkpMns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpMn = await _context.TlkpMn.SingleOrDefaultAsync(m => m.Mnid == id);
            if (tlkpMn == null)
            {
                return NotFound();
            }

            return View(tlkpMn);
        }

        // POST: TlkpMns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tlkpMn = await _context.TlkpMn.SingleOrDefaultAsync(m => m.Mnid == id);
            _context.TlkpMn.Remove(tlkpMn);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TlkpMnExists(int id)
        {
            return _context.TlkpMn.Any(e => e.Mnid == id);
        }
    }
}
