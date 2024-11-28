using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModel
{
    public class OfficerViewModel
    {
        public int Id { get; set; }


        [StringLength(100, ErrorMessage = "Name length must be under 100 characters.")]
        public string Name { get; set; }


        public int PostId { get; set; }

        public bool Status { get; set; }

  
    
        public string WorkStartTime { get; set; }

 
        public string WorkEndTime { get; set; }
        public List<WorkDayViewModel> WorkDays { get; set; }









    }
}
