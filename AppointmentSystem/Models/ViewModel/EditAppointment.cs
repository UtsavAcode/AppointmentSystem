using static AppointmentSystem.Models.Domain.Appointment;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentSystem.Models.ViewModel
{
    public class EditAppointment
    {
        public int Id { get; set; }
       
        [Display(Name = "Officer")]
        public int OfficerId { get; set; }

        
        [Display(Name = "Visitor")]
        public int VisitorId { get; set; }

        
        [MaxLength(100)]
        public string Name { get; set; }

        // Store only the date part (no time)
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        // Store only the time part (no date)
        [Column(TypeName = "Time")]
        public TimeSpan StartTime { get; set; }

        [Column(TypeName = "Time")]
        public TimeSpan EndTime { get; set; }

        // Store UTC dates, but with only date or time part stored in the DB
        [Column(TypeName = "Date")]
        public DateTime AddedOn { get; set; }

        [Column(TypeName = "Date")]
        public DateTime LastUpdatedOn { get; set; }
    }
}
