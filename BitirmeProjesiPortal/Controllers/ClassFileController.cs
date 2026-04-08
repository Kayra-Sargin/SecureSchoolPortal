using BitirmeProjesiPortal.Entities;
using BitirmeProjesiPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
namespace BitirmeProjesiPortal.Controllers
{
    public class ClassFileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClassFileController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public IActionResult Index(int classReferenceId)
        {
            ViewBag.ClassReferenceId = classReferenceId;
            var files = _context.ClassFiles
                                        .Include(x => x.ClassReference.Class)
                                        .Where(x => x.ClassReferenceId == classReferenceId)
                                        .OrderByDescending(x => x.Id)
                                        .ToList();
            return View(files);
        }

        public IActionResult Create(int classReferenceId)
        {
            var model = new ClassFile { ClassReferenceId = classReferenceId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassFile classFile)
        {
            if (classFile.ClassReferenceId == 0 && Request.Form.ContainsKey("ClassReferenceId"))
            {
                classFile.ClassReferenceId = Convert.ToInt32(Request.Form["ClassReferenceId"]);
            }
            ModelState.Remove("ClassReference");
            if (ModelState.IsValid)
            {
                classFile.UploadDate = DateTime.Now;

                if (classFile.File != null && classFile.File.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "classfiles");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string filePath = Path.Combine(uploadsFolder, classFile.File.FileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await classFile.File.CopyToAsync(fileStream);
                    }

                    classFile.FilePath = classFile.File.FileName;
                }

                _context.ClassFiles.Add(classFile);

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Dosya başarıyla yüklendi.";
                    return RedirectToAction(nameof(Index), new { classReferenceId = classFile.ClassReferenceId });
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Veritabanına kaydedilemedi: " + ex.Message);
                }
            }
            return View(classFile);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var classFile = _context.ClassFiles.FirstOrDefault(x => x.Id == id);
            if (classFile != null)
            {
                if (!string.IsNullOrEmpty(classFile.FilePath))
                {
                    string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, classFile.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }

                _context.Remove(classFile);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index), new { classReferenceId = classFile?.ClassReferenceId });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("Path is required.");
            }

            string fullPath = fileName;
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound($"The server cannot find the file at: {fullPath}");
            }

            string downloadName = Path.GetFileName(fullPath);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fullPath, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            return PhysicalFile(fullPath, contentType, downloadName);
        }

    }
}