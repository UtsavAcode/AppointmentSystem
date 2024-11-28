using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Implementation;
using AppointmentSystem.Repository.Interface;
using AppointmentSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using static AppointmentSystem.Models.Domain.Appointment;

namespace AppointmentSystem.Service.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private IAppointmentRepository _repo;
        private IActivityRepository _activityRepo;

        public AppointmentService(IAppointmentRepository repo, IActivityRepository activityRepo)
        {
            _repo = repo;
            _activityRepo = activityRepo;
        }
        public async Task CancelAsync(int id)
        {
            var appointment = await _repo.GetAsync(id);

            if (appointment != null)
            {
                
                appointment.Status = AppointmentStatus.Cancelled;
                appointment.LastUpdatedOn = DateTime.UtcNow;

               
                await _repo.UpdateAsync(appointment);
            }
        }



        public async Task CreateAsync(AppointmentViewmodel model)
        {
            if (model.Date < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException(
                    $"The appointment date cannot be in the past. Provided date: {model.Date.ToShortDateString()}.");
            }

           
            var hasExistingAppointment = await _repo.HasExistingAppointment(model.VisitorId, model.Date);
            if (hasExistingAppointment)
            {
                throw new InvalidOperationException(
                    $"Visitor already has an appointment scheduled for {model.Date.ToShortDateString()}. " +
                    "Only one appointment per day is allowed.");
            }

         
            var isOfficerAvailable = await IsOfficerAvailableAsync(model.OfficerId, DateOnly.FromDateTime(model.Date), model.StartTime, model.EndTime);
            if (!isOfficerAvailable)
            {
                throw new InvalidOperationException("The officer is unavailable during the selected time.");
            }

    
            var utcDate = model.Date.ToUniversalTime();

          
            var appointment = new Appointment
            {
                OfficerId = model.OfficerId,
                VisitorId = model.VisitorId,
                Name = model.Name,
                Date = utcDate,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                AddedOn = DateTime.UtcNow,
                LastUpdatedOn = DateTime.UtcNow
            };

            await _repo.AddAsync(appointment);

        
            await _activityRepo.CreateAppointmentActivityAsync(
                model.OfficerId,
                DateOnly.FromDateTime(model.Date), 
                TimeOnly.FromTimeSpan(model.StartTime),
                DateOnly.FromDateTime(model.Date), 
                TimeOnly.FromTimeSpan(model.EndTime) 
            );
        }

        public async Task UpdateAsync(EditAppointment model)
        {
            if (model.Date < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException(
                    $"The appointment date cannot be in the past. Provided date: {model.Date.ToShortDateString()}.");
            }

       
            var hasExistingAppointment = await _repo.HasExistingAppointmentDate(model.Date);
            if (hasExistingAppointment)
            {
                throw new InvalidOperationException(
                    $"Visitor already has an appointment scheduled for {model.Date.ToShortDateString()}. Only one appointment per day is allowed.");
            }

           
            var isOfficerAvailable = await IsOfficerAvailableAsync(model.OfficerId, DateOnly.FromDateTime(model.Date), model.StartTime, model.EndTime);
            if (!isOfficerAvailable)
            {
                throw new InvalidOperationException("The officer is unavailable during the selected time.");
            }

         
            var appointment = await _repo.GetAsync(model.Id);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found for the given ID.");
            }

         
            appointment.OfficerId = model.OfficerId;
            appointment.VisitorId = model.VisitorId;
            appointment.Name = model.Name;
            appointment.Date = DateTime.SpecifyKind(model.Date, DateTimeKind.Utc);
            appointment.StartTime = model.StartTime;
            appointment.EndTime = model.EndTime;
            appointment.LastUpdatedOn = DateTime.UtcNow;

            await _repo.UpdateAsync(appointment);
        }


        public async Task<List<AllAppointmentViewmodel>> GetAllAsync()
        {
            var appointment = await _repo.GetAllAsync();
            return appointment;
        }


        public async Task<AppointmentViewmodel> GetAsync(int id)
        {
            var appointment = await _repo.GetAsync(id);
            if (appointment == null) return null;

            return new AppointmentViewmodel
            {
                Id = appointment.Id,
                OfficerId = appointment.OfficerId,
                VisitorId = appointment.VisitorId,
                Name = appointment.Name,
                Date = appointment.Date,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status,
                AddedOn = DateTime.UtcNow,
            };
        }



        private async Task<bool> IsOfficerAvailableAsync(int officerId, DateOnly date, TimeSpan startTime, TimeSpan endTime)
        {
            var activities = await _activityRepo.GetActivitiesByOfficerAndDateAsync(officerId, date);

            foreach (var activity in activities)
            {
                TimeSpan activityStart = activity.StartTime.ToTimeSpan();
                TimeSpan activityEnd = activity.EndTime.ToTimeSpan();

                if ((startTime >= activityStart && startTime < activityEnd) ||
                    (endTime > activityStart && endTime <= activityEnd) ||
                    (startTime <= activityStart && endTime >= activityEnd))
                {
                    return false;
                }
            }

            return true;
        }


        public async Task<List<AllAppointmentViewmodel>> GetAppointmentsByOfficerIdAsync(int officerId)
        {
            
            var appointments = await _repo.GetAppointmentsByOfficerIdAsync(officerId);

            return appointments;
        }



        public async Task ToggleStatusAsync(int id)
        {
            var appointment = await _repo.GetAsync(id);

            if (appointment == null)
            {
                throw new KeyNotFoundException($"No appointment found with ID {id}.");
            }

            if (appointment.Status == AppointmentStatus.Active)
            {
                appointment.Status = AppointmentStatus.Deactivated;
            }
            else if (appointment.Status == AppointmentStatus.Deactivated)
            {
                appointment.Status = AppointmentStatus.Active;
            }
            else
            {
                throw new InvalidOperationException("Invalid appointment status to toggle.");
            }

            appointment.LastUpdatedOn = DateTime.UtcNow;
            await _repo.UpdateAsync(appointment);
        }



        public async Task<List<AllAppointmentViewmodel>> GetAppointmentsByVisitorIdAsync(int visitorId)
        {
            var appointments = await _repo.GetAppointmentsByVisitorIdAsync(visitorId);
            return appointments;
        }

    }
}

