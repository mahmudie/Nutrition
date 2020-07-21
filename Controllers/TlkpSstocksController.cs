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
    

    public class TlkpSstocksController : Controller
    {
        private readonly WebNutContext _context;

        public TlkpSstocksController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TlkpSstocks
        public async Task<IActionResult> Index()
        {
            return View(await _context.TlkpSstock.ToListAsync());
        }


        // GET: TlkpSstocks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TlkpSstocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SstockId,Active,Item,Persachet,Buffer,IPDSAMZarib,OPDSAMZarib,Comments")] TlkpSstock tlkpSstock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tlkpSstock);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tlkpSstock);
        }

        // GET: TlkpSstocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpSstock = await _context.TlkpSstock.SingleOrDefaultAsync(m => m.SstockId == id);
            if (tlkpSstock == null)
            {
                return NotFound();
            }
            return View(tlkpSstock);
        }

        // POST: TlkpSstocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SstockId,Active,Item,Persachet,Buffer,IPDSAMZarib,OPDSAMZarib,Comments")] TlkpSstock tlkpSstock)
        {
            if (id != tlkpSstock.SstockId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tlkpSstock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TlkpSstockExists(tlkpSstock.SstockId))
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
            return View(tlkpSstock);
        }

        // GET: TlkpSstocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpSstock = await _context.TlkpSstock.SingleOrDefaultAsync(m => m.SstockId == id);
            if (tlkpSstock == null)
            {
                return NotFound();
            }

            return View(tlkpSstock);
        }

        // POST: TlkpSstocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tlkpSstock = await _context.TlkpSstock.SingleOrDefaultAsync(m => m.SstockId == id);
            _context.TlkpSstock.Remove(tlkpSstock);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TlkpSstockExists(int id)
        {
            return _context.TlkpSstock.Any(e => e.SstockId == id);
        }
    }
}
