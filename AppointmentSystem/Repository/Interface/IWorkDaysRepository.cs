using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Repository.Interface
{
    public interface IWorkDaysRepository
    {
        Task<IEnumerable<AllWorkDaysViewModel>> GetAllAsync();
        Task<WorkDay> GetByIdAsync(int id);
        Task AddAsync(WorkDay workDay);
        Task UpdateAsync(WorkDay workDay);
        Task<WorkDay> GetByOfficerIdAndDayAsync(int officerId, int dayOfWeek);
        Task<WorkDay> GetByOfficerIdAsync(int officerId);


    }
}
