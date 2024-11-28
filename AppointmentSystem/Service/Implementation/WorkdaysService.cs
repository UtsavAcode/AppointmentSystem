using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Interface;
using AppointmentSystem.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AppointmentSystem.Service.Implementation
{
    public class WorkdaysService : IWorkdaysService
    {
        private IWorkDaysRepository _repository;

        public WorkdaysService(IWorkDaysRepository repository)
        {
            _repository = repository;
        }
        public async Task AddWorkDayAsync(WorkDayViewModel model)
        {

            var existingDay = await _repository.GetByOfficerIdAndDayAsync(model.OfficerId, model.DayOfWeek);
            if (existingDay != null)
            {
                throw new InvalidOperationException($"Officer with ID {model.OfficerId} is already assigned to Day {model.DayOfWeek}.");
            }

            var day = new WorkDay
            {
                DayOfWeek = model.DayOfWeek,
                OfficerId = model.OfficerId,
            };

            await _repository.AddAsync(day);
        }

        public async Task<IEnumerable<AllWorkDaysViewModel>> GetAllWorkDaysAsync()
        {
            var days = await _repository.GetAllAsync();
            return days;
        }

        public async Task<WorkDayViewModel> GetWorkByIdAsync(int id)
        {
            var day = await _repository.GetByIdAsync(id);
            if (day == null) return null;
            return new WorkDayViewModel
            {
                DayOfWeek = day.DayOfWeek,
                OfficerId = day.OfficerId,
            };

        }

        public async Task UpdateWorkDayAsync(WorkDayViewModel model)
        {
            var existingOfficer = await _repository.GetByOfficerIdAndDayAsync(model.OfficerId, model.DayOfWeek);
            if (existingOfficer != null && existingOfficer.Id != model.Id)
            {
                throw new InvalidOperationException($"Officer with ID {model.OfficerId} is already assigned to Day {model.DayOfWeek}.");
            }

            var existingDay = await _repository.GetByIdAsync(model.Id);
            if (existingDay == null)
            {
                throw new InvalidOperationException($"WorkDay with ID {model.Id} not found.");
            }


            existingDay.DayOfWeek = model.DayOfWeek;
            existingDay.OfficerId = model.OfficerId;

            await _repository.UpdateAsync(existingDay);
        }


        public async Task RemoveWorkDaysAsync(int officerId)
        {
            await _repository.RemoveWorkDaysAsync(officerId);
        }
    }
}