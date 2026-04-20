using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qaryati.Data;
using Qaryati.Models;

namespace Qaryati.Controllers
{
    public class AdSubmissionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdSubmissionsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdSubmission submission, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    submission.ImageUrl = "/uploads/" + uniqueFileName;
                }

                submission.Status = "Pending";
                submission.CreatedAt = DateTime.Now;

                _context.AdSubmissions.Add(submission);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم إرسال إعلانك بنجاح وسيتم مراجعته قبل النشر.";
                return RedirectToAction(nameof(Create));
            }

            return View(submission);
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var submissions = await _context.AdSubmissions
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(submissions);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var submission = await _context.AdSubmissions.FindAsync(id);
            if (submission == null) return NotFound();

            return View(submission);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdSubmission submission)
        {
            if (id != submission.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.AdSubmissions.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                if (existing == null) return NotFound();

                submission.ImageUrl = existing.ImageUrl;

                _context.Update(submission);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(submission);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var submission = await _context.AdSubmissions.FindAsync(id);
            if (submission == null)
                return NotFound();

            if (submission.Status == "Approved")
                return RedirectToAction(nameof(Index));

            var ad = new Ad
            {
                Title = submission.Title,
                Description = submission.Description,
                AdType = submission.AdType,
                VillageName = submission.VillageName,
                Price = submission.Price,
                AdvertiserName = submission.AdvertiserName,
                PhoneNumber = submission.PhoneNumber,
                ImageUrl = submission.ImageUrl,
                CreatedAt = DateTime.Now
            };

            _context.Ads.Add(ad);

            submission.Status = "Approved";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var submission = await _context.AdSubmissions.FindAsync(id);
            if (submission == null)
                return NotFound();

            submission.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}