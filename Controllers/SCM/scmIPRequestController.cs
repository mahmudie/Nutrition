using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Collections.Generic;
using System;
using DataSystem.Models.SCM;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc.Filters;
using DataSystem.Models.emailhelpers;

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "unicef,pnd,administrator")]
    public class scmIPRequestController : Controller
    {
        private readonly WebNutContext _context;
        //private readonly ActionExecutingContext _filterContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public scmIPRequestController(WebNutContext context, UserManager<ApplicationUser> userManager/*, ActionExecutingContext filterContext*/)
        {
            _context = context;
            _userManager = userManager;
            //_filterContext = filterContext;
        }

        // GET: TlkpOtptfus
        public IActionResult Index()
        {

           var data = _context.LkpCategories.ToList();
            ViewBag.DataSource = data;
            return View();
        }
        public async Task<IActionResult> RUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.scmIPRequests.ToList();
            try
            {
                if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    data = data.Where(m => m.userName == user.UserName).ToList();
                }
                else if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    data = data.ToList();
                }
                else
                {
                    data = data.Where(m => m.tenantId == user.TenantId).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
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
            int count = DataSource.Cast<scmIPRequest>().Count();
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

        public async Task<IActionResult> RInsert([FromBody]CRUDModel<scmIPRequest> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmIPRequest scmIpReq = new scmIPRequest();
            if (scmIpReq == null) { return BadRequest(); }

            scmIpReq.requestId = value.Value.requestId;
            scmIpReq.supplyId = value.Value.supplyId;
            scmIpReq.children = value.Value.children;
            scmIpReq.currentBalance = value.Value.currentBalance;
            scmIpReq.adjustment = value.Value.adjustment;
            scmIpReq.adjustmentReason = value.Value.adjustmentReason;
            scmIpReq.stockForChildren = value.Value.stockForChildren;
            scmIpReq.emergency = value.Value.emergency;
            scmIpReq.emergencyReason = value.Value.emergencyReason;
            scmIpReq.userName = User.Identity.Name;
            scmIpReq.updateDate = DateTime.Now;
            scmIpReq.winterization = value.Value.winterization;
            try
            {
                if (User.IsInRole("administrator") && (user.Unicef!=1 || user.Pnd!=1))
                {
                    _context.Add(scmIpReq);
                    _context.SaveChanges();
                }
            }
            catch(Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> RUpdate([FromBody]CRUDModel<scmIPRequest> value)
        {
            bool sendemail = false;
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var scmIpReq = _context.scmIPRequests.Where(cat=>cat.id==value.Value.id).FirstOrDefault();
            if (scmIpReq != null)
            {
                if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    if (scmIpReq.approveByPnd != true || scmIpReq.approveByUnicef != true)
                    {
                        scmIpReq.requestId = value.Value.requestId;
                        scmIpReq.supplyId = value.Value.supplyId;
                        scmIpReq.children = value.Value.children;
                        scmIpReq.currentBalance = value.Value.currentBalance;
                        scmIpReq.adjustment = value.Value.adjustment;
                        scmIpReq.adjustmentReason = value.Value.adjustmentReason;
                        scmIpReq.stockForChildren = value.Value.stockForChildren;
                        scmIpReq.emergency = value.Value.emergency;
                        scmIpReq.emergencyReason = value.Value.emergencyReason;
                        scmIpReq.userName = user.UserName;
                        scmIpReq.updateDate = DateTime.Now;
                        scmIpReq.commentByIp = value.Value.commentByIp;
                        scmIpReq.winterization = value.Value.winterization;
                    }
                }
                else if (User.IsInRole("unicef") && scmIpReq.approveByUnicef != true)
                {
                    scmIpReq.unicefUserId = user.UserName;
                    scmIpReq.commentByUnicef = value.Value.commentByUnicef;
                    scmIpReq.approveByUnicef = value.Value.approveByUnicef;
                    scmIpReq.unicefInchargeId = user.UserName;
                    if (!string.IsNullOrEmpty(value.Value.commentByUnicef))
                    {
                        sendemail = true;
                    }
                }
                else if (User.IsInRole("pnd") && scmIpReq.approveByPnd != true)
                {
                    scmIpReq.pndUserId = user.UserName;
                    scmIpReq.commentByPnd = value.Value.commentByPnd;
                    scmIpReq.approveByPnd = value.Value.approveByPnd;
                    scmIpReq.pndInchargeId = user.UserName;
                    if (!string.IsNullOrEmpty(value.Value.commentByPnd))
                    {
                        sendemail = true;
                    }
                }

            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user2 = await _userManager.FindByNameAsync(scmIpReq.userName);

            _context.Entry(scmIpReq).State = EntityState.Modified;

            try
            {
                _context.Update(scmIpReq);
                _context.SaveChanges();
                InsertRequestStatus(value.Value.requestId, user.TenantId, user.UserName);
                if (sendemail == true)
                {
                    sendCommentMail(scmIpReq.id, scmIpReq.requestId,user2.Email);
                }
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

        public IActionResult RRemove([FromBody]CRUDModel<scmIPRequest> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if(Exists(id))
            {
                scmIPRequest item = _context.scmIPRequests.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmIPRequests.Remove(item);
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
            return _context.scmIPRequests.Any(e => e.id == id);
        }


        public void InsertRequestStatus(int RequestId, int _TenantId, string _userName)
        {
            int TenantId;
            DateTime updateDate;
            string userName;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("administrator"))
                {
                    TenantId = _TenantId;
                    userName = _userName;
                    updateDate = DateTime.Now;
                    _context.Database.ExecuteSqlCommand("exec dbo.UpdateRequestStatus {0}, {1}, {2}, {3}", RequestId, TenantId, userName, updateDate);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void sendCommentMail(int id, int requestId,string email)
        {
            var unicef_pnd_comment = _context.scmIPRequests.Where(m => m.requestId.Equals(requestId)).FirstOrDefault();
            string pndcomment, unicefcomment, commenttosend=null;
            pndcomment = unicef_pnd_comment.commentByPnd;
            unicefcomment = unicef_pnd_comment.commentByUnicef;

            if (!string.IsNullOrEmpty(pndcomment) && string.IsNullOrEmpty(unicefcomment))
            {
                commenttosend = "<h3>Comment by UNICEF: </h3> " + unicefcomment + "<h3> and Comment by PND: </h3> " + pndcomment;
            }
            else if(!string.IsNullOrEmpty(unicefcomment))
            {
                commenttosend = "<h3>Comment by UNICEF: </h3> " + unicefcomment;
            }
            else if (!string.IsNullOrEmpty(pndcomment))
            {
                commenttosend = "<h3>Comment by PND: </h3> " + pndcomment;
            }
            //string url = _filterContext.HttpContext.Request.Path.ToString();
            string url = "http://localhost:5052/scmrequest/Edit/8";
            commenttosend = commenttosend + " please respond at this link <a href ="+url+ "click here </a>";

            var getrequesperiod = _context.scmRequest.Where(m => m.requestId.Equals(requestId)).FirstOrDefault();
            int periodId =(int)getrequesperiod.requestPeriod;

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
            var implementerid = _context.scmRequest.Where(m => m.requestId.Equals(requestId)).FirstOrDefault();

            var implementerName = _context.Implementers.Where(m => m.ImpCode.Equals(implementerid)).Select(m => m.ImpAcronym).FirstOrDefault();

            var iprequest = _context.scmIPRequests.Where(m => m.id.Equals(id)).FirstOrDefault();
            var supplyitem = _context.TlkpSstock.Where(m => m.SstockId.Equals(iprequest.supplyId)).FirstOrDefault();
            try
            {
                SendEmail(email, commenttosend, getDescription, mails_cc, DateFrom, DateTo, implementerName, supplyitem.Item);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool SendEmail(string Email, string Message, string Period, string CCmails, string DateFrom, string DateTo, string implementerName, string supplyname)
        {

            try
            {
                // Credentials
                var credentials = new NetworkCredential("scmunicef@gmail.com", "$cm@123456");
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("scmunicef@gmail.com"),
                    Subject =  "Comments for period code :" + Period +" item/supply "+supplyname,
                    Body = "<h3> Request period: " + Period + "</h3>" + "<h3> Date From: " + DateFrom + "</h3>" + "<h3> Date To :" + DateTo + "</h3>" + "<h3> Supply item: " + supplyname + "</h3>" + Message
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
        private async Task<string> getEmail(string Username)
        {
            var user = await _userManager.FindByNameAsync(Username);
            return user.Email;
        }
    }
}
