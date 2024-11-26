using static AppointmentSystem.Models.Domain.Appointment;
using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModel
{
    public class AllAppointmentViewmodel
    {
        public int Id { get; set; }

        public int OfficerId { get; set; }


        public int VisitorId { get; set; }

        public string Name { get; set; }


        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
        public DateTime AddedOn { get; set; }

        public AppointmentStatus Status { get; set; }

        public string OfficerName { get; set; }
        public string VisitorName { get; set; }
    }
}
