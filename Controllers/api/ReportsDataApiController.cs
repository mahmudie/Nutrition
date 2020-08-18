using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.GLM;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataSystem.Controllers.api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsDataApiController : ControllerBase
    {

        protected readonly WebNutContext _context;
        public ReportsDataApiController(WebNutContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> Get()
        {
            List<Report> report = _context.Reports.ToList();

            if (report.Count == 0)
            {
                return NotFound("Report not found");
              
            }
            else
            {
                return Ok(new
                {
                    report = report
                });
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Report>> Get(long ID)
        {
            Report report = _context.Reports.Where(r => r.Id == ID.ToString()).FirstOrDefault();

            if (report == null)
            {
                return NotFound("Report not found");

            }
            else
            {
                return Ok(new
                {
                    report = report
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Report>> Post([FromBody] List<Report> reports)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad Request, Didn't Pass validation");
            }
            Boolean result = true;
            string failedvalues = "";
            foreach (var report in reports)
            {
                var users = _context.vusers.Where(m => m.UserName.Equals(report.UserName)).FirstOrDefault();

                if (this.ReportsExists(report.Id))
                {
                    _context.Entry(report).State = EntityState.Modified;

                    try
                    {
                        report.TenantId = users.TenantId;
                        report.UpdateDate = DateTime.Now;
                        _context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        result = false;
                        failedvalues += "Failed FieldId: " + report.Id.ToString() + " |";
                        _context.Entry(report).State = EntityState.Unchanged;
                        _context.SaveChanges();
                    }
                }
                else
                {
                    try
                    {
                        report.TenantId = users.TenantId;
                        report.UpdateDate = DateTime.Now;
                        _context.Reports.Add(report);
                        _context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        result = false;
                        failedvalues += "Failed FieldId: " + report.Id.ToString() + " |";
                        _context.Reports.Remove(report);
                        _context.SaveChanges();
                    }
                }
            }


            if (result == false)
            {
                return BadRequest(failedvalues);
            }
            else
            {
                return Created("Reports Created/Updated", reports);
            }
        }

        public bool ReportsExists(string id)
        {
            return _context.Reports.Any(e => e.Id == id.ToString());
        }
        
    }
}
