using AppointmentSystem.Data;
using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using static AppointmentSystem.Models.Domain.Appointment;

namespace AppointmentSystem.Repository.Implementation
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CancelActivityAsync(int activityId)
        {
            var activity = await _context.Activities.FirstOrDefaultAsync(a => a.ActivityId == activityId);

            if (activity == null)
            {
                throw new KeyNotFoundException("Activity not found.");
            }

            if (activity.Status == ActivityStatus.Cancelled)
            {
                throw new InvalidOperationException("The activity is already cancelled.");
            }

            activity.Status = ActivityStatus.Cancelled;
            await _context.SaveChangesAsync();
        }


        public async Task CreateActivityAsync(ActivityViewModel model)
        {
            // Validate activity type
            if (model.Type == ActivityType.Appointment)
            {
                throw new InvalidOperationException("Appointment activities can only be created via appointment creation.");
            }

            // Validate dates and times
            var startDateTime = model.StartDate.ToDateTime(model.StartTime);
            var endDateTime = model.EndDate.ToDateTime(model.EndTime);

            if (startDateTime >= endDateTime)
            {
                throw new InvalidOperationException("End time must be after start time.");
            }

            // Check for existing activities for the officer in the same time period
            var hasOverlap = await _context.Activities
                .Where(a => a.OfficerId == model.OfficerId &&
                           a.Status == ActivityStatus.Active &&
                           !(a.EndDate < model.StartDate || a.StartDate > model.EndDate))
                .AnyAsync();

            if (hasOverlap)
            {
                throw new InvalidOperationException("Officer already has an activity scheduled during this time period.");
            }

            // Create new activity
            var activity = new Activity
            {
                Type = model.Type,  // Fixed: Using the model's Type instead of ActivityType.Type
                OfficerId = model.OfficerId,
                StartDate = model.StartDate,
                StartTime = model.StartTime,
                EndDate = model.EndDate,
                EndTime = model.EndTime,
                Status = ActivityStatus.Active
            };

            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AllActivitiesViewModel>> GetActivitiesForOfficerAsync(int officerId)
        {
            var activities = await _context.Activities
                .Include(a => a.Officer)
                .Where(a => a.OfficerId == officerId)
                .ToListAsync();

            return activities.Select(activity => new AllActivitiesViewModel
            {
                ActivityId = activity.ActivityId,
                Type = activity.Type,
                OfficerId = activity.OfficerId,
                OfficerName = activity.Officer.Name,
                StartDate = activity.StartDate,
                StartTime = activity.StartTime,
                EndDate = activity.EndDate,
                EndTime = activity.EndTime,
                Status = activity.Status
            });
        }
        public async Task<IEnumerable<AllActivitiesViewModel>> GetAllActivityAsync()
        {
            // Fetch all activities including related Officer data
            var activities = await _context.Activities
                                            .Include(a => a.Officer)  // Ensure the Officer data is included
                                            .ToListAsync();

            // Map the Activities to ActivityViewModels
            var activityViewModels = activities.Select(activity => new AllActivitiesViewModel
            {
                ActivityId = activity.ActivityId,
                Type = activity.Type,
                OfficerId = activity.OfficerId,
                OfficerName = activity.Officer.Name,  // Assuming 'Officer' has a 'Name' property
                StartDate = activity.StartDate,
                StartTime = activity.StartTime,
                EndDate = activity.EndDate,
                EndTime = activity.EndTime,
                Status = activity.Status
            });

            return activityViewModels;
        }


        public async Task<ActivityViewModel> GetActivityByIdAsync(int activityId)
        {
            var activity = await _context.Activities
                .Include(a => a.Officer)
                .FirstOrDefaultAsync(a => a.ActivityId == activityId);

            if (activity == null)
                throw new KeyNotFoundException("Activity not found.");

            return new ActivityViewModel
            {
                ActivityId = activity.ActivityId,
                Type = activity.Type,  // Fixed: Using Type instead of ActivityType
                OfficerId = activity.OfficerId,
                // OfficerName = activity.Officer.Name,
                StartDate = activity.StartDate,
                StartTime = activity.StartTime,
                EndDate = activity.EndDate,
                EndTime = activity.EndTime,
                Status = activity.Status
            };
        }

        public async Task CreateAppointmentActivityAsync(int officerId, DateOnly startDate, TimeOnly startTime, DateOnly endDate, TimeOnly endTime)
        {
            var activity = new Activity
            {
                Type = ActivityType.Appointment,
                OfficerId = officerId,
                StartDate = startDate,
                StartTime = startTime,
                EndDate = endDate,
                EndTime = endTime,
                Status = ActivityStatus.Active
            };

            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateActivityAsync(int activityId, ActivityViewModel model)
        {
            // Retrieve the existing activity
            var activity = await _context.Activities.FirstOrDefaultAsync(a => a.ActivityId == activityId);

            if (activity == null)
            {
                throw new KeyNotFoundException("Activity not found.");
            }

            // Validate new dates and times
            var startDateTime = model.StartDate.ToDateTime(model.StartTime);
            var endDateTime = model.EndDate.ToDateTime(model.EndTime);

            if (startDateTime >= endDateTime)
            {
                throw new InvalidOperationException("End time must be after start time.");
            }

            // Check for overlapping activities for the officer, excluding the current activity
            var hasOverlap = await _context.Activities
                .Where(a => a.OfficerId == model.OfficerId &&
                            a.ActivityId != activityId &&
                            a.Status == ActivityStatus.Active &&
                            !(a.EndDate < model.StartDate || a.StartDate > model.EndDate))
                .AnyAsync();

            if (hasOverlap)
            {
                throw new InvalidOperationException("Officer already has another activity scheduled during this time period.");
            }

            // Update activity properties
            activity.Type = model.Type;
            activity.OfficerId = model.OfficerId;
            activity.StartDate = model.StartDate;
            activity.StartTime = model.StartTime;
            activity.EndDate = model.EndDate;
            activity.EndTime = model.EndTime;
            //  activity.Status = model.Status;

            // Save changes to the database
            await _context.SaveChangesAsync();
        }



        public async Task<IEnumerable<Activity>> GetFutureActivityByIdAsync(int officerId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Activities
                .Where(a => a.OfficerId == officerId &&
                            a.StartDate >= today &&
                            (a.Status == ActivityStatus.Active ||
                             a.Status == ActivityStatus.Deactivated))
                .ToListAsync();
        }

        public async Task<List<Activity>> GetActivitiesByOfficerAndDateAsync(int officerId, DateOnly date)
        {
            return await _context.Activities
                .Where(a => a.OfficerId == officerId && a.StartDate == date && a.Status == ActivityStatus.Active)
                .ToListAsync();
        }




    }
}
