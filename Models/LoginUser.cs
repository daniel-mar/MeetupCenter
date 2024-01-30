using System;
using System.ComponentModel.DataAnnotations;

namespace MeetupCenter.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters!")]
        public string LoginPassword { get; set; }
    }
}