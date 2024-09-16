using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Enums;
using System.Collections.Generic;

namespace EventManagementSystem.Web.ViewModels
{
    public class EventListViewModel
    {
        public IEnumerable<Event> Events { get; set; }
        public string SearchTerm { get; set; }
        public EventCategory? Category { get; set; }
    }
}