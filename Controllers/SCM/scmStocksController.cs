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
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy = "admin")]
    public class scmStocksController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmStocksController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var items = _context.TlkpSstock.Where(m=>m.Active.Equals(true)).Select(m => new
            {
                ItemId = m.SstockId,
                ItemName = m.Item
            }).ToList();

            var warehouses = _context.scmWarehouses.Select(m => new
            {
                WhId = m.WhId,
                WarehouseName = m.RegionsNav.RegionLong+'('+m.ProvincesNav.ProvName+") - "+m.ImplementerNav.ImpAcronym +" = "+m.Location
            }).ToList();


            ViewBag.ItemSource = items;
            ViewBag.WarehouseSource = warehouses;

            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmStocks.ToList();
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
            int count = DataSource.Cast<scmStocks>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmStocks> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmStocks stock = new scmStocks();
            if (stock == null) { return BadRequest(); }

            stock.ItemId = value.Value.ItemId;
            stock.Quantity = value.Value.Quantity;
            stock.BatchNumber = value.Value.BatchNumber;
            stock.WhId = value.Value.WhId;
            stock.DateReceived = value.Value.DateReceived;
            stock.ExpiryDate = value.Value.ExpiryDate;
            stock.UserName = user.UserName;
            stock.TenantId = user.TenantId;
            stock.UpdateDate = DateTime.Now ;
            stock.Comment = value.Value.Comment;

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
        public async Task<IActionResult> Update([FromBody]CRUDModel<scmStocks> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var stock = _context.scmStocks.Where(cat => cat.StockId == value.Value.StockId).FirstOrDefault();
            if (stock != null)
            {
                stock.ItemId = value.Value.ItemId;
                stock.Quantity = value.Value.Quantity;
                stock.BatchNumber = value.Value.BatchNumber;
                stock.WhId = value.Value.WhId;
                stock.DateReceived = value.Value.DateReceived;
                stock.ExpiryDate = value.Value.ExpiryDate;
                stock.UserName = user.UserName;
                stock.TenantId = user.TenantId;
                stock.UpdateDate = DateTime.Now;
                stock.Comment = value.Value.Comment;
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
                if (!Exists(value.Value.WhId))
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

        public IActionResult Remove([FromBody]CRUDModel<scmStocks> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmStocks item = _context.scmStocks.Where(m => m.StockId.Equals(id)).FirstOrDefault();
                _context.scmStocks.Remove(item);
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
            return _context.scmStocks.Any(e => e.StockId == id);
        }
    }
}