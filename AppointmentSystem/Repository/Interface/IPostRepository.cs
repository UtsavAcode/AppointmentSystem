using AppointmentSystem.Models.Domain;

namespace AppointmentSystem.Repository.Interface
{
    public interface IPostRepository
    {
        Task<Post> GetByIdAsync(int id);
        Task<IEnumerable<Post>> GetAllAsync();
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(int id);
    }
}
