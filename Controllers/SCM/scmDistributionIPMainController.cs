using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.SCM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Navigations;

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    public class scmDistributionIPMainController : Controller
    {
        private readonly WebNutContext _context;
        IHostingEnvironment hostingEnv;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmDistributionIPMainController(WebNutContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            this.hostingEnv = env;
        }
        public async Task<IActionResult> Transfers()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = false;
                    ViewBag.gridDelete = false;
                }
                else if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = false;

                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = false;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            var rounds = _context.scmRounds.Select(m => new
            {
                RoundId = m.RoundId,
                RoundDescription = m.RoundDescription + " (" + m.RoundCode + ")"
            }).ToList();

            var imps = _context.Implementers.Select(m => new
            {
                ImpId = m.ImpCode,
                ImpName = m.ImpAcronym
            }).ToList();

            var tenants = _context.Tenants.Select(m => new
            {
                TenantId = m.Id,
                TenantName = m.Name
            }).ToList();

            var provinces = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                ProvinceName = m.ProvName
            }).ToList();

            ViewBag.RoundSources = rounds;
            ViewBag.ImpSources = imps;
            ViewBag.TenantSources = tenants;
            ViewBag.ProvinceSources = provinces;

            return View();
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = false;
                    ViewBag.gridDelete = false;
                }
                else if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = false;

                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = false;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            var rounds = _context.scmRounds.Select(m => new
            {
                RoundId = m.RoundId,
                RoundDescription = m.RoundDescription + " (" + m.RoundCode + ")"
            }).ToList();

            var imps = _context.Implementers.Select(m => new
            {
                ImpId = m.ImpCode,
                ImpName = m.ImpAcronym
            }).ToList();

            var tenants = _context.Tenants.Select(m => new
            {
                TenantId = m.Id,
                TenantName = m.Name
            }).ToList();

            var provinces = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                ProvinceName = m.ProvName
            }).ToList();

            ViewBag.RoundSources = rounds;
            ViewBag.ImpSources = imps;
            ViewBag.TenantSources = tenants;
            ViewBag.ProvinceSources = provinces;

            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";
            ViewBag.content3 = "#Grid3";
            ViewBag.content4 = "#Grid4";
            ViewBag.content5 = "#Grid5";
            ViewBag.content6 = "#Grid6";
            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "General Info", IconCss = "e-tab1" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Stock Distribution", IconCss = "e-tab2" }, Content = ViewBag.content2 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Documentation", IconCss = "e-tab3" }, Content = ViewBag.content3 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Emailing", IconCss = "e-tab4" }, Content = ViewBag.content4 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Acknowledge By IP", IconCss = "e-tab4" }, Content = ViewBag.content5 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Acknowledge By HFs", IconCss = "e-tab4" }, Content = ViewBag.content6 });
            ViewBag.headeritems = headerItems;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = false;

                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = false;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            var dataReq = _context.scmDistributionMain.ToList();

            ViewBag.DataSource = dataReq;


            if (id == null)
            {
                return NotFound();
            }

            var scmrRounds = _context.scmDistributionMain.SingleOrDefault(m => m.DistributionId == id);
            if (scmrRounds == null)
            {
                return NotFound();
            }

            var UsersItems = _context.scmUsersset.Where(m => m.IsUnicefPnd == 1).Select(m => new
            {
                UniUserId = m.Id,
                UniUserName = m.UserName
            }).ToList();

            var ImpUsersItems = _context.scmUsersset.Where(m => m.IsUnicefPnd == 0).Select(m => new
            {
                ImpUserId = m.Id,
                ImpUserName = m.UserName
            }).ToList();

            var items = _context.TlkpSstock.Select(m => new
            {
                ItemId = m.SstockId,
                ItemName = m.Item
            }).ToList();

            var warehouses = _context.scmWarehouses.Where(m => m.LevelId == 2).Select(m => new
            {
                WhId = m.WhId,
                WarehouseName = m.RegionsNav.RegionLong + '(' + m.ProvincesNav.ProvName + ") - " + m.ImplementerNav.ImpAcronym + " = " + m.Location
            }).ToList();

            var stockbalance = _context.scmStockBalance.Select(m => new
            {
                StockId = m.StockId,
                StockItem = m.StockItem
            }).ToList();

            var doctypes = _context.scmDoctypes.Select(m => new
            {
                DocId = m.DocId,
                DocumentType = m.DocumentType
            }).ToList();

            var tenants = _context.Tenants.Select(m => new
            {
                TenantId = m.Id,
                TenantName = m.Name
            }).ToList();

            var facilities = _context.FacilityInfo.Select(m => new
            {
                FacilityId = m.FacilityId,
                FacilityName = m.FacilityId.ToString() + "-" + m.FacilityName
            }).ToList();

            ViewBag.DocSource = doctypes;
            ViewBag.ItemSource = items;
            ViewBag.WarehouseSource = warehouses;
            ViewBag.StockSource = stockbalance;
            ViewBag.UserSource = UsersItems;
            ViewBag.ImpUserSource = ImpUsersItems;
            ViewBag.FacilitySource = facilities;


            List<Implementers> implementers = new List<Implementers>();
            implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpCode = 0, ImpAcronym = "" });

            List<scmRounds> rounds = new List<scmRounds>();
            rounds = (from roud in _context.scmRounds select roud).ToList();
            rounds.Insert(0, new scmRounds { RoundId = 0, RoundDescription = "" });


            List<Provinces> Provinces = new List<Provinces>();
            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
            ViewData["ProvSources"] = new SelectList(Provinces, "ProvCode", "ProvName");

            ViewBag.RoundSource = new SelectList(rounds, "RoundId", "RoundDescription");
            ViewBag.ImpSource = new SelectList(implementers, "ImpCode", "ImpAcronym");
            ViewBag.TenantSources = new SelectList( tenants,"TenantId","TenantName");


            return View(scmrRounds);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(scmDistributionMain scmDistmain)
        {
            int id = scmDistmain.DistributionId;
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (currentUser.IsInRole("administrator"))
            {
                if (ModelState.IsValid)
                {
                    try
                    {

                        var item = _context.scmDistributionMain.SingleOrDefault(m => m.DistributionId == id && m.UserName==user.UserName);

                        item.RoundId = id;
                        item.ImpId = scmDistmain.ImpId;
                        item.DistributionDate = scmDistmain.DistributionDate;
                        item.UserName = user.UserName;
                        item.TenantId = user.TenantId;
                        item.ProvinceId = scmDistmain.ProvinceId;
                        item.UpdateDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Exists(scmDistmain.DistributionId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction("Index");

                }
            }
 

            List<Implementers> implementers = new List<Implementers>();
            implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpCode = 0, ImpAcronym = "" });

            List<scmRounds> rounds = new List<scmRounds>();
            rounds = (from roud in _context.scmRounds select roud).ToList();
            rounds.Insert(0, new scmRounds { RoundId = 0, RoundDescription = "" });

            ViewBag.RoundSource = new SelectList(rounds, "RoundId", "RoundDescription");
            ViewBag.ImpSource = new SelectList(implementers, "ImpCode", "ImpAcronym");

            return View(scmDistmain);
        }

        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Editfacilities(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if ((user.Unicef == 1 || user.Pnd == 1))
            {
                ViewBag.gridAdd = false;
                ViewBag.gridEdit = false;
                ViewBag.gridDelete = false;
                ViewBag.gridOther = 0;
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
            {
                ViewBag.gridAdd = true;
                ViewBag.gridEdit = true;
                ViewBag.gridDelete = true;
                ViewBag.gridOther = 1;
            }

            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";
            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Stock Info", IconCss = "e-tab1" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Distribution to Facilities", IconCss = "e-tab2" }, Content = ViewBag.content2 });
            ViewBag.headeritems = headerItems;


            if (id == 0)
            {
                return NotFound();
            }

            var scmrRounds = _context.scmDistributionsIP.SingleOrDefault(m => m.id == id);
            if (scmrRounds == null)
            {
                return NotFound();
            }


            var items = _context.TlkpSstock.Select(m => new
            {
                SupplyId = m.SstockId,
                SupplyName = m.Item
            }).ToList();

            var facilities = _context.FacilityInfo.Select(m => new
            {
                FacilityId =m.FacilityId,
                FacilityName = m.FacilityId.ToString()+"-"+ m.FacilityName
            }).ToList();

            var facilitiestype = _context.FacilityTypes.Select(m => new
            {
                FacilityTypeId = m.FacTypeCode,
                FacilityTypeName = m.TypeAbbrv
            }).ToList();

            var warehouses = _context.scmWarehouses.Where(m => m.LevelId == 2).Select(m => new
            {
                WhId = m.WhId,
                WarehouseName = m.RegionsNav.RegionLong + '(' + m.ProvincesNav.ProvName + ") - " + m.ImplementerNav.ImpAcronym + " = " + m.Location
            }).ToList();


            var wastetypes = _context.scmWasteTypes.Select(m => new
            {
                WasteTypeId = m.Id,
                WasteTypeName = m.name
            }).ToList();

            ViewBag.ItemSource = new SelectList(items, "SupplyId", "SupplyName");
            ViewBag.ItemSource2 = items;
            ViewBag.WhSource = new SelectList(warehouses, "WhId", "WarehouseName");
            ViewBag.FacilitySource = facilities;
            ViewBag.FacilityTypeSource = facilitiestype;
            ViewBag.WasteTypeSource = wastetypes;
            return View(scmrRounds);
        }


        [Authorize(Roles = "unicef,pnd,administrator")]
        public async Task<IActionResult> UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.scmDistributionMain.ToList();
            if (User.IsInRole("administrator") && (user.Unicef!=1 && user.Pnd!=1))
            {
                data = data.Where(m => m.ReceiverUser == user.UserName).ToList();
            }
            else if ((user.Unicef == 1 || user.Pnd == 1))
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
            int count = DataSource.Cast<scmDistributionMain>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmDistributionMain> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmDistributionMain dist = new scmDistributionMain();
            if (dist == null) { return BadRequest(); }

            dist.RoundId = value.Value.RoundId;
            dist.ImpId = value.Value.ImpId;
            dist.DistributionDate = value.Value.DistributionDate;
            dist.UserName = user.UserName;
            dist.ProvinceId = value.Value.ProvinceId;
            dist.TenantId = user.TenantId;
            dist.UpdateDate = DateTime.Now;

            try
            {
                if (User.IsInRole("administrator"))
                {
                    _context.Add(dist);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> Update([FromBody]CRUDModel<scmDistributionMain> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var dist = _context.scmDistributionMain.Where(cat => cat.DistributionId == value.Value.DistributionId).FirstOrDefault();
            if (dist != null)
            {
                dist.RoundId = value.Value.RoundId;
                dist.ImpId = value.Value.ImpId;
                dist.ProvinceId = value.Value.ProvinceId;
                dist.DistributionDate = value.Value.DistributionDate;
                dist.UserName = user.UserName;
                dist.TenantId = user.TenantId;
                dist.UpdateDate = DateTime.Now;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(dist).State = EntityState.Modified;

            try
            {
                if (User.IsInRole("administrator"))
                {
                    _context.Update(dist);
                    _context.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(value.Value.RoundId))
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

        public IActionResult Remove([FromBody]CRUDModel<scmDistributionMain> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                if (User.IsInRole("administrator"))
                {
                    scmDistributionMain item = _context.scmDistributionMain.Where(m => m.DistributionId.Equals(id)).FirstOrDefault();
                    _context.scmDistributionMain.Remove(item);
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
            return _context.scmDistributionMain.Any(e => e.DistributionId == id);
        }

        /// <summary>
        /// Document management
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public IActionResult DocUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmDocs.ToList();
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
            int count = DataSource.Cast<scmDocs>().Count();
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

        [AcceptVerbs("Post")]
        public async Task<IActionResult> DocInsert(int DistributionId, string value, IList<IFormFile> chunkFile, IList<IFormFile> UploadFiles)
        {
            int docID = 0;
            string documentName = null, message = null;
            string pathSave = null;
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
            // Here you can do deserialization based on your Model class  

            foreach (var dat in data)
            {
                ;
                var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(dat.Value));
                foreach (var check in d)
                {
                    if (check.Key == "docId")
                    {
                        docID = Convert.ToInt32(check.Value);

                    }
                    if (check.Key == "documentName")
                    {
                        documentName = Convert.ToString(check.Value);

                    }
                    if (check.Key == "message")
                    {
                        message = Convert.ToString(check.Value);

                    }
                }
            }

            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            long size = 0;
            string FileName = null, FileType = null, FilePath = null;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                foreach (var file in UploadFiles)
                {
                    var filename = ContentDispositionHeaderValue
                                    .Parse(file.ContentDisposition)
                                    .FileName
                                    .Trim('"');
                    var folders = filename.Split('/');
                    var uploaderFilePath = hostingEnv.WebRootPath + @"\documents\" + DistributionId;
                    pathSave = $@"\wwwroot\documents\" + DistributionId;
                    // for Directory upload
                    if (folders.Length > 1)
                    {
                        for (var i = 0; i < folders.Length - 1; i++)
                        {
                            var newFolder = uploaderFilePath + $@"\{folders[i]}";
                            Directory.CreateDirectory(newFolder);
                            uploaderFilePath = newFolder;
                            filename = folders[i + 1];
                        }
                    }
                    size += file.Length;

                    FileName = filename;
                    FileType = '.' + filename.Split('.')[1];
                    FilePath = uploaderFilePath + $@"\{filename}";

                    //OrdersDetails.GetAllRecords().Insert(0, new OrdersDetails() { OrderID = OrderID, CustomerID = CustomerID, file = new OrdersDetails.File() { name = filename, onlinePath = uploaderFilePath + $@"\{filename}", size = size, type = '.' + filename.Split('.')[1] } });
                    filename = uploaderFilePath + $@"\{filename}";
                    if (!Directory.Exists(uploaderFilePath))
                    {
                        Directory.CreateDirectory(uploaderFilePath);
                    }
                    if (!System.IO.File.Exists(filename))
                    {
                        using (FileStream fs = System.IO.File.Create(filename))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                }
                scmDocs docs = new scmDocs();
                if (docs == null) { return BadRequest(); }
                //docs.docId = docID;
                //docs.distributionId = DistributionId;
                //docs.documentName = documentName;
                //docs.message = message;
                //docs.dateSent = DateTime.Now.Date;
                //docs.updateDate = DateTime.Now.Date;
                //docs.userName = Crrentuser.UserName;
                //docs.fileName = FileName;
                //docs.fileSize = size;
                //docs.fileType = FileType;
                //docs.filePath = FilePath;

                _context.scmDocs.Add(docs);
                _context.SaveChanges();
            }

            catch (Exception e)
            {
                string mm = e.Message;
                Response.Clear();
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
            return Content("");
        }

        //[AcceptVerbs("Post")]
        public async Task<IActionResult> DocUpdate(int id, int DistributionId, string value, IList<IFormFile> chunkFile, IList<IFormFile> UploadFiles)
        {
            int docID = 0;
            string documentName = null, message = null;
            string pathSave = null;
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
            // Here you can do deserialization based on your Model class  

            foreach (var dat in data)
            {
                ;
                var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(dat.Value));
                foreach (var check in d)
                {
                    if (check.Key == "docId")
                    {
                        docID = Convert.ToInt32(check.Value);

                    }
                    if (check.Key == "documentName")
                    {
                        documentName = Convert.ToString(check.Value);

                    }
                    if (check.Key == "message")
                    {
                        message = Convert.ToString(check.Value);

                    }
                }
            }

            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            long size = 0;
            string FileName = null, FileType = null, FilePath = null;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                foreach (var file in UploadFiles)
                {
                    var filename = ContentDispositionHeaderValue
                                    .Parse(file.ContentDisposition)
                                    .FileName
                                    .Trim('"');
                    var folders = filename.Split('/');
                    var uploaderFilePath = hostingEnv.WebRootPath + @"\documents\" + DistributionId;
                    pathSave = $@"\wwwroot\documents\" + DistributionId;
                    // for Directory upload
                    if (folders.Length > 1)
                    {
                        for (var i = 0; i < folders.Length - 1; i++)
                        {
                            var newFolder = uploaderFilePath + $@"\{folders[i]}";
                            Directory.CreateDirectory(newFolder);
                            uploaderFilePath = newFolder;
                            filename = folders[i + 1];
                        }
                    }
                    size += file.Length;

                    FileName = filename;
                    FileType = '.' + filename.Split('.')[1];
                    FilePath = uploaderFilePath + $@"\{filename}";

                    //OrdersDetails.GetAllRecords().Insert(0, new OrdersDetails() { OrderID = OrderID, CustomerID = CustomerID, file = new OrdersDetails.File() { name = filename, onlinePath = uploaderFilePath + $@"\{filename}", size = size, type = '.' + filename.Split('.')[1] } });
                    filename = uploaderFilePath + $@"\{filename}";
                    //if (!Directory.Exists(uploaderFilePath))
                    //{
                    //    Directory.CreateDirectory(uploaderFilePath);
                    //}

                    if (!System.IO.File.Exists(filename))
                    {
                        using (FileStream fs = System.IO.File.Create(filename))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                }
                var docs = _context.scmDocs.Where(cat => cat.id == id).FirstOrDefault();
                if (docs != null)
                {
                    //docs.docId = docID;
                    //docs.distributionId = DistributionId;
                    //docs.documentName = documentName;
                    //docs.message = message;
                    //docs.dateSent = DateTime.Now.Date;
                    //docs.updateDate = DateTime.Now.Date;
                    //docs.userName = Crrentuser.UserName;
                    //docs.fileName = FileName;
                    //docs.fileSize = size;
                    //docs.fileType = FileType;
                    //docs.filePath = FilePath;
                }
                _context.Entry(docs).State = EntityState.Modified;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                try
                {
                    _context.Update(docs);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            catch (Exception e)
            {
                string mm = e.Message;
                Response.Clear();
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
            return Content("");
        }

        public IActionResult DocRemove([FromBody]CRUDModel<scmDocs> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (DocExists(id))
            {
                scmDocs item = _context.scmDocs.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmDocs.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }


            return NoContent();
        }
        private bool DocExists(int id)
        {
            return _context.scmDocs.Any(e => e.id == id);
        }

        [AcceptVerbs("Post")]
        // Upload method for chunk-upload and normal upload
        public IActionResult Save(int Id, int DistributionId, IList<IFormFile> chunkFile, IList<IFormFile> UploadFiles, [FromBody]CRUDModel<scmDocs> value)
        {
            DistributionId= int.Parse(value.Params["ID"].ToString()); 
            long size = 0;
            try
            {
                foreach (var file in UploadFiles)
                {
                    var filename = ContentDispositionHeaderValue
                                    .Parse(file.ContentDisposition)
                                    .FileName
                                    .Trim('"');
                    var folders = filename.Split('/');
                    var uploaderFilePath = hostingEnv.WebRootPath;
                    // for Directory upload
                    if (folders.Length > 1)
                    {
                        for (var i = 0; i < folders.Length - 1; i++)
                        {
                            var newFolder = uploaderFilePath + $@"\{folders[i]}";
                            Directory.CreateDirectory(newFolder);
                            uploaderFilePath = newFolder;
                            filename = folders[i + 1];
                        }
                    }
                    size += file.Length;

                    //OrdersDetails.GetAllRecords().Insert(0, new OrdersDetails() { OrderID = OrderID, CustomerID = CustomerID, file = new OrdersDetails.File() { name = filename, onlinePath = uploaderFilePath + $@"\{filename}", size = size, type = '.' + filename.Split('.')[1] } });
                    filename = uploaderFilePath + $@"\{filename}";
                    if (!System.IO.File.Exists(filename))
                    {
                        using (FileStream fs = System.IO.File.Create(filename))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
            return Content("");
        }
        [HttpGet]
        public FileResult Download(string filename, int id)
        {
            var filePath = Path.Combine(
                      Directory.GetCurrentDirectory(),
                              "wwwroot/documents/" + id);
            Microsoft.Extensions.FileProviders.IFileProvider provider = new PhysicalFileProvider(filePath);
            IFileInfo fileInfo = provider.GetFileInfo(filename);
            var readStream = fileInfo.CreateReadStream();
            var mimeType = "application/pdf";
            return File(readStream, mimeType, filename);
        }

        //Bulk Insert of Distribution 
        public async Task<IActionResult> BulkInsert(int id)
        {
            var getDistSupplyId = _context.scmDistributionsIP.Where(m => m.id == id).FirstOrDefault();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var distmainId = _context.scmDistributionsIP.Where(m => m.id.Equals(id)).FirstOrDefault();

            InsertBulkProcedure(id,getDistSupplyId.stockId, user.TenantId, user.UserName);
            InsertAcknowledgementBulkProcedure((int)distmainId.distributionId, user.UserName);
            return Ok();
        }

        public void InsertBulkProcedure(int? DistId, int? StockId, int TenantId,  string UserName)
        {
           
            DateTime updateDate;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                updateDate = DateTime.Now;
                _context.Database.ExecuteSqlCommand("exec dbo.AddHFDistribution {0},{1},{2},{3},{4}", DistId,StockId, TenantId, UserName, updateDate);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public void InsertAcknowledgementBulkProcedure(int DistId, string UserName)
        {

            DateTime updateDate;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                updateDate = DateTime.Now;
                _context.Database.ExecuteSqlCommand("exec dbo.AddUpdatescmHFsAcknowledgement {0},{1}", DistId, UserName);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //Bulk approval of Distribution 
        public async Task<IActionResult> ApproveHFDistribution(int id)
        {
            var getDistSupplyId = _context.scmDistributionsIP.Where(m => m.id == id).FirstOrDefault();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ApproveHFDistributionProcedure(id, user.TenantId, user.UserName);
            return Ok();
        }
        public void ApproveHFDistributionProcedure(int DistId, int TenantId, string UserName)
        {

            DateTime updateDate;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                updateDate = DateTime.Now;
                _context.Database.ExecuteSqlCommand("exec dbo.ApproveHFDistribution {0},{1},{2}", DistId, TenantId, UserName);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //Bulk approval of Distribution 
        public async Task<IActionResult> DeleteHFDistribution(int id)
        {
            var getDistSupplyId = _context.scmDistributionsIP.Where(m => m.id == id).FirstOrDefault();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            DeleteHFDistributionProcedure(id, user.TenantId, user.UserName);
            return Ok();
        }
        public void DeleteHFDistributionProcedure(int DistId, int TenantId, string UserName)
        {

            DateTime updateDate;

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                updateDate = DateTime.Now;
                _context.Database.ExecuteSqlCommand("exec dbo.DeleteHFDistribution {0},{1},{2}", DistId, TenantId, UserName);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}