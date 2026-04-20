using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qaryati.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "اسم الخدمة")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required]
        [Phone]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "رقم واتساب")]
        public string? WhatsAppNumber { get; set; }

        [StringLength(200)]
        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        [StringLength(100)]
        [Display(Name = "مواعيد العمل")]
        public string? WorkingHours { get; set; }

        [StringLength(100)]
        [Display(Name = "اسم البلد")]
        public string? VillageName { get; set; }

        [Display(Name = "خدمة مميزة")]
        public bool IsFeatured { get; set; }

        [Display(Name = "رابط الصورة")]
        public string? ImageUrl { get; set; }

        [Display(Name = "القسم")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}