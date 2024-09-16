using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using EventManagementSystem.Web.Services;
using EventManagementSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EventManagementSystem.Web.Controllers
{
    [Authorize]
    public class RegistrationsController : Controller
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PdfService _pdfService;
        private readonly ILogger<RegistrationsController> _logger;

        public RegistrationsController(IRegistrationRepository registrationRepository, IEventRepository eventRepository, UserManager<ApplicationUser> userManager, PdfService pdfService, ILogger<RegistrationsController> logger)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
            _userManager = userManager;
            _pdfService = pdfService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var registrations = await _registrationRepository.GetUserRegistrationsAsync(userId);
            return View(registrations);
        }

        [HttpGet]
        public async Task<IActionResult> Register(int eventId)
        {
            var @event = await _eventRepository.GetEventByIdAsync(eventId);
            if (@event == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var isRegistered = await _registrationRepository.IsUserRegisteredForEventAsync(userId, eventId);

            if (isRegistered)
            {
                TempData["ErrorMessage"] = "You are already registered for this event.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            var model = new RegistrationViewModel
            {
                EventId = eventId,
                EventName = @event.Name,
                EventDate = @event.DateAndTime
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            _logger.LogInformation($"Attempting to register for event {model.EventId}");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when attempting to register for event {EventId}", model.EventId);
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("Unable to get user ID for registration");
                return RedirectToAction("Login", "Account");
            }

            var @event = await _eventRepository.GetEventByIdAsync(model.EventId);
            if (@event == null)
            {
                _logger.LogWarning("Attempted to register for non-existent event {EventId}", model.EventId);
                return NotFound();
            }

            var isRegistered = await _registrationRepository.IsUserRegisteredForEventAsync(userId, model.EventId);
            if (isRegistered)
            {
                _logger.LogInformation("User {UserId} attempted to register again for event {EventId}", userId, model.EventId);
                TempData["ErrorMessage"] = "You are already registered for this event.";
                return RedirectToAction("Details", "Events", new { id = model.EventId });
            }

            var registration = new Registration
            {
                EventId = model.EventId,
                UserId = userId,
                RegistrationDate = DateTime.Now
            };

            try
            {
                await _registrationRepository.AddRegistrationAsync(registration);
                _logger.LogInformation("User {UserId} successfully registered for event {EventId}", userId, model.EventId);
                TempData["SuccessMessage"] = "You have successfully registered for the event.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user {UserId} for event {EventId}", userId, model.EventId);
                ModelState.AddModelError(string.Empty, "An error occurred while processing your registration. Please try again.");
                return View(model);
            }
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var registration = await _registrationRepository.GetRegistrationByIdAsync(id);
            if (registration == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (registration.UserId != userId)
            {
                return Forbid();
            }

            return View(registration);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var registration = await _registrationRepository.GetRegistrationByIdAsync(id);
            if (registration == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (registration.UserId != userId)
            {
                return Forbid();
            }

            await _registrationRepository.DeleteRegistrationAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadTicket(int id)
        {
            var registration = await _registrationRepository.GetRegistrationByIdAsync(id);
            if (registration == null)
            {
                _logger.LogWarning($"Registration with ID {id} not found");
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (registration.UserId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to access registration {id} belonging to another user");
                return Forbid();
            }

            if (registration.User == null)
            {
                _logger.LogWarning($"User data not found for registration {id}");
                return NotFound("User data not found");
            }

            if (registration.Event == null)
            {
                _logger.LogWarning($"Event data not found for registration {id}");
                return NotFound("Event data not found");
            }

            try
            {
                var pdfBytes = _pdfService.GenerateTicket(registration);
                return File(pdfBytes, "application/pdf", $"Ticket_{registration.Event.Name}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating PDF for registration {id}");
                return StatusCode(500, "An error occurred while generating the ticket. Please try again later.");
            }
        }
    }
}