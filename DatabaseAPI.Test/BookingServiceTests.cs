using Microsoft.EntityFrameworkCore;
using DatabaseAPI.Data;
using DatabaseAPI.Services;
using DatabaseAPI.Exceptions;
using DatabaseAPI.Models;

namespace DatabaseAPI.Test
{
    public class BookingServiceTests : IDisposable
    {
        DbContextOptions<BookingContext> DbContextOptions { get; set; }
        BookingContext BookingContext { get; set; }
        public BookingServiceTests()
        {
            DbContextOptions = new DbContextOptionsBuilder<BookingContext>()
                .UseInMemoryDatabase("BookingDb").Options;
            BookingContext = new BookingContext(DbContextOptions);
            BookingContext.Database.EnsureCreated();
        }

        [Fact]
        public void CreateBooking_BookingAdded()
        {
            //ARRANGE            
            BookingService bookingService = new BookingService(BookingContext);
            string userName = "";
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now.AddHours(5);

            //ACT
            bookingService.CreateBooking(userName, startTime, endTime);

            //ASSERT
            Assert.Single(BookingContext.Bookings.ToList());
        }

        [InlineData("2025-1-1 12:00:00","2025-1-1 14:00:00")]
        [InlineData("2025-1-1 09:00:00", "2025-1-1 18:00:00")]
        [Theory]
        public void CreateBooking_Overlapping_Exception(string startTime, string endTime)
        {
            //ARRANGE
            BookingService bookingService = new BookingService(BookingContext);
            string userName = "";
            foreach (var item in MockBookings())
            {
                BookingContext.Bookings.Add(item);
            }
            BookingContext.SaveChanges();
            //ACT

            //ASSERT
            Assert.Throws<BookingOverlapException>(() =>
                bookingService.CreateBooking(userName, DateTime.Parse(startTime), DateTime.Parse(endTime)));
        }

        public static List<Booking> MockBookings()
        {
            return new List<Booking>
            {
                new Booking{
                    UserName="Bengt",
                    StartTime=DateTime.Parse("2025-1-1 10:00:00"),
                    EndTime=DateTime.Parse("2025-1-1 11:00:00"),
                },
                new Booking{
                    UserName="Bengt",
                    StartTime=DateTime.Parse("2025-1-1 13:00:00"),
                    EndTime=DateTime.Parse("2025-1-1 15:00:00")
                },
                new Booking{
                    UserName="Pelle",
                    StartTime=DateTime.Parse("2025-1-1 16:00:00"),
                    EndTime=DateTime.Parse("2025-1-1 17:00:00")
                },
            };
        }

        [Fact]
        public void CancelBooking_BookingExists_BookingRemoved()
        {
            //Arrange
            BookingService bookingService = new BookingService(BookingContext);
            string userName = "";
            foreach (var item in MockBookings())
            {
                BookingContext.Bookings.Add(item);
            }
            var booking = new Booking
            {
                Id = 5,
                UserName = "",
                StartTime = DateTime.Parse("2025-1-2 15:00:00"),
                EndTime = DateTime.Parse("2025-1-2 16:00:00")
            };
            BookingContext.Bookings.Add(booking);
            BookingContext.SaveChanges();
            //Act
            bookingService.CancelBooking(5);

            //Assert
            Assert.DoesNotContain(booking, BookingContext.Bookings.ToList());
        }
        [Fact]
        public void CancelBooking_NotFound_Exception()
        {
            //Arrange
            BookingService bookingService = new BookingService(BookingContext);

            //Act

            //Assert
            Assert.Throws<BookingNotFoundException>(() => bookingService.CancelBooking(99));
        }

        [InlineData("Bengt", 2)]
        [InlineData("Pelle", 1)]
        [InlineData("NoNameAtAll", 0)]
        [Theory]
        public void GetBookingsForUser_UserHasBookings_BookingList(string name, int count)
        {
            //Arrange
            BookingService bookingService = new BookingService(BookingContext);
            foreach (var item in MockBookings())
            {
                BookingContext.Bookings.Add(item);
            }
            BookingContext.SaveChanges();
            //Act
            var list = bookingService.GetBookingsForUser(name);
            //Assert
            Assert.True(list.Count == count);
            var test = list.Where(x => x.UserName != name).ToList();
            Assert.Empty(test);
        }

        public void Dispose()
        {
            BookingContext.Database.EnsureDeleted();
        }
    }
}