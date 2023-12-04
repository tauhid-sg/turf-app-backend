namespace TurfBooking.Models
{
    public class Venue
    {
        public int VenueID { get; set; }
        public string VenueImage { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public decimal VenuePrice { get; set; }
        public decimal VenueRating { get; set; }
    }
}
