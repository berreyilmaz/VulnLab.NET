using Microsoft.AspNetCore.Mvc;

namespace VulnLab.NET.Controllers;

[Route("labs/cors")]
public class CorsLabController : Controller
{
    private static readonly HashSet<string> AllowedOrigins = new(StringComparer.OrdinalIgnoreCase)
    {
        "https://trusted.vulnlab.local"
    };

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/Cors.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(string? origin)
    {
        origin ??= string.Empty;
        ViewData["Mode"] = "Vulnerable";
        ViewData["Origin"] = origin;

        // KASITLI ZAFİYET: Origin yansitilir + credentials acik
        if (!string.IsNullOrWhiteSpace(origin))
        {
            Response.Headers["Access-Control-Allow-Origin"] = origin;
            Response.Headers["Access-Control-Allow-Credentials"] = "true";
            ViewData["Acao"] = origin;
            ViewData["Credentials"] = "true";
            ViewData["Result"] = "CORS yanlis yapilandi: Origin yansitildi ve credentials acik.";
        }
        else
        {
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Acao"] = "*";
            ViewData["Credentials"] = "false";
            ViewData["Result"] = "CORS yanlis yapilandi: Access-Control-Allow-Origin: *";
        }

        return View("~/Views/Labs/Cors.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(string? origin)
    {
        origin ??= string.Empty;
        ViewData["Mode"] = "Secure";
        ViewData["Origin"] = origin;

        if (AllowedOrigins.Contains(origin))
        {
            Response.Headers["Access-Control-Allow-Origin"] = origin;
            Response.Headers["Access-Control-Allow-Credentials"] = "true";
            ViewData["Acao"] = origin;
            ViewData["Credentials"] = "true";
            ViewData["Result"] = "CORS guvenli: Origin allowlist'te, header yazildi.";
        }
        else
        {
            ViewData["Acao"] = "(yok)";
            ViewData["Credentials"] = "false";
            ViewData["Result"] = "Reddedildi: Origin allowlist disinda, ACAO yazilmadi.";
        }

        return View("~/Views/Labs/Cors.cshtml");
    }
}
