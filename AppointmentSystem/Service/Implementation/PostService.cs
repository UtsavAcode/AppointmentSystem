using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Interface;
using AppointmentSystem.Service.Interface;

namespace AppointmentSystem.Service.Implementation
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repository;

        public PostService(IPostRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PostViewModel>> GetAllPostsAsync()
        {
            var posts = await _repository.GetAllAsync();
            return posts.Select(post => new PostViewModel
            {
                Id = post.Id,
                Name = post.Name,
                Status = post.Status
            });
        }

        public async Task<PostViewModel?> GetPostByIdAsync(int id)
        {
            var post = await _repository.GetByIdAsync(id);
            if (post == null) return null;

            return new PostViewModel
            {
                Id = post.Id,
                Name = post.Name,
                Status = post.Status
            };
        }

        public async Task CreatePostAsync(PostViewModel model)
        {
            var post = new Post
            {
                Name = model.Name,
                Status = model.Status // Use the status from the model
            };

            await _repository.AddAsync(post);
        }

        public async Task UpdatePostAsync(PostViewModel model)
        {
            var post = await _repository.GetByIdAsync(model.Id);
            if (post == null)
            {
                throw new Exception("Post not found");
            }

            // Update the name while preserving the status
            post.Name = model.Name;
            post.Status = model.Status;

            await _repository.UpdateAsync(post);
        }


        public async Task DeletePostAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
