using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TurfBooking.Models;

namespace TurfBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurfController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TurfController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Turf> turfs = new List<Turf>();

            string sqlDataSource = _configuration.GetConnectionString("TurfConn");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand("usp_get_turf_details", myCon))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        while (myReader.Read())
                        {
                            Turf turf = new Turf
                            {
                                TurfID = myReader.GetInt32(0),
                                FullName = myReader.GetString(1),
                                Email = myReader.GetString(2),
                                PhoneNo = myReader.GetString(3),
                                PlayTime = myReader.GetDateTime(4).ToString("HH:mm"),
                                PlayDate = myReader.GetDateTime(5),
                                TurfName = myReader.GetString(6)
                            };

                            turfs.Add(turf);
                        }

                    }
                }
            }

            return Ok(turfs);
        }


        [HttpPost]
        public IActionResult Post(Turf turf)
        {
            string sqlDataSource = _configuration.GetConnectionString("TurfConn");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand("USP_ADD_TURF", myCon))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    myCommand.Parameters.AddWithValue("@FullName", turf.FullName);
                    myCommand.Parameters.AddWithValue("@Email", turf.Email);
                    myCommand.Parameters.AddWithValue("@PhoneNo", turf.PhoneNo);
                    myCommand.Parameters.AddWithValue("@PlayTime", turf.PlayTime);
                    myCommand.Parameters.AddWithValue("@PlayDate", turf.PlayDate);
                    myCommand.Parameters.AddWithValue("@TurfName", turf.TurfName);

                    myCommand.ExecuteNonQuery();
                }
            }

            return Ok();
        }

        /*[HttpDelete("{TurfID}")]
        public IActionResult Delete(int TurfID)
        {
            string sqlDataSource = _configuration.GetConnectionString("TurfConn");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand("USP_DELETE_TURF", myCon))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    myCommand.Parameters.AddWithValue("@TurfID", TurfID);

                    int rowsAffected = myCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        string successMessage = "Venue deleted successfully.";
                        return Ok(successMessage);
                    }
                    else
                    {
                        return NotFound("Venue not found");
                    }
                }
            }
        }*/

        [HttpDelete("{TurfID}")]
        public IActionResult Delete(int TurfID)
        {
            string sqlDataSource = _configuration.GetConnectionString("TurfConn");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand("USP_SOFT_DELETE_TURF", myCon))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    myCommand.Parameters.AddWithValue("@TurfID", TurfID);

                    int rowsAffected = myCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        string successMessage = "Turf deleted successfully.";
                        return Ok(successMessage);
                    }
                    else
                    {
                        return NotFound("Venue not found");
                    }
                }
            }
        }

    }
}
