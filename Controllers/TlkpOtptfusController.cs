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
    
    public class TlkpOtptfusController : Controller
    {
        private readonly WebNutContext _context;

        public TlkpOtptfusController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TlkpOtptfus
        public async Task<IActionResult> Index()
        {
            return View(await _context.TlkpOtptfu.ToListAsync());
        }


        // GET: TlkpOtptfus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TlkpOtptfus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Otptfuid,Active,AgeGroup")] TlkpOtptfu tlkpOtptfu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tlkpOtptfu);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tlkpOtptfu);
        }

        // GET: TlkpOtptfus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpOtptfu = await _context.TlkpOtptfu.SingleOrDefaultAsync(m => m.Otptfuid == id);
            if (tlkpOtptfu == null)
            {
                return NotFound();
            }
            return View(tlkpOtptfu);
        }

        // POST: TlkpOtptfus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Otptfuid,Active,AgeGroup")] TlkpOtptfu tlkpOtptfu)
        {
            if (id != tlkpOtptfu.Otptfuid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tlkpOtptfu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TlkpOtptfuExists(tlkpOtptfu.Otptfuid))
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
            return View(tlkpOtptfu);
        }

        // GET: TlkpOtptfus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpOtptfu = await _context.TlkpOtptfu.SingleOrDefaultAsync(m => m.Otptfuid == id);
            if (tlkpOtptfu == null)
            {
                return NotFound();
            }

            return View(tlkpOtptfu);
        }

        // POST: TlkpOtptfus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tlkpOtptfu = await _context.TlkpOtptfu.SingleOrDefaultAsync(m => m.Otptfuid == id);
            _context.TlkpOtptfu.Remove(tlkpOtptfu);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TlkpOtptfuExists(int id)
        {
            return _context.TlkpOtptfu.Any(e => e.Otptfuid == id);
        }
    }
}
