using System.ComponentModel.DataAnnotations;

namespace Qaryati.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "اسم القسم")]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        [Display(Name = "وصف القسم")]
        public string? Description { get; set; }

        public List<Service> Services { get; set; } = new List<Service>();
    }
}