using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModel
{
    public class OfficerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name length must be under 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Post selection is required.")]
        public int PostId { get; set; }

        public bool Status { get; set; }

        [Required(ErrorMessage = "Work start time is required.")]
    
        public string WorkStartTime { get; set; }

        [Required(ErrorMessage = "Work end time is required.")]
 
        public string WorkEndTime { get; set; }

       





        
        
    }
}
