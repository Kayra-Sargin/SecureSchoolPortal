using BitirmeProjesiPortal.Entities;
using BitirmeProjesiPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesiPortal.Controllers
{
    public class ClassReferenceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ClassReferenceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

            if (user == null) return Unauthorized();

            ViewBag.UserId = user.Id;
            var classes = await _context.UserAccountClassReferences
                .Where(x => x.UserId == user.Id)
                .Include(x => x.ClassReference)
                    .ThenInclude(cr => cr.Class)
                .Select(x => x.ClassReference) 
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(classes);
        }

        public IActionResult Create(int userId)
        {
            if (_context.Classes != null)
                ViewBag.ClassList = _context.Classes.ToList();
            ViewBag.StudentList = new SelectList(_context.UserAccounts, "Id", "UserName");
            var model = new ClassReference { UserId = userId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassReference classReference, List<int> SelectedStudentIds)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(classReference);
                    await _context.SaveChangesAsync();

                    var enrollments = new List<UserAccountClassReference>();

                    enrollments.Add(new UserAccountClassReference
                    {
                        UserId = classReference.UserId,
                        ClassReferenceId = classReference.Id
                    });

                    if (SelectedStudentIds != null && SelectedStudentIds.Any())
                    {
                        foreach (var studentId in SelectedStudentIds)
                        {
                            if (studentId != classReference.UserId)
                            {
                                enrollments.Add(new UserAccountClassReference
                                {
                                    UserId = studentId,
                                    ClassReferenceId = classReference.Id
                                });
                            }
                        }
                    }
                    _context.UserAccountClassReferences.AddRange(enrollments);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Sınıf başarıyla oluşturuldu ve öğrenciler eklendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Veritabanına kaydedilirken bir hata oluştu. Aynı CRN daha önce eklenmiş olabilir.");
                }
            }

            ViewBag.ClassList = _context.Classes.ToList();
            ViewBag.StudentList = new SelectList(_context.UserAccounts, "Id", "UserName", SelectedStudentIds);

            return View(classReference);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var classReference = await _context.ClassReferences.FindAsync(id);
            if (classReference == null) return NotFound();

            ViewBag.ClassList = await _context.Classes.ToListAsync();

            var enrolledStudentIds = await _context.UserAccountClassReferences
                .Where(x => x.ClassReferenceId == id && x.UserId != classReference.UserId)
                .Select(x => x.UserId)
                .ToListAsync();

            ViewBag.StudentList = new MultiSelectList(_context.UserAccounts, "Id", "UserName", enrolledStudentIds);

            return View(classReference);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, ClassReference classReference, List<int> SelectedStudentIds)
        {
            if (id != classReference.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classReference);

                    var existingEnrollments = await _context.UserAccountClassReferences
                        .Where(x => x.ClassReferenceId == id && x.UserId != classReference.UserId)
                        .ToListAsync();

                    var existingStudentIds = existingEnrollments.Select(e => e.UserId).ToList();
                    SelectedStudentIds ??= new List<int>();

                    var enrollmentsToRemove = existingEnrollments
                        .Where(e => !SelectedStudentIds.Contains(e.UserId))
                        .ToList();
                    _context.UserAccountClassReferences.RemoveRange(enrollmentsToRemove);

                    var studentIdsToAdd = SelectedStudentIds
                        .Except(existingStudentIds)
                        .Where(studentId => studentId != classReference.UserId)
                        .ToList();

                    foreach (var studentId in studentIdsToAdd)
                    {
                        _context.UserAccountClassReferences.Add(new UserAccountClassReference
                        {
                            UserId = studentId,
                            ClassReferenceId = id
                        });
                    }

                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Sınıf ve öğrenciler başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Veritabanına kaydedilirken bir hata oluştu.");
                }
            }
            ViewBag.ClassList = await _context.Classes.ToListAsync();
            ViewBag.StudentList = new MultiSelectList(_context.UserAccounts, "Id", "UserName", SelectedStudentIds);

            return View(classReference);
        }
        private bool ClassReferenceExists(int id)
        {
            return _context.ClassReferences.Any(e => e.Id == id);
        }

        public void Delete(int id)
        {
            var classReference = _context.ClassReferences.Where(x => x.Id == id).FirstOrDefault();
            _context.Remove(classReference);  
            _context.SaveChanges();
        }
    }
}
