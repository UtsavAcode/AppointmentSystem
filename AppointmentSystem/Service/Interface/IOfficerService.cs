using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Service.Interface
{
    public interface IOfficerService
    {
        Task<IEnumerable<AllOfficerViewModel>> GetAllOfficersWithPostAsync();
        Task<OfficerViewModel> GetOfficerByIdAsync(int id);
  
        Task UpdateOfficerAsync(OfficerViewModel model);
        Task<bool> ToggleOfficerStatusAsync(int id, bool status);
        Task<(bool success, string message)> DeactivatePostAsync(int postId);
        Task CreateOfficerAsync(OfficerViewModel model);
        Task<IEnumerable<Officer>> GetActiveOfficersAsync();
        Task<IEnumerable<Officer>> GetActiveOfficersByPostIdAsync(int postId);
    }
}
