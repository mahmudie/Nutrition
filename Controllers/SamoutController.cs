using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DataSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace DataSystem.Controllers
{
    [Authorize(Roles="dataentry")]
    public class SamoutController : Controller
    {
        private readonly WebNutContext _context;

        public SamoutController(WebNutContext context)
        {
            _context = context;
        }
        public IActionResult Create(string nmrid)
        {
            if (nmrid == null) { return NotFound(); }
           var nmr = _context.Nmr.SingleOrDefault(m => m.Nmrid == nmrid);
            if (nmr == null)
            {
                return NotFound();
            }
            var user = User.Identity.Name;
            if (nmr.UserName != user)
            {
                return Unauthorized();
            }
            int[] query = _context.TblOtp.Where(m => m.Nmrid == nmrid).Select(m => m.Otpid).ToArray();
            // int[] model = _context.TlkpOtptfu.Where(m => m.Active.Equals(true) && !m.AgeGroup.ToLower().Trim().Replace(" ","").Contains("6month") && !m.AgeGroup.ToLower().Contains("total") && !query.Contains(m.Otptfuid)).Select(m=>m.Otptfuid).ToArray();
             int[] model = _context.TlkpOtptfu.Where(m => m.Active.Equals(true) && !m.AgeGroup.ToLower().Contains("total") && !query.Contains(m.Otptfuid)).Select(m=>m.Otptfuid).ToArray();
            foreach(int id in model){
                TblOtp Item=new TblOtp();
                Item.Nmrid=nmrid;
                Item.UserName=user;
                Item.Otpid=id;
                _context.TblOtp.Add(Item);
            }
            int[] selectedItems = _context.TblStockOtp.Where(m => m.Nmrid == nmrid).Select(m => m.SstockotpId).ToArray();
            int[] validItems = _context.TlkpSstock.Where(m => m.Active.Equals(true)&&m.OPDSAMZarib>0 && !selectedItems.Contains(m.SstockId)).Select(m=>m.SstockId).ToArray();
            foreach(int id in validItems){
                TblStockOtp Item=new TblStockOtp();
                Item.Nmrid=nmrid;
                Item.UserName=user;
                Item.SstockotpId=id;
                _context.TblStockOtp.Add(Item);
            }
            _context.SaveChanges();
            return Ok();
        }


        public IActionResult EditStock(int? id, string nmrid)
        {
            if (id == null || nmrid == null)
            {
                return NotFound();
            }
            ViewData["SstockotpId"] = new SelectList(_context.TlkpSstock.Where(m=>m.SstockId==id), "SstockId", "Item");
            return View();
        }

        public  IActionResult Edit(int? id, string nmrid)
        {
            if (id == null || nmrid == null)
            {
                return NotFound();
            }
            ViewData["Otptfuid"] = new SelectList(_context.TlkpOtptfu.Where(m => m.Otptfuid == id), "Otptfuid", "AgeGroup");
            return View();
        }

    }
}
