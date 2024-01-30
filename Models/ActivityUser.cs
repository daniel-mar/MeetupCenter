using System;
using System.ComponentModel.DataAnnotations;

namespace MeetupCenter.Models
{
    public class ActivityUser
    {
        [Key]
        public int ActivityUserID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }
        public int ActividadID { get; set; }
        public Actividad Actividad { get; set; }
    }
}