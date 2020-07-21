using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DataSystem.Controllers
{
    [Authorize(Roles="dataentry")]
    public class SaminController : Controller
    {
        private readonly WebNutContext _context;

        public SaminController(WebNutContext context)
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
            int[] query = _context.TblOtptfu.Where(m => m.Nmrid == nmrid).Select(m => m.Otptfuid).ToArray();
            int[] model = _context.TlkpOtptfu.Where(m => m.Active.Equals(true)&& !m.AgeGroup.ToLower().Contains("total") && !query.Contains(m.Otptfuid)).Select(m=>m.Otptfuid).ToArray();
            foreach(int a in model){
                TblOtptfu Item=new TblOtptfu();
                Item.Nmrid=nmrid;
                Item.Otptfuid=a;
                Item.UserName=user;
                _context.TblOtptfu.Add(Item);
            }
            int[] query2 = _context.TblStockIpt.Where(m => m.Nmrid == nmrid).Select(m => m.SstockId).ToArray();
            int[] stock = _context.TlkpSstock.Where(m => m.Active.Equals(true)&&m.IPDSAMZarib>0 && !query2.Contains(m.SstockId)).Select(m=>m.SstockId).ToArray();
            foreach(int a in stock){
                TblStockIpt Item=new TblStockIpt();
                Item.Nmrid=nmrid;
                Item.SstockId=a;
                Item.UserName=user;
                _context.TblStockIpt.Add(Item);
            }
            _context.SaveChanges();
            return Ok();
        }


        public  IActionResult EditStock(int? id,string nmrid)
        {
            if (id == null || nmrid==null)
            {
                return NotFound();
            }
            ViewData["SstockId"] = new SelectList(_context.TlkpSstock.Where(m=>m.SstockId==id), "SstockId", "Item");
            return View();
        }



        private bool TblStockOtpExists(int id, string nmrid)
        {
            return _context.TblStockOtp.Any(e => e.SstockotpId == id && e.Nmrid == nmrid);
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


        private bool TblOtptfuExists(int id)
        {
            return _context.TblOtptfu.Any(e => e.Otptfuid == id);
        }
    }
}