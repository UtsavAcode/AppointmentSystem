using AppointmentSystem.Models.Domain;

namespace AppointmentSystem.Repository.Interface
{
    public interface IOfficerRepository
    {
        Task<IEnumerable<Officer>> GetAllOfficersAsync();
        Task<Officer> GetOfficerByIdAsync(int id);
        Task<bool> InsertOfficerAsync(Officer officer);
        Task<bool> UpdateOfficerAsync(Officer officer);
        Task<IEnumerable<Officer>> GetActiveOfficersByPostIdAsync(int postId);
     
    }
}
