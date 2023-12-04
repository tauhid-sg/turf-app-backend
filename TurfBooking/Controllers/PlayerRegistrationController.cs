using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TurfBooking.Models;

namespace TurfBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerRegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PlayerRegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Player> players = new List<Player>();

            string sqlDataSource = _configuration.GetConnectionString("TurfConn");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand("USP_GET_REGISTRATION_DETAILS", myCon))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        while (myReader.Read())
                        {
                            Player player = new Player
                            {
                                PlayerID = myReader.GetInt32(0),
                                PlayerName = myReader.GetString(1),
                                PlayerEmail = myReader.GetString(2),
                                PlayerPhoneNumber = myReader.GetInt32(3),
                                PlayerPassword = myReader.GetString(4)
                            };

                            players.Add(player);
                        }

                    }
                }
            }

            return Ok(players);
        }

        [HttpPost]
        public IActionResult Post(Player player)
        {
            string sqlDataSource = _configuration.GetConnectionString("TurfConn");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand("USP_Register_Player", myCon))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    myCommand.Parameters.AddWithValue("@PlayerName", player.PlayerName);
                    myCommand.Parameters.AddWithValue("@PlayerEmail", player.PlayerEmail);
                    myCommand.Parameters.AddWithValue("@PlayerPhoneNumber", player.PlayerPhoneNumber);
                    myCommand.Parameters.AddWithValue("@PlayerPassword", player.PlayerPassword);

                    myCommand.ExecuteNonQuery();
                }
            }

            return Ok();
        }

        [HttpPost("login")]
        public IActionResult LoginPlayer([FromBody] LoginModel model)
        {
            string connectionString = _configuration.GetConnectionString("TurfConn");
            bool isValidUser = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("USP_Login_Player", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@EnteredPlayerName", model.PlayerName);
                command.Parameters.AddWithValue("@EnteredPlayerPassword", model.PlayerPassword);

                SqlParameter isValidUserParam = new SqlParameter("@IsValidUser", System.Data.SqlDbType.Bit);
                isValidUserParam.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(isValidUserParam);

                connection.Open();
                command.ExecuteNonQuery();
                isValidUser = (bool)command.Parameters["@IsValidUser"].Value;
            }

            if (isValidUser)
            {
                return Ok(new { message = "Login successful" }); // Login successful
            }
            else
            {
                return Unauthorized(new { message = "Invalid credentials. Please try again." }); // Invalid credentials
            }
        }
    }
}
