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
    [Route("api/samout")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "dataentry,administrator")]
    public class samoutformController : Controller
    {
        private readonly WebNutContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;


        public samoutformController(WebNutContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;

        }
        [HttpGet("que/{nmrid}")]
        [Authorize(Roles = "administrator")]
        public async Task<IEnumerable<Feedback>> que([FromRoute] string nmrid)
        {
            if (nmrid == null)
            {
                NotFound(new { Error = "Bad Request." });
            }
            var data =await _context.Feedback.Where(m =>m.Nmrid==nmrid).ToListAsync();
            return data;
        }
        [HttpGet("admin/{nmrid}")]
        [Authorize(Roles = "administrator")]
        public async Task<IEnumerable<SamoutDto>> adminfind([FromRoute] string nmrid)
        {
            if (nmrid == null)
            {
                NotFound(new { Error = "Bad Request." });
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<TblOtp> query = _context.TblOtp.Where(m => m.Nmrid == nmrid);
            if (user.TenantId != 1)
            {
                query = _context.TblOtp.Where(m => m.Nmrid == nmrid && m.Nmr.Tenant.Equals(user.TenantId));
            }
            var items = query.Include(m => m.Otp);
            var data = _mapper.Map<IEnumerable<TblOtp>, IEnumerable<SamoutDto>>(items);

            return data;
        }

        // GET: api/Form/5
        [HttpGet("find/{nmrid}")]
        [Authorize(Roles = "dataentry")]
        public IEnumerable<SamoutDto> find([FromRoute] string nmrid)
        {
            if (nmrid == null) { NotFound(new { Error = "Bad Request." }); }
            var name = User.Identity.Name;
            var items = _context.TblOtp.Where(m => m.Nmrid == nmrid && m.UserName == name).Include(m => m.Otp);
            var data = _mapper.Map<IEnumerable<TblOtp>, IEnumerable<SamoutDto>>(items);


            return data;
        }


        // PUT: api/Form/5
        [HttpPut("{id}nmrid={nmrid}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromRoute] string nmrid, [FromBody] TblOtp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != model.Otpid)
            {
                return BadRequest();
            }
            if (model.UserName != User.Identity.Name) { return Unauthorized(); }
            var nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == nmrid);
            if (!authorizeAction(nmr))
            {
                return BadRequest();
            }

            nmr.UpdateDate = DateTime.Now;
            nmr.StatusId = 2;
            _context.Entry(model).State = EntityState.Modified;

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

        // DELETE: api/Form/5
        [HttpDelete("{id}id2={id2}")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromRoute]string id2)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TblOtp model = await _context.TblOtp.Where(m => m.Otpid == id && m.Nmrid == id2).Include(m => m.Nmr).SingleOrDefaultAsync();
            if (model == null)
            {
                return NotFound();
            }
            if (authorizeAction(model.Nmr))
            {
                model.Nmr.UpdateDate = DateTime.Now;
                model.Nmr.StatusId = 2;
                _context.TblOtp.Remove(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpGet("stock/{nmrid}")]
        public IEnumerable<stockoutDto> stockList([FromRoute] string nmrid)
        {
            if (nmrid == null) { NotFound(new { Error = "Bad Request." }); }
            var user = User.Identity.Name;
            var model = _context.TblStockOtp.Where(m => m.Nmrid == nmrid && m.UserName == user).Include(m => m.Sstockotp).AsNoTracking().ToList();
            var data = _mapper.Map<IEnumerable<TblStockOtp>, IEnumerable<stockoutDto>>(model);
            return data;
        }
        [HttpGet("adminstock/{nmrid}")]
        [Authorize(Roles = "administrator")]
        public async Task<IEnumerable<stockoutDto>> adminstockList([FromRoute] string nmrid)
        {
            if (nmrid == null)
            {
                NotFound(new { Error = "Bad Request." });
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<TblStockOtp> query = _context.TblStockOtp.Where(m => m.Nmrid == nmrid);
            if (user.TenantId != 1)
            {
                query = _context.TblStockOtp.Where(m => m.Nmrid == nmrid && m.Nmr.Tenant.Equals(user.TenantId));
            }
            var model = query.Include(m => m.Sstockotp).AsNoTracking().ToList();
            var data = _mapper.Map<IEnumerable<TblStockOtp>, IEnumerable<stockoutDto>>(model);
            return data;
        }

        [HttpPut("stock/{id}nmrid={nmrid}")]
        public async Task<IActionResult> putStock([FromRoute] int id, [FromRoute] string nmrid, [FromBody] TblStockOtp item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.SstockotpId)
            {
                return BadRequest();
            }
            var nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == nmrid);
            if (authorizeAction(nmr))
            {
                var stock = await _context.TblStockOtp.Where(m => m.Nmrid == nmrid && m.SstockotpId == id).AsNoTracking().SingleOrDefaultAsync();
                if (stock.UserName != item.UserName)
                {
                    return BadRequest();
                }
                nmr.UpdateDate = DateTime.Now;
                nmr.StatusId = 2;
                item.UserName = nmr.UserName;
                _context.TblStockOtp.Update(item);

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
            return BadRequest();

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
            var user = User.Identity.Name;
            var nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == nmrid);
            if (authorizeAction(nmr))
            {
                nmr.UpdateDate = DateTime.Now;
                nmr.StatusId = 2;
                nmr.OalsKwashiorkor = item.OalsKwashiorkor;
                nmr.OalsMarasmus = item.OalsMarasmus;
                nmr.OawgKwashiorkor = item.OawgKwashiorkor;
                nmr.OawgMarasmus = item.OawgMarasmus;
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

            return BadRequest();
        }



        [HttpDelete("stock/{id}nmrid={nmrid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStock([FromRoute] int id, [FromRoute]string nmrid)
        {
            if (nmrid == null)
            {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == nmrid);
            if (!authorizeAction(nmr))
            {
                return BadRequest();
            }
            TblStockOtp item = await _context.TblStockOtp.SingleOrDefaultAsync(m => m.SstockotpId == id && m.Nmrid == nmrid);
            if (item == null)
            {
                return NotFound();
            }

            if (nmr.UserName != User.Identity.Name) { return Unauthorized(); }
            nmr.UpdateDate = DateTime.Now;
            nmr.StatusId = 2;
            _context.TblStockOtp.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(item);
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
            var item = _context.TblOtp.Where(m => m.Nmrid == nid && m.Otpid == id).SingleOrDefault();
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
            return _context.TblOtp.Any(e => e.Otpid == id && e.Nmrid == nmrid);
        }
        private bool StockExists(int id, string nmrid)
        {
            return _context.TblStockOtp.Any(e => e.SstockotpId == id && e.Nmrid == nmrid);
        }
        private bool authorizeAction(Nmr nmr)
        {
            if (nmr == null)
            {
                return false;
            }
            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {
                return false;
            }
            if (nmr.UserName != User.Identity.Name) { return false; }
            return true;
        }
    }
}