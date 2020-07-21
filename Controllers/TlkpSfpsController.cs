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
    

    public class TlkpSfpsController : Controller
    {
        private readonly WebNutContext _context;

        public TlkpSfpsController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TlkpSfps
        public async Task<IActionResult> Index()
        {
            return View(await _context.TlkpSfp.ToListAsync());
        }



        // GET: TlkpSfps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TlkpSfps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Sfpid,Active,AgeGroup")] TlkpSfp tlkpSfp)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tlkpSfp);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tlkpSfp);
        }

        // GET: TlkpSfps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpSfp = await _context.TlkpSfp.SingleOrDefaultAsync(m => m.Sfpid == id);
            if (tlkpSfp == null)
            {
                return NotFound();
            }
            return View(tlkpSfp);
        }

        // POST: TlkpSfps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Sfpid,Active,AgeGroup")] TlkpSfp tlkpSfp)
        {
            if (id != tlkpSfp.Sfpid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tlkpSfp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TlkpSfpExists(tlkpSfp.Sfpid))
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
            return View(tlkpSfp);
        }

        // GET: TlkpSfps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpSfp = await _context.TlkpSfp.SingleOrDefaultAsync(m => m.Sfpid == id);
            if (tlkpSfp == null)
            {
                return NotFound();
            }

            return View(tlkpSfp);
        }

        // POST: TlkpSfps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tlkpSfp = await _context.TlkpSfp.SingleOrDefaultAsync(m => m.Sfpid == id);
            _context.TlkpSfp.Remove(tlkpSfp);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TlkpSfpExists(int id)
        {
            return _context.TlkpSfp.Any(e => e.Sfpid == id);
        }
    }
}
