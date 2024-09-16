using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Enums;
using EventManagementSystem.Core.Interfaces;
using EventManagementSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventManagementSystem.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IActionResult> Index(string searchTerm, EventCategory? category)
        {
            var events = await _eventRepository.GetFilteredEventsAsync(searchTerm, category);
            var viewModel = new EventListViewModel
            {
                Events = events,
                SearchTerm = searchTerm,
                Category = category
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var @event = await _eventRepository.GetEventByIdAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var @event = new Event
                {
                    Name = model.Name,
                    Description = model.Description,
                    DateAndTime = model.DateAndTime,
                    Location = model.Location,
                    MaxAttendees = model.MaxAttendees,
                    Price = model.Price,
                    Category = model.Category
                };

                await _eventRepository.AddEventAsync(@event);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var @event = await _eventRepository.GetEventByIdAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            var model = new EventViewModel
            {
                Id = @event.Id,
                Name = @event.Name,
                Description = @event.Description,
                DateAndTime = @event.DateAndTime,
                Location = @event.Location,
                MaxAttendees = @event.MaxAttendees,
                Price = @event.Price,
                Category = @event.Category
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, EventViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var @event = await _eventRepository.GetEventByIdAsync(id);
                if (@event == null)
                {
                    return NotFound();
                }

                @event.Name = model.Name;
                @event.Description = model.Description;
                @event.DateAndTime = model.DateAndTime;
                @event.Location = model.Location;
                @event.MaxAttendees = model.MaxAttendees;
                @event.Price = model.Price;
                @event.Category = model.Category;

                await _eventRepository.UpdateEventAsync(@event);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var @event = await _eventRepository.GetEventByIdAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _eventRepository.DeleteEventAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var events = await _eventRepository.SearchEventsAsync(searchTerm);
            return View("Index", events);
        }
    }
}