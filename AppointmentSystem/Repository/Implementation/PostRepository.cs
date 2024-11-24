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

        public async Task<IEnumerable<Post>> GetActivePostAsync()
        {
            return await _context.Posts.Where(post => post.Status).ToListAsync();
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

        public async Task<bool> UpdateAsync(Post post)
        {
            var existingPost = await _context.Posts.FindAsync(post.Id);
            if (existingPost != null)
            {
                existingPost.Name = post.Name;
                existingPost.Status = post.Status; // Ensure the Status is also updated.

                var result = await _context.SaveChangesAsync();
                return result > 0; // Returns true if at least one row was affected.
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
