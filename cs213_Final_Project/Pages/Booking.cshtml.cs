using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using cs213_Final_Project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cs213_Final_Project.Pages
{
    [Authorize]
    public class Index1Model : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Index1Model(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string SpaServiceType { get; set; }

        [BindProperty, DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AppointmentDate { get; set; }

        [BindProperty]
        public string AppointmentTime { get; set; }

        [BindProperty]
        public string StaffName { get; set; }

        [BindProperty]
        public string Duration { get; set; }

        public void OnGet()
        {
            // default values
            SpaServiceType = "Full Body Massage";
            AppointmentDate = DateTime.Today;
            AppointmentTime = "9:00 AM";
            StaffName = "Jo Marinara";
            Duration = "30-40 minutes";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // parse the time string ("9:00 AM") into a TimeSpan
            if (!DateTime.TryParse(AppointmentTime, out var timePart))
            {
                ModelState.AddModelError(nameof(AppointmentTime), "Please pick a valid time slot.");
                return Page();
            }

            // combine date and time
            var appointmentDateTime = AppointmentDate.Date.Add(timePart.TimeOfDay);

            // build booking, including the current user's ID
            var booking = new Booking
            {
                SpaServiceType = SpaServiceType,
                AppointmentDateTime = appointmentDateTime,
                StaffName = StaffName,
                Duration = Duration,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
