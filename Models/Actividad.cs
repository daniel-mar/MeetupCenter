using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// I had an error where Activity is too ambiguous, So now I am
// Using Actividad to not have it my Activity, because Nicole mentioned these things MIGHT 
// cause issues, so using Spanish to keep it different and not the plural version either
// The wireframe labels them as "New Activity".

namespace MeetupCenter.Models
{
    public class Actividad
    {

        [Key]
        public int ActividadID { get; set; }

        [Required(ErrorMessage = "Activity name is required!")]
        [MinLength(3, ErrorMessage = "Activity name must contain at least three characters...")]
        [Display(Name = "Activity Name")]
        public string ActivityName { get; set; }

        [Required(ErrorMessage = "Date is required!")]
        //need error message
        [DataType(DataType.Date, ErrorMessage = "Date cannot be in the past!")]
        [Display(Name = "Activity Date")]
        public DateTime ActivityDate { get; set; }

        [Required(ErrorMessage = "Activity time is required!")]
        [DataType(DataType.Time)]
        [Display(Name = "Activity Time")]
        public DateTime ActivityTime { get; set; }

        [Required(ErrorMessage = "Duration is required!")]
        [Display(Name = "Duration")]
        public int Duration { get; set; }
        public string DurationAmount { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        [MinLength(5, ErrorMessage = "Must be more than 5 characters...")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int UserID { get; set; }
        public User User { get; set; }
        public List<ActivityUser> ActiveUser { get; set; }

    }
}