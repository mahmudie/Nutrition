using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DataSystem.Controllers
{
    [Authorize(Policy = "admin")]

    public class DistrictsController : Controller
    {
        private readonly WebNutContext _context;

        public DistrictsController(WebNutContext context)
        {
            _context = context;
        }

        // GET: Districts
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Districts.Include(d => d.ProvCodeNavigation);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Districts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var districts = await _context.Districts.SingleOrDefaultAsync(m => m.DistCode == id);
            if (districts == null)
            {
                return NotFound();
            }

            return View(districts);
        }

        // GET: Districts/Create
        public IActionResult Create()
        {
            var items = _context.Provinces.Select(s => new
            {
                ProvCode = s.ProvCode,
                description = string.Format("{0} - {1}", s.ProvCode, s.ProvName)
            });
            ViewData["ProvCode"] = new SelectList(items, "ProvCode", "description");
            return View();
        }

        // POST: Districts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DistCode,DistName,DistNameDari,DistNamePashto,ProvCode")] Districts districts)
        {
            if (ModelState.IsValid)
            {
                districts.CreatedDate = DateTime.Now;
                _context.Add(districts);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var items = _context.Provinces.Select(s => new
            {
                ProvCode = s.ProvCode,
                description = string.Format("{0} - {1}", s.ProvCode, s.ProvName)
            });
            ViewData["ProvCode"] = new SelectList(items, "ProvCode", "description");
            return View(districts);
        }

        // GET: Districts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var districts = await _context.Districts.SingleOrDefaultAsync(m => m.DistCode == id);
            if (districts == null)
            {
                return NotFound();
            }
            var items = _context.Provinces.Select(s => new
            {
                ProvCode = s.ProvCode,
                description = string.Format("{0} - {1}", s.ProvCode, s.ProvName)
            });
            ViewData["ProvCode"] = new SelectList(items, "ProvCode", "description");
            return View(districts);
        }

        // POST: Districts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("DistCode,CreatedDate,DistName,DistNameDari,DistNamePashto,ProvCode,CreatedDate")] Districts districts)
        {
            if (id != districts.DistCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(districts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DistrictsExists(districts.DistCode))
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
            var items = _context.Provinces.Select(s => new
            {
                ProvCode = s.ProvCode,
                description = string.Format("{0} - {1}", s.ProvCode, s.ProvName)
            });
            ViewData["ProvCode"] = new SelectList(items, "ProvCode", "description");
            return View(districts);
        }

        // GET: Districts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var districts = await _context.Districts.SingleOrDefaultAsync(m => m.DistCode == id);
            if (districts == null)
            {
                return NotFound();
            }

            return View(districts);
        }

        // POST: Districts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var districts = await _context.Districts.SingleOrDefaultAsync(m => m.DistCode == id);
            _context.Districts.Remove(districts);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool DistrictsExists(string id)
        {
            return _context.Districts.Any(e => e.DistCode == id);
        }
    }
}
