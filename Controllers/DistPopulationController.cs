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
    [Authorize(Roles = "dataentry,administrator")]
    public class DistPopulationController : Controller
    {
        private readonly WebNutContext _context;
        private IHostingEnvironment hostingEnv;
        private readonly UserManager<ApplicationUser> _userManager;
        public IActionResult Index()
        {

            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";

            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Upload Data" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "All Uploads"}, Content = ViewBag.content2 });
            ViewBag.headeritems = headerItems;


            var districts = _context.Districts.Select(m => new
            {
                DistrictId = m.DistCode,
                DistrictName = m.DistName
            }).ToList();

            ViewBag.dists = districts;

            return View();
        }
        public DistPopulationController(IHostingEnvironment env,WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            this.hostingEnv = env;
            _context=context;
            _userManager = userManager;
        }
        public IActionResult PopUploadFiles()
        {
            return View();
        }

        [HttpGet]
        public void DeleteTemp()
        {           
          _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [TempDistPopulation]");
        }

        public void UpdateTempFacilityToMain()
        {           
          _context.Database.ExecuteSqlCommand("exec dbo.UpdateDistPopulation");
        }
        [HttpPost("PopUploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(hostingEnv.ContentRootPath+ @"\App_Data\Template\DistPopulation.xlsx", FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);       
                    }
                }
            }
            DeleteTemp();
            ImportPopData(user.TenantId,user.UserName);
            UpdateTempFacilityToMain();
            
            return RedirectToAction("Index");
        }
        public void ImportPopData(int TenantId,string userName)
        {
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            Stream stream = System.IO.File.Open(System.IO.Directory.GetCurrentDirectory() +
                "\\App_Data\\Template\\DistPopulation.xlsx", FileMode.OpenOrCreate);

            IWorkbook workbook = application.Workbooks.Open(stream);

            IWorksheet sheet = workbook.Worksheets[0];
            int firstRow = sheet.UsedRange.Row;
            int lastRow =sheet.UsedRange.LastRow+1;
            int rows =lastRow-firstRow;
            IList<TempDistPopulation> data =  ExportDataFromExcelSheet(sheet,2, 1, rows, TenantId, userName);

             _context.AddRange(data);
            //Saving the updated file
            _context.SaveChanges();

            workbook.Close();
            excelEngine.Dispose();
            stream.Dispose();
        }
        private IList<TempDistPopulation> ExportDataFromExcelSheet(IWorksheet sheet, int startRowIndex, int startColumnIndex, int lastRowIndex,int TenantId,string userName)
        {
             IList<TempDistPopulation> result = new List<TempDistPopulation>();
            for (int r = startRowIndex; r <= lastRowIndex; r++)
            {
                TempDistPopulation hmis = new TempDistPopulation();
                hmis.PopYear = (int)sheet[r, startColumnIndex+1].Number;
                hmis.DistCode = sheet[r, startColumnIndex + 2].Text;
                hmis.Pop =(int)sheet[r, startColumnIndex + 3].Number;
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

            var data =  _context.TempDistPopulation.Where(m=>m.TenantId.Equals(user.TenantId)).ToList();
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
            int count = DataSource.Cast<TempDistPopulation>().Count();
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

            var data = _context.DistPopulation.Where(m => m.TenantId.Equals(user.TenantId)).ToList();
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
            int count = DataSource.Cast<DistPopulation>().Count();
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