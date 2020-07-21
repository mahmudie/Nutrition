using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace DataSystem.Controllers
{
    public class NotehelperController : Controller
    {
        private readonly WebNutContext _context;

        public NotehelperController(WebNutContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> List()
        {
            var notehelpers = _context.Notehelpers;
            return View(await notehelpers.ToListAsync());
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
        [Authorize(Policy = "admin")]
        public IActionResult Create()
        {
            ViewBag.tools = new[] { "Bold", "Italic", "Underline", "StrikeThrough",
                "FontName", "FontSize", "FontColor", "BackgroundColor",
                "LowerCase", "UpperCase","SuperScript", "SubScript", "|",
                "Formats", "Alignments", "OrderedList", "UnorderedList",
                "Outdent", "Indent", "|",
                "CreateTable", "CreateLink", "|", "ClearFormat", "Print",
                "SourceCode", "FullScreen", "|", "Undo", "Redo" };
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notehelpers notehelpers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notehelpers);
                await _context.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(notehelpers);
        }

        // GET: Districts/Edit/5
        [Authorize(Policy = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.tools = new[] { "Bold", "Italic", "Underline", "StrikeThrough",
                "FontName", "FontSize", "FontColor", "BackgroundColor",
                "LowerCase", "UpperCase","SuperScript", "SubScript", "|",
                "Formats", "Alignments", "OrderedList", "UnorderedList",
                "Outdent", "Indent", "|",
                "CreateTable", "CreateLink", "|", "ClearFormat", "Print",
                "SourceCode", "FullScreen", "|", "Undo", "Redo" };
            if (id == 0)
            {
                return NotFound();
            }

            var notehelpers = await _context.Notehelpers.SingleOrDefaultAsync(m => m.Id == id);
            if (notehelpers == null)
            {
                return NotFound();
            }

            return View(notehelpers);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin")]
        public async Task<IActionResult> Edit(int id, Notehelpers notehelpers)
        {
            if (id != notehelpers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notehelpers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DistrictsExists(notehelpers.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("List");
            }

            return View(notehelpers);
        }

        // GET: Districts/Delete/5
        [Authorize(Policy = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var notehelpers = await _context.Notehelpers.SingleOrDefaultAsync(m => m.Id == id);
            if (notehelpers == null)
            {
                return NotFound();
            }

            return View(notehelpers);
        }

        // POST: Districts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notehelpers = await _context.Notehelpers.SingleOrDefaultAsync(m => m.Id == id);
            _context.Notehelpers.Remove(notehelpers);
            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }

        private bool DistrictsExists(int id)
        {
            return _context.Notehelpers.Any(e => e.Id == id);
        }

        public IActionResult tipandnotes()
        {
            var notehelpers = _context.Notehelpers.ToList();
            //return Json({ data = new Notehelpers});
            return Ok(notehelpers);
            //return Json(notehelpers);
        }
    }
}
