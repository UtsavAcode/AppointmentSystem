using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Repository.Interface
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment model);
        Task UpdateAsync(Appointment model);
        Task<List<AllAppointmentViewmodel>> GetAllAsync();
        Task <Appointment> GetAsync(int id);
        Task CancelAsync(int id);

        Task<bool> HasExistingAppointment(int visitorId, DateTime date);
        Task<bool> HasExistingAppointmentDate(DateTime date);

        Task<IEnumerable<Appointment>> GetFutureAppointmentsByVisitorIdAsync(int visitorId);
    }
}
