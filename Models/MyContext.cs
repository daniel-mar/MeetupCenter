using Microsoft.EntityFrameworkCore;


namespace MeetupCenter.Models
{ 
    public class MyContext : DbContext
    { 
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Actividad> Actividades { get; set; }
        public DbSet<ActivityUser> ActiveUsers { get; set; }
    }
}