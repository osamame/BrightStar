using System;
using System.ComponentModel.DataAnnotations;
using EventManagementSystem.Core.Enums;

namespace EventManagementSystem.Web.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Date and Time")]
        public DateTime DateAndTime { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Maximum Attendees")]
        public int MaxAttendees { get; set; }

        [Range(0, 10000)]
        public decimal? Price { get; set; }

        public EventCategory Category { get; set; }
    }
}