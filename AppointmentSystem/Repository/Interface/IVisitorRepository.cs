using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Repository.Interface
{
    public interface IVisitorRepository
    {
        Task<Visitor> GetVisitorByIdAsync(int id);
        Task<IEnumerable<Visitor>> GetAllVisitorsAsync();
        Task InsertVisitorAsync(Visitor visitor);
        Task UpdateVisitorAsync(Visitor visitor);
        Task DeleteVisitorAsync(int id);
        Task<IEnumerable<Visitor>> GetActiveVisitorsAsync();
        Task<bool> IsVisitorActiveAsync(int visitorId);
    }
}
