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

    public class FeedbackController : Controller
    {
        private readonly WebNutContext _context;

        public FeedbackController(WebNutContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "dataentry,administrator")]

        public async Task<IActionResult> Index(string nmrid)
        {
            if (nmrid == null)
            {
                return NotFound("id not passed");
            }
            var item = await _context.Nmr.Where(m => m.Nmrid.Equals(nmrid)).SingleOrDefaultAsync();
            if (item == null)
            {
                return NotFound();
            }
            var model = new Feedback();
            model.Nmrid = item.Nmrid;
            model.items = _context.Feedback.Where(m => m.Nmrid.Equals(nmrid)).ToList();
            return View(model);
        }
        [Authorize(Roles = "dataentry,administrator")]

        public async Task<IActionResult> List(string nmrid)
        {
            if (nmrid == null)
            {
                return NotFound("id not passed");
            }
            var item = await _context.Nmr.Where(m => m.Nmrid.Equals(nmrid)).SingleOrDefaultAsync();
            if (item == null)
            {
                return NotFound();
            }
            var model = new Feedback();
            model.Nmrid = item.Nmrid;
            model.items = _context.Feedback.Where(m => m.Nmrid.Equals(nmrid)).ToList();
            return View(model);
        }
        [Authorize(Roles = "dataentry,administrator")]
        public IActionResult Create(string id)
        {
            if(id==null){
                return BadRequest();
                
            }
            ViewBag.Nmrid=id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry,administrator")]

        public async Task<IActionResult> Create([Bind("Nmrid,Message")] Feedback item)
        {
            if (ModelState.IsValid)
            {

                var user=User.Identity.Name;
                var model = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == item.Nmrid);
                item.CommentedBy = user;
                item.CommentDate = DateTime.Now;
                if (User.IsInRole("dataentry"))
                {
                    model.StatusId = 5;
                }
                else
                {
                    model.StatusId = 6;
                }

                _context.Update(model);
                await _context.SaveChangesAsync();
                _context.Add(item);
                await _context.SaveChangesAsync();
                if (User.IsInRole("administrator"))
                {
                    return RedirectToAction("list", new { nmrid = item.Nmrid });
                }
                return RedirectToAction("Index", new { nmrid = item.Nmrid });
            }
            return View();
        }

        [Authorize(Roles = "dataentry,administrator")]
        public async Task<IActionResult> Edit(int id)
        {

            var user = User.Identity.Name;
            var item = await _context.Feedback.SingleOrDefaultAsync(m => m.Id == id && m.CommentedBy.Equals(user));
            if (item == null)
            {
                return NotFound();
            }
            if(user!=item.CommentedBy){
                ViewBag.res=1;
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry,administrator")]
        public async Task<IActionResult> Edit(int id, [Bind] Feedback item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var model2 = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == item.Nmrid);
                var model = _context.Feedback.Where(m => m.Id.Equals(id)).SingleOrDefault();
                if(User.Identity.Name == model.CommentedBy){
                try
                {
                        model.CommentedBy = User.Identity.Name;
                        model.Message = item.Message;
                        if (User.IsInRole("dataentry"))
                        {
                            model2.StatusId = 5;
                        }
                        else
                        {
                            model2.StatusId = 6;
                        }
                        _context.Update(model2);
                       await _context.SaveChangesAsync();
                        _context.Update(model);
                        await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DistrictsExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if(User.IsInRole("administrator")){
                    return RedirectToAction("reports","nmr", new { nmrid = model.Nmrid });
                }
                return RedirectToAction("Index", new { nmrid = model.Nmrid });
                }
            }
            return View(item);
        }

        [Authorize(Roles = "dataentry,administrator")]
        public async Task<IActionResult> res(int id)
        {


            var item = await _context.Feedback.SingleOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            if(User.Identity.Name == item.CommentedBy){
                return RedirectToAction("edit", new { id = item.Id });
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry,administrator")]

        public async Task<IActionResult> res(int id, [Bind] Feedback item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var model = _context.Feedback.Where(m => m.Id.Equals(id)).SingleOrDefault();
                var nmr =  _context.Nmr.Where(m =>m.Nmrid==model.Nmrid).SingleOrDefault();
                if(!User.IsInRole("administrator")){
                    if(nmr.UserName!=User.Identity.Name){
                        return BadRequest();
                    }
                }
                try
                {
                    model.CommentedBy = User.Identity.Name;
                    model.Message = item.Message;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DistrictsExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if(!User.IsInRole("administrator")){
                    return RedirectToAction("index", new { nmrid = model.Nmrid });
                }
                return RedirectToAction("reports","nmr", new { nmrid = model.Nmrid });
            }
            return View(item);
        }

        // GET: Districts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = _context.Feedback.Where(m => m.CommentedBy.Equals(User.Identity.Name) && m.Id == id).SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }
            _context.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { nmrid = item.Nmrid });
        }
        private bool DistrictsExists(int id)
        {
            return _context.Feedback.Any(e => e.Id == id);
        }
    }
}
