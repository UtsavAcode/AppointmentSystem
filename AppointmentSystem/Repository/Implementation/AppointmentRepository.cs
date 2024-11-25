using AppointmentSystem.Data;
using AppointmentSystem.Models.Domain;
using AppointmentSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using static AppointmentSystem.Models.Domain.Appointment;

namespace AppointmentSystem.Repository.Implementation
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task AddAsync(Appointment model)
        {
            await _context.Appointments.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int id)
        {
            var appointment = await GetAsync(id);

            if (appointment != null) {
                appointment.Status = AppointmentStatus.Cancelled;
                appointment.LastUpdatedOn = DateTime.UtcNow;
            }
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
           return await _context.Appointments
                .Include(a=> a.Officer)
                .Include(a=>a.Visitor)
                .ToListAsync();
        }

        public async Task<Appointment> GetAsync(int id)
        {
           return await _context.Appointments
                .Include(a => a.Officer)
                .Include(a => a.Visitor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(Appointment model)
        {
            _context.Appointments.Update(model);
            await _context.SaveChangesAsync();
        }
    }
}
