using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.GLM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataSystem.Controllers.api
{
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DateValuesApiController : ControllerBase
    {

        protected readonly WebNutContext _context;
        public DateValuesApiController(WebNutContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<DateValue>>> Get()
        {
            List<DateValue> DateValue = _context.DateValues.ToList();

            if (DateValue.Count == 0)
            {
                return NotFound("DateValue not found");
              
            }
            else
            {
                return Ok(new
                {
                    DateValue = DateValue
                });
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<DateValue>>> Get(long ID)
        {
            List<DateValue> DateValue = _context.DateValues.Where(r => r.ReportId == ID.ToString()).ToList();

            if (DateValue.Count == 0)
            {
                return NotFound("DateValue not found");

            }
            else
            {
                return Ok(new
                {
                    DateValue = DateValue
                });
            }
        }

            [HttpPost]
            public async Task<ActionResult<DateValue>> Post([FromBody] List<DateValue> DateValues)
            {
                if (!ModelState.IsValid)
                {
                return BadRequest("Bad Request, Didn't Pass validation");
            }
                Boolean result = true;

                foreach (var DateValue in DateValues)
                {
                    if (this.DateValuesExists(DateValue.ReportId,DateValue.FieldId))
                    {
                        _context.Entry(DateValue).State = EntityState.Modified;

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
                            _context.DateValues.Add(DateValue);
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
                    return BadRequest();
                }
                else
                {
                    return Created("DateValues Created/Updated",DateValues);
                }
            }

            public bool DateValuesExists(string id, long _attributecolumn)
            {
            return _context.DateValues.Any(e => e.ReportId == id && e.FieldId == _attributecolumn);

            }
        
    }
}
