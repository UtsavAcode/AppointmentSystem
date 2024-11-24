using AppointmentSystem.Data;
using AppointmentSystem.Models.Domain;
using AppointmentSystem.Models.ViewModel;
using AppointmentSystem.Repository.Interface;
using AppointmentSystem.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Service.Implementation
{
    public class OfficerService : IOfficerService
    {
        private readonly IOfficerRepository _officerRepository;
        private readonly IPostRepository _postRepository;
        private readonly ApplicationDbContext _context;

        public OfficerService(IOfficerRepository officerRepository, IPostRepository postRepository, ApplicationDbContext context)
        {
            _officerRepository = officerRepository;
            _postRepository = postRepository;
            _context = context;
        }

        public async Task<IEnumerable<OfficerViewModel>> GetAllOfficersAsync()
        {
            var officers = await _officerRepository.GetAllOfficersAsync();
            return officers.Select(MapToViewModel);
        }

        public async Task<OfficerViewModel> GetOfficerByIdAsync(int id)
        {
            var officer = await _officerRepository.GetOfficerByIdAsync(id);
            return officer != null ? MapToViewModel(officer) : null;
        }






        public async Task UpdateOfficerAsync(OfficerViewModel model)
        {
            var existingUser = await _officerRepository.GetOfficerByIdAsync(model.Id);
            if (existingUser != null)
            {
                existingUser.Name = model.Name;
                existingUser.PostId = model.PostId;
                existingUser.WorkStartTime = model.WorkStartTime;
                existingUser.WorkEndTime = model.WorkEndTime;

                await _officerRepository.UpdateOfficerAsync(existingUser);

            }
        }

        public async Task<bool> ToggleOfficerStatusAsync(int id, bool status)
        {
            var officer = await _officerRepository.GetOfficerByIdAsync(id);
            if (officer == null)
                return false;

            if (status) // If trying to activate
            {
                var post = await _postRepository.GetByIdAsync(officer.PostId);
                if (!post.Status)
                    return false; // Cannot activate if post is inactive
            }

            officer.Status = status;
            return await _officerRepository.UpdateOfficerAsync(officer);
        }

        public async Task<(bool success, string message)> DeactivatePostAsync(int postId)
        {
            var activeOfficers = await _officerRepository.GetActiveOfficersByPostIdAsync(postId);
            if (activeOfficers.Any())
                return (false, "Cannot deactivate the post as it has active officers.");

            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                return (false, "Post not found.");

            post.Status = false; // Set the status to false for deactivation.

            var success = await _postRepository.UpdateAsync(post);
            return success
                ? (true, "Post deactivated successfully.")
                : (false, "Failed to deactivate post.");
        }



        private OfficerViewModel MapToViewModel(Officer officer)
        {
            return new OfficerViewModel
            {
                Id = officer.Id,
                Name = officer.Name,
                PostId = officer.PostId,
                Status = officer.Status,
                WorkStartTime = officer.WorkStartTime,
                WorkEndTime = officer.WorkEndTime,

            };
        }



        public async Task<IEnumerable<Officer>> GetActiveOfficersByPostIdAsync(int postId)
        {
            return await _context.Officers
                .Where(o => o.PostId == postId && o.Status==true)
                .ToListAsync();
        }

        public async Task CreateOfficerAsync(OfficerViewModel model)
        {
            var officer = new Officer
            {

                Name = model.Name,
                PostId = model.PostId,
                Status = model.Status,
                WorkStartTime = model.WorkStartTime,
                WorkEndTime = model.WorkEndTime,

            };

            if (officer != null)
            {
                await _officerRepository.InsertOfficerAsync(officer);
            }

        }
    }
}
