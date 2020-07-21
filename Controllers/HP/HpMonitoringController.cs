using DataSystem.Models;
using DataSystem.Models.HP;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Navigations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "dataentry, administrator")]
    public class HPController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public HPController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FacilityInfo
        public IActionResult Index()
        {
            var data = _context.HpMonitoring.ToList();
            return View(data);
        }

        public async Task<IActionResult> PageData(IDataTablesRequest request)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.vHpMonitoring.ToList();

            try
            {
                if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
                {
                    data = data.Where(m => m.UserName.Equals(user.UserName)).ToList();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
                {
                    data = data.ToList();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    data = data.Where(m => m.UserName.Equals(user.UserName)).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            List<vHpMonitoring> HpMonitoringData;
            if (String.IsNullOrWhiteSpace(request.Search.Value))
            {
                HpMonitoringData = data;
            }
            else
            {

                int a;
                bool result = int.TryParse(request.Search.Value, out a);

                if (result)
                {
                    HpMonitoringData = data.Where(_item => _item.HpmId == a).ToList();
                }

                else if (!result)
                {
                    string search = request.Search.Value.Trim();
                    HpMonitoringData = data.Where(_item => _item.DateOfMonitoring != null && _item.DataCollectorName.ToLower().Contains(search.ToLower())
                    || _item.RespondentName != null && _item.RespondentName.Contains(search)
                    || _item.RespondentContactNo != null && _item.RespondentContactNo.Contains(search)
                    || _item.Province != null && _item.Province.Contains(search)
                    || _item.District != null && _item.District.Contains(search)
                    || _item.FacilityId != 0 && _item.FacilityId.Equals(search)
                    || _item.FacilityName != null && _item.FacilityName.Contains(search)
                    || _item.Implementer != null && _item.Implementer.Contains(search)
                    || _item.HPName != null && _item.HPName.Contains(search)
                    || _item.HPCode != null && _item.HPCode.Contains(search)).ToList();
                }
                else
                {
                    HpMonitoringData = data;
                }
            }
            var dataPage = HpMonitoringData.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, data.Count(), HpMonitoringData.Count(), dataPage);
            return new DataTablesJsonResult(response, true);
        }
        // GET: FacilityInfo/Create
        public IActionResult Create()
        {

            DateTime today_date = DateTime.Now.Date;
            ViewBag.minDate = today_date.AddDays(-90);
            ViewBag.maxDate = today_date.AddDays(1);


            var imps = _context.vmFacilityimps.Select(m => new
            {
                ImpId = m.ImpId,
                Name = m.Implementer,
                FacilityId = m.FacilityId
            }).ToList();

            var imps2 = _context.Implementers.Select(m => new
            {
                OtherImpId = m.ImpCode,
                Name = m.ImpAcronym
            }).ToList();

            var provincs = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                Name = m.ProvName
            }).ToList();

            var districts = _context.Districts.Select(m => new
            {
                DistrictId = m.DistCode,
                Name = m.DistName,
                ProvinceId = m.ProvCode
            }).ToList();



            var facilities = _context.FacilityInfo.Select(m => new
            {
                FacilityId = m.FacilityId,
                Name = m.FacilityId + "-" + m.FacilityName,
                DistrictId = m.DistCode
            }).ToList();

            ViewBag.NGOs = imps;
            ViewBag.NGOs2 = imps2;
            ViewBag.Provinces = provincs;
            ViewBag.Districts = districts;
            ViewBag.Facilities = facilities;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HpMonitoring hpMonitoring)
        {
            var users = _userManager.Users.Where(m => m.UserName.Equals(User.Identity.Name)).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var faciltype = _context.FacilityInfo.Where(m => m.FacilityId.Equals(hpMonitoring.FacilityId)).FirstOrDefault();
                hpMonitoring.UserName = users.UserName;
                hpMonitoring.UpdateDate = DateTime.Now;
                hpMonitoring.TenantId = users.TenantId;
                hpMonitoring.FacilityTypeId = faciltype.FacilityType.Value;

                var implementer = _context.Implementers.Where(m => m.ImpAcronym.Equals(faciltype.Implementer)).FirstOrDefault();

                hpMonitoring.ImpId = implementer.ImpCode;

                if (User.IsInRole("dataentry") || User.IsInRole("administrator"))
                {
                    try
                    {
                        _context.Add(hpMonitoring);
                        await _context.SaveChangesAsync();

                        int id = hpMonitoring.HpmId;
                        //Append Section B. CBNP Kits
                        var cbnkkits = _context.HpCbnpKits.Where(m => m.hpmId.Equals(id)).ToList();
                        if (cbnkkits.Count == 0)
                        {
                            appendB(id, users.UserName, users.TenantId);
                        }

                        //Append Section C.Screening / Growth Monitoring
                        var GrowthMonitoring = _context.HpScreening.Where(m => m.hpmId.Equals(id)).ToList();
                        if (GrowthMonitoring.Count == 0)
                        {
                            appendC(id, users.UserName, users.TenantId);
                        }

                        //Append Section D. Capacity Building/ Knowledges
                        var CapacityBuilding = _context.HpCapacityBuilding.Where(m => m.hpmId.Equals(id)).ToList();
                        if (CapacityBuilding.Count == 0)
                        {
                            appendD(id, users.UserName, users.TenantId);
                        }

                        //Append Section E.Community Nutrition Plan
                        var CommunityNutritionPlan = _context.HpCommunityNutritionPlan.Where(m => m.hpmId.Equals(id)).ToList();
                        if (CommunityNutritionPlan.Count == 0)
                        {
                            appendE(id, users.UserName, users.TenantId);
                        }

                        //Append Section F.Recommendatons
                        var CommunityRecommendations = _context.HpRecommendations.Where(m => m.hpmId.Equals(id)).ToList();
                        if (CommunityRecommendations.Count == 0)
                        {
                            appendF(id, users.UserName, users.TenantId);
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }


                return RedirectToAction("Edit", "HP",new { id = hpMonitoring.HpmId });
            }

            return View(hpMonitoring);
        }
        // GET: FacilityInfo/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            DateTime today_date = DateTime.Now.Date;
            ViewBag.minDate = today_date.AddDays(-90);
            ViewBag.maxDate = today_date.AddDays(1);

            var users = _userManager.Users.Where(m => m.UserName.Equals(User.Identity.Name)).FirstOrDefault();

            try
            {
                if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
                {
                    ViewBag.gridAdd = true;
                    ViewBag.gridAdd2 = true;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = true;
                    ViewBag.gridDelete2 = true;
                    ViewBag.gridOther = 1;

                }
                else if (User.IsInRole("administrator") && (users.Unicef == 1 || users.Pnd == 1))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = false;
                    ViewBag.gridEdit2 = false;
                    ViewBag.gridDelete = false;
                    ViewBag.gridOther = 0;

                }
                else if (User.IsInRole("administrator") && (users.Unicef == 0 && users.Pnd == 0))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = false;
                    ViewBag.gridEdit2 = false;
                    ViewBag.gridDelete = false;
                    ViewBag.gridOther = 1;
                }
                else
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridAdd2 = false;
                    ViewBag.gridEdit = false;
                    ViewBag.gridDelete = false;
                    ViewBag.gridDelete2 = false;
                    ViewBag.gridOther = 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";
            ViewBag.content3 = "#Grid3";
            ViewBag.content4 = "#Grid4";
            ViewBag.content5 = "#Grid5";
            ViewBag.content6 = "#Grid6";

            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "A.General Info", IconCss = "e-tab1" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "B.CBNP Kits", IconCss = "e-tab2" }, Content = ViewBag.content2 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "C.Screening", IconCss = "e-tab2" }, Content = ViewBag.content3 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "D.Capacity Building", IconCss = "e-tab2" }, Content = ViewBag.content4 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "E.Community Plan", IconCss = "e-tab2" }, Content = ViewBag.content5 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "F.Findings/Recommendations", IconCss = "e-tab2" }, Content = ViewBag.content6 });
            ViewBag.headeritems = headerItems;

            if (id == 0)
            {
                return NotFound();
            }

            var hpMonitoring = await _context.HpMonitoring.SingleOrDefaultAsync(m => m.HpmId == id);

            if (hpMonitoring == null)
            {
                return NotFound();
            }

            var imps = _context.vmFacilityimps.Select(m => new
            {
                ImpId = m.ImpId,
                Name = m.Implementer,
                FacilityId = m.FacilityId
            }).ToList();

            var imps2 = _context.Implementers.Select(m => new
            {
                OtherImpId = m.ImpCode,
                Name = m.ImpAcronym
            }).ToList();

            var provincs = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                Name = m.ProvName
            }).ToList();

            var districts = _context.Districts.Select(m => new
            {
                DistrictId = m.DistCode,
                Name = m.DistName,
                ProvinceId = m.ProvCode
            }).ToList();

            var facilities = _context.FacilityInfo.Select(m => new
            {
                FacilityId = m.FacilityId,
                Name = m.FacilityId + "-" + m.FacilityName,
                DistrictId = m.DistCode
            }).ToList();

            var responses = _context.HpResponses.Where(m => m.IsActive).Select(m => new
            {
                ResponseId = m.ResponseId,
                ResponseName = m.ResponseName
            }).ToList();

            var cbnphkits = _context.HpMonitoringlkp.Where(m => m.Section.Equals("B")&& m.IsActive.Equals(true)).Select(m => new
            {
                MonitoringId = m.Id,
                MonitoringName = m.Questionname
            }).ToList();

            var HpScreening = _context.HpMonitoringlkp.Where(m => m.Section.Equals("C") && m.IsActive.Equals(true)).Select(m => new
            {
                MonitoringId = m.Id,
                MonitoringName = m.Questionname
            }).ToList();

            var HpCapacityBuilding = _context.HpMonitoringlkp.Where(m => m.Section.Equals("D") && m.IsActive.Equals(true)).Select(m => new
            {
                MonitoringId = m.Id,
                MonitoringName = m.Questionname
            }).ToList();

            var HpCommunityNutritionPlan = _context.HpMonitoringlkp.Where(m => m.Section.Equals("E") && m.IsActive.Equals(true)).Select(m => new
            {
                MonitoringId = m.Id,
                MonitoringName = m.Questionname
            }).ToList();
            
            var hpRecommendations = _context.HpMonitoringlkp.Where(m => m.Section.Equals("F") && m.IsActive.Equals(true)).Select(m => new
            {
                MonitoringId = m.Id,
                MonitoringName = m.Questionname
            }).ToList();


            ViewBag.NGOs = imps;
            ViewBag.NGOs2 = imps2;
            ViewBag.Provinces = provincs;
            ViewBag.Districts = districts;
            ViewBag.Facilities = facilities;

            ViewBag.Responses = responses;
            ViewBag.HpCbnpKits = cbnphkits;
            ViewBag.HpScreenings = HpScreening;
            ViewBag.HpCapacityBuildings = HpCapacityBuilding;
            ViewBag.HpCommunityNutritionPlans = HpCommunityNutritionPlan;
            ViewBag.HpRecommendations = hpRecommendations;

            return View(hpMonitoring);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HpMonitoring hpMonitoring)
        {
            var users = _userManager.Users.Where(m => m.UserName.Equals(User.Identity.Name)).FirstOrDefault();
            if (ModelState.IsValid)
            {

                try
                {
                    if (User.IsInRole("dataentry")|| User.IsInRole("administrator"))
                    {
                        var faciltype = _context.FacilityInfo.Where(m => m.FacilityId.Equals(hpMonitoring.FacilityId)).FirstOrDefault();
                        var item = _context.HpMonitoring.SingleOrDefault(m => m.HpmId == id);
                        item.DateOfMonitoring = hpMonitoring.DateOfMonitoring;
                        item.DataCollectorName = hpMonitoring.DataCollectorName;
                        item.RespondentName = hpMonitoring.RespondentName;
                        item.RespondentContactNo = hpMonitoring.RespondentContactNo;
                        item.ProvinceId = hpMonitoring.ProvinceId;
                        item.DistrictId = hpMonitoring.DistrictId;
                        item.FacilityId = hpMonitoring.FacilityId;
                        item.FacilityTypeId = faciltype.FacilityType.Value;
                        item.HPName = hpMonitoring.HPName;
                        item.HPCode = hpMonitoring.HPCode;
                        item.ImpId = hpMonitoring.ImpId;
                        item.OtherImpId = hpMonitoring.OtherImpId;
                        item.TenantId = users.TenantId;
                        item.UserName = users.UserName;
                        item.UpdateDate = DateTime.Now;

                        _context.Update(item);
                        await _context.SaveChangesAsync();

      
                        //Append Section B. CBNP Kits
                        var cbnkkits = _context.HpCbnpKits.Where(m => m.hpmId.Equals(id)).ToList();
                        if (cbnkkits.Count == 0)
                        {
                            appendB(id, users.UserName, users.TenantId);
                        }

                        //Append Section C.Screening / Growth Monitoring
                        var GrowthMonitoring = _context.HpScreening.Where(m => m.hpmId.Equals(id)).ToList();
                        if (GrowthMonitoring.Count == 0)
                        {
                            appendC(id, users.UserName, users.TenantId);
                        }

                        //Append Section D. Capacity Building/ Knowledges
                        var CapacityBuilding = _context.HpCapacityBuilding.Where(m => m.hpmId.Equals(id)).ToList();
                        if (CapacityBuilding.Count == 0)
                        {
                            appendD(id, users.UserName, users.TenantId);
                        }

                        //Append Section E.Community Nutrition Plan
                        var CommunityNutritionPlan = _context.HpCommunityNutritionPlan.Where(m => m.hpmId.Equals(id)).ToList();
                        if (CommunityNutritionPlan.Count == 0)
                        {
                            appendE(id, users.UserName, users.TenantId);
                        }

                        //Append Section F.Recommendatons
                        var CommunityRecommendations = _context.HpRecommendations.Where(m => m.hpmId.Equals(id)).ToList();
                        if (CommunityRecommendations.Count == 0)
                        {
                            appendF(id, users.UserName, users.TenantId);
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Exists(hpMonitoring.HpmId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit",id);

            }

            var imps = _context.Implementers.Select(m => new
            {
                ImpId = m.ImpCode,
                Name = m.ImpAcronym
            }).ToList();

            var imps2 = _context.Implementers.Select(m => new
            {
                OtherImpId = m.ImpCode,
                Name = m.ImpAcronym
            }).ToList();

            var provincs = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                Name = m.ProvName
            }).ToList();

            var districts = _context.Districts.Select(m => new
            {
                DistrictId = m.DistCode,
                Name = m.DistName,
                ProvinceId = m.ProvCode
            }).ToList();

            var facilities = _context.FacilityInfo.Select(m => new
            {
                FacilityId = m.FacilityId,
                Name = m.FacilityId + "-" + m.FacilityName,
                DistrictId = m.DistCode
            }).ToList();

            ViewBag.NGOs = imps;
            ViewBag.NGOs2 = imps2;
            ViewBag.Provinces = provincs;
            ViewBag.Districts = districts;
            ViewBag.Facilities = facilities;

            return View(hpMonitoring);
        }
        // GET: FacilityInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hpMonitoring = await _context.HpMonitoring.SingleOrDefaultAsync(m => m.HpmId == id);
            if (hpMonitoring == null)
            {
                return NotFound();
            }
            var imps = _context.Implementers.Select(m => new
            {
                ImpId = m.ImpCode,
                Name = m.ImpAcronym
            }).ToList();

            var imps2 = _context.Implementers.Select(m => new
            {
                OtherImpId = m.ImpCode,
                Name = m.ImpAcronym
            }).ToList();

            var provincs = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                Name = m.ProvName
            }).ToList();

            var districts = _context.Districts.Select(m => new
            {
                DistrictId = m.DistCode,
                Name = m.DistName,
                ProvinceId = m.ProvCode
            }).ToList();

            var facilities = _context.FacilityInfo.Select(m => new
            {
                FacilityId = m.FacilityId,
                Name = m.FacilityId + "-" + m.FacilityName,
                DistrictId = m.DistCode
            }).ToList();

            ViewBag.NGOs = imps;
            ViewBag.NGOs2 = imps2;
            ViewBag.Provinces = provincs;
            ViewBag.Districts = districts;
            ViewBag.Facilities = facilities;

            return View(hpMonitoring);
        }

        // POST: FacilityInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (User.IsInRole("dataentry"))
            {
                var hpm = _context.HpMonitoring.SingleOrDefault(m => m.HpmId == id);
                _context.HpMonitoring.Remove(hpm);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private bool Exists(int id)
        {
            return _context.HpMonitoring.Any(e => e.HpmId == id);
        }

        private bool FacilityInfoExists(int id)
        {
            return _context.SurInfo.Any(e => e.SurveyId == id);
        }

        public void appendB(int id, string UserName, int TenantId)
        {
            DateTime updateDate;
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("dataentry"))
                {
                    updateDate = DateTime.Now;
                    _context.Database.ExecuteSqlCommand("exec dbo.AddHpCbnpKits {0}, {1}, {2}, {3}", id, UserName, TenantId, updateDate);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void appendC(int id, string UserName, int TenantId)
        {
            DateTime updateDate;
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("dataentry"))
                {
                    updateDate = DateTime.Now;
                    _context.Database.ExecuteSqlCommand("exec dbo.AddHpScreening {0}, {1}, {2}, {3}", id, UserName, TenantId, updateDate);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void appendD(int id, string UserName, int TenantId)
        {
            DateTime updateDate;
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("dataentry"))
                {
                    updateDate = DateTime.Now;
                    _context.Database.ExecuteSqlCommand("exec dbo.AddHpCapacityBuilding {0}, {1}, {2}, {3}", id, UserName, TenantId, updateDate);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void appendE(int id, string UserName, int TenantId)
        {
            DateTime updateDate;
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("dataentry"))
                {
                    updateDate = DateTime.Now;
                    _context.Database.ExecuteSqlCommand("exec dbo.AddHpCommunityNutritionPlan {0}, {1}, {2}, {3}", id, UserName, TenantId, updateDate);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void appendF(int id, string UserName, int TenantId)
        {
            DateTime updateDate;
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("dataentry"))
                {
                    updateDate = DateTime.Now;
                    _context.Database.ExecuteSqlCommand("exec dbo.AddHpRecommendations {0}, {1}, {2}, {3}", id, UserName, TenantId, updateDate);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        // Creating Sub forms
        // HpCbnpKits
        public async Task<IActionResult> HpCbnpKitsUrlDatasource([FromBody]DataManagerRequest dm)
        {           
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.HpCbnpKits.ToList();
            if (User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef != 1 || user.Pnd != 1))
            {
                data = data.Where(m => m.tenantId == user.TenantId).ToList();
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
            int count = DataSource.Cast<HpCbnpKits>().Count();
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

        public async Task<IActionResult> HpCbnpKitsInsert([FromBody]CRUDModel<HpCbnpKits> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HpCbnpKits items = new HpCbnpKits();
            if (items == null) { return BadRequest(); }
            items.hpmId = value.Value.hpmId;
            items.monitoringId = value.Value.monitoringId;
            items.responseId = value.Value.responseId;
            items.remarks = value.Value.remarks;
            items.tenantId = Crrentuser.TenantId;
            items.updateDate = DateTime.Now;
            items.userName = Crrentuser.UserName;

            try
            {
                _context.Add(items);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return NoContent();
        }

        public async Task<IActionResult> HpCbnpKitsUpdate([FromBody]CRUDModel<HpCbnpKits> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var items = _context.HpCbnpKits.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (items != null)
            {
                items.hpmId = value.Value.hpmId;
                items.monitoringId = value.Value.monitoringId;
                items.responseId = value.Value.responseId;
                items.remarks = value.Value.remarks;
                items.tenantId = Crrentuser.TenantId;
                items.updateDate = DateTime.Now;
                items.userName = Crrentuser.UserName;
            }
            _context.Entry(items).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(items);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }

        public IActionResult HpCbnpKitsRemove([FromBody]CRUDModel<HpCbnpKits> Value)
        {
            if (HpCbnpKitsExists(Value.Value.id))
            {
                HpCbnpKits item = _context.HpCbnpKits.Where(m => m.id.Equals(Value.Value.id)).FirstOrDefault();
                _context.HpCbnpKits.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        private bool HpCbnpKitsExists(int id)
        {
            return _context.HpCbnpKits.Any(e => e.id == id);
        }

        // Creating Sub forms
        // HpScreening
        public async Task<IActionResult> HpScreeningUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.HpScreening.ToList();
            if (User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef != 1 || user.Pnd != 1))
            {
                data = data.Where(m => m.tenantId == user.TenantId).ToList();
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
            int count = DataSource.Cast<HpScreening>().Count();
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

        public async Task<IActionResult> HpScreeningInsert([FromBody]CRUDModel<HpScreening> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HpScreening items = new HpScreening();
            if (items == null) { return BadRequest(); }
            items.hpmId = value.Value.hpmId;
            items.monitoringId = value.Value.monitoringId;
            items.responseId = value.Value.responseId;
            items.remarks = value.Value.remarks;
            items.tenantId = Crrentuser.TenantId;
            items.updateDate = DateTime.Now;
            items.userName = Crrentuser.UserName;

            try
            {
                _context.Add(items);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return NoContent();
        }

        public async Task<IActionResult> HpScreeningUpdate([FromBody]CRUDModel<HpScreening> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var items = _context.HpScreening.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (items != null)
            {
                items.hpmId = value.Value.hpmId;
                items.monitoringId = value.Value.monitoringId;
                items.responseId = value.Value.responseId;
                items.remarks = value.Value.remarks;
                items.tenantId = Crrentuser.TenantId;
                items.updateDate = DateTime.Now;
                items.userName = Crrentuser.UserName;
            }
            _context.Entry(items).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(items);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }

        public IActionResult HpScreeningRemove([FromBody]CRUDModel<HpScreening> Value)
        {
            if (HpScreeningExists(Value.Value.id))
            {
                HpScreening item = _context.HpScreening.Where(m => m.id.Equals(Value.Value.id)).FirstOrDefault();
                _context.HpScreening.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        private bool HpScreeningExists(int id)
        {
            return _context.HpScreening.Any(e => e.id == id);
        }

        // Creating Sub forms
        // HpCapacityBuilding
        public async Task<IActionResult> HpCapacityBuildingUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.HpCapacityBuilding.ToList();
            if (User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef != 1 || user.Pnd != 1))
            {
                data = data.Where(m => m.tenantId == user.TenantId).ToList();
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
            int count = DataSource.Cast<HpCapacityBuilding>().Count();
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

        public async Task<IActionResult> HpCapacityBuildingInsert([FromBody]CRUDModel<HpCapacityBuilding> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HpCapacityBuilding items = new HpCapacityBuilding();
            if (items == null) { return BadRequest(); }
            items.hpmId = value.Value.hpmId;
            items.monitoringId = value.Value.monitoringId;
            items.responseId = value.Value.responseId;
            items.remarks = value.Value.remarks;
            items.tenantId = Crrentuser.TenantId;
            items.updateDate = DateTime.Now;
            items.userName = Crrentuser.UserName;

            try
            {
                _context.Add(items);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return NoContent();
        }

        public async Task<IActionResult> HpCapacityBuildingUpdate([FromBody]CRUDModel<HpCapacityBuilding> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var items = _context.HpCapacityBuilding.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (items != null)
            {
                items.hpmId = value.Value.hpmId;
                items.monitoringId = value.Value.monitoringId;
                items.responseId = value.Value.responseId;
                items.remarks = value.Value.remarks;
                items.tenantId = Crrentuser.TenantId;
                items.updateDate = DateTime.Now;
                items.userName = Crrentuser.UserName;
            }
            _context.Entry(items).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(items);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }

        public IActionResult HpCapacityBuildingRemove([FromBody]CRUDModel<HpCapacityBuilding> Value)
        {
            if (HpCapacityBuildingExists(Value.Value.id))
            {
                HpCapacityBuilding item = _context.HpCapacityBuilding.Where(m => m.id.Equals(Value.Value.id)).FirstOrDefault();
                _context.HpCapacityBuilding.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
        private bool HpCapacityBuildingExists(int id)
        {
            return _context.HpCapacityBuilding.Any(e => e.id == id);
        }

        // Creating Sub forms
        // HpCommunityNutritionPlan
        public async Task<IActionResult> HpCommunityNutritionPlanUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.HpCommunityNutritionPlan.ToList();
            if (User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef != 1 || user.Pnd != 1))
            {
                data = data.Where(m => m.tenantId == user.TenantId).ToList();
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
            int count = DataSource.Cast<HpCommunityNutritionPlan>().Count();
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

        public async Task<IActionResult> HpCommunityNutritionPlanInsert([FromBody]CRUDModel<HpCommunityNutritionPlan> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HpCommunityNutritionPlan items = new HpCommunityNutritionPlan();
            if (items == null) { return BadRequest(); }
            items.hpmId = value.Value.hpmId;
            items.monitoringId = value.Value.monitoringId;
            items.responseId = value.Value.responseId;
            items.remarks = value.Value.remarks;
            items.tenantId = Crrentuser.TenantId;
            items.updateDate = DateTime.Now;
            items.userName = Crrentuser.UserName;

            try
            {
                _context.Add(items);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return NoContent();
        }

        public async Task<IActionResult> HpCommunityNutritionPlanUpdate([FromBody]CRUDModel<HpCommunityNutritionPlan> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var items = _context.HpCommunityNutritionPlan.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (items != null)
            {
                items.hpmId = value.Value.hpmId;
                items.monitoringId = value.Value.monitoringId;
                items.responseId = value.Value.responseId;
                items.remarks = value.Value.remarks;
                items.tenantId = Crrentuser.TenantId;
                items.updateDate = DateTime.Now;
                items.userName = Crrentuser.UserName;
            }
            _context.Entry(items).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(items);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }

        public IActionResult HpCommunityNutritionPlanRemove([FromBody]CRUDModel<HpCommunityNutritionPlan> Value)
        {
            if (HpCommunityNutritionPlanExists(Value.Value.id))
            {
                HpCommunityNutritionPlan item = _context.HpCommunityNutritionPlan.Where(m => m.id.Equals(Value.Value.id)).FirstOrDefault();
                _context.HpCommunityNutritionPlan.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
        private bool HpCommunityNutritionPlanExists(int id)
        {
            return _context.HpCommunityNutritionPlan.Any(e => e.id == id);
        }        
        
        // Creating Sub forms
        // HPRecommenations
        public async Task<IActionResult> HpRecommendationsDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.HpRecommendations.ToList();
            if (User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef != 1 || user.Pnd != 1))
            {
                data = data.Where(m => m.tenantId == user.TenantId).ToList();
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
            int count = DataSource.Cast<HpRecommendations>().Count();
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

        public async Task<IActionResult> HpRecommendationsInsert([FromBody]CRUDModel<HpRecommendations> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HpRecommendations items = new HpRecommendations();
            if (items == null) { return BadRequest(); }
            items.hpmId = int.Parse(value.Params["ID"].ToString());
            items.monitoringId = value.Value.monitoringId;
            items.keyFindings = value.Value.keyFindings;
            items.responsiblePersonUnit = value.Value.responsiblePersonUnit;
            items.contributingPersonUnit = value.Value.contributingPersonUnit;
            items.deadline = value.Value.deadline;
            items.recommendationStatus = value.Value.recommendationStatus;
            items.dateOfCompletion = value.Value.dateOfCompletion;
            items.remarks = value.Value.remarks;
            items.tenantId = Crrentuser.TenantId;
            items.updateDate = DateTime.Now;
            items.userName = Crrentuser.UserName;

            try
            {
                _context.Add(items);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return NoContent();
        }

        public async Task<IActionResult> HpRecommendationisUpdate([FromBody]CRUDModel<HpRecommendations> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var items = _context.HpRecommendations.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (items != null)
            {
                items.hpmId = int.Parse(value.Params["ID"].ToString()); 
                items.monitoringId = value.Value.monitoringId;
                items.keyFindings = value.Value.keyFindings;
                items.responsiblePersonUnit = value.Value.responsiblePersonUnit;
                items.contributingPersonUnit = value.Value.contributingPersonUnit;
                items.deadline = value.Value.deadline;
                items.recommendationStatus = value.Value.recommendationStatus;
                items.dateOfCompletion = value.Value.dateOfCompletion;
                items.remarks = value.Value.remarks;
                items.tenantId = Crrentuser.TenantId;
                items.updateDate = DateTime.Now;
                items.userName = Crrentuser.UserName;
            }
            _context.Entry(items).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(items);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }

        public async Task<IActionResult> HpRecommendationsRemove([FromBody]CRUDModel<HpRecommendations> Value)
        {
            if (HpRecommendationsExists(Value.Value.id))
            {
                HpRecommendations item = await _context.HpRecommendations.Where(m => m.id.Equals(Value.Value.id)).FirstOrDefaultAsync();
                _context.HpRecommendations.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
        private bool HpRecommendationsExists(int id)
        {
            return _context.HpRecommendations.Any(e => e.id == id);
        }
        public void DeleteRecommendation(int id)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("dataentry"))
                {
                    _context.Database.ExecuteSqlCommand("exec dbo.DeleteRecommendations {0}", id);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}