using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Interface;
using AppointmentSystem.Service.Interface;
using static AppointmentSystem.Models.Domain.Appointment;

namespace AppointmentSystem.Service.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private IAppointmentRepository _repo;

        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }
        public async Task CancelAsync(int id)
        {
            await _repo.CancelAsync(id);
        }

        public async Task CreateAsync(AppointmentViewmodel model)
        {
            var appointment = new Appointment
            {
                Name = model.Name,
                OfficerId = model.OfficerId,
                VisitorId = model.VisitorId,
                Status = model.Status,
                Date = model.Date.ToUniversalTime(),  // Convert to UTC
                StartTime = model.StartTime.ToUniversalTime(),  // Convert to UTC
                EndTime = model.EndTime.ToUniversalTime(),  // Convert to UTC
                AddedOn = DateTime.UtcNow,  // Use UTC time
                LastUpdatedOn = DateTime.UtcNow,  //
            };

            await _repo.AddAsync(appointment);
        }

        public async Task<List<AppointmentViewmodel>> GetAllAsync()
        {
           var appointment = await _repo.GetAllAsync();
            return appointment.Select(a=> new AppointmentViewmodel {

                Id = a.Id,
                OfficerId = a.OfficerId,
                VisitorId = a.VisitorId,
                Name = a.Name,
                Date = a.Date,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status
            }).ToList();
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
                StartTime = appointment.StartTime.Date.ToUniversalTime(),
                EndTime = appointment.EndTime.ToUniversalTime(),
                Status = appointment.Status,
                AddedOn = DateTime.UtcNow,
            };
        }

        public async Task UpdateAsync(AppointmentViewmodel model)
        {
            var appointment = await _repo.GetAsync(model.Id);
            if (appointment == null) return;

            appointment.OfficerId = model.OfficerId;
            appointment.VisitorId = model.VisitorId;
            appointment.Name = model.Name;
            appointment.Date = model.Date;
            appointment.StartTime = model.StartTime;
            appointment.EndTime = model.EndTime;
            appointment.LastUpdatedOn = DateTime.UtcNow;

            await _repo.UpdateAsync(appointment);
        }
    }
}
