using AppointmentSystem.Data;
using AppointmentSystem.Models.Domain;
using AppointmentSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;

namespace AppointmentSystem.Repository.Implementation
{
    public class PostRepository : IPostRepository
    {

        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts.ToListAsync();
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts.FindAsync(id);
        }

        public async Task AddAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            var existingPost = await _context.Posts.FindAsync(post.Id);
            if (existingPost != null)
            {
                existingPost.Name = post.Name;
            

                await _context.SaveChangesAsync(); // Save changes to the database
            }
            else
            {
                throw new InvalidOperationException("Post not found");
            }
        }


        public async Task DeleteAsync(int id)
        {
            var post = await GetByIdAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

    }
}
