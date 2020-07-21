using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace TEST1.Controllers
{
    public class DataFormsController : Controller
    {
        protected readonly WebNutContext _context;

        public DataFormsController(WebNutContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dataforms = _context.DataForms
                .OrderBy(m => m.Name)
                .ToList();

            return View(dataforms);
        }

        //public IActionResult Create(long? SectionId)
        //{
        //    if (SectionId != null)
        //    {
        //        var viewModel = new DataFormViewModel()
        //        {
        //            SectionId = (long)SectionId,
        //            Sections = _context.Sections.ToList()
        //        };

        //        return View(viewModel);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Sections");
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(DataFormViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        viewModel.Sections = _context.Sections.ToList();

        //        return View(viewModel);
        //    }

        //    _context.DataForms.Add(new DataForm
        //    {
        //        Title = viewModel.Title,
        //        SectionId = viewModel.SectionId,
        //        SortOrder = viewModel.SortOrder
        //    });

        //    _context.SaveChanges();

        //    return RedirectToAction("Index", "DataForms", new { SectionId = viewModel.SectionId });
        //}

        //public IActionResult Edit(long Id)
        //{
        //    var dataform = _context.DataForms.Find(Id);
        //    var viewModel = new DataFormViewModel();

        //    viewModel.Id = dataform.Id;
        //    viewModel.Title = dataform.Title;
        //    viewModel.SectionId = dataform.SectionId;
        //    viewModel.SortOrder = dataform.SortOrder;

        //    viewModel.Sections = _context.Sections.ToList();

        //    return View(viewModel);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(DataFormViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        viewModel.Sections = _context.Sections.ToList();

        //        return View(viewModel);
        //    }

        //    var dataform = _context.DataForms.Find(viewModel.Id);

        //    dataform.Title = viewModel.Title;
        //    dataform.SectionId = viewModel.SectionId;
        //    dataform.SortOrder = viewModel.SortOrder;

        //    _context.Entry(dataform).State = EntityState.Modified;

        //    _context.SaveChanges();

        //    return RedirectToAction("Index", "DataForms", new { SectionId = viewModel.SectionId });
        //}

        //public IActionResult Delete(long Id)
        //{
        //    var dataform = _context.DataForms.Find(Id);

        //    return View(dataform);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Delete(LongId dto)
        //{
        //    var dataform = _context.DataForms.Find(dto.Id);

        //    _context.DataForms.Remove(dataform);
        //    _context.SaveChanges();

        //    return RedirectToAction("Index", "DataForms", new { SectionId = dataform.SectionId });
        //}
    }
}