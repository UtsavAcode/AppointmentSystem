using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Repository.Interface
{
    public interface IActivityRepository
    {
        Task CancelActivityAsync(int activityId);
        Task CreateActivityAsync(ActivityViewModel model);
      Task<IEnumerable<AllActivitiesViewModel>> GetActivitiesForOfficerAsync(int officerId);
        Task<ActivityViewModel> GetActivityByIdAsync(int activityId);
        Task CreateAppointmentActivityAsync(int officerId, DateOnly startDate, TimeOnly startTime, DateOnly endDate, TimeOnly endTime);
        Task<IEnumerable<AllActivitiesViewModel>> GetAllActivityAsync();
        Task UpdateActivityAsync(int activityId, ActivityViewModel model);
        Task<IEnumerable<Activity>> GetFutureActivityByIdAsync(int officerId);
        Task<List<Activity>> GetActivitiesByOfficerAndDateAsync(int officerId, DateOnly date);

    }
}
