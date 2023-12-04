using Microsoft.Data.SqlClient;
using System.Data;

namespace TurfBooking.Data
{
    public class PlayerRepository
    {
        private readonly IConfiguration _configuration;

        public PlayerRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> ValidatePlayerLogin(string PlayerName, string PlayerPassword)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("TurfConn")))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("ValidatePlayerLogin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.Add(new SqlParameter("@PlayerName", PlayerName));
                    command.Parameters.Add(new SqlParameter("@PlayerPassword", PlayerPassword));

                    // Add output parameter for the result
                    SqlParameter isValidParameter = new SqlParameter();
                    isValidParameter.ParameterName = "@IsValid";
                    isValidParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(isValidParameter);

                    await command.ExecuteNonQueryAsync();

                    // Get the result
                    int isValidValue = (int)isValidParameter.Value;
                    bool isValid = (isValidValue == 1);

                    return isValid;
                }
            }
        }

    }
}
