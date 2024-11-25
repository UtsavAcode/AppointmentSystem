using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Repository.Interface
{
    public interface IOfficerRepository
    {
        Task<IEnumerable<AllOfficerViewModel>> GetAllOfficersAsync();
        Task<Officer> GetOfficerByIdAsync(int id);
        Task<bool>InsertOfficerAsync(Officer officer);
        Task<bool> UpdateOfficerAsync(Officer officer);
        Task<IEnumerable<Officer>> GetActiveOfficersByPostIdAsync(int postId);
     
    }
}
