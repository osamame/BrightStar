using System;
using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Web.ViewModels
{
    public class RegistrationViewModel
    {
        public int EventId { get; set; }

        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }
    }
}