using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Service.Interface
{
    public interface IAppointmentService
    {
        Task CreateAsync(AppointmentViewmodel model);
        Task UpdateAsync (AppointmentViewmodel model);
        Task <AppointmentViewmodel> GetAsync (int id);
        Task <List<AppointmentViewmodel>> GetAllAsync ();
        Task CancelAsync(int id);
    }
}
