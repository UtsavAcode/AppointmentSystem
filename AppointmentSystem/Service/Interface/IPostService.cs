using AppointmentSystem.Models.ViewModel;

namespace AppointmentSystem.Service.Interface
{
    public interface IPostService
    {
        Task<IEnumerable<PostViewModel>> GetAllPostsAsync();
        Task<PostViewModel?> GetPostByIdAsync(int id);
        Task CreatePostAsync(PostViewModel model);
        Task UpdatePostAsync(PostViewModel model);
        Task DeletePostAsync(int id);
    }
}
