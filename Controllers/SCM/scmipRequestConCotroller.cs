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
    public class scmipRequestConfController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmipRequestConfController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmipRequestConfirmation.ToList();
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
            int count = DataSource.Cast<scmipRequestConfirmation>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmipRequestConfirmation> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int RequestId = int.Parse(value.Params["ID"].ToString());
            scmipRequestConfirmation item = new scmipRequestConfirmation();
            if (item == null) { return BadRequest(); }

            item.requestId = RequestId;
            item.isSubmitted = false;
            item.submissionDate = value.Value.submissionDate;
            item.emailMessage = value.Value.emailMessage;
            item.reasonId = value.Value.reasonId;
            item.userName = user.UserName;
            item.tenantId = user.TenantId;
            item.updateDate = DateTime.Now.Date;
            item.sendEmail = value.Value.sendEmail;

            var mails = _context.scmmailgroup.Where(m => m.isactive.Equals(true)).FirstOrDefault();
            var mails_to = mails.toemails;
            var mails_cc = mails.ccemails;



            try
            {
                if ((user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.Add(item);
                    _context.SaveChanges();
                    if (value.Value.sendEmail == true)
                    {
                        var requestItems = _context.vscmRequestList.Where(m => m.RequestId.Equals(item.requestId)).FirstOrDefault();

                        string message = value.Value.emailMessage;
                        string implementerName = requestItems.Implementer;
                        string getDescription = requestItems.PeriodName;
                        string DateFrom = requestItems.PeriodFrom.ToString();
                        string DateTo = requestItems.PeriodTo.ToString();
                        string yearMonthFrom = requestItems.Yearmonthfrom;
                        string yearMonthTo = requestItems.Yearmonthto;
                        SendEmail(mails_to, message, getDescription, mails_cc, DateFrom, DateTo, implementerName, yearMonthFrom, yearMonthTo);
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> Update([FromBody]CRUDModel<scmipRequestConfirmation> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var item = _context.scmipRequestConfirmation.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            int RequestId = int.Parse(value.Params["ID"].ToString());
            if (item != null)
            {
                item.requestId = RequestId;
                item.isSubmitted = false;
                item.submissionDate = value.Value.submissionDate;
                item.emailMessage = value.Value.emailMessage;
                item.reasonId = value.Value.reasonId;
                item.userName = user.UserName;
                item.tenantId = user.TenantId;
                item.updateDate = DateTime.Now.Date;
                item.sendEmail = value.Value.sendEmail;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Get emails from scmmailgroup for using in cc
            var mails = _context.scmmailgroup.Where(m => m.isactive.Equals(true)).FirstOrDefault();
            var mails_to = mails.toemails;
            var mails_cc = mails.ccemails;

            var requestItems = _context.vscmRequestList.Where(m => m.RequestId.Equals(RequestId)).FirstOrDefault();

            string message = value.Value.emailMessage;
            string implementerName = requestItems.Implementer;
            string getDescription = requestItems.PeriodName;
            string DateFrom = requestItems.PeriodFrom.ToString();
            string DateTo = requestItems.PeriodTo.ToString();
            string yearMonthFrom = requestItems.Yearmonthfrom;
            string yearMonthTo = requestItems.Yearmonthto;

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                if ((user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.Update(item);
                    _context.SaveChanges();
                    if (value.Value.sendEmail == true)
                    {
                        SendEmail(mails_to, message, getDescription, mails_cc, DateFrom, DateTo, implementerName, yearMonthFrom, yearMonthTo);
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(value.Value.reasonId))
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

        public async Task<IActionResult> Remove([FromBody]CRUDModel<scmipRequestConfirmation> Value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmipRequestConfirmation item = _context.scmipRequestConfirmation.Where(m => m.id.Equals(id)).FirstOrDefault();
                if ( (user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.scmipRequestConfirmation.Remove(item);
                    _context.SaveChanges();
                }
            }
            else
            {
                return BadRequest(ModelState);
            }


            return NoContent();
        }

        private bool Exists(int id)
        {
            return _context.scmipRequestConfirmation.Any(e => e.id == id);
        }

        public bool SendEmail(string Tomails, string Message, string Period, string CCmails, string DateFrom, string DateTo, string implementerName,string yearMonthFrom,string yearMonthTo)
        {

            try
            {
                // Credentials
                var credentials = new NetworkCredential("scmimplementer@gmail.com", "$cm@123456");
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("scmimplementer@gmail.com"),
                    Subject ="From "+ implementerName + " request for period/quarter :" + Period,
                    Body = "<h3> Request type: " + Period + "</h3>" + "<h3> Date From: " + 
                    DateFrom + "</h3>" + "<h3> Date To :" + DateTo + "</h3>"+
                    "<h3> Year/Month From :" + yearMonthFrom + "</h3>" +
                    "<h3> Year/Month To :" + yearMonthTo + "</h3>" +Message
                };
                mail.IsBodyHtml = true;
                foreach (var address in Tomails.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.To.Add(new MailAddress(address));
                }
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