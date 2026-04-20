using System.ComponentModel.DataAnnotations;

namespace Qaryati.Models
{
    public class Suggestion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "اسم مقدم الاقتراح")]
        public string SubmitterName { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "رقم مقدم الاقتراح")]
        public string? SubmitterPhone { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "اسم الخدمة")]
        public string ServiceName { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "وصف الخدمة")]
        public string? ServiceDescription { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(20)]
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

        [Required]
        [StringLength(100)]
        [Display(Name = "اسم القسم المقترح")]
        public string CategoryName { get; set; } = string.Empty;

        [Display(Name = "مميز")]
        public bool IsFeatured { get; set; }

        [StringLength(300)]
        [Display(Name = "الصورة")]
        public string? ImageUrl { get; set; }

        [StringLength(50)]
        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "تاريخ الإرسال")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}