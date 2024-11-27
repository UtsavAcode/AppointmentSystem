using AppointmentSystem.Data;
using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Implementation;
using AppointmentSystem.Repository.Interface;
using AppointmentSystem.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static AppointmentSystem.Models.Domain.Appointment;

namespace AppointmentSystem.Service.Implementation
{
    public class OfficerService : IOfficerService
    {
        private readonly IOfficerRepository _officerRepository;
        private readonly IPostRepository _postRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IVisitorRepository _visitorRepository;
        private readonly ApplicationDbContext _context;

        public OfficerService(IOfficerRepository officerRepository, IVisitorRepository visitorRepository, IPostRepository postRepository, ApplicationDbContext context, IActivityRepository activityRepository)
        {
            _officerRepository = officerRepository;
            _postRepository = postRepository;
            _context = context;
            _activityRepository = activityRepository;
            _visitorRepository = visitorRepository;
        }

        public async Task<OfficerViewModel> GetOfficerByIdAsync(int id)
        {
            var officer = await _officerRepository.GetOfficerByIdAsync(id);
            return officer != null ? MapToViewModel(officer) : null;
        }



        public async Task<IEnumerable<AllOfficerViewModel>> GetAllOfficersWithPostAsync()
        {
            var officers = await _officerRepository.GetAllOfficersAsync();
            return officers;
        }

        public async Task UpdateOfficerAsync(OfficerViewModel model)
        {
            var existingUser = await _officerRepository.GetOfficerByIdAsync(model.Id);
            if (existingUser != null)
            {
                existingUser.Name = model.Name;
                existingUser.PostId = model.PostId;

                // Parse WorkStartTime and WorkEndTime strings into TimeOnly
                if (TimeOnly.TryParse(model.WorkStartTime, out var parsedWorkStartTime) &&
                    TimeOnly.TryParse(model.WorkEndTime, out var parsedWorkEndTime))
                {
                    // Convert TimeOnly back to string before assigning
                    existingUser.WorkStartTime = parsedWorkStartTime.ToString("HH:mm");
                    existingUser.WorkEndTime = parsedWorkEndTime.ToString("HH:mm");

                    // Cancel invalid activities
                    await CancelInvalidActivities(existingUser.Id, parsedWorkStartTime, parsedWorkEndTime);
                }
                else
                {
                    throw new FormatException("WorkStartTime or WorkEndTime is not in a valid TimeOnly format.");
                }

                await _officerRepository.UpdateOfficerAsync(existingUser);
            }
        }



        private async Task CancelInvalidActivities(int officerId, TimeOnly workStartTime, TimeOnly workEndTime)
        {
            // Retrieve all future activities for the officer
            var futureActivities = await _activityRepository.GetFutureActivityByIdAsync(officerId);

            foreach (var activity in futureActivities)
            {
                // Skip past activities
                if (activity.StartDate < DateOnly.FromDateTime(DateTime.UtcNow) ||
                    (activity.StartDate == DateOnly.FromDateTime(DateTime.UtcNow) && activity.StartTime < TimeOnly.FromDateTime(DateTime.UtcNow)))
                {
                    continue;
                }

                // Check if the activity falls outside the officer's new work schedule
                if (activity.StartTime < workStartTime || activity.EndTime > workEndTime)
                {
                    // Update activity status to Cancelled
                    activity.Status = ActivityStatus.Cancelled;

                    // Update the activity in the database
                    await _activityRepository.UpdateActivityAsync(activity.ActivityId, new ActivityViewModel
                    {
                        ActivityId = activity.ActivityId,
                        Type = activity.Type,
                        OfficerId = activity.OfficerId,
                        StartDate = activity.StartDate,
                        StartTime = activity.StartTime,
                        EndDate = activity.EndDate,
                        EndTime = activity.EndTime,
                        Status = ActivityStatus.Cancelled
                    });
                }
            }
        }



        public async Task<bool> ToggleOfficerStatusAsync(int id, bool status)
        {
            // Retrieve the officer by ID
            var officer = await _officerRepository.GetOfficerByIdAsync(id);
            if (officer == null)
                return false;

            if (status) // If trying to activate
            {
                // Ensure the post associated with the officer is active
                var post = await _postRepository.GetByIdAsync(officer.PostId);
                if (!post.Status)
                    return false; // Cannot activate if post is inactive

                // Reactivate related future activities that were deactivated
                await ReactivateActivities(officer.Id);
            }
            else // Deactivating officer
            {
                // Deactivate related future active activities
                await DeactivateActivities(officer.Id);
            }

            // Update the officer's status
            officer.Status = status;
            return await _officerRepository.UpdateOfficerAsync(officer);
        }


