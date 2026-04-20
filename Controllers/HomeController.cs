using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qaryati.Data;
using Qaryati.Models;
using System.Diagnostics;

namespace Qaryati.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(AppDbContext context, ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? villageName)
        {
            var categories = await _context.Categories
                .Include(c => c.Services)
                .ToListAsync();

            var featuredQuery = _context.Services
                .Include(s => s.Category)
                .Where(s => s.IsFeatured);

            if (!string.IsNullOrWhiteSpace(villageName))
            {
                villageName = villageName.Trim();
                featuredQuery = featuredQuery.Where(s => s.VillageName != null && 
                    s.VillageName.ToLower().Contains(villageName.ToLower()));
                ViewBag.SelectedVillage = villageName;
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                featuredQuery = featuredQuery.Where(s =>
                    s.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    (s.Description != null && s.Description.ToLower().Contains(searchTerm.ToLower())));
            }

            var featuredServices = await featuredQuery
                .OrderBy(s => s.Id)
                .Take(6)
                .ToListAsync();

            ViewBag.FeaturedServices = featuredServices;

            return View(categories);
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.Services)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> AllServices(string? villageName, string? searchTerm)
        {
            var query = _context.Services
                .Include(s => s.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(villageName))
            {
                villageName = villageName.Trim();
                query = query.Where(s => s.VillageName != null && 
                    s.VillageName.ToLower().Contains(villageName.ToLower()));
                ViewBag.SelectedVillage = villageName;
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(s =>
                    s.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    (s.Description != null && s.Description.ToLower().Contains(searchTerm.ToLower())) ||
                    (s.Address != null && s.Address.ToLower().Contains(searchTerm.ToLower())) ||
                    (s.VillageName != null && s.VillageName.ToLower().Contains(searchTerm.ToLower())) ||
                    (s.Category != null && s.Category.Name.ToLower().Contains(searchTerm.ToLower())));
                ViewBag.SearchTerm = searchTerm;
            }

            var services = await query
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(services);
        }

        public IActionResult SuggestionForm()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSuggestion(Suggestion suggestion, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    suggestion.ImageUrl = "/uploads/" + uniqueFileName;
                }

                suggestion.Status = "Pending";
                suggestion.CreatedAt = DateTime.Now;

                _context.Suggestions.Add(suggestion);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم إرسال اقتراحك بنجاح وسيتم مراجعته من الأدمن.";
                return RedirectToAction(nameof(SuggestionForm));
            }

            return View("SuggestionForm");
        }

        public async Task<IActionResult> CategoryServices(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Services)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        public async Task<IActionResult> ServiceDetails(int id)
        {
            var service = await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound();

            return View(service);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                ViewBag.SearchTerm = searchTerm;
                return View(new List<Service>());
            }

            searchTerm = searchTerm.Trim();
            var services = await _context.Services
                .Include(s => s.Category)
                .Where(s =>
                    s.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    (s.Description != null && s.Description.ToLower().Contains(searchTerm.ToLower())) ||
                    (s.Address != null && s.Address.ToLower().Contains(searchTerm.ToLower())) ||
                    (s.VillageName != null && s.VillageName.ToLower().Contains(searchTerm.ToLower())) ||
                    (s.Category != null && s.Category.Name.ToLower().Contains(searchTerm.ToLower())))
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            return View(services);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}