using AppointmentSystem.Data;

using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AppointmentSystem.Repository.Implementation
{
    public class WorkDaysRepository : IWorkDaysRepository
    {
        private ApplicationDbContext _context;

        public WorkDaysRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(WorkDay workDay)
        {
            _context.WorkDays.Add(workDay);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<AllWorkDaysViewModel>> GetAllAsync()
        {

            var days = await _context.WorkDays.Include(o => o.Officer).ToListAsync();

            return days.Select(o => new AllWorkDaysViewModel
            {
                Id = o.Id,
                DayOfWeek = o.DayOfWeek,
                OfficerId = o.OfficerId,
                OfficerName = o.Officer?.Name,
            });
        }

        public async Task<WorkDay?> GetByIdAsync(int id)
        {
            return await _context.WorkDays.FindAsync(id);
        }

        public async Task UpdateAsync(WorkDay workDay)
        {
            
            var existingDay = await _context.WorkDays.FindAsync(workDay.Id);

            
            if (existingDay == null)
            {
                throw new InvalidOperationException($"WorkDay with ID {workDay.Id} not found.");
            }

            
            existingDay.DayOfWeek = workDay.DayOfWeek;
            existingDay.OfficerId = workDay.OfficerId; 

           
            await _context.SaveChangesAsync();
        }


        public async Task RemoveWorkDaysAsync(int officerId)
        {
            var workDays = await _context.WorkDays.Where(wd => wd.OfficerId == officerId).ToListAsync();
            _context.WorkDays.RemoveRange(workDays);
            await _context.SaveChangesAsync();
        }

        public async Task<WorkDay> GetByOfficerIdAndDayAsync(int officerId, int dayOfWeek)
        {
            return await _context.WorkDays
                .FirstOrDefaultAsync(wd => wd.OfficerId == officerId && wd.DayOfWeek == dayOfWeek);
        }

        public async Task<WorkDay> GetByOfficerIdAsync(int officerId)
        {
            return await _context.WorkDays
                .FirstOrDefaultAsync(x => x.OfficerId == officerId);
        }

    }
}