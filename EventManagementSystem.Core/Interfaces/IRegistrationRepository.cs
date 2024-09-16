using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagementSystem.Core.Entities;

namespace EventManagementSystem.Core.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<IEnumerable<Registration>> GetUserRegistrationsAsync(string userId);
        Task<Registration> GetRegistrationByIdAsync(int id);
        Task AddRegistrationAsync(Registration registration);
        Task DeleteRegistrationAsync(int id);
        Task<bool> IsUserRegisteredForEventAsync(string userId, int eventId);
    }
}