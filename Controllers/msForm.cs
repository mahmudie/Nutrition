using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Identity;

namespace DataSystem.Controllers
{
    [Produces("application/json")]
    [Route("api/msform")]
    [Authorize(Roles = "dataentry,administrator")]
    public class msForm : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public msForm(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        [Authorize(Roles = "administrator")]
        [HttpGet("admin/{nmrid}")]
        public async Task<IActionResult> GetAdmin([FromRoute]string nmrid)
        {
            if (nmrid == null) { return BadRequest(); }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<TblMn> query = _context.TblMn.Where(m => m.Nmrid == nmrid);
            if (user.TenantId != 1)
            {
                query = _context.TblMn.Where(m => m.Nmrid == nmrid && m.Nmr.Tenant.Equals(user.TenantId));
            }
            var model = query
            .Select(m => new TblmnDto()
            {
                Mnid = m.Mnid,
                Nmrid = m.Nmrid,
                Remarks = m.Remarks,
                refbyCHW = m.refbyCHW,
                chu2f = m.chu2f,
                UpdateDate = m.UpdateDate,
                chu2m = m.chu2m,
                Mnitems = m.Mn.Mnitems
            });
            if (model == null) { return BadRequest(); }
            return Ok(model);

        }

        // GET api/values/5
        [HttpGet("{nmrid}")]
        public IActionResult Get([FromRoute]string nmrid)
        {
            if (nmrid == null) { return BadRequest(); }
            var user = User.Identity.Name;
            var model = _context.TblMn.Where(m => m.Nmrid == nmrid && m.UserName == user).Select(m => new TblmnDto()
            {
                Mnid = m.Mnid,
                Nmrid = m.Nmrid,
                Remarks = m.Remarks,
                refbyCHW = m.refbyCHW,
                chu2f = m.chu2f,
                UpdateDate = m.UpdateDate,
                chu2m = m.chu2m,
                Mnitems = m.Mn.Mnitems
            });
            if (model == null) { return BadRequest(); }
            return Ok(model);

        }

        // PUT api/values/5
        [HttpPut("{id}nmrid={nmrid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Put([FromRoute] int id, [FromRoute]string nmrid, [FromBody] TblMn item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != item.Mnid || nmrid != item.Nmrid)
            {
                return BadRequest();
            }
            var user = User.Identity.Name;
            var update = DateTime.Now;
            var report = _context.Nmr.SingleOrDefault(m => m.Nmrid == nmrid);
            if (report == null) { return BadRequest(); }

            if (report.StatusId == 3 || report.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            if (report.UserName != user) { return Unauthorized(); }

            report.UpdateDate = update;
            report.StatusId = 2;
            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id, nmrid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}nmrid={nmrid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromRoute]string nmrid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TblMn item = await _context.TblMn.Where(m => m.Mnid == id && m.Nmrid == nmrid).Include(m => m.Nmr).SingleOrDefaultAsync();
            if (item == null)
            {
                return NotFound();
            }
            if (item.Nmr.StatusId == 3 || item.Nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }

            var update = DateTime.Now;
            if (item.Nmr == null) { return BadRequest(); }
            var user = User.Identity.Name;
            if (item.Nmr.UserName != user) { return Unauthorized(); }
            item.Nmr.UpdateDate = update;
            item.Nmr.StatusId = 2;
            _context.TblMn.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        private bool Exists(int id, string nmrid)
        {
            return _context.TblMn.Any(e => e.Mnid == id && e.Nmrid == nmrid);
        }

    }
}
