
namespace DatabaseAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
