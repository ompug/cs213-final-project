using System;
using Microsoft.AspNetCore.Identity;

namespace cs213_Final_Project.Data
{
    public class Booking
    {
        public int Id { get; set; }
        public string SpaServiceType { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string StaffName { get; set; }
        public string Duration { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
