using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Implementation;
using AppointmentSystem.Repository.Interface;
using AppointmentSystem.Service.Interface;
using static AppointmentSystem.Models.Domain.Appointment;

namespace AppointmentSystem.Service.Implementation
{
    public class VisitorService : IVisitorService
    {
        private readonly IVisitorRepository _visitorRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IOfficerRepository _officerRepo;

        public VisitorService(IVisitorRepository visitorRepository, IAppointmentRepository appointmentRepository,IOfficerRepository officerRepo)
        {
            _visitorRepository = visitorRepository;
            _appointmentRepository = appointmentRepository;
            _officerRepo = officerRepo;
        }

        public async Task<VisitorViewModel> GetVisitorByIdAsync(int id)
        {
            var visitor = await _visitorRepository.GetVisitorByIdAsync(id);
            if (visitor == null)
            {
                return null;
            }

       
            var visitorViewModel = new VisitorViewModel
            {
                Id = visitor.Id,
                Name = visitor.Name,
                MobileNumber = visitor.MobileNumber,
                EmailAddress = visitor.EmailAddress,
                Status = visitor.Status
            };

            return visitorViewModel;
        }

        
        public async Task<IEnumerable<VisitorViewModel>> GetAllVisitorsAsync()
        {
            var visitors = await _visitorRepository.GetAllVisitorsAsync();
            return visitors.Select(v => new VisitorViewModel
            {
                Id = v.Id,
                Name = v.Name,
                MobileNumber = v.MobileNumber,
                EmailAddress = v.EmailAddress,
                Status = v.Status
            });
        }

       
        public async Task CreateVisitorAsync(VisitorViewModel visitorViewModel)
        {
            var visitor = new Visitor
            {
                Name = visitorViewModel.Name,
                MobileNumber = visitorViewModel.MobileNumber,
                EmailAddress = visitorViewModel.EmailAddress,
                Status = visitorViewModel.Status=true,
            };
            await _visitorRepository.InsertVisitorAsync(visitor);
        }

     
        public async Task UpdateVisitorAsync(VisitorViewModel visitorViewModel)
        {
            var existingVisitor = await _visitorRepository.GetVisitorByIdAsync(visitorViewModel.Id);
            if (existingVisitor != null)
            {
                existingVisitor.Name = visitorViewModel.Name;
                existingVisitor.MobileNumber = visitorViewModel.MobileNumber;
                existingVisitor.EmailAddress = visitorViewModel.EmailAddress;
                existingVisitor.Status = visitorViewModel.Status;

                
                await _visitorRepository.UpdateVisitorAsync(existingVisitor);
            }
        }

        public async Task DeactivateFutureAppointmentsAsync(int visitorId)
        {
            var futureAppointments = await _appointmentRepository.GetFutureAppointmentsByVisitorIdAsync(visitorId);
            foreach (var appointment in futureAppointments)
            {
                if (appointment.Status == AppointmentStatus.Active)  
                {
                    appointment.Status = AppointmentStatus.Deactivated;
                    appointment.LastUpdatedOn = DateTime.UtcNow;
                    await _appointmentRepository.UpdateAsync(appointment);
                }
            }
        }

        public async Task ReactivateFutureAppointmentsAsync(int visitorId)
        {
            var futureAppointments = await _appointmentRepository.GetFutureAppointmentsByVisitorIdAsync(visitorId);
            foreach (var appointment in futureAppointments)
            {
                // Only process deactivated appointments
                if (appointment.Status == AppointmentStatus.Deactivated)
                {
                    var officerIsActive = await _officerRepo.IsOfficerActiveAsync(appointment.OfficerId);
                    if (officerIsActive)
                    {
                        appointment.Status = AppointmentStatus.Active;
                        appointment.LastUpdatedOn = DateTime.UtcNow;
                        await _appointmentRepository.UpdateAsync(appointment);
                    }
                }
            }
        }

        public async Task<IEnumerable<VisitorViewModel>> GetActiveVisitorsAsync()
        {
            var activeVisitors = await _visitorRepository.GetActiveVisitorsAsync();
            var visitorViewModels = activeVisitors.Select(v => new VisitorViewModel
            {
                Id = v.Id,
                Name = v.Name,
                Status = v.Status,
              
            });

            return visitorViewModels;
        }

    }
}