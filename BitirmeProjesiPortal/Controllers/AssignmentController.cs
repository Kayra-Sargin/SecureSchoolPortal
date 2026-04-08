using BitirmeProjesiPortal.Entities;
using BitirmeProjesiPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BitirmeProjesiPortal.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AssignmentController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public IActionResult Index(int classReferenceId)
        {
            ViewBag.ClassReferenceId = classReferenceId;
            var assignments = _context.Assignments
                                        .Include(x => x.ClassReference.Class)
                                        .Where(x => x.ClassReferenceId == classReferenceId)
                                        .OrderByDescending(x => x.Id)
                                        .ToList();
            return View(assignments);
        }

        public IActionResult Create(int classReferenceId)
        {
            var model = new Assignment { ClassReferenceId = classReferenceId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                if (assignment.AnswerFile != null && assignment.AnswerFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "assignments");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string filePath = Path.Combine(uploadsFolder, assignment.AnswerFile.FileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await assignment.AnswerFile.CopyToAsync(fileStream);
                    }

                    assignment.AnswerFilePath = "/uploads/assignments/" + assignment.AnswerFile.FileName;
                }

                _context.Assignments.Add(assignment);

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"{assignment.Header} ödevi başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index), new { classReferenceId = assignment.ClassReferenceId });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Ödev veritabanına kaydedilemedi.");
                }
            }
            return View(assignment);
        }

        public IActionResult Edit(int id)
        {
            var assignment = _context.Assignments.Find(id);
            if (assignment == null)
            {
                return NotFound();
            }
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Assignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAssignment = await _context.Assignments.FindAsync(id);
                    if (existingAssignment == null) return NotFound();

                    existingAssignment.Header = assignment.Header;
                    existingAssignment.Content = assignment.Content;

                    if (assignment.AnswerFile != null && assignment.AnswerFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "assignments");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string filePath = Path.Combine(uploadsFolder, assignment.AnswerFile.FileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await assignment.AnswerFile.CopyToAsync(fileStream);
                        }

                        existingAssignment.AnswerFilePath = "/uploads/assignments/" + assignment.AnswerFile.FileName;
                    }

                    _context.Update(existingAssignment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { classReferenceId = existingAssignment.ClassReferenceId });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Güncelleme sırasında hata oluştu.");
                }
            }
            return View(assignment);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var assignment = _context.Assignments.FirstOrDefault(x => x.Id == id);
            if (assignment != null)
            {
                if (!string.IsNullOrEmpty(assignment.AnswerFilePath))
                {
                    string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, assignment.AnswerFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }

                _context.Remove(assignment);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index), new { classReferenceId = assignment?.ClassReferenceId });
        }
    }
}