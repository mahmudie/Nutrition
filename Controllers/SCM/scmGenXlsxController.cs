using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;
using System;
using System.IO;
using System.Linq;
using Syncfusion.EJ2.Spreadsheet;

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    public class scmGenXlsxController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public scmGenXlsxController(WebNutContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(int id)
        {

            var data = _context.scmdashrequest.Where(m=>m.RequestId.Equals(id))
                .GroupBy(m=>new {m.RequestId, m.Tracker, m.Program, m.Item })
                .Select(m=>new
                {
                    RequestId = m.Key.RequestId,
                    Tracker=m.Key.Tracker,
                    Program=m.Key.Program,
                    Item=m.Key.Item,
                    Children=m.Sum(s=>s.Children),
                    Ballance=m.Sum(s=>s.Ballance),
                    NewStock=m.Sum(s=>s.NewStock),
                    BufferStock=m.Sum(s=>s.BufferStock),
                    Adjustment=m.Sum(s=>s.Adjustment),
                    Total =m.Sum(s=>s.Total)
                }).ToList();

            if (!data.Any())
            {
                return BadRequest();
            }

            //Instantiate the spreadsheet creation engine.
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet worksheet = workbook.Worksheets[0];

            this._context.Database.SetCommandTimeout(3000);

            //Merging and additing title
            worksheet.Range["A2"].Text = "Implementer Level Request";
            worksheet.Range["A2:J2"].Merge();
            worksheet.Range["A2"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

            worksheet.Range["A2"].CellStyle.Font.Size = 16;
            worksheet.Range["A2"].CellStyle.Font.Bold = true;
            //Add Column labels
            worksheet.Range["A3"].Text = "RequestId";
            worksheet.Range["B3"].Text = "Tracker";
            worksheet.Range["C3"].Text = "Program";
            worksheet.Range["D3"].Text = "Stock Item";
            worksheet.Range["E3"].Text = "Total Children";
            worksheet.Range["F3"].Text = "Estimated Stock";
            worksheet.Range["G3"].Text = "Buffer Stock";
            worksheet.Range["H3"].Text = "Balance";
            worksheet.Range["I3"].Text = "Adjustment";
            worksheet.Range["J3"].Text = "Total";

            worksheet.Range["A3:J3"].CellStyle.Font.Size = 12;
            worksheet.Range["A3:J3"].CellStyle.Font.Bold = true;


            int startRow = 4;
            foreach (var j in data)
            {
                if (j.RequestId > 0)
                {
                    worksheet.Range["A" + startRow].Number = j.RequestId;
                    worksheet.Range["B" + startRow].Text = j.Tracker;
                    worksheet.Range["C" + startRow].Text = j.Program;
                    worksheet.Range["D" + startRow].Text = j.Item;
                    worksheet.Range["E" + startRow].Number = j.Children;
                    worksheet.Range["F" + startRow].Number = j.NewStock;
                    worksheet.Range["G" + startRow].Number = j.BufferStock;
                    worksheet.Range["H" + startRow].Number = j.Ballance;
                    worksheet.Range["I" + startRow].Number = j.Adjustment;
                    worksheet.Range["J" + startRow].Number = (double)j.Total;
                    
                }
                startRow += 1;
            }

            worksheet.Range["A2"].Text = "Implementer Level Request";
            worksheet.Range["A2:J2"].Merge();
            worksheet.Range["A2"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

            worksheet.Range["A2"].CellStyle.Font.Size = 16;
            worksheet.Range["A2"].CellStyle.Font.Bold = true;

            worksheet.AutofitColumn(2);
            worksheet.AutofitColumn(3);
            worksheet.AutofitColumn(4);
            worksheet.AutofitColumn(5);
            worksheet.AutofitColumn(6);
            worksheet.AutofitColumn(7);
            worksheet.AutofitColumn(8);
            worksheet.AutofitColumn(9);

            if (System.IO.File.Exists(ResolveApplicationPath("SCM_request.xlsx")))
            {
                try
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(ResolveApplicationPath("SCM_request.xlsx"));
                }
                catch (Exception e) { }
            }

            worksheet.Name = "Requests IP Level";
            MemoryStream ms = new MemoryStream();
            FileStream outputStream = new FileStream(ResolveApplicationPath("SCM_request.xlsx"), FileMode.Create);

            workbook.SaveAs(outputStream); 

            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return RedirectToAction("Spreadsheet");
            //return File(ms, ContentType, ResolveApplicationPath("SCM_request.xlsx"));
        }

        public IActionResult Spreadsheet()
        {
            return View();
        }

        private string ResolveApplicationPath(string fileName)
        {
            return _hostingEnvironment.WebRootPath + "//Template//" + fileName;
        }

        public IActionResult Open(IFormCollection openRequest)
        {
            OpenRequest open = new OpenRequest();
            open.File = openRequest.Files[0];
            return Content(Workbook.Open(open));
        }

        public IActionResult Save(SaveSettings saveSettings)
        {
            return Workbook.Save(saveSettings);
        }
    }
}