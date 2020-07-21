using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.SCM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Navigations;
using Syncfusion.XlsIO;

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    public class scmrequestController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmrequestController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "administrator,unicef,pnd")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.vscmRequestList.Where(m=>m.Requesttype.Equals("Quarterly")).ToList();

            try
            {
                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    data = data.ToList();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    data = data.Where(m => m.UserName == user.UserName).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return View(data);
        }
        [Authorize(Roles = "administrator,unicef,pnd")]
        public async Task<IActionResult> Annual()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.vscmRequestList.Where(m=>m.Requesttype.Equals("Annual")).ToList();

            try
            {
                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    data = data.ToList();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    data = data.Where(m => m.UserName == user.UserName).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return View(data);
        }
        public IActionResult Create()
        {
            var imps = _context.Implementers.Select(m => new
            {
                ImpId = m.ImpCode,
                Name = m.ImpAcronym
            }).ToList();


            var provincs = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                Name = m.ProvName
            }).ToList();

            var reportrounds = _context.scmRounds.Where(w=>w.IsActive.Equals(true) &&w.RequesttypeId.Equals(2)).Select(m => new
            {
                RoundId =m.RoundId, 
                RoundName =m.RoundCode + " - "+m.RoundDescription 
            }).ToList();

            //ViewBag.ReqType = new ReqType().ReqTypeList();
            //ViewBag.ReqPeriod = new ReqPeriod().ReqPeriodList();
            //ViewBag.StartYear = new StartYear().StartYearList();
            //ViewBag.StartMonth = new StartMonth().StartMonthList();
            //ViewBag.EndYear = new EndYear().EndYearList();
            //ViewBag.EndMonth = new EndMonth().EndMonthList();
            ViewBag.popupHeight = "auto";

            ViewBag.imps= imps;
            ViewBag.provinces = provincs;
            ViewBag.rounds = reportrounds;
            return View();
        }

        //Calcuate start year and start month for extracting data
        public int RequestStart(int year, int month, int period)
        {
            int StartYear, StartMonth;
            StartYear = year;
            int tempYear=0, tempMonth = 0;

            StartMonth= month;
            if (period == 2)
            {
                if (StartMonth<4)
                {
                    tempYear = StartYear - 1;
                    tempMonth = 9 + StartMonth;
                }
                else
                {
                    tempYear = StartYear;
                    tempMonth = StartMonth - 3;
                }
            }
            else if (period == 1)
            {
                tempYear = StartYear - 1;
                tempMonth = StartMonth;
            }

            return tempYear * 100+tempMonth;
        }

        //Calcuate End year and End month for extracting data
        public int RequestEnd(int year, int month, int period)
        {
            int EndYear, EndMonth;
            EndYear = year;
            int tempYear = 0, tempMonth = 0;

            EndMonth = month;
            if (period == 2)
            {
                if (EndMonth < 4)
                {
                    tempYear = EndYear - 1;
                    tempMonth = 9 + EndMonth;
                }
                else
                {
                    tempYear = EndYear;
                    tempMonth = EndMonth - 3;
                }
            }
            else if (period == 1)
            {
                tempYear = EndYear;
                tempMonth = EndMonth;
            }

            return tempYear * 100 + tempMonth;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(scmRequest ScmRequest)
        {
            //ViewBag.ReqType = new ReqType().ReqTypeList();
            //ViewBag.ReqPeriod = new ReqPeriod().ReqPeriodList();
            //ViewBag.StartYear = new StartYear().StartYearList();
            //ViewBag.StartMonth = new StartMonth().StartMonthList();
            //ViewBag.EndYear = new EndYear().EndYearList();
            //ViewBag.EndMonth = new EndMonth().EndMonthList();
            ViewBag.popupHeight = "auto";

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int TimeStart=0, TimeEnd=0, TenandId;
            DateTime updateDate;
            string userName;

            

            TenandId = user.TenantId;
            userName = user.UserName;
            updateDate = DateTime.Now;

            var rounds = _context.scmRounds.Where(s => s.RoundId.Equals(ScmRequest.requestPeriod)).FirstOrDefault();

            //int yearmonth =ScmRequest.startYear * 100 + ScmRequest.startMonth;
            try
            {
                TimeStart = RequestStart(rounds.YearFrom, rounds.MonthFrom, rounds.RequesttypeId);
                TimeEnd = RequestEnd(rounds.YearTo, rounds.MonthTo, rounds.RequesttypeId);
                //if (rounds.RequesttypeId == 2)
                //{
                //    TimeEnd = (rounds.YearFrom) * 100 + rounds.MonthFrom;
                //}
                //else if (rounds.RequesttypeId == 1)
                //{
                //    TimeEnd = (rounds.YearFrom) * 100 + rounds.MonthTo;
                //}
            }
            catch (Exception)
            {

                throw;
            }

            if (ModelState.IsValid)
            {
                if(User.IsInRole("administrator"))
                {
                    ScmRequest.typeOfRequest =rounds.RequesttypeId.ToString();
                    ScmRequest.yearOfRequest = rounds.YearFrom;
                    ScmRequest.startYear = rounds.YearFrom;
                    ScmRequest.startMonth = rounds.MonthFrom;
                    ScmRequest.endYear = rounds.YearTo;
                    ScmRequest.endMonth = rounds.MonthTo;
                    ScmRequest.TimeStart = TimeStart;
                    ScmRequest.TimeEnd = TimeEnd;
                    ScmRequest.tenantId = user.TenantId;
                    ScmRequest.userName = user.UserName;
                    ScmRequest.updateDate = DateTime.Now;
                    _context.Add(ScmRequest);

                    await _context.SaveChangesAsync();

                   //int RequestId = ScmRequest.requestId;
                    //_context.Database.ExecuteSqlCommand("AddRequest_To_TempRequest @TimeStart={0}, @TimeEnd={1},@TenantId={2},@UpdateDate={3},@UserName={4},@RequestId={5}", TimeStart, TimeEnd, TenandId, updateDate, userName, RequestId);
                    return RedirectToAction("Edit",new { id = ScmRequest.requestId });
                }
            }

            var imps = _context.Implementers.Select(m => new
            {
                ImpId = m.ImpCode,
                Name = m.ImpAcronym
            }).ToList();


            var provincs = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                Name = m.ProvName
            }).ToList();

            var reportrounds = _context.scmRounds.Where(w => w.IsActive.Equals(true)).Select(m => new
            {
                RoundId = m.RoundId,
                RoundName = m.RoundCode + " - " + m.RoundDescription
            }).ToList();


            ViewBag.imps = imps;
            ViewBag.provinces = provincs;
            ViewBag.rounds = reportrounds;
            return View(ScmRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
                {
                    ViewBag.gridAdd = true;
                    ViewBag.gridEdit = true;
                    ViewBag.gridEdit2 = true;
                    ViewBag.gridDelete = true;
                    ViewBag.gridOther = 1;

                }
                else if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = true;
                    ViewBag.gridEdit2 = false;
                    ViewBag.gridDelete = false;
                    ViewBag.gridOther = 0;

                }
                else if ( (user.Unicef == 0 && user.Pnd == 0))
                {
                    ViewBag.gridAdd = true;
                    ViewBag.gridEdit = true;
                    ViewBag.gridEdit2 = true;
                    ViewBag.gridDelete = true;
                    ViewBag.gridOther = 1;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            var reqdata = _context.scmRequest.Where(m => m.requestId.Equals(id)).FirstOrDefault();
            var round = _context.scmRounds.Where(m => m.RoundId.Equals(reqdata.requestPeriod)).FirstOrDefault();
            if (round.RequesttypeId==1)
            {
                ViewBag.title = "Annual Supply Requisition";
            }
            else
            {
                ViewBag.title = "Quarterly Supply Requisition";
            }

            // combo box data
            //ViewBag.ReqType = new ReqType().ReqTypeList();
            //ViewBag.ReqPeriod = new ReqPeriod().ReqPeriodList();
            //ViewBag.StartYear = new StartYear().StartYearList();
            //ViewBag.StartMonth = new StartMonth().StartMonthList();
            //ViewBag.EndYear = new EndYear().EndYearList();
            //ViewBag.EndMonth = new EndMonth().EndMonthList();
            ViewBag.popupHeight = "auto";

            if (id == null)
            {
                return NotFound();
            }

            var scmrequest = _context.scmRequest.SingleOrDefault(m => m.requestId == id);
            if (scmrequest == null)
            {
                return NotFound();
            }

            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";
            ViewBag.content3 = "#Grid3";
            ViewBag.content4 = "#Grid4";
            ViewBag.content5 = "#Grid5";
            ViewBag.content8 = "#Grid8";

            //ViewBag.content6 = "#Grid6";
            if (user.Unicef == 1 | user.Pnd == 1)
            {
                ViewBag.content6 = "#Grid6";
                ViewBag.userlevel = 1;
            }
            else
            {
                ViewBag.userlevel = 0;
            }
            if (user.Unicef == 0 && user.Pnd == 0)
            {
                ViewBag.content7 = "#Grid7";
                ViewBag.adminlevel = 1;
            }
            else
            {
                ViewBag.adminlevel = 0;
            }

            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Request Info",IconCss= "e-tab1" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Facilities", IconCss= "e-tab2" }, Content = ViewBag.content2 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "HF Consumption-based", IconCss = "e-tab3" }, Content = ViewBag.content3 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "HF Average-based", IconCss = "e-tab3" }, Content = ViewBag.content5 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "IP Estimation", IconCss = "e-tab4" }, Content = ViewBag.content4 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Contacts", IconCss = "e-tab4" }, Content = ViewBag.content8 });
            //headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "IP Estimation", IconCss = "e-tab4" }, Content = ViewBag.content6 });
            if (user.Unicef == 1 | user.Pnd == 1)
            {
                headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Request Status", IconCss = "e-tab5" }, Content = ViewBag.content6 });
            }
            if (user.Unicef == 0 && user.Pnd == 0)
            {
                headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Request Submission", IconCss = "e-tab5" }, Content = ViewBag.content7 });
            }

            ViewBag.headeritems = headerItems;

            var facility = _context.FacilityInfo.Select(m => new
            {
                FacilityId = m.FacilityId,
                FacilityName =m.FacilityId+"-"+ m.FacilityName
            }).ToList();

            var districts = _context.Districts.Select(m => new
            {
                DistrictId = m.DistCode,
                DistrictName = m.DistName
            }).ToList();


            var provinces = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                ProvinceName = m.ProvName
            }).ToList();

            var supplies = _context.TlkpSstock.Select(m => new
            {
                SupplyId = m.SstockId,
                Item = m.Item
            }).ToList();

            var facilitytypes = _context.FacilityTypes.Select(m => new
            {
                FacilityTypeId = m.FacTypeCode,
                FacilityTypeName = m.TypeAbbrv
            }).ToList();

            var reportrounds = _context.scmRounds.Where(w => w.IsActive.Equals(true) && w.RequesttypeId.Equals(2)).Select(m => new
            {
                RoundId = m.RoundId,
                RoundName = m.RoundCode + " - " + m.RoundDescription
            }).ToList();

            var requestreasons = _context.scmRequestReason.Select(m=>new
            {
                ReasonId=m.reasonId,
                ReasonName=m.reasonName
            }).ToList();

            var requestStatusItems = _context.scmRequeststatusitems.Select(m => new
            {
                StatusId = m.id,
                StatusName = m.statusName
            }).ToList();

            ViewBag.FacilitySource = facility;
            ViewBag.DistrictSource = districts;
            ViewBag.ProvinceSource = provinces;
            ViewBag.SupplySource = supplies;
            ViewBag.FacilityTypeSource = facilitytypes;
            ViewBag.RequestReasonSource = requestreasons;
            ViewBag.RequestStatusItemsSource = requestStatusItems;

            //Return notes/tips
            var notedata = _context.Notehelpers.ToList();

            ViewBag.mainpage = notedata.Where(m=>m.SectionCode.Equals("SR001")).Select(m => m.Tip).FirstOrDefault();
            ViewBag.poctab = notedata.Where(m=>m.SectionCode.Equals("SR002")).Select(m => m.Tip).FirstOrDefault();
            //var dataReq = _context.ScmHFRequest.Include(m => m.scmHFReqDetails).ToList();

            //ViewBag.DataSource = dataReq;


            List<Implementers> implementers = new List<Implementers>();
            implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpCode = 0, ImpAcronym = "" });

            List<Provinces> Provinces = new List<Provinces>();
            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "", ProvName = "" });

            ViewBag.imps = new SelectList(implementers, "ImpCode", "ImpAcronym");
            ViewBag.provinces = new SelectList(Provinces, "ProvCode", "ProvName");
            ViewBag.rounds = reportrounds;
            return View(scmrequest);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveIPRequest(int id)
        {
            if (User.IsInRole("administrator"))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                InsertIPRequest(id, user.TenantId, user.UserName);             
            }
            return Ok();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, scmRequest ScmRequest)
        {
            //ViewBag.ReqType = new ReqType().ReqTypeList();
            //ViewBag.ReqPeriod = new ReqPeriod().ReqPeriodList();
            //ViewBag.StartYear = new StartYear().StartYearList();
            //ViewBag.StartMonth = new StartMonth().StartMonthList();
            //ViewBag.EndYear = new EndYear().EndYearList();
            //ViewBag.EndMonth = new EndMonth().EndMonthList();
            ViewBag.popupHeight = "auto";

            int TimeStart=0, TimeEnd=0;

            var rounds = _context.scmRounds.Where(s => s.RoundId.Equals(ScmRequest.requestPeriod)).FirstOrDefault();

            //int yearmonth =ScmRequest.startYear * 100 + ScmRequest.startMonth;
            try
            {
                TimeStart = RequestStart(rounds.YearFrom, rounds.MonthFrom, rounds.RequesttypeId);
                TimeEnd = RequestEnd(rounds.YearTo, rounds.MonthTo, rounds.RequesttypeId);
                //if (rounds.RequesttypeId == 2)
                //{
                //    TimeEnd = (rounds.YearFrom) * 100 + rounds.MonthFrom;
                //}
                //else if (rounds.RequesttypeId == 1)
                //{
                //    TimeEnd = (rounds.YearFrom) * 100 + rounds.MonthTo;
                //}
            }
            catch (Exception)
            {

                throw;
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            InsertRquestDetails_ipd(id, user.TenantId, user.UserName,TimeStart,TimeEnd);
            InsertRquestDetails_opd(id, user.TenantId, user.UserName, TimeStart, TimeEnd);
            //InsertHRRequest(user.TenantId, user.UserName);
            InsertHFRequestDetails(user.TenantId, user.UserName,id);

            if (ModelState.IsValid)
            {

                try
                {
                    if(User.IsInRole("administrator"))
                    {
                        var item = _context.scmRequest.SingleOrDefault(m => m.requestId == id);
                        item.impId = ScmRequest.impId;
                        item.provinceId = ScmRequest.provinceId;
                        item.requestBy = ScmRequest.requestBy;
                        item.typeOfRequest = ScmRequest.typeOfRequest;
                        ScmRequest.yearOfRequest = rounds.YearFrom;
                        ScmRequest.startYear = rounds.YearFrom;
                        ScmRequest.startMonth = rounds.MonthFrom;
                        ScmRequest.endYear = rounds.YearTo;
                        ScmRequest.endMonth = rounds.MonthTo;
                        ScmRequest.TimeStart = TimeStart;
                        ScmRequest.TimeEnd = TimeEnd;
                        item.tenantId = user.TenantId;
                        item.userName = user.UserName;
                        item.updateDate = DateTime.Now;
                        item.requestDate = ScmRequest.requestDate;

                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!requestExits(ScmRequest.requestId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit",new { id = id });

            }
            var reportrounds = _context.scmRounds.Where(w => w.IsActive.Equals(true)).Select(m => new
            {
                RoundId = m.RoundId,
                RoundName = m.RoundCode + " - " + m.RoundDescription
            }).ToList();

            List<Implementers> implementers = new List<Implementers>();
            implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpCode = 0, ImpAcronym = "select" });


            List<Provinces> Provinces = new List<Provinces>();
            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "", ProvName = "select" });

            ViewBag.imps = new SelectList(implementers, "ImpAcronym", "ImpAcronym");
            ViewBag.provinces = new SelectList(Provinces, "ProvCode", "ProvName");
            ViewBag.rounds = reportrounds;
            return View(ScmRequest);
        }

        private bool requestExits(int id)
        {
            return _context.scmRequest.Any(e => e.requestId == id);
        }
        public void InsertRquestDetails_ipd(int id, int _TenantId, string _userName, int TimeStart, int TimeEnd)
        {
            int TenantId, RequestId;
            DateTime updateDate;
            string userName;

            var user =  _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if(User.IsInRole("administrator"))
                {
                    TenantId = _TenantId;
                    userName = _userName;
                    updateDate = DateTime.Now;
                    RequestId = id;
                    _context.Database.ExecuteSqlCommand("exec dbo.AddRequest_To_TempRequest_ipd {0}, {1}, {2}, {3}, {4}, {5}", RequestId, TenantId, userName, updateDate, TimeStart, TimeEnd);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> EditPartial([FromBody] CRUDModel<scmIPRequest> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
                {
                    ViewBag.gridAdd = true;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = true;
                    ViewBag.gridUnicef = false;
                    ViewBag.gridPnd = false;
                    ViewBag.gridIP = true;
                }
                else if ((User.IsInRole("administrator") || User.IsInRole("unicef") || User.IsInRole("pnd")) && (user.Unicef == 1 || user.Pnd == 1))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = false;
                    ViewBag.gridUnicef = true;
                    ViewBag.gridPnd = true;
                    ViewBag.gridIP = false;

                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    ViewBag.gridAdd = true;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = true;
                    ViewBag.gridUnicef = false;
                    ViewBag.gridPnd = false;
                    ViewBag.gridIP = true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            var supplies = _context.TlkpSstock.Select(m => new
            {
                SupplyId = m.SstockId,
                Item = m.Item
            }).ToList();

            ViewBag.SupplySource = supplies;

            return PartialView("_DialogEditPartial", value.Value);
        }

        public async Task<IActionResult> AddPartial([FromBody] CRUDModel<scmIPRequest> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
                {
                    ViewBag.gridAdd = true;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = true;
                    ViewBag.gridUnicef = false;
                    ViewBag.gridPnd = false;
                    ViewBag.gridIP = true;
                }
                else if ((User.IsInRole("administrator") || User.IsInRole("unicef") || User.IsInRole("pnd")) && (user.Unicef == 1 || user.Pnd == 1))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = false;
                    ViewBag.gridUnicef = true;
                    ViewBag.gridPnd = true;
                    ViewBag.gridIP = false;

                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    ViewBag.gridAdd = true;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = true;
                    ViewBag.gridUnicef = false;
                    ViewBag.gridPnd = false;
                    ViewBag.gridIP = true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            var supplies = _context.TlkpSstock.Select(m => new
            {
                SupplyId = m.SstockId,
                Item = m.Item
            }).ToList();

            ViewBag.SupplySource = supplies;

            return PartialView("_DialogAddPartial");
        }

        public void InsertRquestDetails_opd(int id, int _TenantId, string _userName, int TimeStart,int TimeEnd)
        {
            int TenantId, RequestId;
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
                    RequestId = id;
                    _context.Database.ExecuteSqlCommand("exec dbo.AddRequest_To_TempRequest_opd {0}, {1}, {2}, {3}, {4}, {5}", RequestId, TenantId, userName, updateDate, TimeStart, TimeEnd);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void InsertHRRequest(int TenantId, string UserName)
        {
            DateTime updateDate;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("administrator"))
                {
                    updateDate = DateTime.Now;
                    _context.Database.ExecuteSqlCommand("exec dbo.AddHFRequest {0},{1},{2}", TenantId, updateDate, UserName);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void InsertIPRequest(int RequestId,int TenantId, string UserName)
        {
            DateTime updateDate;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("administrator"))
                {
                    updateDate = DateTime.Now;
                    _context.Database.ExecuteSqlCommand("exec dbo.Add_To_IPRequest {0},{1},{2},{3}", RequestId, TenantId, UserName, updateDate);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> RUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.scmIPRequests.ToList();
            if (User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef != 1 || user.Pnd != 1))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
            {
                data = data.ToList();
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

        public async Task<IActionResult> HFUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.scmHFReqDetails.Where(m=>m.Esttype.Equals("Consumption-based")).ToList();
            if (User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef != 1 || user.Pnd != 1))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
            {
                data = data.ToList();
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
            int count = DataSource.Cast<scmHFReqDetails>().Count();
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
        public async Task<IActionResult> HFUrlDatasourceAvg([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.scmHFReqDetails.Where(m => m.Esttype.Equals("Average-based")).ToList();
            if (User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef != 1 || user.Pnd != 1))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
            {
                data = data.ToList();
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
            int count = DataSource.Cast<scmHFReqDetails>().Count();
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
        public void InsertHFRequestDetails(int TenantId, string UserName,int id)
        {
            DateTime updateDate;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("administrator"))
                {
                    updateDate = DateTime.Now;
                    _context.Database.ExecuteSqlCommand("exec dbo.AddHFRequestDetails {0},{1},{2},{3}", TenantId, updateDate, UserName, id);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmHFReqDetails> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmHFReqDetails hfreqs = new scmHFReqDetails();
            if (hfreqs == null) { return BadRequest(); }
            hfreqs.requestId = value.Value.requestId;
            hfreqs.facilityId = value.Value.facilityId;
            hfreqs.facilityTypeId = value.Value.facilityTypeId;
            hfreqs.children = value.Value.children;
            hfreqs.buffer = value.Value.buffer;
            hfreqs.currentBalance = value.Value.currentBalance;
            hfreqs.program = value.Value.program;
            hfreqs.stockForChildren = value.Value.stockForChildren;
            hfreqs.adjustment = value.Value.adjustment;
            hfreqs.adjComment = value.Value.adjComment;
            hfreqs.tenantId = Crrentuser.TenantId;
            hfreqs.updateDate = value.Value.updateDate;
            hfreqs.userName = Crrentuser.UserName;

            try
            {
                if (User.IsInRole("administrator"))
                {
                    _context.Add(hfreqs);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> InsertAvg([FromBody]CRUDModel<scmHFReqDetails> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmHFReqDetails hfreqs = new scmHFReqDetails();
            if (hfreqs == null) { return BadRequest(); }
            hfreqs.requestId = value.Value.requestId;
            hfreqs.facilityId = value.Value.facilityId;
            hfreqs.facilityTypeId = value.Value.facilityTypeId;
            hfreqs.children = value.Value.children;
            hfreqs.buffer = value.Value.buffer;
            hfreqs.currentBalance = value.Value.currentBalance;
            hfreqs.program = value.Value.program;
            hfreqs.stockForChildren = value.Value.stockForChildren;
            hfreqs.adjustment = value.Value.adjustment;
            hfreqs.adjComment = value.Value.adjComment;
            hfreqs.tenantId = Crrentuser.TenantId;
            hfreqs.updateDate = value.Value.updateDate;
            hfreqs.userName = Crrentuser.UserName;

            try
            {
                _context.Add(hfreqs);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> Update([FromBody]CRUDModel<scmHFReqDetails> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var hfreqs = _context.scmHFReqDetails.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (hfreqs != null)
            {
                if (hfreqs == null) { return BadRequest(); }
                hfreqs.requestId = value.Value.requestId;
                hfreqs.facilityId = value.Value.facilityId;
                hfreqs.facilityTypeId = value.Value.facilityTypeId;
                hfreqs.children = value.Value.children;
                hfreqs.buffer = value.Value.buffer;
                hfreqs.currentBalance = value.Value.currentBalance;
                hfreqs.program = value.Value.program;
                hfreqs.stockForChildren = value.Value.stockForChildren;
                hfreqs.adjustment = value.Value.adjustment;
                hfreqs.adjComment = value.Value.adjComment;
                hfreqs.tenantId = Crrentuser.TenantId;
                hfreqs.updateDate = value.Value.updateDate;
                hfreqs.userName = Crrentuser.UserName;
            }
            _context.Entry(hfreqs).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(hfreqs);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReqExists(value.Value.id))
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

        public IActionResult Remove([FromBody]CRUDModel<scmHFReqDetails> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (ReqExists(id))
            {
                scmHFReqDetails item = _context.scmHFReqDetails.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmHFReqDetails.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }


            return NoContent();
        }
        public async Task<IActionResult> UpdateAvg([FromBody]CRUDModel<scmHFReqDetails> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var hfreqs = _context.scmHFReqDetails.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (hfreqs != null)
            {
                if (hfreqs == null) { return BadRequest(); }
                hfreqs.requestId = value.Value.requestId;
                hfreqs.facilityId = value.Value.facilityId;
                hfreqs.facilityTypeId = value.Value.facilityTypeId;
                hfreqs.children = value.Value.children;
                hfreqs.buffer = value.Value.buffer;
                hfreqs.currentBalance = value.Value.currentBalance;
                hfreqs.program = value.Value.program;
                hfreqs.stockForChildren = value.Value.stockForChildren;
                hfreqs.adjustment = value.Value.adjustment;
                hfreqs.adjComment = value.Value.adjComment;
                hfreqs.tenantId = Crrentuser.TenantId;
                hfreqs.updateDate = value.Value.updateDate;
                hfreqs.userName = Crrentuser.UserName;
            }
            _context.Entry(hfreqs).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(hfreqs);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReqExists(value.Value.id))
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

        public IActionResult RemoveAvg([FromBody]CRUDModel<scmHFReqDetails> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (ReqExists(id))
            {
                scmHFReqDetails item = _context.scmHFReqDetails.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmHFReqDetails.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }


            return NoContent();
        }
        private bool ReqExists(int id)
        {
            return _context.scmHFReqDetails.Any(e => e.id == id);
        }


        //Delete whole Request - GridThree
        public async Task<IActionResult> DeleteCurrectRequest(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            DeleteHFRequests(id, user.TenantId, user.UserName);
            return Ok();
        }

        //Approve Whole Request - GridThree
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ApproveHFRequests(id, user.TenantId, user.UserName);
            return Ok();
        }


        //Delete whole Request - GridFive
        public async Task<IActionResult> DeleteCurrectRequestAvgbased(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            DeleteHFRequestsAvgbased(id, user.TenantId, user.UserName);
            return Ok();
        }

        //Approve Whole Request - GridFive
        public async Task<IActionResult> ApproveRequestAvgbased(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ApproveHFRequestsAvgbased(id, user.TenantId, user.UserName);
            return Ok();
        }

        // Method to approve request - Gridthree
        public void ApproveHFRequests(int RequestId, int TenantId, string UserName)
        {
            DateTime updateDate;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                updateDate = DateTime.Now;
                _context.Database.ExecuteSqlCommand("exec dbo.ApproveHFRequestDetails {0},{1},{2}", RequestId, TenantId, UserName);

            }
            catch (Exception)
            {

                throw;
            }
        }

        // Method to delete request - Gridthree
        public void DeleteHFRequests(int RequestId, int TenantId, string UserName)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                _context.Database.ExecuteSqlCommand("exec dbo.DeleteHFRequestDetails {0},{1},{2}", RequestId, TenantId, UserName);

            }
            catch (Exception)
            {

                throw;
            }
        }

        // Method to approve request - GridFive
        public void ApproveHFRequestsAvgbased(int RequestId, int TenantId, string UserName)
        {
            DateTime updateDate;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                updateDate = DateTime.Now;
                _context.Database.ExecuteSqlCommand("exec dbo.ApproveHFRequestDetailsAvgbased {0},{1},{2}", RequestId, TenantId, UserName);

            }
            catch (Exception)
            {

                throw;
            }
        }


        //Average-based estimation - GridFive
        public async Task<IActionResult> AddHFAveragebasedEstimation(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            AddHFRequestsAvgbased(id, user.TenantId, user.UserName);
            return Ok();
        }
        public void AddHFRequestsAvgbased(int RequestId, int TenantId, string UserName)
        {
            DateTime updateDate;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                updateDate = DateTime.Now;
                _context.Database.ExecuteSqlCommand("exec dbo.AddReEstimatedAverage {0},{1},{2},{3}", RequestId, TenantId, UserName, updateDate);

            }
            catch (Exception)
            {

                throw;
            }
        }
        // Method to delete request - GridFive
        public void DeleteHFRequestsAvgbased(int RequestId, int TenantId, string UserName)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                _context.Database.ExecuteSqlCommand("exec dbo.DeleteHFRequestDetailsAvgbased {0},{1},{2}", RequestId, TenantId, UserName);

            }
            catch (Exception)
            {

                throw;
            }
        }
        //Method for creating pivottable -Gridthree
        public IActionResult GenerateHFPivotTable(int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var pivotData = _context.scmrptRequestpivot.Where(m=>m.RequestId==id).ToList();


            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook = application.Workbooks.Create(2);

            IWorksheet sheet = workbook.Worksheets[0];
            try
            {
                sheet.Range["A1"].Text = "Id";
                sheet.Range["B1"].Text = "RequestId";
                sheet.Range["C1"].Text = "FacilityId";
                sheet.Range["D1"].Text = "FacilityName";
                sheet.Range["E1"].Text = "FacilityTypeId";
                sheet.Range["F1"].Text = "TypeAbbrv";
                sheet.Range["G1"].Text = "SupplyId";
                sheet.Range["H1"].Text = "Item";
                sheet.Range["I1"].Text = "Program";
                sheet.Range["J1"].Text = "Children";
                sheet.Range["K1"].Text = "CurrentBalance";
                sheet.Range["L1"].Text = "Adjustment";
                sheet.Range["M1"].Text = "StockForChildren";
                sheet.Range["N1"].Text = "TotalNeeded";
                sheet.Range["O1"].Text = "Buffer";
                sheet.Range["P1"].Text = "AdjComment";
                sheet.Range["Q1"].Text = "District";
                sheet.Range["R1"].Text = "Esttype";


                sheet.Range["A2"].Text = "%Reports.Id";
                sheet.Range["B2"].Text = "%Reports.RequestId";
                sheet.Range["C2"].Text = "%Reports.FacilityId";
                sheet.Range["D2"].Text = "%Reports.FacilityName";
                sheet.Range["E2"].Text = "%Reports.FacilityTypeId";
                sheet.Range["F2"].Text = "%Reports.TypeAbbrv";
                sheet.Range["G2"].Text = "%Reports.SupplyId";
                sheet.Range["H2"].Text = "%Reports.Item";
                sheet.Range["I2"].Text = "%Reports.Program";
                sheet.Range["J2"].Text = "%Reports.Children";
                sheet.Range["K2"].Text = "%Reports.CurrentBalance";
                sheet.Range["L2"].Text = "%Reports.Adjustment";
                sheet.Range["M2"].Text = "%Reports.StockForChildren";
                sheet.Range["N2"].Text = "%Reports.TotalNeeded";
                sheet.Range["O2"].Text = "%Reports.Buffer";
                sheet.Range["P2"].Text = "%Reports.AdjComment";
                sheet.Range["Q2"].Text = "%Reports.District";
                sheet.Range["R2"].Text = "%Reports.Esttype";

                ITemplateMarkersProcessor marker = workbook.CreateTemplateMarkersProcessor();

                marker.AddVariable("Reports", pivotData);

                marker.ApplyMarkers();
                sheet.Name = "Data";
            }
            catch (Exception)
            {

                throw;
            }

            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";

            pivotSheet["A2"].Text = "HF Level Request";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;
            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityId"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["TypeAbbrv"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Esttype"].Axis = PivotAxisTypes.Page;

            pivotTable.Fields["Program"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["Item"].Axis = PivotAxisTypes.Row;

            IPivotField Children = pivotTable.Fields["Children"];
            IPivotField CurrentBalance = pivotTable.Fields["CurrentBalance"];
            IPivotField StockForChildren = pivotTable.Fields["StockForChildren"];
            IPivotField Adjustment = pivotTable.Fields["Adjustment"];
            IPivotField TotalStock = pivotTable.Fields["TotalNeeded"];


            pivotTable.DataFields.Add(Children, "Total Children", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(StockForChildren, "Estimated Stock", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(CurrentBalance, "Current Balance", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(Adjustment, "Adjustment", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(TotalStock, "Total Needed", PivotSubtotalTypes.Sum);
            pivotTable.ShowDataFieldInRow = false;


            IPivotTableOptions option = pivotTable.Options;
            option.ErrorString = "X";
            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleMedium4;

            pivotSheet.Activate();
            string ContentType = "Application/msexcel";
            string filename = "HF Level Request"+ "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

        // Batch update
        public async Task<IActionResult> BatchUpdateConsumption([FromBody]CRUDModel value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (value.Changed != null)
            {
                for (var i = 0; i < value.Changed.Count(); i++)
                {
                    try
                    {
                        var req = value.Changed[i];
                        scmHFReqDetails hfreqs = _context.scmHFReqDetails.Where(or => or.id == req.id).FirstOrDefault();

                        hfreqs.requestId = req.requestId;
                        hfreqs.facilityId = req.facilityId;
                        hfreqs.supplyId = req.supplyId;
                        hfreqs.facilityTypeId = req.facilityTypeId;
                        hfreqs.children = req.children;
                        hfreqs.buffer = req.buffer;
                        hfreqs.currentBalance = req.currentBalance;
                        hfreqs.program = req.program;
                        hfreqs.stockForChildren = req.stockForChildren;
                        hfreqs.adjustment = req.adjustment;
                        hfreqs.adjComment = req.adjComment;
                        hfreqs.tenantId = Crrentuser.TenantId;
                        hfreqs.updateDate = DateTime.Now.Date;
                        hfreqs.userName = Crrentuser.UserName;
                        _context.Update(hfreqs);
                        _context.SaveChanges();
                    }
                    catch (Exception ex )
                    {

                        throw;
                    }

                }
            }
            if (value.Deleted != null)
            {
                for (var i = 0; i < value.Deleted.Count(); i++)
                {
                    _context.scmHFReqDetails.Remove(_context.scmHFReqDetails.Where(or => or.id == value.Deleted[i].id).FirstOrDefault());
                    _context.SaveChanges();
                }
            }
            if (value.Added != null)
            {
                for (var i = 0; i < value.Added.Count(); i++)
                {
                    _context.Add(value.Added[i]);
                    _context.SaveChanges();
                }
            }
            var data = _context.scmHFReqDetails.ToList();
            return Json(data);
        }

        public class CRUDModel
        {
            public List<scmHFReqDetails> Added { get; set; }
            public List<scmHFReqDetails> Changed { get; set; }
            public List<scmHFReqDetails> Deleted { get; set; }
            public scmHFReqDetails Value { get; set; }
            public int key { get; set; }
            public string action { get; set; }
        }
        //Combo boxes data 
        public class ReqType
        {
            public string ReqTypeName { get; set; }
            public string ReqTypeId { get; set; }

            public List<ReqType> ReqTypeList()
            {
                List<ReqType> reqtype = new List<ReqType>();
                reqtype.Add(new ReqType() { ReqTypeName = "Annual", ReqTypeId = "1" });
                reqtype.Add(new ReqType() { ReqTypeName = "Quarterly", ReqTypeId = "2" });
                return reqtype;
            }
        }
        public class ReqPeriod
        {
            public string ReqPeriodName { get; set; }
            public string ReqPeriodId { get; set; }

            public string ReqTypeId { get; set; }

            public List<ReqPeriod> ReqPeriodList()
            {
                List<ReqPeriod> reqperiod = new List<ReqPeriod>();
                reqperiod.Add(new ReqPeriod() { ReqPeriodName = "Q1", ReqPeriodId = "101", ReqTypeId = "2" });
                reqperiod.Add(new ReqPeriod() { ReqPeriodName = "Q2", ReqPeriodId = "102", ReqTypeId = "2" });
                reqperiod.Add(new ReqPeriod() { ReqPeriodName = "Q3", ReqPeriodId = "103", ReqTypeId = "2" });
                reqperiod.Add(new ReqPeriod() { ReqPeriodName = "Q4", ReqPeriodId = "104", ReqTypeId = "2" });
                reqperiod.Add(new ReqPeriod() { ReqPeriodName = "Annual", ReqPeriodId = "105", ReqTypeId = "1" });
                return reqperiod;
            }
        }
        public class StartYear
        {
            public int StartYearName { get; set; }
            public int StartYearId { get; set; }

            public List<StartYear> StartYearList()
            {
                List<StartYear> startyear = new List<StartYear>();
                startyear.Add(new StartYear() { StartYearName = 1397, StartYearId = 1397 });
                startyear.Add(new StartYear() { StartYearName = 1398, StartYearId = 1398 });
                startyear.Add(new StartYear() { StartYearName = 1399, StartYearId = 1399 });
                startyear.Add(new StartYear() { StartYearName = 1400, StartYearId = 1400 });
                startyear.Add(new StartYear() { StartYearName = 1401, StartYearId = 1401 });
                startyear.Add(new StartYear() { StartYearName = 1402, StartYearId = 1402 });
                startyear.Add(new StartYear() { StartYearName = 1403, StartYearId = 1403 });
                startyear.Add(new StartYear() { StartYearName = 1404, StartYearId = 1404 });
                startyear.Add(new StartYear() { StartYearName = 1405, StartYearId = 1405 });
                startyear.Add(new StartYear() { StartYearName = 1406, StartYearId = 1406 });
                startyear.Add(new StartYear() { StartYearName = 1407, StartYearId = 1407 });
                return startyear;
            }
        }
        public class StartMonth
        {
            public int StartMonthName { get; set; }
            public int StartMonthId { get; set; }


            public List<StartMonth> StartMonthList()
            {
                List<StartMonth> startmonth = new List<StartMonth>();
                startmonth.Add(new StartMonth() { StartMonthName = 1, StartMonthId = 1 });
                startmonth.Add(new StartMonth() { StartMonthName = 2, StartMonthId = 2 });
                startmonth.Add(new StartMonth() { StartMonthName = 3, StartMonthId = 3 });
                startmonth.Add(new StartMonth() { StartMonthName = 4, StartMonthId = 4 });
                startmonth.Add(new StartMonth() { StartMonthName = 5, StartMonthId = 5 });
                startmonth.Add(new StartMonth() { StartMonthName = 6, StartMonthId = 6 });
                startmonth.Add(new StartMonth() { StartMonthName = 7, StartMonthId = 7 });
                startmonth.Add(new StartMonth() { StartMonthName = 8, StartMonthId = 8 });
                startmonth.Add(new StartMonth() { StartMonthName = 9, StartMonthId = 9 });
                startmonth.Add(new StartMonth() { StartMonthName = 10, StartMonthId = 10 });
                startmonth.Add(new StartMonth() { StartMonthName = 11, StartMonthId = 11 });
                startmonth.Add(new StartMonth() { StartMonthName = 12, StartMonthId = 12 });
                return startmonth;
            }
        }
        public class EndYear
        {
            public int EndYearName { get; set; }
            public int EndYearId { get; set; }


            public List<EndYear> EndYearList()
            {
                List<EndYear> endyear = new List<EndYear>();
                endyear.Add(new EndYear() { EndYearName = 1397, EndYearId = 1397 });
                endyear.Add(new EndYear() { EndYearName = 1398, EndYearId = 1398 });
                endyear.Add(new EndYear() { EndYearName = 1399, EndYearId = 1399 });
                endyear.Add(new EndYear() { EndYearName = 1400, EndYearId = 1400 });
                endyear.Add(new EndYear() { EndYearName = 1401, EndYearId = 1401 });
                endyear.Add(new EndYear() { EndYearName = 1402, EndYearId = 1402 });
                endyear.Add(new EndYear() { EndYearName = 1403, EndYearId = 1403 });
                endyear.Add(new EndYear() { EndYearName = 1404, EndYearId = 1404 });
                endyear.Add(new EndYear() { EndYearName = 1405, EndYearId = 1405 });
                endyear.Add(new EndYear() { EndYearName = 1406, EndYearId = 1406 });
                endyear.Add(new EndYear() { EndYearName = 1407, EndYearId = 1407 });
                return endyear;
            }
        }
        public class EndMonth
        {
            public int EndMonthName { get; set; }
            public int EndMonthId { get; set; }

            public List<EndMonth> EndMonthList()
            {
                List<EndMonth> endmonth = new List<EndMonth>();
                endmonth.Add(new EndMonth() { EndMonthName = 1, EndMonthId = 1 });
                endmonth.Add(new EndMonth() { EndMonthName = 2, EndMonthId = 2 });
                endmonth.Add(new EndMonth() { EndMonthName = 3, EndMonthId = 3 });
                endmonth.Add(new EndMonth() { EndMonthName = 4, EndMonthId = 4 });
                endmonth.Add(new EndMonth() { EndMonthName = 5, EndMonthId = 5 });
                endmonth.Add(new EndMonth() { EndMonthName = 6, EndMonthId = 6 });
                endmonth.Add(new EndMonth() { EndMonthName = 7, EndMonthId = 7 });
                endmonth.Add(new EndMonth() { EndMonthName = 8, EndMonthId = 8 });
                endmonth.Add(new EndMonth() { EndMonthName = 9, EndMonthId = 9 });
                endmonth.Add(new EndMonth() { EndMonthName = 10, EndMonthId =10 });
                endmonth.Add(new EndMonth() { EndMonthName = 11, EndMonthId =11 });
                endmonth.Add(new EndMonth() { EndMonthName = 12, EndMonthId =12 });
                return endmonth;
            }
        }

        //Point of consumption
        public async Task<IActionResult> UrlDataSourcepoc([FromBody]DataManagerRequest dm)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.ScmPOCs.ToList();
            try
            {
                if ( (user.Unicef == 1 || user.Pnd == 1))
                {
                    data = data.ToList();
                }
                else
                {
                    data = data.Where(m => m.TenantId == user.TenantId).ToList();
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
            int count = DataSource.Cast<scmPOC>().Count();
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

        //REquest status
        public async Task<IActionResult> UrlDataSourceReqststus([FromBody]DataManagerRequest dm)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.scmRequeststage.ToList();

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
            int count = DataSource.Cast<scmRequeststage>().Count();
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

        //Status request insert
        public async Task<IActionResult> ReqstatInsert([FromBody]CRUDModel<scmRequeststage> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int RequestId = int.Parse(value.Params["ID"].ToString());

            scmRequeststage status = new scmRequeststage();
            if (status == null) { return BadRequest(); }

            status.requestId = RequestId;

            status.statusId = value.Value.statusId;
            status.confirmed = value.Value.confirmed;
            status.statusUpdateDate = value.Value.statusUpdateDate;
            status.remarks = value.Value.remarks;
            status.userName = user.UserName;
            status.updateDate = DateTime.Now.Date;
            status.finalizeandemail = value.Value.finalizeandemail;

            var user2 = await _userManager.FindByNameAsync(_context.scmRequest.Select(m => m.userName).FirstOrDefault());

            var mails = _context.scmmailgroup.Where(m => m.isactive.Equals(true)).FirstOrDefault();
            var mails_cc = mails.ccemails;
            var mails_to = user2.Email;

            try
            {
                if ((User.IsInRole("administrator") || User.IsInRole("unicef") || User.IsInRole("pnd")) && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.Add(status);
                    _context.SaveChanges();
                    if (value.Value.finalizeandemail == true)
                    {
                        //var reqstage = _context.scmRequeststage.Where(m => m.id == value.Value.id).FirstOrDefault();
                        var requestStatus = _context.scmRequeststatusitems.Where(m => m.id == status.id).FirstOrDefault();

                        var requestItems = _context.vscmRequestList.Where(m => m.RequestId.Equals(RequestId)).FirstOrDefault();
                        string message = "<h2> Your request status/stage</h2>";
                        string stage = "<h3> Request Stage:" + requestStatus.statusName + "</h3>";
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

        // Status request update
        public async Task<IActionResult> ReqstatUpdate([FromBody]CRUDModel<scmRequeststage> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var status = _context.scmRequeststage.Where(cat => cat.statusId == value.Value.statusId).FirstOrDefault();

            int RequestId = int.Parse(value.Params["ID"].ToString());

            if (status != null)
            {
                status.requestId = RequestId;

                status.statusId = value.Value.statusId;
                status.confirmed = value.Value.confirmed;
                status.statusUpdateDate = value.Value.statusUpdateDate;
                status.remarks = value.Value.remarks;
                status.userName = user.UserName;
                status.updateDate = DateTime.Now.Date;
                status.finalizeandemail = value.Value.finalizeandemail;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user2 = await _userManager.FindByNameAsync(_context.scmRequest.Select(m=>m.userName).FirstOrDefault());

            var mails = _context.scmmailgroup.Where(m => m.isactive.Equals(true)).FirstOrDefault();
            var mails_cc = mails.ccemails;
            var mails_to = user2.Email;

            _context.Entry(status).State = EntityState.Modified;

            try
            {
                if ((User.IsInRole("administrator") || User.IsInRole("unicef") || User.IsInRole("pnd")) && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.Update(status);
                    _context.SaveChanges();
                    if (value.Value.finalizeandemail == true)
                    {
                        var reqstage = _context.scmRequeststage.Where(m => m.id == value.Value.id).FirstOrDefault();
                        var requestStatus = _context.scmRequeststatusitems.Where(m => m.id == reqstage.id).FirstOrDefault();

                        var requestItems = _context.vscmRequestList.Where(m => m.RequestId.Equals(RequestId)).FirstOrDefault();
                        string message = "<h2> Your request status/stage</h2>";
                        string stage = "<h3> Request Stage:" + requestStatus.statusName + "</h3>";
                        message = message + stage;
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
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestStatusExists(value.Value.statusId))
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

        // Request status - remove
        public async Task<IActionResult> ReqstatRemove([FromBody]CRUDModel<scmRequeststage> Value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (RequestStatusExists(id))
            {
                scmRequeststage item = _context.scmRequeststage.Where(m => m.id.Equals(id)).FirstOrDefault();
                if ((User.IsInRole("administrator") || User.IsInRole("unicef") || User.IsInRole("pnd")) && (user.Unicef == 1 || user.Pnd == 1))
                {
                    try
                    {
                        _context.scmRequeststage.Remove(item);
                        _context.SaveChanges();
                    }
                    catch (Exception ex )
                    {

                        throw ex;
                    }

                }
            }
            else
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

        //Request status - check request status existing
        private bool RequestStatusExists(int id)
        {
            return _context.scmRequeststage.Any(e => e.id == id);
        }

        public bool SendEmail(string Tomails, string Message, string Period, string CCmails, string DateFrom, string DateTo, string implementerName, string yearMonthFrom, string yearMonthTo)
        {

            try
            {
                // Credentials
                var credentials = new NetworkCredential("scmunicef@gmail.com", "$cm@123456");
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("scmunicef@gmail.com"),
                    Subject = "Your request " + Period + " status",
                    Body = "<h3> Request : " + Period + "</h3>" + "<h3> Date From: " +
                    DateFrom + "</h3>" + "<h3> Date To :" + DateTo + "</h3>" +
                    "<h3> Year/Month From :" + yearMonthFrom + "</h3>" +
                    "<h3> Year/Month To :" + yearMonthTo + "</h3>" + Message
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(Tomails));
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
        // GET: FacilityInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = await _context.scmRequest.SingleOrDefaultAsync(m => m.requestId == id);
            if (req == null)
            {
                return NotFound();
            }

            var reportrounds = _context.scmRounds.Where(w => w.IsActive.Equals(true) && w.RequesttypeId.Equals(2)).Select(m => new
            {
                RoundId = m.RoundId,
                RoundName = m.RoundCode + " - " + m.RoundDescription
            }).ToList();

            List<Implementers> implementers = new List<Implementers>();
            implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpCode = 0, ImpAcronym = "" });

            List<Provinces> Provinces = new List<Provinces>();
            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "", ProvName = "" });

            ViewBag.imps = new SelectList(implementers, "ImpCode", "ImpAcronym");
            ViewBag.provinces = new SelectList(Provinces, "ProvCode", "ProvName");
            ViewBag.rounds = reportrounds;

            return View(req);
        }

        // POST: FacilityInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var req = await _context.scmRequest.SingleOrDefaultAsync(m => m.requestId == id);
            _context.scmRequest.Remove(req);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }

}