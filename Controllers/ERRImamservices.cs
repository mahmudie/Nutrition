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
    [Route("api/errimamservices")]
    [Authorize(Roles = "dataentry,administrator,pnd,unicef")]
    public class ERRImamservices : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ERRImamservices(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        [Authorize(Roles = "administrator")]
        [HttpGet("admin/{ernmrid}")]
        public async Task<IActionResult> GetAdmin([FromRoute]int ernmrid)
        {
            if (ernmrid == 0) { return BadRequest(); }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<EmrImamServices> query = _context.EmrImamServices.Where(m => m.ErnmrId == ernmrid);
            if (user.TenantId != 1)
            {
                query = _context.EmrImamServices.Where(m => m.ErnmrId == ernmrid && m.Ernmr.Tenant.Equals(user.TenantId));
            }
            var model = query
            .Select(m => new EmrImamServicesDto()
            {
                IndicatorId = m.IndicatorId,
                ErnmrId = m.ErnmrId,
                Male = m.Male,
                Female = m.Female,
                Cures=m.Cures,
                Deaths=m.Deaths,
                Defaulters=m.Defaulters,
                Referouts=m.Referouts,
                UpdateDate = m.UpdateDate,
                UserName = user.UserName,
                IndicatorName = m.tlkpEmrIndicators.IndicatorName
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
            var model = _context.EmrImamServices.Where(m => m.ErnmrId == ernmrid).Select(m => new EmrImamServicesDto()
            {
                IndicatorId = m.IndicatorId,
                ErnmrId = m.ErnmrId,
                Male = m.Male,
                Female = m.Female,
                Cures = m.Cures,
                Deaths = m.Deaths,
                Defaulters = m.Defaulters,
                Referouts = m.Referouts,
                UpdateDate = m.UpdateDate,
                UserName = user,
                IndicatorName = m.tlkpEmrIndicators.IndicatorName
            });
            if (model == null) { return BadRequest(); }
            return Ok(model);

        }

        // PUT api/values/5
        [HttpPut("{id}ernmrid={ernmrid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Put([FromRoute] int id, [FromRoute]int ernmrid, [FromBody] EmrImamServices item)
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

            //if (report.UserName != user) { return Unauthorized(); }

            report.UpdateDate = update;
            var indicat = _context.EmrImamServices.Where(m => m.ErnmrId == ernmrid && m.IndicatorId == id).SingleOrDefault();
            indicat.Male = item.Male;
            indicat.Female = item.Female;
            indicat.Cures = item.Cures;
            indicat.Deaths = item.Deaths;
            indicat.Defaulters = item.Defaulters;
            indicat.Referouts = item.Referouts;
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

            EmrImamServices item = await _context.EmrImamServices.Where(m => m.IndicatorId == id && m.ErnmrId == ernmrid).Include(m => m.Ernmr).SingleOrDefaultAsync();
            if (item == null)
            {
                return NotFound();
            }

            var update = DateTime.Now;
            if (item.Ernmr == null) { return BadRequest(); }
            var user = User.Identity.Name;
            if (item.UserName != user) { return Unauthorized(); }
            item.UpdateDate = update;
            _context.EmrImamServices.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        private bool Exists(int id, int ernmrid)
        {
            return _context.EmrImamServices.Any(e => e.IndicatorId == id && e.ErnmrId == ernmrid);
        }

    }
}
