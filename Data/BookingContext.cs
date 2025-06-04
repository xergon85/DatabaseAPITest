using DatabaseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAPI.Data
{
    public class BookingContext: DbContext
    {
        public DbSet<Booking> Bookings { get; set; }
        public BookingContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }
    }
}