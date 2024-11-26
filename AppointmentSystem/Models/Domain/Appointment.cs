using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentSystem.Models.Domain
{
    public class Appointment
    {
        public int Id { get; set; }

        [ForeignKey("Officer")]
        public int OfficerId { get; set; }

        [ForeignKey("Visitor")]
        public int VisitorId { get; set; }
        public string Name { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Active;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }


        public Officer Officer { get; set; }
        public Visitor Visitor { get; set; }

        public enum AppointmentStatus
        {
            Active,
            Cancelled,
            Deactivated,
            Completed
        }


    }
}