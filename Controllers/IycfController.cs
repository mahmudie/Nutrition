using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using DataSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace DataSystem.Controllers
{
    [Produces("application/json")]
    [Route("api/Iycf")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles="dataentry,administrator")]
    public class IycfController : Controller
    {
        private readonly WebNutContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public IycfController(WebNutContext context,IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper=mapper;
            _userManager = userManager;
            
        }
        [Authorize(Roles="administrator")]
        [HttpGet("admin/{nmrid}")]
        public async Task<IEnumerable<IycfDto>> adminGet([FromRoute] string nmrid)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<TblIycf> query = _context.TblIycf.Where(m=>m.Nmrid==nmrid);
            if (user.TenantId != 1)
            {
                query = _context.TblIycf.Where(m => m.Nmrid == nmrid && m.Nmr.Tenant.Equals(user.TenantId));
            }            
             var data=query.Include(m=>m.Iycf).ToList();
             var model=_mapper.Map<IEnumerable<TblIycf>, IEnumerable<IycfDto>>(data);
             return model;
        }

        // GET: api/Iycf
        [HttpGet("{nmrid}")]
        public IEnumerable<IycfDto> GetTblIycf([FromRoute] string nmrid)
        {
            var user=User.Identity.Name;
            var data= _context.TblIycf.Where(m => m.Nmrid == nmrid&&m.UserName==user).Include(m=>m.Iycf).ToList();
            var model=_mapper.Map<IEnumerable<TblIycf>, IEnumerable<IycfDto>>(data);
            return model;
        }
        [HttpPost("new/{nmrid}")]
       public  IActionResult Create(string nmrid)
        {
            if (nmrid == null)
            {
                return BadRequest();
            }
            int[] query = _context.TblIycf.Where(m => m.Nmrid == nmrid).Select(m => m.Iycfid).ToArray();
            int[] model = _context.TlkpIycf.Where(m => m.Active.Equals(true) && !query.Contains(m.Iycfid)).Select(m=>m.Iycfid).ToArray();
            var user=User.Identity.Name;
            foreach(int id in model){
             TblIycf item=new TblIycf();
             item.UserName=user;
             item.Nmrid=nmrid;
             item.Iycfid=id;
             _context.TblIycf.Add(item);
            }
              _context.SaveChanges();
            return NoContent();
        }


        // PUT: api/Iycf/5
        [HttpPut("{id}nmrid={nmrid}")]
        public async Task<IActionResult> PutTblIycf([FromRoute] int id,[FromRoute] string nmrid, [FromBody] TblIycf item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.Iycfid || nmrid!=item.Nmrid)
            {
                return BadRequest();
            }
            var nmr=await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == nmrid);
            if(nmr==null){
                return BadRequest();
            }
            if(nmr.StatusId==3 || nmr.HfactiveStatusId!=1){
               return BadRequest();
            }
            var user=User.Identity.Name;
            if(nmr.UserName!=user){return Unauthorized();}
            item.UpdateDate = DateTime.Now;
            nmr.StatusId=2;
            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblIycfExists(id, nmrid))
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


        // DELETE: api/Iycf/5
        [HttpDelete("{id}nmrid={nmrid}")]
        public async Task<IActionResult> DeleteTblIycf([FromRoute] int id, [FromRoute] string nmrid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TblIycf tblIycf = await _context.TblIycf.Where(m => m.Iycfid == id && m.Nmrid==nmrid).Include(m=>m.Nmr).SingleOrDefaultAsync();
            if (tblIycf == null)
            {
                return NotFound();
            }
             if(tblIycf.Nmr.StatusId==3 || tblIycf.Nmr.HfactiveStatusId!=1){
               return BadRequest();
            }
            var user=tblIycf.UserName;
            if(tblIycf.UserName!=user){return Unauthorized();}
            tblIycf.UpdateDate = DateTime.Now;
            tblIycf.Nmr.StatusId=2; 
            _context.TblIycf.Remove(tblIycf);
            await _context.SaveChangesAsync();

            return Ok(tblIycf);
        }

        private bool TblIycfExists(int id,string nmrid)
        {
            return _context.TblIycf.Any(e => e.Iycfid == id &&e.Nmrid==nmrid);
        }
    }
}