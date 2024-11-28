using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Interface;
using AppointmentSystem.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AppointmentSystem.Service.Implementation
{
    public class ActivityService : IActivityService

    {
        private readonly IActivityRepository _activityRepository;

        public ActivityService(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

    
        public async Task CreateActivityAsync(ActivityViewModel model)
        {
            await _activityRepository.CreateActivityAsync(model);
        }

        public async Task CancelActivityAsync(int activityId)
        {
            await _activityRepository.CancelActivityAsync(activityId);
        }

        public async Task<IEnumerable<AllActivitiesViewModel>> GetAllActivitiesAsync()
        {
            return await _activityRepository.GetAllActivityAsync();

        }

        public async Task<ActivityViewModel> GetActivityByIdAsync(int activityId)
        {
            try
            {
              
                var activity = await _activityRepository.GetActivityByIdAsync(activityId);

                if (activity == null)
                    throw new Exception("Activity not found.");

                return activity;
            }
            catch (KeyNotFoundException ex)
            {
                
                throw new Exception($"Error retrieving activity: {ex.Message}");
            }
            catch (Exception ex)
            {
                
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }
        }


        public async Task UpdateActivityAsync(int activityId, ActivityViewModel model)
        {

            await _activityRepository.UpdateActivityAsync(activityId, model);
        }
    }
}
