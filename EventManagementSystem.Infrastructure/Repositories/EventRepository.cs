using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Enums;
using EventManagementSystem.Core.Interfaces;
using EventManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagementSystem.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EMDbContext _context;

        public EventRepository(EMDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            return await _context.Events
                .Where(e => e.DateAndTime > DateTime.Now)
                .OrderBy(e => e.DateAndTime)
                .ToListAsync();
        }

        public async Task AddEventAsync(Event @event)
        {
            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEventAsync(Event @event)
        {
            _context.Events.Update(@event);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEventAsync(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm)
        {
            return await _context.Events
                .Where(e => e.Name.Contains(searchTerm) || e.Description.Contains(searchTerm) || e.Location.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetFilteredEventsAsync(string searchTerm, EventCategory? category)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.Name.Contains(searchTerm) || e.Description.Contains(searchTerm) || e.Location.Contains(searchTerm));
            }

            if (category.HasValue)
            {
                query = query.Where(e => e.Category == category.Value);
            }

            return await query.ToListAsync();
        }
    }
}