using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModel
{
    public class WorkDayViewModel
    {
        public int Id { get; set; }
        public int OfficerId { get; set; }

        [Range(1, 7, ErrorMessage = "Day of the week must be between 1 and 7.")]
        public int DayOfWeek { get; set; }
      
    }
}
