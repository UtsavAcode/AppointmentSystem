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
            // Ensure status is always true when creating
            model.Status = true;

            var post = new Post
            {
                Name = model.Name,
                Status = true // Explicitly set to true
            };
            await _repository.AddAsync(post);
        }
        public async Task UpdatePostAsync(PostViewModel model)
        {
            var post = new Post
            {
                Id = model.Id,
                Name = model.Name,
           
            };

            // Make sure the repository is updating the post correctly
            await _repository.UpdateAsync(post);
        }


        public async Task DeletePostAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}


