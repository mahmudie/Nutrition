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
  
    public class ImplementerController : Controller
    {

        private readonly WebNutContext _context;

        public ImplementerController(WebNutContext context)
        {
            _context = context;
            
        }

        // GET: Implementer
        public async Task<IActionResult> Index()
        {
            return View(await _context.Implementers.ToListAsync());
        }


        // GET: Implementer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Implementer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImpCode,AfghanistanAddress,ImpAcronym,ImpName,ImpNameDari,ImpNamePashto,ImpStatus,OtherAddress,RegistrationDate")] Implementers implementers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(implementers);
                try
                {
                   await _context.SaveChangesAsync();
                }
                catch (DbUpdateException )
                {
                    ModelState.AddModelError(String.Empty, "Something went wrong maybe this id already exists.");             
                    return View(implementers);     
                }
                return RedirectToAction("Index");
            }
            return View(implementers);
        }

        // GET: Implementer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var implementers = await _context.Implementers.SingleOrDefaultAsync(m => m.ImpCode == id);
            if (implementers == null)
            {
                return NotFound();
            }
            return View(implementers);
        }

        // POST: Implementer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImpCode,AfghanistanAddress,ImpAcronym,ImpName,ImpNameDari,ImpNamePashto,ImpStatus,OtherAddress,RegistrationDate,IsScm")] Implementers implementers)
        {
            if (id != implementers.ImpCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(implementers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImplementersExists(implementers.ImpCode))
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
            return View(implementers);
        }

        // GET: Implementer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var implementers = await _context.Implementers.SingleOrDefaultAsync(m => m.ImpCode == id);
            if (implementers == null)
            {
                return NotFound();
            }

            return View(implementers);
        }

        // POST: Implementer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var implementers = await _context.Implementers.SingleOrDefaultAsync(m => m.ImpCode == id);
            _context.Implementers.Remove(implementers);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ImplementersExists(int id)
        {
            return _context.Implementers.Any(e => e.ImpCode == id);
        }
    }
}
