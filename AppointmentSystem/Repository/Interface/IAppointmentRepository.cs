using AppointmentSystem.Models.Domain;

namespace AppointmentSystem.Repository.Interface
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment model);
        Task UpdateAsync(Appointment model);
        Task <List<Appointment>> GetAllAsync();
        Task <Appointment> GetAsync(int id);
        Task CancelAsync(int id);
    }
}
