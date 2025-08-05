using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using cs213_Final_Project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace cs213_Final_Project.Pages
{
    [Authorize]
    public class MyAppointmentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public MyAppointmentsModel(ApplicationDbContext context) => _context = context;

        public List<Booking> UpcomingAppointments { get; private set; } = new();
        public List<Booking> PastAppointments { get; private set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.Now;

            var userBookings = _context.Bookings
                .Where(b => b.UserId == userId);

            UpcomingAppointments = await userBookings
                .Where(b => b.AppointmentDateTime >= now)
                .OrderBy(b => b.AppointmentDateTime)
                .ToListAsync();

            PastAppointments = await userBookings
                .Where(b => b.AppointmentDateTime < now)
                .OrderByDescending(b => b.AppointmentDateTime)
                .ToListAsync();
        }
    }
}
