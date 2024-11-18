using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net; // Не забудьте установить пакет BCrypt.Net

[ApiController]
[Route("api/[controller]")]
public class UserTestsController : ControllerBase
{
    private readonly string _connectionString;
    private readonly ILogger<UserTestsController> _logger; // Для логирования

    public UserTestsController(IConfiguration configuration, ILogger<UserTestsController> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    [HttpPost("tests")]
    public async Task<IActionResult> CreateTest([FromBody] TestCreationDto testDto, int userId)
    {
        // Авторизация (Пример -  проверка, что userId соответствует текущему пользователю)
        // В реальном приложении используйте полноценную систему авторизации (JWT, etc.)
        if (userId <= 0 || !User.IsInRole("Teacher")) //Пример проверки роли
        {
            return Unauthorized();
        }


        // Валидация данных
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            try
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction()) //Транзакция для атомарности операции
                {
                    using (var command = new SqlCommand("INSERT INTO tests (TestName, Description, CreatorId) VALUES (@testName, @description, @creatorId); SELECT SCOPE_IDENTITY();", connection, transaction))
                    {
                        command.Parameters.AddWithValue("@testName", testDto.TestName);
                        command.Parameters.AddWithValue("@description", testDto.Description);
                        command.Parameters.AddWithValue("@creatorId", userId);
                        var newTestId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        // ...  (здесь можно добавить вставку вопросов и ответов в связанные таблицы) ...

                        transaction.Commit();
                        return CreatedAtAction(nameof(GetTest), new { id = newTestId }, new Test { TestId = newTestId, TestName = testDto.TestName, Description = testDto.Description, CreatorId = userId });
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error during test creation"); // Логирование ошибки
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test"); //Логирование общих ошибок
                return StatusCode(500, "Internal Server Error");
            }
        }
    }


    [HttpGet("tests/{id}")]
    public async Task<IActionResult> GetTest(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            try
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM tests WHERE TestId = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return Ok(new Test
                            {
                                TestId = reader.GetInt32(0),
                                TestName = reader.GetString(1),
                                Description = reader.GetString(2),
                                CreatorId = reader.GetInt32(3)
                            });
                        }
                        return NotFound();
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error during test retrieval");
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }


    [HttpPut("tests/{id}")]
    public async Task<IActionResult> UpdateTest(int id, [FromBody] TestUpdateDto testDto)
    {
        // ... (аналогичная обработка, как в CreateTest, но с UPDATE запросом) ...
    }

    [HttpGet("results")]
    public async Task<IActionResult> GetUserResults(int userId)
    {
        // ... (код получения результатов, аналогичный предыдущему, но с обработкой ошибок) ...
    }


}


// DTO (Data Transfer Objects) для предотвращения передачи лишних данных
public class TestCreationDto
{
  [Required]
  public string TestName { get; set; }
  public string Description { get; set; }
  // ... другие поля
}

public class TestUpdateDto
{
  public string TestName { get; set; }
  public string Description { get; set; }
  // ... другие поля
}

public class Test
{
    public int TestId { get; set; }
    public string TestName { get; set; }
    public string Description { get; set; }
    public int CreatorId { get; set; }
    // ... другие поля
}
