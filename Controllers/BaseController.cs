using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        protected string Slugify(string txt, string separator = "_")
        {
            string str = "";

            // remove accent
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            str = System.Text.Encoding.ASCII.GetString(bytes);

            // remove diacritics
            str = str.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in str)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            str = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            str = str.ToLower();

            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", separator); // hyphens   

            return str;
        }
    }
}