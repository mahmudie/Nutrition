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
    public class TextValuesApiController : ControllerBase
    {

        protected readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public TextValuesApiController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TextValue>>> Get()
        {
            List<TextValue> TextValue = _context.TextValues.ToList();

            if (TextValue.Count == 0)
            {
                return NotFound("TextValue not found");
              
            }
            else
            {
                return Ok(new
                {
                    TextValue = TextValue
                });
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<TextValue>>> Get(long ID)
        {
            List<TextValue> TextValue = _context.TextValues.Where(r => r.ReportId == ID.ToString()).ToList();

            if (TextValue.Count == 0)
            {
                return NotFound("TextValue not found");

            }
            else
            {
                return Ok(new
                {
                    TextValue = TextValue
                });
            }
        }

            [HttpPost]
            public async Task<ActionResult<TextValue>> Post([FromBody] List<TextValue> TextValues)
            {
                if (!ModelState.IsValid)
                {
                return BadRequest("Bad Request, Didn't Pass validation");
            }
                Boolean result = true;

                foreach (var TextValue in TextValues)
                {
                    if (this.TextValuesExists(TextValue.ReportId,TextValue.FieldId))
                    {
                        _context.Entry(TextValue).State = EntityState.Modified;

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
                            _context.TextValues.Add(TextValue);
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
                    return Created("TextValues Created/Updated",TextValues);
                }
            }

            public bool TextValuesExists(string id, long _attributecolumn)
            {
            return _context.TextValues.Any(e => e.ReportId == id && e.FieldId == _attributecolumn);

            }
        
    }
}
