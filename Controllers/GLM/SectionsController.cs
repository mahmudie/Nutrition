using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.GLM.Dtos;
using DataSystem.Models;
using DataSystem.Models.GLM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace TEST1.Controllers
{
    public class SectionsController : Controller
    {
        protected readonly WebNutContext _context;

        public SectionsController(WebNutContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var sections = _context.Sections
                .Include(m => m.DataForm)
                .OrderBy(m => m.SortOrder)
                .ToList();

            return View(sections);
        }

        public IActionResult Create()
        {
            // get the last sort-order value 
            var lastOrder = _context.Sections
                .Where(m => m.Id != 0)
                .OrderByDescending(m => m.SortOrder)
                .Select(m => m.SortOrder)
                .FirstOrDefault();

            var viewModel = new SectionViewModel()
            {
                SortOrder = lastOrder + 1,
                DataForms = _context.DataForms.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SectionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.DataForms = _context.DataForms.ToList();

                return View(viewModel);
            }

            _context.Sections.Add(new Section
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                SortOrder = viewModel.SortOrder,
                DataFormId = viewModel.DataFormId
            });

            _context.SaveChanges();

            return RedirectToAction("Index", "Sections");
        }

        public IActionResult Edit(long Id)
        {
            var section = _context.Sections.Find(Id);
            var viewModel = new SectionViewModel();

            viewModel.Id = section.Id;
            viewModel.Title = section.Title;
            viewModel.Description = section.Description;
            viewModel.SortOrder = section.SortOrder;
            viewModel.DataFormId = section.DataFormId;

            viewModel.DataForms = _context.DataForms.ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SectionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.DataForms = _context.DataForms.ToList();

                return View(viewModel);
            }

            var section = _context.Sections.Find(viewModel.Id);

            section.Title = viewModel.Title;
            section.Description = viewModel.Description;
            section.SortOrder = viewModel.SortOrder;
            section.DataFormId = viewModel.DataFormId;

            _context.Entry(section).State = EntityState.Modified;

            _context.SaveChanges();

            return RedirectToAction("Index", "Sections");
        }

        public IActionResult Delete(long Id)
        {
            var section = _context.Sections.Find(Id);

            return View(section);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(LongId dto)
        {
            var section = _context.Sections.Find(dto.Id);

            _context.Sections.Remove(section);
            _context.SaveChanges();

            // remove orphan questions
            var questions = _context.Questions
                .Where(m => m.SectionId == dto.Id)
                .ToList();

            _context.Questions.RemoveRange(questions);
            _context.SaveChanges();

            // remove orphan columns
            var columns = _context.Columns
                .Where(m => m.SectionId == dto.Id)
                .ToList();

            _context.Columns.RemoveRange(columns);
            _context.SaveChanges();

            return RedirectToAction("Index", "Sections");
        }
    }
}