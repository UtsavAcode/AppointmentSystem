using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Service.Interface
{
    public interface IVisitorService
    {
        Task<VisitorViewModel> GetVisitorByIdAsync(int id);
        Task<IEnumerable<VisitorViewModel>> GetAllVisitorsAsync();
        Task CreateVisitorAsync(VisitorViewModel visitorViewModel);
        Task UpdateVisitorAsync(VisitorViewModel visitorViewModel);
     
    }
}
