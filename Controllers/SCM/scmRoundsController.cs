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
    public class scmRoundsController : Controller
    {
        private readonly WebNutContext _context;

        public scmRoundsController(WebNutContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var mfrom = new int[] { 1, 4, 7,10 };
            var mto = new int[] { 3, 6, 9,12 };

            var yearfrom = _context.Yearlist.ToList();
            var yearto = _context.Yearlist.ToList();
            var requesttypes = _context.scmRequesttype.ToList();
            var monthsFrom = _context.scmMonths.Where(m => mfrom.Contains(m.MonthId)).ToList();
            var monthsTo = _context.scmMonths.Where(m => mto.Contains(m.MonthId)).ToList();

            ViewBag.YearFroms = yearfrom;
            ViewBag.YearTos = yearto;
            ViewBag.RequestTypes = requesttypes;
            ViewBag.MonthsFrom = monthsFrom;
            ViewBag.MonthsTo = monthsTo;

            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmRounds.ToList();
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
            int count = DataSource.Cast<scmRounds>().Count();
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

        public IActionResult Insert([FromBody]CRUDModel<scmRounds> value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmRounds round = new scmRounds();
            if (round == null) { return BadRequest(); }

            round.RoundCode = value.Value.RoundCode;
            round.RoundDescription = value.Value.RoundDescription;
            round.PeriodFrom = value.Value.PeriodFrom;
            round.PeriodTo = value.Value.PeriodTo;
            round.YearFrom = value.Value.YearFrom;
            round.MonthFrom = value.Value.MonthFrom;
            round.YearTo = value.Value.YearTo;
            round.MonthTo = value.Value.MonthTo;
            round.RequesttypeId = value.Value.RequesttypeId;
            round.IsActive = value.Value.IsActive;
            try
            {
                _context.Add(round);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public IActionResult Update([FromBody]CRUDModel<scmRounds> value)
        {
            var round = _context.scmRounds.Where(cat => cat.RoundId == value.Value.RoundId).FirstOrDefault();
            if (round != null)
            {
                round.RoundCode = value.Value.RoundCode;
                round.RoundDescription = value.Value.RoundDescription;
                round.PeriodFrom = value.Value.PeriodFrom;
                round.PeriodTo = value.Value.PeriodTo;
                round.IsActive = value.Value.IsActive;
                round.YearFrom = value.Value.YearFrom;
                round.MonthFrom = value.Value.MonthFrom;
                round.YearTo = value.Value.YearTo;
                round.MonthTo = value.Value.MonthTo;
                round.RequesttypeId = value.Value.RequesttypeId;
                round.IsActive = value.Value.IsActive;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(round).State = EntityState.Modified;

            try
            {
                _context.Update(round);
                _context.SaveChanges();
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

        public IActionResult Remove([FromBody]CRUDModel<scmRounds> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmRounds item = _context.scmRounds.Where(m => m.RoundId.Equals(id)).FirstOrDefault();
                _context.scmRounds.Remove(item);
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
            return _context.scmRounds.Any(e => e.RoundId == id);
        }
    }
}