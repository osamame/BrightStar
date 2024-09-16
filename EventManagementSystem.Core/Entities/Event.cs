using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EventManagementSystem.Core.Enums;

namespace EventManagementSystem.Core.Entities
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime DateAndTime { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        [Required]
        [Range(1, 10000)]
        public int MaxAttendees { get; set; }

        [Range(0, 10000)]
        public decimal? Price { get; set; }

        public EventCategory Category { get; set; }

        public ICollection<Registration> Registrations { get; set; }
    }
}