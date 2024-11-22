using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Interface;
using AppointmentSystem.Service.Interface;

namespace AppointmentSystem.Service.Implementation
{
    public class VisitorService:IVisitorService
    {
        private readonly IVisitorRepository _visitorRepository;
        

        public VisitorService(IVisitorRepository visitorRepository)
        {
            _visitorRepository = visitorRepository;
            
        }

        public async Task<VisitorViewModel> GetVisitorByIdAsync(int id)
        {
            var visitor = await _visitorRepository.GetVisitorByIdAsync(id);
            if (visitor == null)
            {
                return null;
            }

            // Manual mapping
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

        // Manually map VisitorViewModel to Visitor
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
            }).ToList();
        }

        // Create a new visitor: Manually map VisitorViewModel to Visitor
        public async Task CreateVisitorAsync(VisitorViewModel visitorViewModel)
        {
            var visitor = new Visitor
            {
                Id = visitorViewModel.Id,
                Name = visitorViewModel.Name,
                MobileNumber = visitorViewModel.MobileNumber,
                EmailAddress = visitorViewModel.EmailAddress,
                Status = visitorViewModel.Status
            };

            await _visitorRepository.InsertVisitorAsync(visitor);
        }

        // Update an existing visitor: Manually map VisitorViewModel to Visitor
        public async Task UpdateVisitorAsync(VisitorViewModel visitorViewModel)
        {
            var visitor = new Visitor
            {
                Id = visitorViewModel.Id,
                Name = visitorViewModel.Name,
                MobileNumber = visitorViewModel.MobileNumber,
                EmailAddress = visitorViewModel.EmailAddress,
                Status = visitorViewModel.Status
            };

            await _visitorRepository.UpdateVisitorAsync(visitor);
        }

       
    }
}
