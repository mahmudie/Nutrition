using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DataSystem.Controllers
{
    public class BaseController : Controller
    {
        protected string _uploadDir;
        protected List<string> _allowedExtensions;
        private IHostingEnvironment _hostingEnv;

        public BaseController()
        {
            _uploadDir = "/uploads/";
            _allowedExtensions = new List<string>()
            {
                ".jpg",".jpeg","gif",".png",".svg",
                ".doc",".docx",".xls",".xlsx",".ppt",".pptx",".pdf",
                ".zip",".rar"
            };
        }

        protected void AlertError(string text)
        {

            TempData["Alert"] = true;
            TempData["AlertStatus"] = false;
            TempData["AlertText"] = text;
        }

        protected void AlertSuccess(string text)
        {

            TempData["Alert"] = true;
            TempData["AlertStatus"] = true;
            TempData["AlertText"] = text;
        }
    }
}