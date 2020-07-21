using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Navigations;
using Syncfusion.XlsIO;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy = "admin")]
    public class hmisindicatorValuesController : Controller
    {
        private readonly WebNutContext _context;
        private IHostingEnvironment hostingEnv;
        private readonly UserManager<ApplicationUser> _userManager;
        public IActionResult Index()
        {

            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";

            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Upload Screen" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Uploaded Data"}, Content = ViewBag.content2 });
            ViewBag.headeritems = headerItems;


            var facility = _context.FacilityInfo.Select(m => new
            {
                FacilityId = m.FacilityId,
                FacilityName = m.FacilityId + '-' + m.FacilityName
            }).ToList();

            var facilityTypes = _context.FacilityTypes.Select(m => new
            {
                FacilityTypeId = m.FacTypeCode,
                FacilityTypeName = m.FacType
            }).ToList();

            var hmisIndicators = _context.Hmisindicators.Select(m => new
            {
                IndicatorId = m.IndicatorId,
                IndicatorName = m.IndicatorDescription
            }).ToList();

            ViewBag.FacilitySource = facility;
            ViewBag.FacilityTypes = facilityTypes;
            ViewBag.HMISIndicators = hmisIndicators;

            return View();
        }
        public hmisindicatorValuesController(IHostingEnvironment env,WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            this.hostingEnv = env;
            _context=context;
            _userManager = userManager;
        }
        public IActionResult HMISUploadFiles()
        {
            return View();
        }

        [HttpGet]
        public void DeleteTemp()
        {           
          _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [TempHMISIndicatorValues]");
        }

        public void UpdateTempFacilityToMain()
        {           
          _context.Database.ExecuteSqlCommand("exec dbo.UpdateHMISIndicatorValues");
        }
        [HttpPost("HMISUploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(hostingEnv.ContentRootPath+ @"\App_Data\Template\HMISIndicatorValues.xlsx", FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);       
                    }
                }
            }
            DeleteTemp();
            ImportHMISData(user.TenantId,user.UserName);
            UpdateTempFacilityToMain();
            
            return RedirectToAction("Index");
        }
        public void ImportHMISData(int TenantId,string userName)
        {
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            Stream stream = System.IO.File.Open(System.IO.Directory.GetCurrentDirectory() +
                "\\App_Data\\Template\\HMISIndicatorValues.xlsx", FileMode.OpenOrCreate);

            IWorkbook workbook = application.Workbooks.Open(stream);

            IWorksheet sheet = workbook.Worksheets[0];
            int firstRow = sheet.UsedRange.Row;
            int lastRow =sheet.UsedRange.LastRow+1;
            int rows =lastRow-firstRow;
            IList<TemphmisindicatorValues> data =  ExportDataFromExcelSheet(sheet,2, 1, rows, TenantId, userName);

             _context.AddRange(data);
            //Saving the updated file
            _context.SaveChanges();

            workbook.Close();
            excelEngine.Dispose();
            stream.Dispose();
        }
        private IList<TemphmisindicatorValues> ExportDataFromExcelSheet(IWorksheet sheet, int startRowIndex, int startColumnIndex, int lastRowIndex,int TenantId,string userName)
        {
             IList<TemphmisindicatorValues> result = new List<TemphmisindicatorValues>();
            for (int r = startRowIndex; r <= lastRowIndex; r++)
            {
                TemphmisindicatorValues hmis = new TemphmisindicatorValues();
                hmis.FacilityId = (int)sheet[r, startColumnIndex].Number;
                hmis.FacilityTypeId =(int)sheet[r, startColumnIndex + 1].Number;
                hmis.Year = (int)sheet[r, startColumnIndex + 2].Number;
                hmis.Month = (int)sheet[r, startColumnIndex +3].Number;
                hmis.IndicatorId= (int)sheet[r, startColumnIndex + 4].Number;
                hmis.GrantId= sheet[r, startColumnIndex + 5].Text;
                hmis.Program = sheet[r, startColumnIndex +6].Text;
                hmis.Implementer = sheet[r, startColumnIndex +7].Text;
                if((int)sheet[r, startColumnIndex + 8].Number== -2147483648)
                {
                    hmis.Num = 0;
                }
                else
                {
                    hmis.Num = (int)sheet[r, startColumnIndex + 8].Number;
                }
                if((int)sheet[r, startColumnIndex + 9].Number==-2147483648)
                {
                    hmis.Denom = 0;
                }
                else
                {
                    hmis.Denom = (int)sheet[r, startColumnIndex + 9].Number;
                }

                hmis.TenantId = TenantId;
                hmis.UserName = userName;
                hmis.UploadDate = DateTime.Now.Date;

                result.Add(hmis);
            }          
            return result;
        }

        public async Task<IActionResult> UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var data =  _context.TemphmisindicatorValues.Where(m=>m.TenantId.Equals(user.TenantId)).ToList();
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
            int count = DataSource.Cast<TemphmisindicatorValues>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return  dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        public async Task<IActionResult> PermUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var data = _context.HMISIndicatorValues.Where(m => m.tenantId.Equals(user.TenantId)).ToList();
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
            int count = DataSource.Cast<HMISIndicatorValues>().Count();
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
    }
}