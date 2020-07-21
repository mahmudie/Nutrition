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
    public class scmEmailController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmEmailController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var ImpUsersItems = _context.scmUsersset.Where(m => m.IsUnicefPnd == 0).Select(m => new
            {
                ImpUserId = m.Id,
                ImpUserName = m.UserName
            }).ToList();

            ViewBag.ImpUserSource = ImpUsersItems;
            return View();
        }

        public IActionResult EmailUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmEmail.ToList();
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
            int count = DataSource.Cast<scmEmail>().Count();
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


        public async Task<IActionResult> EmailInsert([FromBody]CRUDModel<scmEmail> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            var touser = await _userManager.FindByNameAsync(value.Value.emailToUser);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string name = null, emailid = null, message = null;

            scmEmail email = new scmEmail();
            if (email == null) { return BadRequest(); }

            email.distributionId = int.Parse(value.Params["ID"].ToString());
            email.emailToUser = value.Value.emailToUser;
            email.emailToEmail = touser.Email;
            email.emailFrom = Crrentuser.Email;
            email.message = value.Value.message;
            email.dateSent = value.Value.dateSent;
            name = value.Value.emailToUser;
            emailid = touser.Email;
            message = value.Value.message;


            //Get RoundId from scmDistributionMain
            var getdistributions = _context.scmDistributionMain.Where(m => m.DistributionId.Equals(int.Parse(value.Params["ID"].ToString()))).ToList();
            int periodId = 0;
            foreach (var item in getdistributions)
            {
                periodId = item.RoundId;
            }
            //Get emails from scmmailgroup for using in cc
            var mails_cc = _context.scmmailgroup.Select(m => m.ccemails).FirstOrDefault();

            //Get Roud description
            var get_period = _context.scmRounds.Where(m => m.RoundId.Equals(periodId)).ToList();
            string getDescription = null;
            string DateFrom = null;
            string DateTo = null;

            foreach (var item in get_period)
            {
                getDescription = item.RoundDescription;
                DateFrom = item.PeriodFrom.ToString();
                DateTo = item.PeriodTo.ToString();

            }
            //Get Implementer
            var implementerid = _context.scmDistributionMain.Where(m => m.DistributionId.Equals(int.Parse(value.Params["ID"].ToString()))).Select(m => m.ImpId).FirstOrDefault();

            var implementerName = _context.Implementers.Where(m => m.ImpCode.Equals(implementerid)).Select(m => m.ImpAcronym).FirstOrDefault();


            try
            {
                var distribution = _context.scmDistributionMain.Where(m => m.DistributionId.Equals(int.Parse(value.Params["ID"].ToString()))).FirstOrDefault();
                distribution.ReceiverUser = value.Value.emailToUser;
                _context.Update(distribution);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            try
            {
                _context.Add(email);
                _context.SaveChanges();
                SendEmail(name,email.emailToEmail, message,getDescription,mails_cc,DateFrom,DateTo,implementerName);
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }

        public async Task<IActionResult> Update([FromBody]CRUDModel<scmEmail> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            var touser = await _userManager.FindByNameAsync(value.Value.emailToUser);

            string name=null, emailid=null, message=null;
            
            var email = _context.scmEmail.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (email != null)
            {

                email.distributionId = int.Parse(value.Params["ID"].ToString());
                email.emailToUser = value.Value.emailToUser;
                email.emailToEmail = touser.Email;
                email.emailFrom = Crrentuser.Email;
                email.message = value.Value.message;
                email.dateSent = value.Value.dateSent;
                name = value.Value.emailToUser;
                emailid = touser.Email;
                message = value.Value.message;
            }

            //Get RoundId from scmDistributionMain
            var getdistributions = _context.scmDistributionMain.Where(m => m.DistributionId.Equals(int.Parse(value.Params["ID"].ToString()))).ToList();
            int periodId = 0;
            foreach(var item in getdistributions)
            {
                periodId = item.RoundId;
            }

            //Get emails from scmmailgroup for using in cc
            var mails_cc = _context.scmmailgroup.Where(m=>m.isactive.Equals(true)).Select(m => m.ccemails).FirstOrDefault();

            //Get Roud description
            var get_period = _context.scmRounds.Where(m => m.RoundId.Equals(periodId)).ToList();
            string getDescription = null;
            string DateFrom = null;
            string DateTo = null;

            foreach(var item in get_period)
            {
                getDescription = item.RoundDescription;
                DateFrom = item.PeriodFrom.ToString();
                DateTo = item.PeriodTo.ToString();

            }

            //Get Implementer
            var implementerid = _context.scmDistributionMain.Where(m => m.DistributionId.Equals(int.Parse(value.Params["ID"].ToString()))).Select(m=>m.ImpId).FirstOrDefault();

            var implementerName = _context.Implementers.Where(m => m.ImpCode.Equals(implementerid)).Select(m => m.ImpAcronym).FirstOrDefault();
            
            _context.Entry(email).State = EntityState.Modified;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var distribution = _context.scmDistributionMain.Where(m => m.DistributionId.Equals(int.Parse(value.Params["ID"].ToString()))).FirstOrDefault();
                distribution.ReceiverUser = value.Value.emailToUser;
                _context.Update(distribution);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            try
            {
                _context.Update(email);
                _context.SaveChanges();
                SendEmail(name, email.emailToEmail, message, getDescription, mails_cc,DateFrom,DateTo, implementerName);
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

        public IActionResult Remove([FromBody]CRUDModel<scmEmail> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmEmail item = _context.scmEmail.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmEmail.Remove(item);
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
            return _context.scmEmail.Any(e => e.id == id);
        }

        private async Task<string> getEmail(string Username)
        {
            var user =await _userManager.FindByNameAsync(Username);
            return user.Email;
        }

        public bool SendEmail(string Name, string Email, string Message, string Period, string CCmails,string DateFrom, string DateTo,string implementerName)
        {

            try
            {
                // Credentials
                var credentials = new NetworkCredential("scmunicef@gmail.com", "$cm@123456");
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("scmunicef@gmail.com"),
                    Subject = implementerName+"-Supply of items for period/quarter :"+Period,
                    Body = "<h3> Request type: "+Period+"</h3>" + "<h3> Date From: "+DateFrom+"</h3>" +"<h3> Date To :"+DateTo+"</h3>"+  Message
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
    }
}