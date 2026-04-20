using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qaryati.Data;
using Qaryati.Models;

namespace Qaryati.Controllers
{
    [Authorize]
    public class SuggestionsController : Controller
    {
        private readonly AppDbContext _context;

        public SuggestionsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var suggestions = await _context.Suggestions
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View(suggestions);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var suggestion = await _context.Suggestions.FindAsync(id);
            if (suggestion == null)
                return NotFound();

            return View(suggestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Suggestion suggestion)
        {
            if (id != suggestion.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingSuggestion = await _context.Suggestions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                if (existingSuggestion == null)
                    return NotFound();

                suggestion.ImageUrl = existingSuggestion.ImageUrl;

                _context.Update(suggestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(suggestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);
            if (suggestion == null)
                return NotFound();

            if (suggestion.Status == "Approved")
                return RedirectToAction(nameof(Index));

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == suggestion.CategoryName);

            if (category == null)
            {
                category = new Category
                {
                    Name = suggestion.CategoryName,
                    Description = "تمت إضافته من اقتراح المستخدمين"
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            var service = new Service
            {
                Name = suggestion.ServiceName,
                Description = suggestion.ServiceDescription,
                PhoneNumber = suggestion.PhoneNumber,
                WhatsAppNumber = suggestion.WhatsAppNumber,
                Address = suggestion.Address,
                WorkingHours = suggestion.WorkingHours,
                VillageName = suggestion.VillageName,
                IsFeatured = suggestion.IsFeatured,
                CategoryId = category.Id,
                ImageUrl = suggestion.ImageUrl
            };

            _context.Services.Add(service);

            suggestion.Status = "Approved";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);
            if (suggestion == null)
                return NotFound();

            suggestion.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}