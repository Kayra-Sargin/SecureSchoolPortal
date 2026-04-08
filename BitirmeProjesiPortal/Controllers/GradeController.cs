using BitirmeProjesiPortal.Entities;
using BitirmeProjesiPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using RazorEngineCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RazorEngine = RazorEngineCore.RazorEngine;

namespace BitirmeProjesiPortal.Controllers
{
    public class GradeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GradeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public IActionResult Index(int classReferenceId)
        {
            ViewBag.ClassReferenceId = classReferenceId;
            var grades = _context.Grades
                                 .Include(x => x.ClassReference.Class)
                                 .Include(x => x.ExamType)
                                 .Include(x => x.Student)
                                 .Where(x => x.ClassReferenceId == classReferenceId)
                                 .OrderByDescending(x => x.Id)
                                 .ToList();
            return View(grades);
        }

        public IActionResult Create(int classReferenceId)
        {
            var students = _context.UserAccounts
                    .Where(u => _context.UserAccountClassReferences
                        .Any(ucr => ucr.ClassReferenceId == classReferenceId && ucr.UserId == u.Id))
                    .ToList();

            var examTypes = _context.ExamTypes.ToList();

            ViewBag.Students = new SelectList(students, "Id", "UserName");
            ViewBag.ExamTypes = new SelectList(examTypes, "Id", "ExamTypeName");

            var model = new Grade { ClassReferenceId = classReferenceId };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Grade grade)
        {
            if (ModelState.IsValid)
            {
                _context.Grades.Add(grade);

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Not başarıyla girildi.";
                    return RedirectToAction(nameof(Index), new { classReferenceId = grade.ClassReferenceId });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Not veritabanına kaydedilemedi.");
                }
            }
            return View(grade);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var grade = _context.Grades.FirstOrDefault(x => x.Id == id);

            if (grade != null)
            {
                var classRefId = grade.ClassReferenceId;

                _context.Grades.Remove(grade);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index), new { classReferenceId = classRefId });
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult PreviewEmailVulnerable([FromBody] EmailPreviewRequest request)
        {
            try
            {
                string vulnerableTemplate = request.Template.Replace("{{YORUM}}", request.CommentText ?? "");

                IRazorEngine razorEngine = new RazorEngine();
                IRazorEngineCompiledTemplate compiledTemplate = razorEngine.Compile(vulnerableTemplate);

                string renderedResult = compiledTemplate.Run(new
                {
                    StudentName = request.StudentName ?? "Öğrenci",
                    Grade = request.Grade ?? "0"
                });

                return Json(new { success = true, preview = renderedResult });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Şablon derleme hatası: " + ex.Message });
            }
        }

        public class EmailPreviewRequest
        {
            public string Template { get; set; }
            public string StudentName { get; set; }
            public string Grade { get; set; }
            public string CommentText { get; set; }
        }
    }
}