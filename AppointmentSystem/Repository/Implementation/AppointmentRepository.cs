using AppointmentSystem.Data;
using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
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

            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                appointment.LastUpdatedOn = DateTime.UtcNow;
            }
        }

        public async Task<List<AllAppointmentViewmodel>> GetAllAsync()
        {
            return await _context.Appointments
                 .Include(a => a.Officer)
                 .Include(a => a.Visitor)
                 .Select(a => new AllAppointmentViewmodel
                 {
                     Id = a.Id,
                     OfficerId = a.OfficerId,
                     VisitorId = a.VisitorId,
                     Name = a.Name,
                     Date = a.Date,
                     StartTime = a.StartTime,
                     EndTime = a.EndTime,
                     AddedOn = a.AddedOn,
                     Status = a.Status,
                     OfficerName = a.Officer.Name, // Assuming Officer has a Name property
                     VisitorName = a.Visitor.Name
                 }).ToListAsync();
        }

        public async Task<Appointment> GetAsync(int id)
        {
            return await _context.Appointments
                 .Include(a => a.Officer)
                 .Include(a => a.Visitor)
                 .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<bool> HasExistingAppointment(int visitorId, DateTime date)
        {
            var dateUtc = date.Date.ToUniversalTime();

            return await _context.Appointments
                .AnyAsync(a => a.VisitorId == visitorId &&
                              a.Date.Date == dateUtc.Date &&  // Compare both dates in UTC
                              a.Status == AppointmentStatus.Active);
        }

        public async Task<bool> HasExistingAppointmentDate( DateTime date)
        {
            var dateUtc = date.Date.ToUniversalTime();

            return await _context.Appointments
                .AnyAsync(a => 
                              a.Date.Date == dateUtc.Date &&  // Compare both dates in UTC
                              a.Status == AppointmentStatus.Active);
        }

        public async Task UpdateAsync(Appointment model)
        {
            _context.Appointments.Update(model); // Update the tracked entity
            await _context.SaveChangesAsync();  // Persist changes to the database
        }

       


        public async Task<IEnumerable<Appointment>> GetFutureAppointmentsByVisitorIdAsync(int visitorId)
        {
            return await _context.Appointments
                .Where(a => a.VisitorId == visitorId &&
                           a.Date.Date >= DateTime.UtcNow.Date &&
                           (a.Status == AppointmentStatus.Active ||
                            a.Status == AppointmentStatus.Deactivated))
                .ToListAsync();
        }

    }
}
