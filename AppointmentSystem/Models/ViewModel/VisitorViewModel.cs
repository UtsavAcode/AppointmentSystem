using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModel
{
    public class VisitorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^\d+${11}", ErrorMessage = "Please enter valid mobile number")]
        public int MobileNumber { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        public bool Status { get; set; }
    }
}

