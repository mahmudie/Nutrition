using System;
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

    public class ProvinceController : Controller
    {
        private readonly WebNutContext _context;

        public ProvinceController(WebNutContext context)
        {
            _context = context;
        }

        // GET: Proviences
        public async Task<IActionResult> Index()
        {
            return View(await _context.Provinces.ToListAsync());
        }

        // GET: Proviences/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var provience = await _context.Provinces.SingleOrDefaultAsync(m => m.ProvCode == id);
            if (provience == null)
            {
                return NotFound();
            }

            return View(provience);
        }

        // GET: Proviences/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proviences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProvCode,CreatedDate,ProvName,ProveNameDari,ProveNamePashto,AGHCHOCode,AIMSCode")] Provinces provience)
        {
            if (ModelState.IsValid)
            {
                provience.CreatedDate = DateTime.Now;
                _context.Add(provience);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(provience);
        }

        // GET: Proviences/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var provience = await _context.Provinces.SingleOrDefaultAsync(m => m.ProvCode == id);
            if (provience == null)
            {
                return NotFound();
            }
            return View(provience);
        }

        // POST: Proviences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProvCode,CreatedDate,ProvName,ProveNameDari,ProveNamePashto,AGHCHOCode,AIMSCode")] Provinces provience)
        {
            if (id != provience.ProvCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(provience);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProvienceExists(provience.ProvCode))
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
            return View(provience);
        }

        // GET: Proviences/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var provience = await _context.Provinces.SingleOrDefaultAsync(m => m.ProvCode == id);
            if (provience == null)
            {
                return NotFound();
            }

            return View(provience);
        }

        // POST: Proviences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var provience = await _context.Provinces.SingleOrDefaultAsync(m => m.ProvCode == id);
            _context.Provinces.Remove(provience);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProvienceExists(string id)
        {
            return _context.Provinces.Any(e => e.ProvCode == id);
        }
    }
}
