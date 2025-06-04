using DatabaseAPI.Data;
using DatabaseAPI.Exceptions;
using DatabaseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAPI.Services
{
    public class BookingService
    {
        private readonly BookingContext bookingContext;

        public BookingService(BookingContext bookingContext)
        {
            this.bookingContext = bookingContext;
        }

        public void CancelBooking(int v)
        {
            var booking = bookingContext.Bookings.FirstOrDefault(x => x.Id == v);
            if (booking == null)
            {
                throw new BookingNotFoundException();
            }
            bookingContext.Remove(booking);
            bookingContext.SaveChanges();
        }

        public void CreateBooking(string userName, DateTime startTime, DateTime endTime)
        {
            Booking booking = new Booking{UserName=userName, StartTime=startTime, EndTime=endTime};
            var bookings = bookingContext.Bookings
                .Where(t => t.StartTime < endTime && t.EndTime > startTime).ToList();
            if (bookings.Count>0)
            {
                //AlreadyBooked(5) < NewBooking(6) && AlreadyBooked(5.30) > NewBooking(5.20)
                throw new BookingOverlapException();
            }
            bookingContext.Bookings.Add(booking);
            bookingContext.SaveChanges();
        }

        public List<Booking> GetBookingsForUser(string name)
        {
            return bookingContext.Bookings.Where(n=>n.UserName==name).ToList();
        }
    }
}
