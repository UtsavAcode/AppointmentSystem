using System.ComponentModel.DataAnnotations;
using static AppointmentSystem.Models.Domain.Appointment;

namespace AppointmentSystem.Models.ViewModel
{
    public class AppointmentViewmodel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Officer")]
        public int OfficerId { get; set; }

        [Required]
        [Display(Name = "Visitor")]
        public int VisitorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        public DateTime AddedOn { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}
