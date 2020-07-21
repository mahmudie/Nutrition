using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator")]
    public class StatusController : Controller
    {
        private readonly WebNutContext _context;

        public StatusController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: Status
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblkpStatus.ToListAsync());
        }


        // GET: Status/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Status/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusId,StatusDescription")] TblkpStatus tblkpStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblkpStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tblkpStatus);
        }

        // GET: Status/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblkpStatus = await _context.TblkpStatus.SingleOrDefaultAsync(m => m.StatusId == id);
            if (tblkpStatus == null)
            {
                return NotFound();
            }
            return View(tblkpStatus);
        }

        // POST: Status/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StatusId,StatusDescription")] TblkpStatus tblkpStatus)
        {
            if (id != tblkpStatus.StatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblkpStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblkpStatusExists(tblkpStatus.StatusId))
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
            return View(tblkpStatus);
        }

        // GET: Status/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblkpStatus = await _context.TblkpStatus.SingleOrDefaultAsync(m => m.StatusId == id);
            if (tblkpStatus == null)
            {
                return NotFound();
            }

            return View(tblkpStatus);
        }

        // POST: Status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblkpStatus = await _context.TblkpStatus.SingleOrDefaultAsync(m => m.StatusId == id);
            _context.TblkpStatus.Remove(tblkpStatus);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TblkpStatusExists(int id)
        {
            return _context.TblkpStatus.Any(e => e.StatusId == id);
        }
    }
}
