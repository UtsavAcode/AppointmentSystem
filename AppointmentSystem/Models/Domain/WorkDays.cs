namespace AppointmentSystem.Models.Domain
{
    public class WorkDays
    {
        public int OfficerId { get; set; }
        public int DayOfWeek { get; set; }

        public Officer Officer { get; set; }
    }
}
