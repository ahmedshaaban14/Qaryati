using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Qaryati.Models;

namespace Qaryati.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }

        public DbSet<Ad> Ads { get; set; }
        public DbSet<AdSubmission> AdSubmissions { get; set; }
    }
}