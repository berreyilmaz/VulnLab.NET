using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VulnLab.NET.Models;

namespace VulnLab.NET.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static readonly IReadOnlyList<LabDefinition> Labs =
    [
        new() { Slug = "sql-injection", Title = "SQL Injection", Difficulty = "Beginner", Route = "/labs/sql-injection" },
        new() { Slug = "xss", Title = "Cross Site Scripting (XSS)", Difficulty = "Beginner", Route = "/labs/xss" },
        new() { Slug = "csrf", Title = "CSRF", Difficulty = "Beginner", Route = "/labs/csrf" },
        new() { Slug = "idor", Title = "IDOR", Difficulty = "Intermediate", Route = "/labs/idor" },
        new() { Slug = "file-upload", Title = "File Upload", Difficulty = "Intermediate", Route = "/labs/file-upload" },
        new() { Slug = "ssrf", Title = "SSRF", Difficulty = "Intermediate", Route = "/labs/ssrf" },
        new() { Slug = "xxe", Title = "XXE", Difficulty = "Intermediate", Route = "/labs/xxe" },
        new() { Slug = "jwt", Title = "JWT", Difficulty = "Intermediate", Route = "/labs/jwt" },
        new() { Slug = "race-condition", Title = "Race Condition", Difficulty = "Advanced", Route = "/labs/race-condition" },
        new() { Slug = "command-injection", Title = "Command Injection", Difficulty = "Advanced", Route = "/labs/command-injection" },
        new() { Slug = "open-redirect", Title = "Open Redirect", Difficulty = "Beginner", Route = "/labs/open-redirect" },
        new() { Slug = "path-traversal", Title = "Path Traversal", Difficulty = "Intermediate", Route = "/labs/path-traversal" },
        new() { Slug = "mass-assignment", Title = "Mass Assignment", Difficulty = "Intermediate", Route = "/labs/mass-assignment" },
        new() { Slug = "broken-auth", Title = "Broken Authentication", Difficulty = "Intermediate", Route = "/labs/broken-auth" },
        new() { Slug = "cors", Title = "CORS Misconfiguration", Difficulty = "Intermediate", Route = "/labs/cors" },
    ];

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(Labs);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
