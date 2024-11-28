using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Service.Interface
{
    public interface IAppointmentService
    {
        Task CreateAsync(AppointmentViewmodel model);
        Task UpdateAsync (EditAppointment model);
        Task <AppointmentViewmodel> GetAsync (int id);
        Task <List<AllAppointmentViewmodel>> GetAllAsync ();
        Task CancelAsync(int id);
    
        Task<List<AllAppointmentViewmodel>> GetAppointmentsByOfficerIdAsync(int officerId);
        Task<List<AllAppointmentViewmodel>> GetAppointmentsByVisitorIdAsync(int visitorId);
    }
}
