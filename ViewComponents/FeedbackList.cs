using DataSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.ViewComponents
{
    public class FeedbackList: ViewComponent
    {
        private readonly WebNutContext _context;

        public FeedbackList(WebNutContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(string nmrid)
        {
            var model = new Feedback();
            model.items = _context.Feedback.Where(m => m.Nmrid.Equals(nmrid)).ToList();
            return View(model);
        }
    }
}
