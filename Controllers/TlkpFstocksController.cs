
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
    
    public class TlkpFstocksController : Controller
    {
        private readonly WebNutContext _context;

        public TlkpFstocksController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TlkpFstocks
        public async Task<IActionResult> Index()
        {
            return View(await _context.TlkpFstock.ToListAsync());
        }


        // GET: TlkpFstocks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TlkpFstocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StockId,Active,DistAmountKg,Item,Buffer,Zarib")] TlkpFstock tlkpFstock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tlkpFstock);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tlkpFstock);
        }

        // GET: TlkpFstocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpFstock = await _context.TlkpFstock.SingleOrDefaultAsync(m => m.StockId == id);
            if (tlkpFstock == null)
            {
                return NotFound();
            }
            return View(tlkpFstock);
        }

        // POST: TlkpFstocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockId,Active,DistAmountKg,Item,Buffer,Zarib")] TlkpFstock tlkpFstock)
        {
            if (id != tlkpFstock.StockId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tlkpFstock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TlkpFstockExists(tlkpFstock.StockId))
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
            return View(tlkpFstock);
        }

        // GET: TlkpFstocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tlkpFstock = await _context.TlkpFstock.SingleOrDefaultAsync(m => m.StockId == id);
            if (tlkpFstock == null)
            {
                return NotFound();
            }

            return View(tlkpFstock);
        }

        // POST: TlkpFstocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tlkpFstock = await _context.TlkpFstock.SingleOrDefaultAsync(m => m.StockId == id);
            _context.TlkpFstock.Remove(tlkpFstock);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TlkpFstockExists(int id)
        {
            return _context.TlkpFstock.Any(e => e.StockId == id);
        }
    }
}
