using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.SCM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy = "admin")]
    public class EstnotificationController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EstnotificationController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var rounds = _context.scmRounds.Select(m => new
            {
                RoundId = m.RoundId,
                RoundName=m.RoundDescription
            }).ToList();

            var imps = _context.Implementers.Select(m => new
            {
                ImpId = m.ImpCode,
                ImpName = m.ImpAcronym
            }).ToList();

            var provinces = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                Province = m.ProvName
            }).ToList();


            //Child table
            var childtable = _context.scmEstNotification.ToList();

            ViewBag.RoudSource = rounds;
            ViewBag.ImpSource = imps;
            ViewBag.ProvinceSource = provinces;
            ViewBag.dataSource = childtable;
            

            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmEstsubmission.ToList();
            IEnumerable DataSource = data;
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<scmEstsubmission>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmEstsubmission> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);


            scmEstsubmission estsub = new scmEstsubmission();
            if (estsub == null) { return BadRequest(); }

            estsub.roundId = value.Value.roundId;
            estsub.startDate = value.Value.startDate;
            estsub.deadlineDate = value.Value.deadlineDate;
            estsub.userName = user.UserName;
            estsub.updateDate = DateTime.Now;
            estsub.tenantId = user.TenantId;
            estsub.emailmessage = value.Value.emailmessage;

            try
            {
                _context.Add(estsub);
                _context.SaveChanges();
                _context.Database.ExecuteSqlCommand("exec dbo.Add_To_scmEstNotification {0}", estsub.id);

            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }


        public async Task<IActionResult> Update([FromBody]CRUDModel<scmEstsubmission> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var estsub = _context.scmEstsubmission.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (estsub != null)
            {
                estsub.roundId = value.Value.roundId;
                estsub.startDate = value.Value.startDate;
                estsub.deadlineDate = value.Value.deadlineDate;
                estsub.completed = value.Value.completed;
                estsub.datecompleted = value.Value.datecompleted;
                estsub.userName = user.UserName;
                estsub.updateDate = DateTime.Now;
                estsub.tenantId = user.TenantId;
                estsub.emailmessage = value.Value.emailmessage;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(estsub).State = EntityState.Modified;

            try
            {
                _context.Update(estsub);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(value.Value.id))
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

        public IActionResult Remove([FromBody]CRUDModel<scmEstsubmission> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmEstsubmission item = _context.scmEstsubmission.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmEstsubmission.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }


            return NoContent();
        }

        private bool Exists(int id)
        {
            return _context.scmEstsubmission.Any(e => e.id == id);
        }

        private bool Existssub(int id)
        {
            return _context.scmEstNotification.Any(e => e.id == id);
        }
        //Child grid
        public IActionResult CUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmEstNotification.ToList();
            IEnumerable DataSource = data;
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<scmEstNotification>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        public async Task<IActionResult> CUpdate([FromBody]CRUDModel<scmEstNotification> value)
        {
            //It fires the CUpdate now
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var estsub = _context.scmEstNotification.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (estsub != null)
            {
                estsub.impId = value.Value.impId;
                estsub.provinceId = value.Value.provinceId;
                estsub.datesubmitted = value.Value.datesubmitted;
                estsub.email = value.Value.email;
                estsub.completed = value.Value.completed;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(estsub).State = EntityState.Modified;

            try
            {
                _context.Update(estsub);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Existssub(value.Value.id))
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

        public IActionResult CRemove([FromBody]CRUDModel<scmEstNotification> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Existssub(id))
            {
                scmEstNotification item = _context.scmEstNotification.Where(m => m.id.Equals(id)).FirstOrDefault();

                _context.scmEstNotification.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }


            return NoContent();
        }

        public async Task<IActionResult> EmailNotification(int id)
        {
            var currentUser = _context.vmEstNotification.Where(m => m.Id.Equals(id)).ToList();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var estsub = _context.scmEstNotification.Where(cat => cat.id == id).FirstOrDefault();
            if (estsub != null)
            {
                estsub.email=estsub.email+1;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string name=null, email=null, message=null, province=null, implementer=null;
            string StartDate=null;
            string DeadlineDate=null;
            string period = null;
            string ccmails = null;
            int notications = 0;
            notications = estsub.email;
            ccmails = _context.scmmailgroup.Where(m => m.isactive.Equals(true)).Select(m=>m.ccemails).First();

            foreach (var item in currentUser)
            {
                name = item.Username;
                email = item.Email;
                message = item.Emailmessage;
                province = item.Province;
                implementer = item.Implementer;
                StartDate = item.StartDate.ToString();
                DeadlineDate = item.DeadlineDate.ToString();
                period = item.RoundDescription;
            }

            _context.Entry(estsub).State = EntityState.Modified;

            try
            {
                SendEmail(name, email, ccmails, message, province, implementer,StartDate, DeadlineDate,period, notications);
                _context.Update(estsub);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Existssub(id))
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


        public bool SendEmail(string Name, string Email, string CCmails, string Message, string province, string implementer,string startdate,string deadlinedate,string period,int numberofemails)
        {

            try
            {
                var credentials = new NetworkCredential("scmunicef@gmail.com", "$cm@123456");
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("scmunicef@gmail.com"),
                    Subject = "Test - SCM mail for province = " + province + " Imp = " + implementer,
                    Body = "<h3> Date From = " + startdate + "</h3>" + "<h3> Deadline = " + deadlinedate + "</h3>" + "<h3> Period = " + period + "</h3>" + "<h3> Number of notification = " + numberofemails + "</h3>" + Message
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(Email));
                foreach (var address in CCmails.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.CC.Add(new MailAddress(address));
                }
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials,
                    Timeout = int.MaxValue
                };

                client.Send(mail);
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        public ActionResult Opennotificationlist()
        {
            return RedirectToAction("Index", "scmNotifylist");
        }
    }
}