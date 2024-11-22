using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models.ViewModel
{
    public class VisitorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be a 10-digit number.")]
        public int MobileNumber { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }

        public bool Status { get; set; } = true;
    }
}

