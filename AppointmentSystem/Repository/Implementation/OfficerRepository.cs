﻿using AppointmentSystem.Data;
using AppointmentSystem.Models.Domain;
using AppointmentSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Repository.Implementation
{
    public class OfficerRepository:IOfficerRepository
    {
        private readonly ApplicationDbContext _context;

        public OfficerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Officer>> GetAllOfficersAsync()
        {
            return await _context.Officers
                .Include(o => o.Post)
                .ToListAsync();
        }

        public async Task<Officer> GetOfficerByIdAsync(int id)
        {
            return await _context.Officers
                .Include(o => o.Post)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> InsertOfficerAsync(Officer officer)
        {
            await _context.Officers.AddAsync(officer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateOfficerAsync(Officer officer)
        {
            _context.Officers.Update(officer);
            return await _context.SaveChangesAsync() > 0;
        }

   

        public async Task<IEnumerable<Officer>> GetActiveOfficersByPostIdAsync(int postId)
        {
            return await _context.Officers
                .Where(o => o.PostId == postId && o.Status==true)
                .ToListAsync();
        }
        public async Task<bool> CheckScheduleOverlap(int postId, string newStartTime, string newEndTime)
        {
            if (string.IsNullOrEmpty(newStartTime) || string.IsNullOrEmpty(newEndTime))
            {
                throw new ArgumentException("Start time and end time cannot be null or empty");
            }

            if (!TimeSpan.TryParse(newStartTime, out TimeSpan startTime) ||
                !TimeSpan.TryParse(newEndTime, out TimeSpan endTime))
            {
                throw new ArgumentException("Invalid time format");
            }

            var existingOfficers = await GetActiveOfficersByPostId(postId);

            foreach (var officer in existingOfficers)
            {
                if (TimeSpan.TryParse(officer.WorkStartTime, out TimeSpan existingStart) &&
                    TimeSpan.TryParse(officer.WorkEndTime, out TimeSpan existingEnd))
                {
                    if (startTime < existingEnd && endTime > existingStart)
                    {
                        return true; // Overlap exist
                    }
                }
            }

            return false; // No overlap
        }

        public async Task<IEnumerable<Officer>> GetActiveOfficersByPostId(int postId)
        {
            return await _context.Officers
                .Where(o => o.PostId == postId && o.Status)
                .ToListAsync();
        }
    }
}
