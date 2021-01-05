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
using Microsoft.AspNetCore.Http;

namespace DataSystem.Controllers
{

    public class ApiSourceController : Controller
    {
        private readonly WebNutContext _context;
        public ApiSourceController(WebNutContext context)
        {
            _context = context;
        }

        public IActionResult Api()
        {
            //var data = _context.Provincescases.ToList();
            return Ok();
        }
    }
}