        private async Task ReactivateActivities(int officerId)
        {
            // Retrieve all future activities for the officer
            var futureActivities = await _activityRepository.GetFutureActivityByIdAsync(officerId);

            foreach (var activity in futureActivities)
            {
                // Check if the activity is currently deactivated
                if (activity.Status == ActivityStatus.Deactivated)
                {
                    // Check if the visitor associated with the activity is active
                    var isVisitorActive = await _visitorRepository.IsVisitorActiveAsync(activity.ActivityId);

                    if (isVisitorActive) // Ensure the visitor status allows reactivation
                    {
                        // Set the activity status to Active
                        activity.Status = ActivityStatus.Active;

                        // Update the activity in the database
                        await _activityRepository.UpdateActivityAsync(activity.ActivityId, new ActivityViewModel
                        {
                            ActivityId = activity.ActivityId,
                            Type = activity.Type,
                            OfficerId = activity.OfficerId,
                            StartDate = activity.StartDate,
                            StartTime = activity.StartTime,
                            EndDate = activity.EndDate,
                            EndTime = activity.EndTime,
                            Status = ActivityStatus.Active
                        });
                    }
                }
            }
        }

        private async Task DeactivateActivities(int officerId)
        {
            // Retrieve all future activities for the specified visitor (officer)
            var futureActivities = await _activityRepository.GetFutureActivityByIdAsync(officerId);

            // Loop through the activities and update their status
            foreach (var activity in futureActivities)
            {
                if (activity.Status == ActivityStatus.Active)
                {
                    // Set the activity status to Deactivated
                    activity.Status = ActivityStatus.Deactivated;

                    // Update the activity in the database
                    await _activityRepository.UpdateActivityAsync(activity.ActivityId, new ActivityViewModel
                    {
                        ActivityId = activity.ActivityId,
                        Type = activity.Type,
                        OfficerId = activity.OfficerId,
                        StartDate = activity.StartDate,
                        StartTime = activity.StartTime,
                        EndDate = activity.EndDate,
                        EndTime = activity.EndTime,
                        Status = ActivityStatus.Deactivated
                    });
                }
            }
        }


        public async Task<(bool success, string message)> DeactivatePostAsync(int postId)
        {
            var activeOfficers = await _officerRepository.GetActiveOfficersByPostIdAsync(postId);
            if (activeOfficers.Any())
                return (false, "Cannot deactivate the post as it has active officers.");

            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                return (false, "Post not found.");

            post.Status = false;

            var success = await _postRepository.UpdateAsync(post);
            return success
                ? (true, "Post deactivated successfully.")
                : (false, "Failed to deactivate post.");
        }



        private OfficerViewModel MapToViewModel(Officer officer)
        {
            return new OfficerViewModel
            {
                Id = officer.Id,
                Name = officer.Name,
                PostId = officer.PostId,
                Status = officer.Status,
                WorkStartTime = officer.WorkStartTime,
                WorkEndTime = officer.WorkEndTime,

            };
        }



        public async Task<IEnumerable<Officer>> GetActiveOfficersByPostIdAsync(int postId)
        {
            return await _context.Officers
                .Where(o => o.PostId == postId && o.Status == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<Officer>> GetActiveOfficersAsync()
        {
            return await _context.Officers.Where(o=>o.Status==true).ToListAsync();
        }
       
        public async Task CreateOfficerAsync(OfficerViewModel model)
        {
            var officer = new Officer
            {

                Name = model.Name,
                PostId = model.PostId,
                Status = model.Status,
                WorkStartTime = model.WorkStartTime,
                WorkEndTime = model.WorkEndTime,

            };

            if (officer != null)
            {
                await _officerRepository.InsertOfficerAsync(officer);
            }

        }

        public async Task<bool> IsOfficerAvailableAsync(int officerId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            // Retrieve activities for the officer on the specified date
            var activities = await _context.Activities
                .Where(a => a.OfficerId == officerId &&
                            a.StartDate <= date &&
                            a.EndDate >= date &&
                            a.Status == ActivityStatus.Active &&
                            (a.Type == ActivityType.Leave || a.Type == ActivityType.Break || a.Type == ActivityType.Appointment))
                .ToListAsync();

            // Check for time overlaps
            foreach (var activity in activities)
            {
                if ((startTime >= activity.StartTime && startTime < activity.EndTime) ||
                    (endTime > activity.StartTime && endTime <= activity.EndTime) ||
                    (startTime <= activity.StartTime && endTime >= activity.EndTime))
                {
                    return false; // Officer is not available
                }
            }

            return true; // Officer is available
        }



    }
}
