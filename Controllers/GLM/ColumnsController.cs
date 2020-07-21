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
    public class ColumnsController : Controller
    {
        protected readonly WebNutContext _context;

        public ColumnsController(WebNutContext context)
        {
            _context = context;
        }

        public IActionResult Index(long? SectionId)
        {
            if (SectionId != null) {
                var columns = _context.Columns
                .Include(m => m.Section)
                .Where(m => m.SectionId == SectionId)
                .OrderBy(m => m.SortOrder)
                .ToList();

                ViewData["SectionId"] = SectionId;

                return View(columns);
            } else
            {
                return RedirectToAction("Index", "Sections");
            }
        }

        public IActionResult Create(long? SectionId)
        {
            if (SectionId != null)
            {
                // get the last sort-order value 
                var lastOrder = (_context.Columns
                    .Where(m => m.SectionId == SectionId)
                    .OrderByDescending(m => m.SortOrder)
                    .Select(m => m.SortOrder)
                    .FirstOrDefault() ?? 0);

                var viewModel = new ColumnViewModel()
                {
                    SectionId = (long)SectionId,
                    SortOrder = lastOrder + 1,
                    Sections = _context.Sections.ToList(),

                    DividendColumns = _context.Columns
                        .Where(m => m.SectionId == SectionId)
                        .Where(m => m.ColumnType == "standard")
                        .ToList(),

                    DivisorColumns = _context.Columns
                        .Where(m => m.SectionId == SectionId)
                        .Where(m => m.ColumnType == "standard")
                        .ToList(),
                };

                return View(viewModel);
            } else
            {
                return RedirectToAction("Index", "Sections");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ColumnViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Sections = _context.Sections.ToList();

                viewModel.DividendColumns = _context.Columns
                    .Where(m => m.SectionId == viewModel.SectionId)
                    .Where(m => m.ColumnType == "standard")
                    .ToList();

                viewModel.DivisorColumns = _context.Columns
                    .Where(m => m.SectionId == viewModel.SectionId)
                    .Where(m => m.ColumnType == "standard")
                    .ToList();

                return View(viewModel);
            }

            _context.Columns.Add(new Column
            {
                Title = viewModel.Title,
                SectionId = viewModel.SectionId,
                SortOrder = viewModel.SortOrder,
                ColumnType = viewModel.ColumnType,
                DividendColumn = viewModel.DividendColumn,
                DivisorColumn = viewModel.DivisorColumn,
            });

            _context.SaveChanges();

            return RedirectToAction("Index", "Columns", new { SectionId = viewModel.SectionId });
        }

        public IActionResult Edit(long Id)
        {
            var column = _context.Columns.Find(Id);
            var viewModel = new ColumnViewModel();

            viewModel.Id = column.Id;
            viewModel.Title = column.Title;
            viewModel.SectionId = column.SectionId;
            viewModel.SortOrder = column.SortOrder;
            viewModel.ColumnType = column.ColumnType;
            viewModel.DividendColumn = column.DividendColumn;
            viewModel.DivisorColumn = column.DivisorColumn;

            viewModel.Sections = _context.Sections.ToList();

            viewModel.DividendColumns = _context.Columns
                .Where(m => m.SectionId == column.SectionId)
                .Where(m => m.ColumnType == "standard")
                .ToList();

            viewModel.DivisorColumns = _context.Columns
                .Where(m => m.SectionId == column.SectionId)
                .Where(m => m.ColumnType == "standard")
                .ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ColumnViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Sections = _context.Sections.ToList();

                viewModel.DividendColumns = _context.Columns
                    .Where(m => m.SectionId == viewModel.SectionId)
                    .Where(m => m.ColumnType == "standard")
                    .ToList();

                viewModel.DivisorColumns = _context.Columns
                    .Where(m => m.SectionId == viewModel.SectionId)
                    .Where(m => m.ColumnType == "standard")
                    .ToList();

                return View(viewModel);
            }

            var column = _context.Columns.Find(viewModel.Id);

            column.Title = viewModel.Title;
            column.SectionId = viewModel.SectionId;
            column.SortOrder = viewModel.SortOrder;
            column.ColumnType = viewModel.ColumnType;
            column.DividendColumn = viewModel.DividendColumn;
            column.DivisorColumn = viewModel.DivisorColumn;

            _context.Entry(column).State = EntityState.Modified;

            _context.SaveChanges();

            return RedirectToAction("Index", "Columns", new { SectionId = viewModel.SectionId });
        }

        public IActionResult Delete(long Id)
        {
            var column = _context.Columns.Find(Id);

            return View(column);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(LongId dto)
        {
            var column = _context.Columns.Find(dto.Id);

            _context.Columns.Remove(column);
            _context.SaveChanges();

            return RedirectToAction("Index", "Columns", new { SectionId = column.SectionId });
        }
    }
}