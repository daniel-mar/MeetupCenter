using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetupCenter.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters long!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters!")]
        public string Password { get; set; }

        // Done setting up database, the following does not need to be a column in User
        [NotMapped]
        [Required(ErrorMessage = "Please confirm password!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match!")]
        public string ConfirmPassword { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // I do not want to have a conflict with any tables
        // I can use this as an Event as well, because it is an activity to meetup for.
        public List<Actividad> CreatedActivities { get; set; }
        public List<ActivityUser> ActiveUser { get; set; }
    }
}