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
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NumberValuesApiController : ControllerBase
    {

        protected readonly WebNutContext _context;
        public NumberValuesApiController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<NumberValue>>> Get()
        {
            List<NumberValue> NumberValue = _context.NumberValues.ToList();

            if (NumberValue.Count == 0)
            {
                return NotFound("NumberValue not found");
              
            }
            else
            {
                return Ok(new
                {
                    NumberValue = NumberValue
                });
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<NumberValue>>> Get(long ID)
        {
            List<NumberValue> NumberValue = _context.NumberValues.Where(r => r.ReportId == ID.ToString()).ToList();

            if (NumberValue.Count == 0)
            {
                return NotFound("NumberValue not found");

            }
            else
            {
                return Ok(new
                {
                    NumberValue = NumberValue
                });
            }
        }

            [HttpPost]
            public async Task<ActionResult<NumberValue>> Post([FromBody] List<NumberValue> NumberValues)
            {
                if (!ModelState.IsValid)
                {
                return BadRequest("Bad Request, Didn't Pass validation");
            }
                Boolean result = true;
            string failedvalues = "";

                foreach (var NumberValue in NumberValues)
                {
                    if (this.NumberValuesExists(NumberValue.ReportId,NumberValue.FieldId))
                    {
                   
                        _context.Entry(NumberValue).State = EntityState.Modified;

                        try
                        {
                            _context.SaveChanges();
                        }
                        catch (Exception)
                        {
                            result = false;
                        failedvalues += "Failed FieldId: "+  NumberValue.FieldId.ToString() + " |";
                        _context.NumberValues.Remove(NumberValue);
                        _context.SaveChanges();


                    }
                    }
                    else
                    {
                        try
                        {
                            _context.NumberValues.Add(NumberValue);
                            _context.SaveChanges();
                        }
                        catch (Exception)
                        {
                        
                        result = false;
                        failedvalues += "Failed FieldId: " + NumberValue.FieldId.ToString() + " |";
                        _context.NumberValues.Remove(NumberValue);

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
                    return Created("NumberValues Created/Updated",NumberValues);
                }
            }

            public bool NumberValuesExists(string id, long _attributecolumn)
            {
            return _context.NumberValues.Any(e => e.ReportId == id && e.FieldId == _attributecolumn);

            }
        
    }
}
