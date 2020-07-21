using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.GLM.Dtos;
using DataSystem.Models;
using DataSystem.Models.GLM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DataSystem.Controllers
{
    public class QuestionsController : Controller
    {
        protected readonly WebNutContext _context;

        public QuestionsController(WebNutContext context)
        {
            _context = context;
        }

        public IActionResult Index(long? SectionId)
        {
            if (SectionId != null)
            {
                var questions = _context.Questions
                .Include(m => m.Section)
                .Where(m => m.SectionId == SectionId)
                .OrderBy(m => m.SortOrder)
                .ToList();

                ViewData["SectionId"] = SectionId;

                return View(questions);
            }
            else
            {
                return RedirectToAction("Index", "Sections");
            }
        }

        public IActionResult Create(long? SectionId)
        {
            if (SectionId != null)
            {
                // get the last sort-order value 
                var lastOrder = (_context.Questions
                    .Where(m => m.SectionId == SectionId)
                    .OrderByDescending(m => m.SortOrder)
                    .Select(m => m.SortOrder)
                    .FirstOrDefault() ?? 0);

                var viewModel = new QuestionViewModel()
                {
                    SectionId = (long)SectionId,
                    SortOrder = lastOrder + 1,
                    Sections = _context.Sections.ToList()
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Sections");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(QuestionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Sections = _context.Sections.ToList();

                return View(viewModel);
            }

            _context.Questions.Add(new Question
            {
                Title = viewModel.Title,
                SectionId = viewModel.SectionId,
                SortOrder = viewModel.SortOrder
            });

            _context.SaveChanges();

            return RedirectToAction("Index", "Questions", new { SectionId = viewModel.SectionId });
        }

        public IActionResult Edit(long Id)
        {
            var question = _context.Questions.Find(Id);
            var viewModel = new QuestionViewModel();

            viewModel.Id = question.Id;
            viewModel.Title = question.Title;
            viewModel.SectionId = question.SectionId;
            viewModel.SortOrder = question.SortOrder;

            viewModel.Sections = _context.Sections.ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(QuestionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Sections = _context.Sections.ToList();

                return View(viewModel);
            }

            var question = _context.Questions.Find(viewModel.Id);

            question.Title = viewModel.Title;
            question.SectionId = viewModel.SectionId;
            question.SortOrder = viewModel.SortOrder;

            _context.Entry(question).State = EntityState.Modified;

            _context.SaveChanges();

            return RedirectToAction("Index", "Questions", new { SectionId = viewModel.SectionId });
        }

        public IActionResult Delete(long Id)
        {
            var question = _context.Questions.Find(Id);

            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(LongId dto)
        {
            var question = _context.Questions.Find(dto.Id);

            _context.Questions.Remove(question);
            _context.SaveChanges();

            return RedirectToAction("Index", "Questions", new { SectionId = question.SectionId });
        }
    }
}