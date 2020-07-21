using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using DataSystem.Models.ViewModels;
namespace DataSystem.Controllers
{

    [Authorize]   
    public class BnaqController : Controller
    {
        private readonly WebNutContext _context;

        public BnaqController(WebNutContext context)
        {
            _context = context;    
        }

        // POST: Bnaq/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("bnaq/edit/{nmrid}/{senderForm}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string nmrid,int senderForm, [Bind("BoysScreened,ChwstrainedScreening,GirlsScreened,IpdAdmissionsByChws,IpdRutfstockOutWeeks,MamAddminsionByChws,MamRusfstockoutWeeks,Nmrid,NoHealthWorkers,OpdAdmissionsByChws,OpdRutfstockOutWeeks,Plwreported,UpdateDate,UserName")] qpartialVm form)
        {
            if ( nmrid!=form.Nmrid)
            {
                return NotFound("Wrong Request");
            }
            var item= await _context.Nmr.Where(m=>m.Nmrid==nmrid).SingleOrDefaultAsync();
            if(item==null){
                return NotFound("No Data Foud");
            }
            if(item.UserName!=User.Identity.Name){
                  return BadRequest("Authorization Failed.");
            }
            if (ModelState.IsValid)
            {
               switch(senderForm){
                    case 1 :
                        item.IpdAdmissionsByChws=form.IpdAdmissionsByChws;
                        item.IpdRutfstockOutWeeks=form.IpdRutfstockOutWeeks;
                        item.StatusId=2;
                        item.UpdateDate=DateTime.Now;
                    break; 
                     case 2:
                        item.OpdAdmissionsByChws=form.OpdAdmissionsByChws;
                        item.OpdRutfstockOutWeeks=form.OpdRutfstockOutWeeks;
                        item.StatusId=2;
                        item.UpdateDate=DateTime.Now;
                    break;                    
                    case 3 :
                        item.MamAddminsionByChws=form.MamAddminsionByChws;
                        item.MamRusfstockoutWeeks=form.MamRusfstockoutWeeks;
                        item.StatusId=2;
                        item.UpdateDate=DateTime.Now;
                    break; 
                    case 4 :
                        item.GirlsScreened=form.GirlsScreened;
                        item.BoysScreened=form.BoysScreened;
                        item.Plwreported=form.Plwreported;                        
                        item.StatusId=2;
                        item.UpdateDate=DateTime.Now;
                    break;
                    case 5:
                        item.StatusId = 7;
                        item.UpdateDate = DateTime.Now;
                        break;
                    default:
                         break;    
               }
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblBnaqExists(item.Nmrid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return View(item);
        }


        private bool TblBnaqExists(string nmrid)
        {
            return _context.Nmr.Any(e => e.Nmrid==nmrid);
        }
    }
}
