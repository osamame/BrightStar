using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Enums;

namespace EventManagementSystem.Core.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();
        Task AddEventAsync(Event @event);
        Task UpdateEventAsync(Event @event);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);
        Task<IEnumerable<Event>> GetFilteredEventsAsync(string searchTerm, EventCategory? category);
    }
}