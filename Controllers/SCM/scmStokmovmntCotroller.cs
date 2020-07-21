using System;
using System.Collections;
using System.Collections.Generic;
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
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.PivotTables;

namespace DataSystem.Controllers.SCM
{
    public class scmStokmovmntController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmStokmovmntController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = await _context.rptIPStockmovement.ToListAsync();
            if (user.Unicef==0 && user.Pnd==0)
            {
                data = data.Where(m => m.tenantId == user.TenantId).ToList();
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
            int count = DataSource.Cast<rptIPStockmovement>().Count();
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

        //Child grid
        public async Task<IActionResult> CUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = await _context.rptIPStockmovementdetails.ToListAsync();
            if (user.Unicef == 0 && user.Pnd == 0)
            {
                data = data.Where(m => m.tenantId == user.TenantId).ToList();
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
            int count = DataSource.Cast<rptIPStockmovementdetails>().Count();
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

        public IActionResult MovementPivot()
        {
            var pivotData = _context.rptIPStockmovementdetails.ToList();


            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = application.Workbooks.Create(2);

            IWorksheet sheet = workbook.Worksheets[0];
            try
            {
                sheet.Range["A1"].Text = "id";
                sheet.Range["B1"].Text = "period";
                sheet.Range["C1"].Text = "implementer";
                sheet.Range["D1"].Text = "consignee";
                sheet.Range["E1"].Text = "item";
                sheet.Range["F1"].Text = "batchNumber";
                sheet.Range["G1"].Text = "dateFrom";
                sheet.Range["H1"].Text = "dateTo";
                sheet.Range["I1"].Text = "quantity";
                sheet.Range["J1"].Text = "dispatch";
                sheet.Range["K1"].Text = "expiryDate";
                sheet.Range["L1"].Text = "loss";
                sheet.Range["M1"].Text = "damage";
                sheet.Range["N1"].Text = "expiration";
                sheet.Range["O1"].Text = "totalWaste";
                sheet.Range["P1"].Text = "balance";


                sheet.Range["A2"].Text = "%Reports.id";
                sheet.Range["B2"].Text = "%Reports.period";
                sheet.Range["C2"].Text = "%Reports.implementer";
                sheet.Range["D2"].Text = "%Reports.consignee";
                sheet.Range["E2"].Text = "%Reports.item";
                sheet.Range["F2"].Text = "%Reports.batchNumber";
                sheet.Range["G2"].Text = "%Reports.dateFrom";
                sheet.Range["H2"].Text = "%Reports.dateTo";
                sheet.Range["I2"].Text = "%Reports.quantity";
                sheet.Range["J2"].Text = "%Reports.dispatch";
                sheet.Range["K2"].Text = "%Reports.expiryDate";
                sheet.Range["L2"].Text = "%Reports.loss";
                sheet.Range["M2"].Text = "%Reports.damage";
                sheet.Range["N2"].Text = "%Reports.expiration";
                sheet.Range["O2"].Text = "%Reports.totalWaste";
                sheet.Range["P2"].Text = "%Reports.balance";

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

            pivotSheet["A2"].Text = "Stock Movement Report";
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
            pivotTable.Fields["implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["consignee"].Axis = PivotAxisTypes.Page;

            pivotTable.Fields["period"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["item"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["batchNumber"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["expiryDate"].Axis = PivotAxisTypes.Row;

            IPivotField Quantity = pivotTable.Fields["quantity"];
            IPivotField Dispatch = pivotTable.Fields["dispatch"];
            IPivotField Balance = pivotTable.Fields["balance"];
            IPivotField Loss = pivotTable.Fields["loss"];
            IPivotField Damage = pivotTable.Fields["damage"];
            IPivotField Expiration = pivotTable.Fields["expiration"];
            IPivotField TotalWaste = pivotTable.Fields["totalWaste"];


            pivotTable.DataFields.Add(Quantity, "Quantity ", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(Dispatch, "Dispatch ", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(Balance, "Balance ", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(Loss, "Loss ", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(Damage, "Damage ", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(Expiration, "Expiration ", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(TotalWaste, "Total Wastages ", PivotSubtotalTypes.Sum);
            pivotTable.ShowDataFieldInRow = false;

            //(pivotTable.Options as PivotTableOptions).ShowGridDropZone = true;

            IPivotTableOptions option = pivotTable.Options;
            //option.RowLayout = PivotTableRowLayout.Tabular;

            option.ErrorString = "X";
            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleMedium4;

            pivotSheet.Activate();
            string ContentType = "Application/msexcel";
            string filename = "Stock Movement" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

        //Method for creating pivottable -Gridthree
        public IActionResult RequestPivot()
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var pivotData = _context.scmrptRequestpivot.Where(m=>m.RequesttypeId==2).ToList();


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
            string filename = "HF Level Request" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }
        public IActionResult RequestPivotAnnual()
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var pivotData = _context.scmrptRequestpivot.Where(m => m.RequesttypeId == 1).ToList();


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
            string filename = "HF Level Request" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }
    }
}