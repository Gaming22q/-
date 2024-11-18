using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Text;

// ... (остальные using-ы, если они нужны)

public class UsersController : ControllerBase
{
    private readonly string _connectionString;

    public UsersController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection"); // Получение строки подключения из конфигурации
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            try
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT user_id, username, email FROM users", connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    var users = new List<User>();
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2)
                        });
                    }
                    return Ok(users);
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
      //  Валидация данных (пример с DataAnnotations)
      if (!ModelState.IsValid)
      {
          return BadRequest(ModelState);
      }
        using (var connection = new SqlConnection(_connectionString))
        {
            try
            {
                await connection.OpenAsync();
                // Хеширование пароля (пример, используйте более надежный алгоритм)
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password); // Используйте стороннюю библиотеку для хеширования

                using (var command = new SqlCommand("INSERT INTO users (username, email, password_hash) VALUES (@username, @email, @passwordHash)", connection))
                {
                    command.Parameters.AddWithValue("@username", user.Username);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@passwordHash", hashedPassword);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
                    }
                    return BadRequest("Error creating user");
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }
    // ... (остальные методы GET, PUT, DELETE)


}
// ... (остальные классы и методы)


public class User
{
    public int UserId { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; } // Не хешируйте сами, используйте БCrypt

    // ... другие поля
}
