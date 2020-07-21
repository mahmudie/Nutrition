using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataSystem.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilitiesInfoApiController : ControllerBase
    {
        protected readonly WebNutContext _context;
        public FacilitiesInfoApiController(WebNutContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var data = _context.vmFacilityimps.ToList();
            return Ok(new { FacilityData = data });
        }
    }
}
