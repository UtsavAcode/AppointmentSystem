namespace AppointmentSystem.Models.Domain
{
    public class Visitor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool Status { get; set; } =true;

       
    }
}
