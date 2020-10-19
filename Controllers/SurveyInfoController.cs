using DataSystem.Models;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Navigations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator,unicef,pnd,otherentry")]
    //[Authorize(Policy = "admin")]
    public class SurInfoController : Controller
    {
        IHostingEnvironment hostingEnv;
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        protected readonly List<string> AlowedExtensions;
        public SurInfoController(WebNutContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            hostingEnv = env;
            AlowedExtensions = new List<string>() { ".doc", ".docx", ".pdf", ".xls", ".xlsx" };
        }

        // GET: FacilityInfo
        public IActionResult Index()
        {
            var survinfo = _context.SurInfo.ToList();
            return View(survinfo);
        }

        public IActionResult SurvUrlDatasource([FromBody]DataManagerRequest dm)
        {

            var data = _context.SurInfo.ToList();

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
            int count = DataSource.Cast<SurveyInfo>().Count();
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

        //[AcceptVerbs("Post")]
        //public async Task<IActionResult> SurvInsert(string value, IList<IFormFile> UploadFiles)
        //{
        //    int docID = 0;
        //    string documentName = null, message = null;
        //    string pathSave = null;
        //    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
        //    // Here you can do deserialization based on your Model class  

        //    foreach (var dat in data)
        //    {
        //        ;
        //        var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(dat.Value));
        //        foreach (var check in d)
        //        {
        //            if (check.Key == "docId")
        //            {
        //                docID = Convert.ToInt32(check.Value);

        //            }
        //            if (check.Key == "documentName")
        //            {
        //                documentName = Convert.ToString(check.Value);

        //            }
        //            if (check.Key == "message")
        //            {
        //                message = Convert.ToString(check.Value);

        //            }
        //        }
        //    }

        //    var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);
        //    long size = 0;
        //    string FileName = null, FileType = null, FilePath = null;

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        foreach (var file in UploadFiles)
        //        {
        //            var filename = ContentDispositionHeaderValue
        //                            .Parse(file.ContentDisposition)
        //                            .FileName
        //                            .Trim('"');
        //            var folders = filename.Split('/');
        //            var uploaderFilePath = hostingEnv.WebRootPath + @"\survey\" + SurveyId;
        //            pathSave = $@"\wwwroot\survey\" + SurveyId;
        //            // for Directory upload
        //            if (folders.Length > 1)
        //            {
        //                for (var i = 0; i < folders.Length - 1; i++)
        //                {
        //                    var newFolder = uploaderFilePath + $@"\{folders[i]}";
        //                    Directory.CreateDirectory(newFolder);
        //                    uploaderFilePath = newFolder;
        //                    filename = folders[i + 1];
        //                }
        //            }
        //            size += file.Length;

        //            FileName = filename;
        //            FileType = '.' + filename.Split('.')[1];
        //            FilePath = uploaderFilePath + $@"\{filename}";

        //            //OrdersDetails.GetAllRecords().Insert(0, new OrdersDetails() { OrderID = OrderID, CustomerID = CustomerID, file = new OrdersDetails.File() { name = filename, onlinePath = uploaderFilePath + $@"\{filename}", size = size, type = '.' + filename.Split('.')[1] } });
        //            filename = uploaderFilePath + $@"\{filename}";
        //            if (!Directory.Exists(uploaderFilePath))
        //            {
        //                Directory.CreateDirectory(uploaderFilePath);
        //            }
        //            if (!System.IO.File.Exists(filename))
        //            {
        //                using (FileStream fs = System.IO.File.Create(filename))
        //                {
        //                    file.CopyTo(fs);
        //                    fs.Flush();
        //                }
        //            }
        //        }
        //        scmDocs docs = new scmDocs();
        //        if (docs == null) { return BadRequest(); }
        //        docs.docId = docID;
        //        docs.distributionId = DistributionId;
        //        docs.documentName = documentName;
        //        docs.message = message;
        //        docs.dateSent = DateTime.Now.Date;
        //        docs.updateDate = DateTime.Now.Date;
        //        docs.userName = Crrentuser.UserName;
        //        docs.fileName = FileName;
        //        docs.fileSize = size;
        //        docs.fileType = FileType;
        //        docs.filePath = FilePath;

        //        _context.SurInfo.Add(new SurveyInfo
        //        {
        //            SurveyAccro = survInfo.SurveyAccro,
        //            SurveyFull = survInfo.SurveyFull,
        //            StartDate = survInfo.StartDate,
        //            EndDate = survInfo.EndDate,
        //            LeadBy = survInfo.LeadBy,
        //            ImpBy = survInfo.ImpBy,
        //            SurveyYear = survInfo.SurveyYear,
        //            Abstract = survInfo.Abstract,
        //            ReportLink = survInfo.ReportLink,
        //            DatasetLink = survInfo.DatasetLink,
        //            DatasetDetails = survInfo.DatasetDetails,
        //            UserName = users.UserName,
        //            UpdateDate = DateTime.Now,
        //            TenantId = users.TenantId,
        //            Writers = survInfo.Writers,
        //            FileName = FileName,
        //            FileType = FileType,
        //            FilePath = FilePath,
        //        });
        //        _context.SaveChanges();
        //    }

        //    catch (Exception e)
        //    {
        //        string mm = e.Message;
        //        Response.Clear();
        //        Response.StatusCode = 204;
        //        Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
        //        Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
        //    }
        //    return Content("");
        //}
        public IActionResult PageData(IDataTablesRequest request)
        {
            var data = _context.SurInfo.AsNoTracking().ToList();
            List<SurveyInfo> SurveyData;
            if (String.IsNullOrWhiteSpace(request.Search.Value))
            {
                SurveyData = data;
            }
            else
            {

                int a;
                bool result = int.TryParse(request.Search.Value, out a);

                if (result)
                {
                    SurveyData = data.Where(_item => _item.SurveyId == a).ToList();
                }

                else if (!result)
                {
                    string search = request.Search.Value.Trim();
                    SurveyData = data.Where(_item => _item.SurveyAccro != null && _item.SurveyAccro.ToLower().Contains(search.ToLower())
                    || _item.SurveyFull != null && _item.SurveyFull.Contains(search)
                    || _item.SurveyFull != null && _item.SurveyFull.Contains(search)
                    || _item.SurveyYear != 0 && _item.SurveyYear.Equals(search)
                    || _item.LeadBy != null && _item.LeadBy.Equals(search)
                    || _item.ImpBy != null && _item.ImpBy.Equals(search)
                    || _item.Writers != null && _item.Writers.Contains(search)).ToList();
                }
                else
                {
                    SurveyData = data;
                }
            }
            var dataPage = SurveyData.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, data.Count(), SurveyData.Count(), dataPage);
            return new DataTablesJsonResult(response, true);
        }
        // GET: FacilityInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survInfo = await _context.SurInfo.SingleOrDefaultAsync(m => m.SurveyId == id);
            if (survInfo == null)
            {
                return NotFound();
            }
            return View(survInfo);
        }

        // GET: FacilityInfo/Create
        public IActionResult Create()
        {
            var viewModel = new SurveyInfovm();
            return View(viewModel);
        }


        // POST: FacilityInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyInfovm survInfo)
        {

            string fileExtension = null;
            string filename = null;
            string filepath = null;
            string filepath2 = null;

            var users = _userManager.Users.Where(m => m.UserName.Equals(User.Identity.Name)).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var file = survInfo.Attachment;

                if (file != null && file.Length > 0)
                {
                    fileExtension = Path.GetExtension(file.FileName);

                    if (AlowedExtensions.Contains(fileExtension))
                    {
                        var subFolder = "";
                        subFolder = survInfo.SurveyYear +"_"+ survInfo.SurveyAccro;
                        var newFolder = "";
                        newFolder = @"wwwroot\survey\" + subFolder;
                        filename = file.FileName;
                        filepath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\survey\" + subFolder+@"\", filename );
                        Directory.CreateDirectory(newFolder);
                        var stream = new FileStream(filepath, FileMode.Create);

                        file.CopyTo(stream);
                        filepath2 = subFolder;
                    }
                }

                _context.SurInfo.Add(new SurveyInfo
                {
                    SurveyAccro = survInfo.SurveyAccro,
                    SurveyFull = survInfo.SurveyFull,
                    LeadBy = survInfo.LeadBy,
                    ImpBy = survInfo.ImpBy,
                    SurveyYear = survInfo.SurveyYear,
                    Abstract = survInfo.Abstract,
                    UserName = users.UserName,
                    UpdateDate = DateTime.Now,
                    TenantId = users.TenantId,
                    Writers = survInfo.Writers,
                    FileName = filename,
                    Attachment = filename,
                    FileType = fileExtension,
                    FilePath = filepath2,

                });
                try
                {
                    if (User.Identity.IsAuthenticated & (User.IsInRole("unicef")|| User.IsInRole("pnd")))
                    {
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {

                }
                return RedirectToAction("Index");
            }

            return View(survInfo);
        }
        // GET: FacilityInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";

            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Survey Info", IconCss = "e-tab1" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Survey Indicator Details", IconCss = "e-tab2" }, Content = ViewBag.content2 });
            ViewBag.headeritems = headerItems;



            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.SurInfo.SingleOrDefaultAsync(m => m.SurveyId == id);

            if (survey == null)
            {
                return NotFound();
            }


            getSurvInfos.SurvId = id;
            helpers.SurveyHelperMainId = id;

            var categories = _context.LkpCategories.Select(m => new
            {
                CategoryId = m.CategoryId,
                CategoryName = m.CategoryName
            }).ToList();

            var themes = _context.LkpThematicAreas.Select(m => new
            {
                ThemeId = m.ThemeId,
                ThematicArea = m.ThematicArea
            }).ToList();


            var disaggregation = _context.LkpDisaggregations.Select(m => new
            {
                DisaggregId = m.DisaggregId,
                Disaggregation = m.Disaggregation
            }).ToList();

            var viewModel = new SurveyInfovm()
            {
                SurveyId = survey.SurveyId,
                SurveyAccro = survey.SurveyAccro,
                SurveyFull = survey.SurveyFull,
                LeadBy = survey.LeadBy,
                ImpBy = survey.ImpBy,
                SurveyYear = survey.SurveyYear,
                Abstract = survey.Abstract,
                UserName = survey.UserName,
                UpdateDate = survey.UpdateDate,
                Writers = survey.Writers,
                DeleteAttachement=false,
                FileName=survey.FileName,
                FilePath=survey.FilePath
            };

            var indicators = _context.lkpSurveyIndicators.Select(m => new
            {
                IndicatorId = m.indicatorId,
                IndicatorName =m.indicatorName
            }).ToList();

            ViewBag.CategoriesSource = categories;
            ViewBag.ThemesSource = themes;
            ViewBag.DisaggregationSource = disaggregation;
            ViewBag.IndicatorSource = indicators;


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SurveyInfovm survInfo)
        {

            string fileExtension = null;
            string filename = null;
            string filepath = null;
            string filepath2 = null;

            var users = await _userManager.Users.Where(m => m.UserName.Equals(User.Identity.Name)).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                //DElete 

                var surv = _context.SurInfo.SingleOrDefault(m => m.SurveyId == id);
                if (survInfo.DeleteAttachement == true)
                {
                    var deletePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\survey\" + surv.FilePath+@"\", surv.Attachment);

                    if (System.IO.File.Exists(deletePath))
                    {
                        try
                        {
                            System.GC.Collect();
                            System.GC.WaitForPendingFinalizers();
                            System.IO.File.Delete(deletePath);

                        }
                        catch (Exception e) { }
                    }
                }
                //edit
                var file = survInfo.Attachment;

                if (file != null && file.Length > 0)
                {
                    fileExtension = Path.GetExtension(file.FileName);

                    if (AlowedExtensions.Contains(fileExtension))
                    {
                        var subFolder = "";
                        subFolder = survInfo.SurveyYear + "_" + survInfo.SurveyAccro;
                        var newFolder = "";
                        newFolder = @"wwwroot\survey\" + subFolder;
                        filename = file.FileName;
                        filepath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\survey\" + subFolder, filename);
                        Directory.CreateDirectory(newFolder);
                        var stream = new FileStream(filepath, FileMode.Create);

                        file.CopyTo(stream);
                        filepath2 = subFolder;
                    }
                }
                try
                {

                    var survey = _context.SurInfo.Find(survInfo.SurveyId);
                    survey.SurveyAccro = survInfo.SurveyAccro;
                    survey.SurveyFull = survInfo.SurveyFull;
                    survey.LeadBy = survInfo.LeadBy;
                    survey.ImpBy = survInfo.ImpBy;
                    survey.SurveyYear = survInfo.SurveyYear;
                    survey.Abstract = survInfo.Abstract;
                    survey.UserName = users.UserName;
                    survey.UpdateDate = DateTime.Now.Date;
                    survey.Writers = survInfo.Writers;
                    survey.TenantId = users.TenantId;
                    if (!String.IsNullOrEmpty(filename))
                    {
                        survey.FileName = filename;
                        survey.Attachment = filename;
                        survey.FileType = fileExtension;
                        survey.FilePath = filepath2;
                    }

                    _context.Update(survey);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityInfoExists(survInfo.SurveyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "SurInfo", id);
            }

            return NoContent();
        }

        public IActionResult ChangeAttachment(int Id)
        {
            var surv = _context.SurInfo.Find(Id);

              var viewModel = new SurveyInfovm()
            {
                SurveyId = surv.SurveyId,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeAttachment(SurveyInfovm viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var survinfo = _context.SurInfo.Find(viewModel.SurveyId);

            // check if previous file exists, if so remove it before adding new
            if (survinfo.Attachment != null && survinfo.Attachment != "")
            {
                var deletePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\survey" + survinfo.FilePath, survinfo.Attachment);

                if (System.IO.File.Exists(deletePath))
                {
                    try
                    {
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        System.IO.File.Delete(deletePath);

                    }
                    catch (Exception e) { }
                }

                survinfo.Attachment = null;

                _context.Entry(survinfo).State = EntityState.Modified;
                _context.SaveChanges();
            }

            // add the new attachment

            var file = viewModel.Attachment;

            string fileExtension = null;
            string filename = null;
            string filepath = null;
            string filepath2 = null;

            if (file != null && file.Length > 0)
            {
                fileExtension = Path.GetExtension(file.FileName);

                if (AlowedExtensions.Contains(fileExtension))
                {
                    var subFolder = "";
                    subFolder = survinfo.SurveyYear + "_" + survinfo.SurveyAccro;
                    var newFolder = "";
                    newFolder = @"wwwroot\survey\" + subFolder;
                    filename = file.FileName;
                    filepath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\survey\" + subFolder, filename + fileExtension);
                    Directory.CreateDirectory(newFolder);
                    var stream = new FileStream(filepath, FileMode.Create);

                    file.CopyTo(stream);
                    filepath2 = @"\" + subFolder + @"\";
                }
            }
                survinfo.Attachment = filename + fileExtension;
                survinfo.FilePath = filepath2;
                survinfo.FileType = fileExtension;
                survinfo.FileName = filename + fileExtension;

                _context.Entry(survinfo).State = EntityState.Modified;
                _context.SaveChanges();

                return RedirectToAction("Edit", "SurvInfo",survinfo.SurveyId);           
        }
        // GET: FacilityInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survInfo = await _context.SurInfo.SingleOrDefaultAsync(m => m.SurveyId == id);
            if (survInfo == null)
            {
                return NotFound();
            }

            return View(survInfo);
        }

        // POST: FacilityInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var survInfo = await _context.SurInfo.SingleOrDefaultAsync(m => m.SurveyId == id);
            var deletePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\survey\"+ survInfo.FilePath+@"\", survInfo.Attachment);
            var path = @"wwwroot\survey\" + survInfo.FilePath + @"\";
            if (System.IO.File.Exists(deletePath))
            {
                try
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(deletePath);

                    bool isEmpty = !Directory.EnumerateFiles(path).Any();
                    if (isEmpty)
                        System.IO.Directory.Delete(path);
                }
                catch (Exception e) { }
            }
            _context.SurInfo.Remove(survInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Categories()
        {
            var Categories = _context.LkpCategories.Select(m => new
            {
                CategoryId = m.CategoryId,
                CategoryName = m.CategoryName
            }).ToList();

            return Json(Categories);
        }

        public IActionResult Disaggregation([FromBody]ExtendedDataManager dm)
        {
            //if(dm.Where.count())
            //List<LkpDisaggregation> Disaggreagtion = new List<LkpDisaggregation>() ;
            //IEnumerable Disaggregation;
            //if (dm.Where.Count == 0)
            //{
            //    Disaggreagtion = _context.LkpDisaggregations.Select(m => new {
            //        DisaggregId = m.DisaggregId,
            //        CategoryId = m.CategoryId,
            //        Disaggregation = m.Disaggregation
            //    }).ToList();
            //}
            //else
            //{
            //    Disaggreagtion = _context.LkpDisaggregations.Select(m => new {
            //        DisaggregId = m.DisaggregId,
            //        CategoryId = m.CategoryId,
            //        Disaggregation = m.Disaggregation
            //    }).Where(m => m.CategoryId == (int)dm.Where[0].value).ToList();
            //}
            var data = _context.LkpDisaggregations.ToList();
            var dataop = new DataOperations();
            IEnumerable GridData = null;
            if (dm.Where != null && dm.Where.Count > 0)
            {
                GridData = dataop.PerformFiltering(data, dm.Where, dm.Where[0].Operator);
            }
            else
            {
                GridData = data;
            }

            //var Disaggregation = _context.LkpDisaggregations.Select(m => new
            //{
            //    DisaggregId = m.DisaggregId,
            //    CategoryId = m.CategoryId,
            //    Disaggregation = m.Disaggregation
            //}).Where(m=>m.CategoryId==(int)dm.Where[0].value).ToList();

            return dm.RequiresCounts ? Json(new { result = GridData, count = GridData.Cast<LkpDisaggregation>().Count() }) : Json(GridData);
        }

        public class ExtendedDataManager : DataManagerRequest
        {
            public IDictionary<string, string> @params;
        }
        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.SurveyResults.Where(f => f.SurveyId.Equals(getSurvInfos.SurvId)).ToList();

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
            int count = DataSource.Cast<SurveyResults>().Count();
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

        public IActionResult Insert([FromBody]CRUDModel<SurveyResults> value)
        {

            int? id = helpers.SurveyHelperMainId;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var users = _userManager.Users.Where(usr => usr.UserName.Equals(User.Identity.Name)).FirstOrDefault();
            //SurveyResults survResults = _context.SurveyResults.FirstOrDefault(m => m.SurveyId == id);
            SurveyResults survResults = new SurveyResults();
            if (survResults == null) { return BadRequest(); }

            survResults.SurveyId = (int)id;
            survResults.CategoryId = value.Value.CategoryId;
            survResults.DisaggregId = value.Value.DisaggregId;
            survResults.ThemeId = value.Value.ThemeId;
            survResults.IndicatorId = value.Value.IndicatorId;
            survResults.IndicatorValue = value.Value.IndicatorValue;
            survResults.CINational = value.Value.CINational;
            survResults.Year = value.Value.Year;
            survResults.Month = value.Value.Month;
            survResults.UserName = users.UserName;
            survResults.UpdateDate = DateTime.Now;
            survResults.TenantId = users.TenantId;
            survResults.Remarks = value.Value.Remarks;


            try
            {
                _context.Add(survResults);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public IActionResult Update([FromBody]CRUDModel<SurveyResults> value)
        {
            var users = _userManager.Users.Where(usr => usr.UserName.Equals(User.Identity.Name)).FirstOrDefault();
            var survResults = _context.SurveyResults.Where(srv => srv.IndResultId == value.Value.IndResultId).FirstOrDefault();
            if (survResults != null)
            {
                survResults.DisaggregId = value.Value.DisaggregId;
                survResults.ThemeId = value.Value.ThemeId;
                survResults.IndicatorId = value.Value.IndicatorId;
                survResults.IndicatorValue = value.Value.IndicatorValue;
                survResults.CINational = value.Value.CINational;
                survResults.Year = value.Value.Year;
                survResults.CategoryId = value.Value.CategoryId;
                survResults.Month = value.Value.Month;
                survResults.UserName = users.UserName;
                survResults.UpdateDate = DateTime.Now;
                survResults.TenantId = users.TenantId;
                survResults.Remarks = value.Value.Remarks;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(survResults).State = EntityState.Modified;

            try
            {
                _context.Update(survResults);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(value.Value.IndResultId))
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

        public IActionResult Remove([FromBody]CRUDModel<SurveyResults> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                SurveyResults item = _context.SurveyResults.Where(m => m.IndResultId.Equals(id)).FirstOrDefault();
                _context.SurveyResults.Remove(item);
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
            return _context.SurveyResults.Any(e => e.IndResultId == id);
        }

        private bool FacilityInfoExists(int id)
        {
            return _context.SurInfo.Any(e => e.SurveyId == id);
        }
    }

}