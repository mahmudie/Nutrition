using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DataSystem.Controllers
{
    [Authorize(Roles="administrator")]
    [Authorize(Policy = "admin")]
    
    public class FacilityTypesController : Controller
    {
        private readonly WebNutContext _context;

        public FacilityTypesController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: FacilityTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.FacilityTypes.ToListAsync());
        }

        // GET: FacilityTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilityTypes = await _context.FacilityTypes.SingleOrDefaultAsync(m => m.FacTypeCode == id);
            if (facilityTypes == null)
            {
                return NotFound();
            }

            return View(facilityTypes);
        }

        // GET: FacilityTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FacilityTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacTypeCode,FacType,FacTypeCatCode,FacTypeDari,FacTypePashto,TypeAbbrv")] FacilityTypes facilityTypes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(facilityTypes);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(facilityTypes);
        }

        // GET: FacilityTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilityTypes = await _context.FacilityTypes.SingleOrDefaultAsync(m => m.FacTypeCode == id);
            if (facilityTypes == null)
            {
                return NotFound();
            }
            return View(facilityTypes);
        }

        // POST: FacilityTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacTypeCode,FacType,FacTypeCatCode,FacTypeDari,FacTypePashto,TypeAbbrv")] FacilityTypes facilityTypes)
        {
            if (id != facilityTypes.FacTypeCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facilityTypes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityTypesExists(facilityTypes.FacTypeCode))
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
            return View(facilityTypes);
        }

        // GET: FacilityTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilityTypes = await _context.FacilityTypes.SingleOrDefaultAsync(m => m.FacTypeCode == id);
            if (facilityTypes == null)
            {
                return NotFound();
            }

            return View(facilityTypes);
        }

        // POST: FacilityTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facilityTypes = await _context.FacilityTypes.SingleOrDefaultAsync(m => m.FacTypeCode == id);
            _context.FacilityTypes.Remove(facilityTypes);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool FacilityTypesExists(int id)
        {
            return _context.FacilityTypes.Any(e => e.FacTypeCode == id);
        }
    }
}
