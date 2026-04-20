using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qaryati.Data;
using Qaryati.ViewModels;

namespace Qaryati.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                CategoriesCount = await _context.Categories.CountAsync(),
                ServicesCount = await _context.Services.CountAsync(),
                FeaturedServicesCount = await _context.Services.CountAsync(s => s.IsFeatured)
            };

            return View(model);
        }
    }
}