using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using DataSystem.Models.ViewModels;
using System;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace DataSystem.Controllers
{
    [Produces("application/json")]
    [Route("api/opdmam")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "dataentry,administrator")]
    public class opdmamController : Controller
    {
        private readonly WebNutContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public opdmamController(WebNutContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;

        }
        [Authorize(Roles = "administrator")]
        [HttpGet("admin/{nmrid}")]
        public async Task<IEnumerable<mamVM>> adminGet([FromRoute] string nmrid)
        {
            if (nmrid == null)
            {
                NotFound(new { Error = "not found" });
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<TblMam> query = _context.TblMam.Where(m => m.Nmrid == nmrid);
            if (user.TenantId != 1)
            {
                query = _context.TblMam.Where(m => m.Nmrid == nmrid && m.Nmr.Tenant.Equals(user.TenantId));
            }
            var items = query.Include(m => m.Mam).AsNoTracking().ToList();
            var data = _mapper.Map<IEnumerable<TblMam>, IEnumerable<mamVM>>(items);
            return data;
        }

        [HttpPut("partial/{nmrid}")]
        public async Task<IActionResult> partialForm([FromRoute] string nmrid, [FromBody] opdViewMNodel item)
        {
            if (nmrid == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = User.Identity.Name;
            var nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == nmrid);
            if (nmr == null)
            {
                return BadRequest();
            }
            if (nmr.UserName != user)
            {
                return Unauthorized();
            }
            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            nmr.UpdateDate = DateTime.Now;
            nmr.StatusId = 2;
            nmr.SfpAls = item.SfpAls;
            nmr.SfpAwg = item.SfpAwg;

            try
            {
                _context.Update(nmr);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // GET: api/Form/5
        [HttpGet("find/{nmrid}")]
        public IEnumerable<mamVM> search([FromRoute] string nmrid)
        {
            if (nmrid == null) { NotFound(new { Error = "not found" }); }
            var name = User.Identity.Name;
            var items = _context.TblMam.Where(m => m.Nmrid == nmrid && m.UserName == name).Include(m => m.Mam).AsNoTracking().ToList();
            var data = _mapper.Map<IEnumerable<TblMam>, IEnumerable<mamVM>>(items);
            return data;
        }


        // PUT: api/Form/5
        [HttpPut("{id}nmrid={nmrid}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromRoute] string nmrid, [FromBody] TblMam item)
        {
            if (nmrid == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.Mamid)
            {
                return BadRequest();
            }
            var nmr = await _context.Nmr.Where(m => m.Nmrid == nmrid).SingleOrDefaultAsync();
            if (nmr == null)
            {
                return NotFound();
            }
            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            if (User.Identity.Name == nmr.UserName)
            {
                nmr.UpdateDate = DateTime.Now;
                nmr.StatusId = 2;
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
            else { return Unauthorized(); }

        }

        // DELETE: api/Form/5
        [HttpDelete("{id}id2={id2}")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromRoute]string id2)
        {
            if (id2 == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var item = await _context.TblMam.Where(m => m.Mamid == id && m.Nmrid == id2).SingleOrDefaultAsync();
            var report = await _context.Nmr.Where(m => m.Nmrid == id2).SingleOrDefaultAsync();
            if (item == null || report == null)
            {
                return NotFound();
            }
            if (report.StatusId == 3 || report.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            var user = User.Identity.Name;
            if (user == report.UserName)
            {
                item.Nmr.StatusId = 2;
                item.Nmr.UpdateDate = DateTime.Now;
                _context.TblMam.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(item);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("stock/{nmrid}")]
        public IEnumerable<fstockViewModel> stockList([FromRoute] string nmrid)
        {
            if (nmrid == null) { NotFound(new { Error = "not found" }); }
            var user = User.Identity.Name;
            var model = _context.TblFstock.Where(m => m.Nmrid == nmrid && m.UserName == user).Include(m => m.Stock).AsNoTracking().ToList();
            var data = _mapper.Map<IEnumerable<TblFstock>, IEnumerable<fstockViewModel>>(model);

            return data;
        }
        [HttpPut("stock/{id}nmrid={nmrid}")]
        public async Task<IActionResult> putStock([FromRoute] int id, [FromRoute] string nmrid, [FromBody] TblFstock item)
        {
            if (nmrid == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.StockId)
            {
                return BadRequest();
            }
            var nmr = await _context.Nmr.Where(m => m.Nmrid == nmrid).SingleOrDefaultAsync();
            if (nmr == null)
            {
                return NotFound();
            }
            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            if (User.Identity.Name == item.UserName)
            {
                nmr.StatusId = 2;
                nmr.UpdateDate = DateTime.Now;
                _context.Entry(item).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(id, nmrid))
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
            else { return BadRequest(); }


        }

        [HttpDelete("stock/{id}nmrid={nmrid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStock([FromRoute] int id, [FromRoute]string nmrid)
        {
            if (nmrid == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Nmr nmr = await _context.Nmr.Where(m => m.Nmrid == nmrid).SingleOrDefaultAsync();
            TblFstock item = await _context.TblFstock.SingleOrDefaultAsync(m => m.StockId == id && m.Nmrid == nmrid);
            if (item == null || nmr == null)
            {
                return NotFound();
            }
            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            if (nmr.UserName == User.Identity.Name)
            {
                nmr.UpdateDate = DateTime.Now;
                nmr.StatusId = 2;
                _context.TblFstock.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(item);
            }
            else { return Unauthorized(); }
        }

        [HttpGet("adminstock/{nmrid}")]
        [Authorize(Roles = "administrator")]
        public async Task<IEnumerable<fstockViewModel>> adminstockList([FromRoute] string nmrid)
        {
            if (nmrid == null)
            {
                NotFound(new { Error = "not found" });
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<TblFstock> query = _context.TblFstock.Where(m => m.Nmrid == nmrid);
            if (user.TenantId != 1)
            {
                query = _context.TblFstock.Where(m => m.Nmrid == nmrid && m.Nmr.Tenant.Equals(user.TenantId));
            }
            var model = query.Include(m => m.Stock).AsNoTracking().ToList();
            var data = _mapper.Map<IEnumerable<TblFstock>, IEnumerable<fstockViewModel>>(model);

            return data;
        }

        [HttpGet("total/{id}nmrid={nmrid}")]
        public int beginning(string nmrid, int id)
        {
            string[] words = nmrid.Split('-');
            int facid = Convert.ToInt32(words[0]);
            int year = Convert.ToInt32(words[1]);
            int month = Convert.ToInt32(words[2]) - 1;
            if (month == 0)
            {
                month = 12;
                year = year - 1;
            }
            string nid = String.Format("{0}-{1}-{2}", facid, year, month);
            var valid = _context.Nmr.Where(m => m.Nmrid == nmrid).SingleOrDefault();
            if (valid == null)
            {
                return 0;
            }
            var item = _context.TblMam.Where(m => m.Nmrid == nid && m.Mamid == id).SingleOrDefault();
            if (item == null)
            {
                return 0;
            }
            int ending = (item.TMale.GetValueOrDefault() + item.TFemale.GetValueOrDefault() + item.Totalbegin.GetValueOrDefault()
            + item.ReferIn.GetValueOrDefault())
            - (item.Cured.GetValueOrDefault() + item.NonCured.GetValueOrDefault() + item.Deaths.GetValueOrDefault() + item.Defaulters.GetValueOrDefault() + item.Transfers.GetValueOrDefault());
            return ending;
        }



        private bool Exists(int id, string nmrid)
        {
            return _context.TblMam.Any(e => e.Mamid == id && e.Nmrid == nmrid);
        }
        private bool StockExists(int id, string nmrid)
        {
            return _context.TblFstock.Any(e => e.StockId == id && e.Nmrid == nmrid);
        }
    }
}