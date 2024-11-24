using AppointmentSystem.Models.ViewModel;
using System.Threading.Tasks;

namespace AppointmentSystem.Service.Interface
{
    public interface IPostService
    {
        Task<IEnumerable<PostViewModel>> GetAllPostsAsync();
        Task<PostViewModel?> GetPostByIdAsync(int id);
        Task CreatePostAsync(PostViewModel model);
        Task UpdatePostAsync(PostViewModel model);
        Task DeletePostAsync(int id);
        Task<IEnumerable<PostViewModel>> GetActivePostAsync();
        Task<(bool success, string message)> DeactivatePostAsync(int postId);
    }
}
