using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qaryati.Data;
using Qaryati.Models;

namespace Qaryati.Controllers
{
    public class AdsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string? villageName, string? searchTerm)
        {
            var query = _context.Ads.AsQueryable();

            if (!string.IsNullOrWhiteSpace(villageName))
            {
                query = query.Where(a => a.VillageName != null && a.VillageName.Contains(villageName));
                ViewBag.SelectedVillage = villageName;
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a =>
                    a.Title.Contains(searchTerm) ||
                    (a.Description != null && a.Description.Contains(searchTerm)) ||
                    (a.AdType != null && a.AdType.Contains(searchTerm)));
                ViewBag.SearchTerm = searchTerm;
            }

            var ads = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(ads);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ad = await _context.Ads.FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null)
                return NotFound();

            return View(ad);
        }

        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var ads = await _context.Ads
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(ads);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ad = await _context.Ads.FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null)
                return NotFound();

            return View(ad);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ad = await _context.Ads.FindAsync(id);

            if (ad != null)
            {
                if (!string.IsNullOrEmpty(ad.ImageUrl))
                {
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, ad.ImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _context.Ads.Remove(ad);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Manage));
        }
    }
}