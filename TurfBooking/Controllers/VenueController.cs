using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TurfBooking.Models;

namespace TurfBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenueController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VenueController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Venue> venues = new List<Venue>();

            string sqlDataSource = _configuration.GetConnectionString("TurfConn");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand("USP_GET_VENUE", myCon))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        while (myReader.Read())
                        {
                            Venue venue = new Venue
                            {
                                VenueID = myReader.GetInt32(0),
                                VenueImage = myReader.GetString(1),
                                VenueName = myReader.GetString(2),
                                VenueLocation = myReader.GetString(3),
                                VenuePrice = myReader.GetDecimal(4),
                                VenueRating = myReader.GetDecimal(5)
                            };

                            venues.Add(venue);
                        }

                    }
                }
            }

            return Ok(venues);
        }


    }
}
