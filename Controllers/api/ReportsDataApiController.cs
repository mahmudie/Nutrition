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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsDataApiController : ControllerBase
    {

        protected readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReportsDataApiController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest("Bad Request, Didn't Pass validation");
            }
            Boolean result = true;

            foreach (var report in reports)
            {
                if (this.ReportsExists(report.Id))
                {
                    _context.Entry(report).State = EntityState.Modified;

                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }
                else
                {
                    try
                    {
                        report.TenantId = user.TenantId;
                        _context.Reports.Add(report);
                        _context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }
            }


            if (result == false)
            {
                return Ok();
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
