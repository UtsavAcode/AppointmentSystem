using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Service.Interface
{
    public interface IActivityService
    {
        Task CreateActivityAsync(ActivityViewModel model);
        Task <IEnumerable<AllActivitiesViewModel>> GetAllActivitiesAsync();
        Task CancelActivityAsync(int activityId);
        Task UpdateActivityAsync(int activityId, ActivityViewModel model);
        Task<ActivityViewModel> GetActivityByIdAsync(int activityId);
        //ActivityViewModel GetActivityById(int activityId);
        //IEnumerable<ActivityViewModel> GetActivitiesForOfficer(int officerId);
        //void CreateAppointmentActivity(int officerId, DateOnly startDate, TimeOnly startTime, DateOnly endDate, TimeOnly endTime);
    }
}
