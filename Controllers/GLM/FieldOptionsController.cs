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
    public class FieldOptionsController : Controller
    {
        protected readonly WebNutContext _context;

        public FieldOptionsController(WebNutContext context)
        {
            _context = context;
        }

        public IActionResult Create(long? FieldId)
        {
            if (FieldId != null)
            {
                var viewModel = new FieldOptionViewModel()
                {
                    FieldId = (long)FieldId
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Edit", "Fields", new { Id = FieldId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FieldOptionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            _context.FieldOptions.Add(new FieldOption
            {
                Value = viewModel.Value,
                Caption = viewModel.Caption,
                FieldId = viewModel.FieldId
            });

            _context.SaveChanges();

            return RedirectToAction("Edit", "Fields", new { Id = viewModel.FieldId });
        }

        public IActionResult Edit(long? Id)
        {
            var option = _context.FieldOptions.Find(Id);
            var viewModel = new FieldOptionViewModel();

            viewModel.Id = option.Id;
            viewModel.Value = option.Value;
            viewModel.Caption = option.Caption;
            viewModel.FieldId = option.FieldId;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FieldOptionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var fieldOption = _context.FieldOptions.Find(viewModel.Id);

            fieldOption.Value = viewModel.Value;
            fieldOption.Caption = viewModel.Caption;

            _context.Entry(fieldOption).State = EntityState.Modified;

            _context.SaveChanges();

            return RedirectToAction("Edit", "Fields", new { Id = viewModel.FieldId });
        }

        public IActionResult Delete(long Id)
        {
            var fieldOption = _context.FieldOptions.Find(Id);

            return View(fieldOption);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(LongId dto)
        {
            var fieldOption = _context.FieldOptions.Find(dto.Id);

            _context.FieldOptions.Remove(fieldOption);
            _context.SaveChanges();

            return RedirectToAction("Edit", "Fields", new { Id = fieldOption.FieldId });
        }
    }
}