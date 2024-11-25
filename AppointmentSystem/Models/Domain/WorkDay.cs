namespace AppointmentSystem.Models.Domain
{
    public class WorkDay
    {
        public int Id {  get; set; }
        public int OfficerId { get; set; }
        public int DayOfWeek { get; set; }
        public Officer Officer { get; set; }
    }
}
