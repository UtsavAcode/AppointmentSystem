using AppointmentSystem.Data;
using AppointmentSystem.Models.Domain;
using AppointmentSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;

namespace AppointmentSystem.Repository.Implementation
{
    public class VisitorRepository : IVisitorRepository
    {
        private readonly ApplicationDbContext _context;

        public VisitorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Visitor> GetVisitorByIdAsync(int id)
        {
            return await _context.Visitors.SingleOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Visitor>> GetAllVisitorsAsync()
        {
            return await _context.Visitors.ToListAsync();
        }

        public async Task InsertVisitorAsync(Visitor visitor)
        {
            await _context.Visitors.AddAsync(visitor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVisitorAsync(Visitor visitor)
        {
            // No need to attach or set state - just update
            _context.Update(visitor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVisitorAsync(int id)
        {
            var visitor = await _context.Visitors.SingleOrDefaultAsync(v => v.Id == id);
            if (visitor != null)
            {
                _context.Visitors.Remove(visitor);
                await _context.SaveChangesAsync();
            }
        }

     
   

        public async Task<IEnumerable<Visitor>> GetActiveVisitorsAsync()
        {
            return await _context.Visitors
                .Where(v => v.Status == true)
                .ToListAsync();
        }

    }
}