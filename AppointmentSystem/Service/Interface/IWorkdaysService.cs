using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Service.Interface
{
    public interface IWorkdaysService
    {
        Task AddWorkDayAsync(WorkDayViewModel model);
        Task UpdateWorkDayAsync(WorkDayViewModel model);
        Task<IEnumerable<AllWorkDaysViewModel>> GetAllWorkDaysAsync();
        Task<WorkDayViewModel> GetWorkByIdAsync(int id);
    }
}
