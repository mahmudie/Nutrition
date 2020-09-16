using DataSystem.Models;
using DataSystem.Models.SCM;
using DataSystem.Models.ViewModels;
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

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    public class scmRecdispoController : BaseController
    {
        private readonly WebNutContext _context;
        IHostingEnvironment hostingEnv;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmRecdispoController(
            WebNutContext context, 
            UserManager<ApplicationUser> userManager, 
            IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            this.hostingEnv = env;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";

            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Stock Detail", IconCss = "e-tab1" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Recall/Disposal Status", IconCss = "e-tab2" }, Content = ViewBag.content2 });
            ViewBag.headeritems = headerItems;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    ViewBag.gridAdd = false;
                    ViewBag.gridEdit = false;
                    ViewBag.gridDelete = false;

                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    ViewBag.gridAdd = true;
                    ViewBag.gridEdit = true;
                    ViewBag.gridDelete = true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            if (id == null)
            {
                return NotFound();
            }

            var scmTransfers = _context.vscmDistributiontransfer.SingleOrDefault(m => m.id == id);
            if (scmTransfers == null)
            {
                return NotFound();
            }

            var warehouses = _context.scmWarehouses.Where(m => m.LevelId == 2).Select(m => new
            {
                WhId = m.WhId,
                WarehouseName = m.RegionsNav.RegionLong + '(' + m.ProvincesNav.ProvName + ") - " + m.ImplementerNav.ImpAcronym + " = " + m.Location
            }).ToList();

            var wastetypes = _context.scmWasteTypes.Select(m => new
            {
                WasteId = m.Id,
                WasteName = m.name
            }).ToList();

            ViewBag.WarehouseSource = warehouses;
            ViewBag.WasteTypeSource = wastetypes;

            return View(scmTransfers);
        }
        public async Task<IActionResult> UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.vscmDistributiontransfer.ToList();
            if ((user.Unicef == 1 || user.Pnd == 1))
            {
                data = data.ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
            {
                data = data.Where(m => m.tenantId.Equals(user.TenantId)).ToList();
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
            int count = DataSource.Cast<vscmDistributiontransfer>().Count();
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

        public IActionResult EditAttachment(int? id = null)
        {
            id = (id ?? 0);

            if (_context.scmRecalldisposal.Any(m => m.id == id))
            {
                var data = _context.scmRecalldisposal.Find(id);

                var viewModel = new scmAttachment()
                {
                    Id = data.id,
                    AttachmentName = data.Attachment
                };

                return View(viewModel);
            }

            return RedirectToAction("Index", "scmRecdispo");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAttachment(scmAttachment viewModel)
        {
            // check for validation
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // check if record exists
            if (!_context.scmRecalldisposal.Any(m => m.id == viewModel.Id))
            {
                return RedirectToAction("Index", "scmRecdispo");
            }

            // get the current record
            var RecDisp = _context.scmRecalldisposal.Find(viewModel.Id);

            // check if there is a file already if there is then remove it 
            if (!String.IsNullOrWhiteSpace(RecDisp.Attachment))
            {
                var fileToBeDeleted = Path.Combine(this.hostingEnv.WebRootPath, "uploads\\", RecDisp.Attachment); // type the directory in this way for it to work

                if (System.IO.File.Exists(fileToBeDeleted))
                {
                    System.IO.File.Delete(fileToBeDeleted);
                }

                RecDisp.Attachment = null;
            }

            // handle the upload 
            var file = viewModel.Attachment;

            if (file != null && file.Length > 0)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var filename = "StocksRecallDisposalDoc_" + viewModel.Id + fileExtension;
                var filepath = this.hostingEnv.WebRootPath + _uploadDir + filename;

                if (_allowedExtensions.Contains(fileExtension))
                {
                    try
                    {
                        // save file 
                        using (var stream = System.IO.File.Create(filepath))
                        {
                            file.CopyToAsync(stream);
                        }

                        // update database
                        RecDisp.Attachment = filename;
                        _context.Entry(RecDisp).State = EntityState.Modified;
                        _context.SaveChanges();

                        AlertSuccess("File uploaded successfully.");

                        return Redirect(Url.Action("EditAttachment", "scmRecdispo") + "?Id=" + viewModel.Id);
                    }
                    catch
                    {
                        AlertError("Error uploading file.");

                        return Redirect(Url.Action("EditAttachment", "scmRecdispo") + "?id=" + viewModel.Id);
                    }
                }
                else
                {
                    AlertError("Invalid file type.");

                    return Redirect(Url.Action("EditAttachment", "scmRecdispo") + "?id=" + viewModel.Id);
                }
            }
            else
            {
                AlertError("No file selected.");

                return Redirect(Url.Action("EditAttachment", "scmRecdispo") + "?id=" + viewModel.Id);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DownloadAttachment(scmAttachment viewModel)
        {
            // check if record exists
            if (!_context.scmRecalldisposal.Any(m => m.id == viewModel.Id))
            {
                AlertError("Record not found.");

                return RedirectToAction("Index", "scmRecdispo");
            }

            var RecDisp = _context.scmRecalldisposal.Find(viewModel.Id);

            if (String.IsNullOrWhiteSpace(RecDisp.Attachment))
            {
                AlertError("File not found.");

                return RedirectToAction("Index", "scmRecdispo");
            }

            var filepath = this.hostingEnv.WebRootPath + _uploadDir + RecDisp.Attachment;

            return PhysicalFile(filepath, "application/octet-stream", RecDisp.Attachment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveAttachment(scmAttachment viewModel)
        {
            // check if record exists 
            if (!_context.scmRecalldisposal.Any(m => m.id == viewModel.Id))
            {
                return Redirect(Url.Action("EditAttachment", "scmRecdispo") + "?id=" + viewModel.Id);
            }

            // get the current record
            var RecDisp = _context.scmRecalldisposal.Find(viewModel.Id);

            // check if there is a file already if there is then remove it 
            if (!String.IsNullOrWhiteSpace(RecDisp.Attachment))
            {
                var fileToBeDeleted = Path.Combine(this.hostingEnv.WebRootPath, "uploads\\", RecDisp.Attachment);

                if (System.IO.File.Exists(fileToBeDeleted))
                {
                    System.IO.File.Delete(fileToBeDeleted);
                }

                RecDisp.Attachment = null;
            }

            // update database
            _context.Entry(RecDisp).State = EntityState.Modified;
            _context.SaveChanges();

            AlertSuccess("File removed successfully.");

            return Redirect(Url.Action("EditAttachment", "scmRecdispo") + "?Id=" + viewModel.Id);
        }

        public async Task<IActionResult> DisposalUrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.scmRecalldisposal.ToList();
            if ((user.Unicef == 1 || user.Pnd == 1))
            {
                data = data.ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
            {
                data = data.Where(m => m.tenantId.Equals(user.TenantId)).ToList();
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
            int count = DataSource.Cast<scmRecalldisposal>().Count();
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
        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmRecalldisposal> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmRecalldisposal item = new scmRecalldisposal();
            if (item == null) { return BadRequest(); }

            item.ipdistributionId = int.Parse(value.Params["ID"].ToString());
            item.whId = value.Value.whId;
            item.wasteId = value.Value.wasteId;
            item.dateOfRecall = value.Value.dateOfRecall;
            item.dateOfDisposal = value.Value.dateOfDisposal;
            item.quantity = value.Value.quantity;
            item.placeDisposal = value.Value.placeDisposal;
            item.tenantId = user.TenantId;
            item.userName = user.UserName;
            item.updateDate = DateTime.Now.Date;
            item.remarks = value.Value.remarks;

            try
            {
                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    return NoContent();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.Add(item);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> Update([FromBody]CRUDModel<scmRecalldisposal> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var item = _context.scmRecalldisposal.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (item != null)
            {
                item.ipdistributionId = int.Parse(value.Params["ID"].ToString());
                item.whId = value.Value.whId;
                item.wasteId = value.Value.wasteId;
                item.dateOfRecall = value.Value.dateOfRecall;
                item.dateOfDisposal = value.Value.dateOfDisposal;
                item.quantity = value.Value.quantity;
                item.placeDisposal = value.Value.placeDisposal;
                item.tenantId = user.TenantId;
                item.userName = user.UserName;
                item.updateDate = DateTime.Now.Date;
                item.remarks = value.Value.remarks;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(item).State = EntityState.Modified;

            try
            {
                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    return NoContent();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.Update(item);
                    _context.SaveChanges();
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

        public async Task<IActionResult> Remove([FromBody]CRUDModel<scmRecalldisposal> Value)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmRecalldisposal item = _context.scmRecalldisposal.Where(m => m.id.Equals(id)).FirstOrDefault();

                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    return NoContent();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.scmRecalldisposal.Remove(item);
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
            return _context.scmRecalldisposal.Any(e => e.id == id);
        }
    }
}