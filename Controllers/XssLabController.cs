using Microsoft.AspNetCore.Mvc;

namespace VulnLab.NET.Controllers;

[Route("labs/xss")]
public class XssLabController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/Xss.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(string? comment)
    {
        ViewData["Mode"] = "Vulnerable";
        ViewData["Input"] = comment ?? string.Empty;
        return View("~/Views/Labs/Xss.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(string? comment)
    {
        ViewData["Mode"] = "Secure";
        ViewData["Input"] = comment ?? string.Empty;
        return View("~/Views/Labs/Xss.cshtml");
    }
}
