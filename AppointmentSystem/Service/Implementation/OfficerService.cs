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
        private readonly IWorkdaysService _workdaysService;

        public OfficerService(IOfficerRepository officerRepository, IWorkdaysService workdaysService, IVisitorRepository visitorRepository, IPostRepository postRepository, ApplicationDbContext context, IActivityRepository activityRepository)
        {
            _officerRepository = officerRepository;
            _postRepository = postRepository;
            _context = context;
            _activityRepository = activityRepository;
            _visitorRepository = visitorRepository;
           _workdaysService =  workdaysService;
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
            var existingOfficer = await _officerRepository.GetOfficerByIdAsync(model.Id);
            if (existingOfficer != null)
            {
                
                existingOfficer.Name = model.Name;
                existingOfficer.PostId = model.PostId;

               
                if (TimeOnly.TryParse(model.WorkStartTime, out var parsedWorkStartTime) &&
                    TimeOnly.TryParse(model.WorkEndTime, out var parsedWorkEndTime))
                {
                    existingOfficer.WorkStartTime = parsedWorkStartTime.ToString("HH:mm");
                    existingOfficer.WorkEndTime = parsedWorkEndTime.ToString("HH:mm");

                   
                    await CancelInvalidActivities(existingOfficer.Id, parsedWorkStartTime, parsedWorkEndTime);
                }
                else
                {
                    throw new FormatException("WorkStartTime or WorkEndTime is not in a valid TimeOnly format.");
                }
 
                await _officerRepository.UpdateOfficerAsync(existingOfficer);

              
                if (model.WorkDays != null && model.WorkDays.Any())
                {
                    
                    await _workdaysService.RemoveWorkDaysAsync(existingOfficer.Id);

                     
                    foreach (var workDay in model.WorkDays)
                    {
                        var workDayModel = new WorkDayViewModel
                        {
                            OfficerId = existingOfficer.Id,
                            DayOfWeek = workDay.DayOfWeek
                        };
                         
                        await _workdaysService.AddWorkDayAsync(workDayModel);
                    }
                }
            }
        }



        private async Task CancelInvalidActivities(int officerId, TimeOnly workStartTime, TimeOnly workEndTime)
        {
            
            var futureActivities = await _activityRepository.GetFutureActivityByIdAsync(officerId);

            foreach (var activity in futureActivities)
            {
                
                if (activity.StartDate < DateOnly.FromDateTime(DateTime.UtcNow) ||
                    (activity.StartDate == DateOnly.FromDateTime(DateTime.UtcNow) && activity.StartTime < TimeOnly.FromDateTime(DateTime.UtcNow)))
                {
                    continue;
                }

                
                if (activity.StartTime < workStartTime || activity.EndTime > workEndTime)
                {
                    
                    activity.Status = ActivityStatus.Cancelled;

                    
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
            
            var officer = await _officerRepository.GetOfficerByIdAsync(id);
            if (officer == null)
                return false;

            if (status) 
            {
           
                var post = await _postRepository.GetByIdAsync(officer.PostId);
                if (!post.Status)
                    return false;  

                
                await ReactivateActivities(officer.Id);
            }
            else  
            {
                 
                await DeactivateActivities(officer.Id);
            }

           
            officer.Status = status;
            return await _officerRepository.UpdateOfficerAsync(officer);
        }


        private async Task ReactivateActivities(int officerId)
        {
             
            var futureActivities = await _activityRepository.GetFutureActivityByIdAsync(officerId);

            foreach (var activity in futureActivities)
            {
                 
                if (activity.Status == ActivityStatus.Deactivated)
                {
                    
                    var isVisitorActive = await _visitorRepository.IsVisitorActiveAsync(activity.ActivityId);

                    if (isVisitorActive)  
                    {

                        activity.Status = ActivityStatus.Active;


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

            var futureActivities = await _activityRepository.GetFutureActivityByIdAsync(officerId);


            foreach (var activity in futureActivities)
            {
                if (activity.Status == ActivityStatus.Active)
                {

                    activity.Status = ActivityStatus.Deactivated;


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
            return await _context.Officers.Where(o => o.Status == true).ToListAsync();
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

                
                if (model.WorkDays != null && model.WorkDays.Any())
                {
                    foreach (var workDay in model.WorkDays)
                    {
                        var workDayModel = new WorkDayViewModel
                        {
                            OfficerId = officer.Id,
                            DayOfWeek = workDay.DayOfWeek
                        };

                        
                        await _workdaysService.AddWorkDayAsync(workDayModel);
                    }
                }
            }
        }


        public async Task<bool> IsOfficerAvailableAsync(int officerId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {

            var activities = await _context.Activities
                .Where(a => a.OfficerId == officerId &&
                            a.StartDate <= date &&
                            a.EndDate >= date &&
                            a.Status == ActivityStatus.Active &&
                            (a.Type == ActivityType.Leave || a.Type == ActivityType.Break || a.Type == ActivityType.Appointment))
                .ToListAsync();


            foreach (var activity in activities)
            {
                if ((startTime >= activity.StartTime && startTime < activity.EndTime) ||
                    (endTime > activity.StartTime && endTime <= activity.EndTime) ||
                    (startTime <= activity.StartTime && endTime >= activity.EndTime))
                {
                    return false;
                }
            }

            return true;



        }
    }
}