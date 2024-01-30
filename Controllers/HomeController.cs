using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MeetupCenter.Models;


// Not default, added after
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MeetupCenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ===============================================
        // Reusable SessionID -- the function will call this
        // to return the session information dynamically
        private int? uid
        {
            get
            {
                return HttpContext.Session.GetInt32("UserID");
            }
        }
        private bool isLoggedIn
        {
            get
            {
                return uid != null;
            }
        }
        // END OF REUSABLE FUNCTIONS
        // ================================================

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/Register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Is taken");
                }
            }
            if (ModelState.IsValid == false)
            {
                return View("Index");
            }

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserID", newUser.UserID);
            HttpContext.Session.SetString("Nme", newUser.Name);
            return RedirectToAction("Dashboard");

        }

        [HttpPost("/Login")]
        public IActionResult Login(LoginUser loginUser)
        {
            if (ModelState.IsValid)
            {
                User dbUser = _context.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);
                // no user with that email
                if (dbUser == null)
                {
                    // FIXED validations
                    ModelState.AddModelError("LoginEmail", "Email not found.");
                    return View("Index");
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var pwCompareResult = hasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);

                if (pwCompareResult == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Invalid Password.");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserID", dbUser.UserID);
                HttpContext.Session.SetString("Name", dbUser.Name);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        // Logout - working on dashboard + view activity
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // After the models work, validations, adding users to db work

        [HttpGet("/Dashboard")]
        public IActionResult Dashboard()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }

            List<Actividad> allActivities = _context.Actividades.ToList();
            List<Actividad> newActivities = _context.Actividades.OrderBy(date => date.ActivityDate).Include(c => c.ActiveUser).Include(user => user.User).ToList();
            return View("Dashboard", newActivities);
        }

        [HttpGet("/NewActivity")]
        public IActionResult CreateActivity()
        {
            return View("NewActivity");
        }

        [HttpPost("/NewActivity")]
        public IActionResult NewActivity(Actividad newActivity)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid == false)
            {
                Console.WriteLine("Fail");
                return View("NewActivity");
            }
            if (newActivity.ActivityDate < DateTime.Now)
            {
                // FIXED validations
                ModelState.AddModelError("ActivityDate", "Must be in the future!");
                return View("NewActivity");
            }

            User NewActivityCreater = _context.Users.FirstOrDefault(u => u.UserID == (int)uid);
            newActivity.User = NewActivityCreater;
            _context.Add(newActivity);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        // View ONE activity
        [HttpGet("/ActivityDetails/{ActividadID}")]
        public IActionResult ActivityDetails(int ActividadID)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }

            Actividad activityInfo = _context.Actividades.Include(a => a.ActiveUser).ThenInclude(ActiveUser => ActiveUser.User).Include(b => b.User).FirstOrDefault(w => w.ActividadID == ActividadID);

            if (activityInfo == null)
            {
                return RedirectToAction("Dashboard");
            }

            return View("ActivityDetails", activityInfo);

        }


        // JOIN WORKS - instead of having two seperate routes, can do 1
        [HttpGet("/Activity/Join/{ActividadID}")]
        public IActionResult Join(int ActividadID)
        {
            // this will call our reusable function, checking if the user is already logged in
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }

            // store user from table that matches the logged in userID 
            ActivityUser ActiveUser = _context.ActiveUsers.FirstOrDefault(a => a.UserID == (int)uid && a.ActividadID == ActividadID);

            // If there is no one that joined, it makes that and adds to table
            if (ActiveUser == null)
            {
                // no user found, create
                ActivityUser activeUser = new ActivityUser()
                {
                    ActividadID = ActividadID,
                    UserID = (int)uid
                };
                _context.ActiveUsers.Add(activeUser);
            }
            else
            {
                // user must have existed so do this, meaning already joined
                _context.ActiveUsers.Remove(ActiveUser);
            }
            // update database and do dashboard
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }


        // Working
        [HttpPost("Activity/Delete/{ActividadID}")]
        public IActionResult Delete(int ActividadID)
        {
            // store the activity ID to know which one to remove
            Actividad Activity = _context.Actividades.FirstOrDefault(w => w.ActividadID == ActividadID);

            if (Activity == null)
            {
                return RedirectToAction("Dashboard");
            }

            _context.Actividades.Remove(Activity);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }











        // =================== NOT REQUIRED, WILL NOT GO BELOW THIS LINE =============
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
