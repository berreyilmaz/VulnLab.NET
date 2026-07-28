using Microsoft.AspNetCore.Mvc;
using VulnLab.NET.Models;

namespace VulnLab.NET.Controllers;

[Route("labs/sql-injection")]
public class SqlInjectionLabController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/SqlInjection.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(string username)
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();

        // Demo tablo + veri
        var setup = connection.CreateCommand();
        setup.CommandText = """
            CREATE TABLE Users (Id INTEGER PRIMARY KEY, Username TEXT, Role TEXT);
            INSERT INTO Users (Username, Role) VALUES ('alice', 'admin');
            INSERT INTO Users (Username, Role) VALUES ('bob', 'user');
            INSERT INTO Users (Username, Role) VALUES ('charlie', 'user');
        """;
        setup.ExecuteNonQuery();

        // KASITLI ZAFİYET
        var sql = $"SELECT Id, Username, Role FROM Users WHERE Username = '{username}'";
        var cmd = connection.CreateCommand();
        cmd.CommandText = sql;

        var results = new List<LabUser>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new LabUser
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Role = reader.GetString(2)
            });
        }

        ViewData["Query"] = sql;
        ViewData["Input"] = username;
        return View("~/Views/Labs/SqlInjection.cshtml", results);
    }

    [HttpPost("secure")]
    public IActionResult Secure(string username)
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();

        var setup = connection.CreateCommand();
        setup.CommandText = """
            CREATE TABLE Users (Id INTEGER PRIMARY KEY, Username TEXT, Role TEXT);
            INSERT INTO Users (Username, Role) VALUES ('alice', 'admin');
            INSERT INTO Users (Username, Role) VALUES ('bob', 'user');
            INSERT INTO Users (Username, Role) VALUES ('charlie', 'user');
        """;
        setup.ExecuteNonQuery();

        // GUVENLI: parametreli sorgu
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Username, Role FROM Users WHERE Username = $username";
        cmd.Parameters.AddWithValue("$username", username ?? string.Empty);

        var results = new List<LabUser>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new LabUser
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Role = reader.GetString(2)
            });
        }

        ViewData["Query"] = cmd.CommandText;
        ViewData["Input"] = username;
        ViewData["Mode"] = "Secure";
        return View("~/Views/Labs/SqlInjection.cshtml", results);
    }
}