using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    public class scmDistributionsIPController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmDistributionsIPController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var UsersItems = _context.scmUsersset.Where(m=>m.IsUnicefPnd==1).Select(m => new
            {
                Id = m.Id,
                User = m.UserName
            }).ToList();

            var ImpUsersItems = _context.scmUsersset.Where(m => m.IsUnicefPnd == 0).Select(m => new
            {
                Id = m.Id,
                User = m.UserName
            }).ToList();
            var items = _context.TlkpSstock.Select(m => new
            {
                ItemId = m.SstockId,
                ItemName = m.Item
            }).ToList();

            var warehouses = _context.scmWarehouses.Where(m=>m.LevelId==2).Select(m => new
            {
                WhId = m.WhId,
                WarehouseName = m.RegionsNav.RegionLong+'('+m.ProvincesNav.ProvName+") - "+m.ImplementerNav.ImpAcronym +" = "+m.Location
            }).ToList();

            var stockbalance= _context.scmStockBalance.Select(m => new
            {
                StockId = m.StockId,
                StockItem = m.StockItem
            }).ToList();


            ViewBag.ItemSource = items;
            ViewBag.WarehouseSource = warehouses;
            ViewBag.StockSource = stockbalance;
            ViewBag.UserSource = UsersItems;
            ViewBag.ImpUserSource = ImpUsersItems;

            return View();
        }
        public IActionResult Edit(int? id)
        {
            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";
            ViewBag.content3 = "#Grid3";
            ViewBag.content4 = "#Grid4";
            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "General Info", IconCss = "e-tab1" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Stock Distribution", IconCss = "e-tab2" }, Content = ViewBag.content2 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Documentation", IconCss = "e-tab3" }, Content = ViewBag.content3 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Emailing", IconCss = "e-tab4" }, Content = ViewBag.content4 });
            ViewBag.headeritems = headerItems;

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

            ViewBag.DocSource = doctypes;
            ViewBag.ItemSource = items;
            ViewBag.WarehouseSource = warehouses;
            ViewBag.StockSource = stockbalance;
            ViewBag.UserSource = UsersItems;
            ViewBag.ImpUserSource = ImpUsersItems;

            List<Implementers> implementers = new List<Implementers>();
            implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpCode = 0, ImpAcronym = "" });

            List<scmRounds> rounds = new List<scmRounds>();
            rounds = (from roud in _context.scmRounds select roud).ToList();
            rounds.Insert(0, new scmRounds { RoundId = 0, RoundDescription = "" });

            ViewBag.RoundSource = new SelectList(rounds, "RoundId", "RoundDescription");
            ViewBag.ImpSource = new SelectList(implementers, "ImpCode", "ImpAcronym");
            return View(scmrRounds);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(scmDistributionMain scmDistmain)
        {
            int id = scmDistmain.DistributionId;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (ModelState.IsValid)
            {
                try
                {

                    var item = _context.scmDistributionMain.SingleOrDefault(m => m.DistributionId == id);

                    item.RoundId = id;
                    item.ImpId = scmDistmain.ImpId;
                    item.DistributionDate = scmDistmain.DistributionDate;
                    item.UserName = user.UserName;
                    item.TenantId = user.TenantId;
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

        public IActionResult Editfacilities(int id)
        {
            ViewBag.content1 = "#Grid1";
            ViewBag.content2 = "#Grid2";
            List<TabTabItem> headerItems = new List<TabTabItem>();
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Stock Info", IconCss = "e-tab1" }, Content = ViewBag.content1 });
            headerItems.Add(new TabTabItem { Header = new TabHeader { Text = "Distribution to Facilities", IconCss = "e-tab2" }, Content = ViewBag.content2 });
            ViewBag.headeritems = headerItems;

            //var dataReq = _context.scmDistributionsIP.Where(m=>m.Id==id).ToList();

            //ViewBag.DataSource = dataReq;


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
                StockId = m.SstockId,
                StockName = m.Item
            }).ToList();

            var warehouses = _context.scmWarehouses.Where(m => m.LevelId == 2).Select(m => new
            {
                WhId = m.WhId,
                WarehouseName = m.RegionsNav.RegionLong + '(' + m.ProvincesNav.ProvName + ") - " + m.ImplementerNav.ImpAcronym + " = " + m.Location
            }).ToList();

            ViewData["WhSource"] = warehouses;
            ViewData["ItemSource"] = items;
            return View(scmrRounds);
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmDistributionsIP.ToList();
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
            int count = DataSource.Cast<scmDistributionsIP>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmDistributionsIP> value)
        {
            //int mainid = (int)value.Params["ID"];

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmDistributionsIP stock = new scmDistributionsIP();
            if (stock == null) { return BadRequest(); }
            stock.stockId = value.Value.stockId;
            stock.whId = value.Value.whId;
            stock.distributionId =int.Parse(value.Params["ID"].ToString());
            stock.quantity = value.Value.quantity;
            stock.batchNumber = getBatchNumber(value.Value.stockId);
            
            stock.issueTo = value.Value.issueTo;
            stock.issueDate = value.Value.issueDate;
            stock.issueBy = value.Value.issueBy;
            stock.updateDate = DateTime.Now;
            stock.userName = user.UserName;
            stock.tenantId = user.TenantId;

            try
            {
                _context.Add(stock);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }

        public async Task<IActionResult> Update([FromBody]CRUDModel<scmDistributionsIP> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var stock = _context.scmDistributionsIP.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (stock != null)
            {
                stock.stockId = value.Value.stockId;
                stock.whId = value.Value.whId;
                stock.distributionId = int.Parse(value.Params["ID"].ToString());
                stock.quantity = value.Value.quantity;
                stock.batchNumber = getBatchNumber(value.Value.stockId);
                stock.issueTo = value.Value.issueTo;
                stock.issueDate = value.Value.issueDate;
                stock.issueBy = value.Value.issueBy;
                stock.updateDate = DateTime.Now;
                stock.userName = user.UserName;
                stock.tenantId = user.TenantId;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                _context.Update(stock);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(value.Value.stockId))
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

        public IActionResult Remove([FromBody]CRUDModel<scmDistributionsIP> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmDistributionsIP item = _context.scmDistributionsIP.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmDistributionsIP.Remove(item);
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
            return _context.scmDistributionsIP.Any(e => e.id == id);
        }

        private int getItemId(int id)
        {
            var item= _context.scmStocks.FirstOrDefault(e => e.StockId == id);
            return item.ItemId;
        }
        private string getBatchNumber(int id)
        {
            var item = _context.scmStocks.FirstOrDefault(e => e.StockId == id);
            return item.BatchNumber;
        }
    }
}