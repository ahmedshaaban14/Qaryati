using System.ComponentModel.DataAnnotations;

namespace Qaryati.Models
{
    public class Ad
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "عنوان الإعلان")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "وصف الإعلان")]
        public string? Description { get; set; }

        [StringLength(100)]
        [Display(Name = "نوع الإعلان")]
        public string? AdType { get; set; }

        [StringLength(100)]
        [Display(Name = "اسم البلد")]
        public string? VillageName { get; set; }

        [StringLength(50)]
        [Display(Name = "السعر")]
        public string? Price { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "اسم المعلن")]
        public string AdvertiserName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(300)]
        [Display(Name = "الصورة")]
        public string? ImageUrl { get; set; }

        [Display(Name = "تاريخ النشر")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}