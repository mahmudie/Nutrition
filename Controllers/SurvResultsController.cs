using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Identity;
using DataSystem.Components;
using DataSystem.Models.ViewModels;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy ="admin")]
    public  static  class getSurvInfos
    {
        public  static int? SurvId { get; set; }
    }
    public class SurvResultsController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public SurvResultsController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        public IActionResult Disaggregation()
        {
            var Disaggregation = _context.LkpDisaggregations.Select(m => new
            {
                DisaggregId = m.DisaggregId,
                CategoryId = m.CategoryId,
                Disaggregation = m.Disaggregation
            }).ToList();

            return Json(Disaggregation);
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.SurveyResults.Where(f=>f.SurveyId.Equals(getSurvInfos.SurvId)).ToList();
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
            var data = TempData["SurvData"];
            var surv =new SurveyInfo();
            if (TempData["SurvData"] != null)
            {
                surv = (SurveyInfo)TempData["SurvData"];
            }
                if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var users = _userManager.Users.Where(usr => usr.UserName.Equals(User.Identity.Name)).FirstOrDefault();
            SurveyResults survResults = _context.SurveyResults.FirstOrDefault(m=>m.SurveyId==surv.SurveyId);
            //SurveyResults survResults = new SurveyResults();
            if (survResults == null) { return BadRequest(); }

            survResults.SurveyId = value.Value.SurveyId;
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
            catch(Exception ex)
            {

            }

            return NoContent();
        }
        public IActionResult Update([FromBody]CRUDModel<SurveyResults> value)
        {
            var users = _userManager.Users.Where(usr => usr.UserName.Equals(User.Identity.Name)).FirstOrDefault();
            var survResults = _context.SurveyResults.Where(srv=>srv.IndResultId==value.Value.IndResultId).FirstOrDefault();
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
            if(Exists(id))
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
    }
}
