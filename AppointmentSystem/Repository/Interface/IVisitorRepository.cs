using AppointmentSystem.Models.Domain;

namespace AppointmentSystem.Repository.Interface
{
    public interface IVisitorRepository
    {
        Task<Visitor> GetVisitorByIdAsync(int id);
        Task<IEnumerable<Visitor>> GetAllVisitorsAsync();
        Task InsertVisitorAsync(Visitor visitor);
        Task UpdateVisitorAsync(Visitor visitor);
        Task DeleteVisitorAsync(int id);
    }
}
