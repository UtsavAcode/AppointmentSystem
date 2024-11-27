using AppointmentSystem.Models.Domain;

public enum ActivityType
{
    Leave,
    Break,
    Appointment
}

// Enum for activity status
public enum ActivityStatus
{
    Active,
    Deactivated,
    Cancelled
}

public class Activity
{
    public int ActivityId { get; set; }
    public ActivityType Type { get; set; }
    public int OfficerId { get; set; }
    public DateOnly StartDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly EndTime { get; set; }
    public ActivityStatus Status { get; set; }
    public Officer Officer { get; set; }
}