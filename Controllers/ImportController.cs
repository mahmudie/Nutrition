using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator")]
    [Authorize(Policy = "admin")]
    public class ImportController : Controller
    {
        private readonly WebNutContext _context;
       private IHostingEnvironment hostingEnv;
        public IActionResult Index()
        {
            return View();
        }
        public ImportController(IHostingEnvironment env,WebNutContext context)
        {
            this.hostingEnv = env;
            _context=context;
        }
        public IActionResult UploadFiles()
        {
            return View();
        }

        [HttpGet]
        public void DeleteTemp()
        {           
          _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE [TempFacility]");
        }

        public void UpdateTempFacilityToMain()
        {           
          _context.Database.ExecuteSqlCommandAsync("exec dbo.UpdateFacilityInfo");
        }
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(hostingEnv.ContentRootPath+@"\App_Data\Template\HealthFacilities.xlsx", FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);       
                    }
                }
            }
            DeleteTemp();
            ImportData();
            UpdateTempFacilityToMain();
            
            return RedirectToAction("Index","FacilityInfo");
        }
        public void ImportData()
        {
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            Stream stream = System.IO.File.Open(System.IO.Directory.GetCurrentDirectory() +
                "\\App_Data\\Template\\HealthFacilities.xlsx", FileMode.OpenOrCreate);

            IWorkbook workbook = application.Workbooks.Open(stream);

            IWorksheet sheet = workbook.Worksheets[0];
            int firstRow = sheet.UsedRange.Row;
            int lastRow =sheet.UsedRange.LastRow+1;
            int rows =lastRow-firstRow;
           // IList<TempFacility> data =  ExportDataFromExcelSheet(sheet,2, 1, rows);

            // _context.AddRange(data);
            //Saving the updated file
            _context.SaveChanges();

            workbook.Close();
            excelEngine.Dispose();
            stream.Dispose();
        }
        //private IList<TempFacility> ExportDataFromExcelSheet(IWorksheet sheet, int startRowIndex, int startColumnIndex, int lastRowIndex)
        //{
        //    IList<TempFacility> result = new List<TempFacility>();
        //    for (int r = startRowIndex; r <= lastRowIndex; r++)
        //    {
        //        TempFacility Facility = new TempFacility();
        //        Facility.FacilityID = (int)sheet[r, startColumnIndex].Number;
        //        Facility.DistCode = sheet[r, startColumnIndex + 1].Value;
        //        Facility.FacilityName = sheet[r, startColumnIndex + 2].Text;
        //        Facility.FacilityNameDari = sheet[r, startColumnIndex + 3].Text;
        //        Facility.FacilityNamePashto = sheet[r, startColumnIndex + 4].Text;
        //        Facility.FacilityType = (int)sheet[r, startColumnIndex + 5].Number;
        //        Facility.Location = sheet[r, startColumnIndex + 6].Text;
        //        Facility.LocationDari = sheet[r, startColumnIndex + 7].Text;
        //        Facility.LocationPashto = sheet[r, startColumnIndex + 8].Text;
        //        Facility.ViliCode = sheet[r, startColumnIndex + 9].Text;
        //        if (sheet[r, startColumnIndex + 10].HasNumber)
        //            Facility.Lat = Convert.ToDouble(sheet[r, startColumnIndex + 10].Number);
        //        if (sheet[r, startColumnIndex + 11].HasNumber)
        //            Facility.Lon = Convert.ToDouble(sheet[r, startColumnIndex + 11].Number);
        //        Facility.Implementer = sheet[r, startColumnIndex + 12].Text;
        //        Facility.SubImplementer = sheet[r, startColumnIndex + 13].Text;
        //        Facility.ActiveStatus = sheet[r, startColumnIndex + 14].Text;
        //        Facility.DateEstablished = sheet[r, startColumnIndex + 15].DateTime;
        //        result.Add(Facility);
        //    }          
        //    return result;
        //}
    }
}