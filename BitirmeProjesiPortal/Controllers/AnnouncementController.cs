using BitirmeProjesiPortal.Entities;
using BitirmeProjesiPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesiPortal.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AnnouncementController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public IActionResult Index(int classReferenceId)
        {
            ViewBag.ClassReferenceId = classReferenceId;
            var announcements = _context.Announcements
                                        .Include(x => x.ClassReference.Class)
                                        .Where(x => x.ClassReferenceId == classReferenceId)
                                        .OrderByDescending(x => x.Id)
                                        .ToList();
            return View(announcements);
        }

        public IActionResult Create(int classReferenceId)
        {
            var model = new Announcement { ClassReferenceId = classReferenceId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                _context?.Announcements?.Add(announcement);

                try
                {
                    _context.SaveChanges();
                    TempData["Success"] = $"{announcement.Header} duyurusu başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index), new { classReferenceId = announcement.ClassReferenceId });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Duyuru veritabanına kaydedilemedi.");
                }
            }
            return View(announcement);
        }

        public IActionResult Edit(int id)
        {
            var announcement = _context?.Announcements?.Find(id);
            if (announcement == null)
            {
                return NotFound();
            }
            return View(announcement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Announcement announcement)
        {
            if (id != announcement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(announcement);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index), new { classReferenceId = announcement.ClassReferenceId });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Güncelleme sırasında hata oluştu.");
                }
            }
            return View(announcement);
        }

        public void Delete(int id)
        {
            var announcement = _context.Announcements.Where(x => x.Id == id).FirstOrDefault();
            _context.Remove(announcement);
            _context.SaveChanges();
        }
    }
}
