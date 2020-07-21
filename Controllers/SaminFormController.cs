using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using DataSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using AutoMapper;

namespace DataSystem.Controllers
{
    [Produces("application/json")]
    [Route("api/samin")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "dataentry,administrator")]
    public class FormController : Controller
    {
        private readonly WebNutContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public FormController(WebNutContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        [Authorize(Roles = "administrator")]
        [HttpGet("admin/{nmrid}")]
        public async Task<IEnumerable<SaminDto>> adminGet([FromRoute] string nmrid)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<TblOtptfu> query = _context.TblOtptfu.Where(m => m.Nmrid == nmrid);
            if (user.TenantId != 1)
            {
                query = _context.TblOtptfu.Where(m =>m.Nmrid == nmrid&& m.Nmr.Tenant.Equals(user.TenantId));
            }            
            if (nmrid == null) { BadRequest(); }

            var items = query.Include(m => m.Otptfu).AsNoTracking().ToList();
            var data = _mapper.Map<IEnumerable<TblOtptfu>, IEnumerable<SaminDto>>(items);
            return data;
        }

        // GET: api/Form/5
        [HttpGet("find/{nmrid}")]
        public IEnumerable<SaminDto> search([FromRoute] string nmrid)
        {
            if (nmrid == null) { BadRequest(); }
            var name = User.Identity.Name;
            var items = _context.TblOtptfu.Where(m => m.Nmrid == nmrid && m.UserName == name).Include(m => m.Otptfu).AsNoTracking().ToList();
            var data = _mapper.Map<IEnumerable<TblOtptfu>, IEnumerable<SaminDto>>(items);
            return data;
        }


        // PUT: api/Form/5
        [HttpPut("{id}nmrid={nmrid}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromRoute] string nmrid, [FromBody] TblOtptfu item)
        {
            if (!TryValidateModel(item))
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.Otptfuid)
            {
                return BadRequest();
            }
            Nmr nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == item.Nmrid);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TblOtptfu item = await _context.TblOtptfu.Where(m => m.Otptfuid == id && m.Nmrid == id2).Include(m => m.Nmr).SingleOrDefaultAsync();
            if (item == null)
            {
                return NotFound();
            }
            if (item.Nmr.StatusId == 3 || item.Nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            var user = User.Identity.Name;
            if (user == item.Nmr.UserName)
            {
                item.Nmr.UpdateDate = DateTime.Now;
                item.Nmr.StatusId = 2;
                _context.TblOtptfu.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(item);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPut("partial/{nmrid}")]
        public async Task<IActionResult> partialForm([FromRoute] string nmrid, [FromBody] SaminPartial item)
        {
            if (nmrid == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == nmrid);
            if (nmr == null)
            {
                return BadRequest();
            }
            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            var user = User.Identity.Name;
            if (nmr.UserName != user)
            {
                return Unauthorized();
            }
            nmr.UpdateDate = DateTime.Now;
            nmr.StatusId = 2;
            nmr.IalsKwashiorkor = item.IalsKwashiorkor;
            nmr.IalsMarasmus = item.IalsMarasmus;
            nmr.IawgKwashiorkor = item.IawgKwashiorkor;
            nmr.IawgMarasmus = item.IawgMarasmus;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        [HttpGet("stock/{nmrid}")]
        public IEnumerable<stockinDto> stockList([FromRoute] string nmrid)
        {
            if (nmrid == null) { BadRequest(); }
            var user = User.Identity.Name;
            var items = _context.TblStockIpt.Where(m => m.Nmrid == nmrid && m.UserName == user).Include(m => m.Sstock).AsNoTracking().ToList();
            var data = _mapper.Map<IEnumerable<TblStockIpt>, IEnumerable<stockinDto>>(items);
            return data;
        }
        [HttpGet("adminstock/{nmrid}")]
        [Authorize(Roles = "administrator")]
        public async  Task<IEnumerable<stockinDto>> adminstockList([FromRoute] string nmrid)
        {
            if (nmrid == null) { BadRequest(); }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<TblStockIpt> query = _context.TblStockIpt.Where(m => m.Nmrid == nmrid);
            if (user.TenantId != 1)
            {
                query = _context.TblStockIpt.Where(m =>m.Nmrid == nmrid&& m.Nmr.Tenant.Equals(user.TenantId));
            }                       
            var items = query.Include(m => m.Sstock).AsNoTracking().ToList();
            var data = _mapper.Map<IEnumerable<TblStockIpt>, IEnumerable<stockinDto>>(items);
            return data;
        }

        [HttpPut("stock/{id}nmrid={nmrid}")]
        public async Task<IActionResult> putStock([FromRoute] int id, [FromRoute] string nmrid, [FromBody] TblStockIpt item)
        {
            if (nmrid == null) { BadRequest(); }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.SstockId)
            {
                return BadRequest();
            }
            Nmr nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == item.Nmrid);
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
                var stock = await _context.TblStockIpt.Where(m => m.Nmrid == nmrid && m.SstockId == id).AsNoTracking().SingleOrDefaultAsync();
                if (stock.UserName != item.UserName)
                {
                    return BadRequest();
                }
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
            else { return Unauthorized(); }


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

            TblStockIpt item = await _context.TblStockIpt.SingleOrDefaultAsync(m => m.SstockId == id && m.Nmrid == nmrid);
            if (item == null)
            {
                return NotFound();
            }
            Nmr nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == item.Nmrid);
            if (nmr == null)
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
                _context.TblStockIpt.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(item);
            }
            else { return Unauthorized(); }
        }

        [HttpGet("total/{id}nmrid={nmrid}")]
        public int beginning(string nmrid, int id)
        {
            if (nmrid == null || id == 0)
            {
                return 0;
            }
            string[] words = nmrid.Split('-');
            int facid = Convert.ToInt32(words[0]);
            int year = Convert.ToInt32(words[1]);
            int month = Convert.ToInt32(words[2]) - 1;
            if (month == 0)
            {
                year = year - 1;
                month = 12;
            }

            string nid = String.Format("{0}-{1}-{2}", facid, year, month);
            var valid = _context.Nmr.Where(m => m.Nmrid == nmrid).SingleOrDefault();
            if (valid == null)
            {
                return 0;
            }
            var item = _context.TblOtptfu.Where(m => m.Nmrid == nid && m.Otptfuid == id).SingleOrDefault();
            if (item == null)
            {
                return 0;
            }
            int ending = (item.TMale.GetValueOrDefault() + item.TFemale.GetValueOrDefault() + item.Totalbegin.GetValueOrDefault()
            + item.Defaultreturn.GetValueOrDefault() + item.Fromsfp.GetValueOrDefault() + item.Fromscotp.GetValueOrDefault())
            - (item.Cured.GetValueOrDefault() + item.NonCured.GetValueOrDefault() + item.Death.GetValueOrDefault() + item.Default.GetValueOrDefault() + item.RefOut.GetValueOrDefault());
            return ending;
        }


        private bool Exists(int id, string nmrid)
        {
            return _context.TblOtptfu.Any(e => e.Otptfuid == id && e.Nmrid == nmrid);
        }
        private bool StockExists(int id, string nmrid)
        {
            return _context.TblStockIpt.Any(e => e.SstockId == id && e.Nmrid == nmrid);
        }
    }
}