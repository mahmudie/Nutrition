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
    [Authorize(Roles = "dataentry,administrator,pnd,unicef")]
    [Produces("application/json")]
    [Route("api/errindicators")]

    public class ERRIndicators : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ERRIndicators(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        [HttpGet("admin/{ernmrid}")]
        public async Task<IActionResult> GetAdmin([FromRoute]int ernmrid)
        {
            if (ernmrid == 0) { return BadRequest(); }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<EmrIndicators> query = _context.EmrIndicators.Where(m => m.ErnmrId == ernmrid);
            if (user.TenantId != 1)
            {
                query = _context.EmrIndicators.Where(m => m.ErnmrId == ernmrid && m.Ernmr.Tenant.Equals(user.TenantId));
            }
            var model = query
            .Select(m => new ERRIndicatorsDto()
            {
                IndicatorId = m.IndicatorId,
                ErnmrId = m.ErnmrId,
                Male = m.Male,
                Female = m.Female,
                UpdateDate = m.UpdateDate,
                UserName = user.UserName,
                IndicatorName = m.lkpEmrIndicators.IndicatorName
            });
            if (model == null) { return BadRequest(); }
            return Ok(model);

        }

        // GET api/values/5
        [HttpGet("{ernmrid}")]
        public IActionResult Get([FromRoute]int ernmrid)
        {
            if (ernmrid == 0) { return BadRequest(); }
            var user = User.Identity.Name;
            var model = _context.EmrIndicators.Where(m => m.ErnmrId == ernmrid).Select(m => new ERRIndicatorsDto()
            {
                IndicatorId = m.IndicatorId,
                ErnmrId = m.ErnmrId,
                Male = m.Male,
                Female = m.Female,
                UpdateDate = m.UpdateDate,
                UserName = user,
                IndicatorName = m.lkpEmrIndicators.IndicatorName
            });
            if (model == null) { return BadRequest(); }
            return Ok(model);

        }

        // PUT api/values/5
        [HttpPut("{id}ernmrid={ernmrid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Put([FromRoute] int id, [FromRoute]int ernmrid, [FromBody] EmrIndicators item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != item.IndicatorId || ernmrid != item.ErnmrId)
            {
                return BadRequest();
            }
            var user = User.Identity.Name;
            var update = DateTime.Now;
            var report = _context.Ernmr.SingleOrDefault(m => m.ErnmrId == ernmrid);
            if (report == null) { return BadRequest(); }
        
            report.UpdateDate = update;
            var indicat = _context.EmrIndicators.Where(m => m.ErnmrId == ernmrid && m.IndicatorId == id).SingleOrDefault();
            indicat.Male = item.Male;
            indicat.Female = item.Female;
            indicat.UpdateDate = update;
            indicat.UserName = user;

            _context.Entry(indicat).State = EntityState.Modified;

            try
            {
                if (report.UserName != user) { return Unauthorized(); }
                _context.Update(indicat);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id, ernmrid))
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

        [HttpDelete("{id}ernmrid={ernmrid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromRoute]int ernmrid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EmrIndicators item = await _context.EmrIndicators.Where(m => m.IndicatorId == id && m.ErnmrId == ernmrid).Include(m => m.Ernmr).SingleOrDefaultAsync();
            if (item == null)
            {
                return NotFound();
            }

            var update = DateTime.Now;
            if (item.Ernmr == null) { return BadRequest(); }
            var user = User.Identity.Name;
            if (item.UserName != user) { return Unauthorized(); }
            item.UpdateDate = update;
            _context.EmrIndicators.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        private bool Exists(int id, int ernmrid)
        {
            return _context.EmrIndicators.Any(e => e.IndicatorId == id && e.ErnmrId == ernmrid);
        }

    }
}
