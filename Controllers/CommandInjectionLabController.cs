using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace VulnLab.NET.Controllers;

[Route("labs/command-injection")]
public class CommandInjectionLabController : Controller
{
    private static readonly Regex SafeHostRegex = new("^[a-zA-Z0-9.-]+$", RegexOptions.Compiled);
    private static readonly string[] InjectionTokens = [";", "&&", "||", "|", "$(", "`"];

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/CommandInjection.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(string? host)
    {
        host ??= string.Empty;
        ViewData["Mode"] = "Vulnerable";
        ViewData["Host"] = host;

        var command = $"ping -c 1 {host}";
        ViewData["Command"] = command;

        var hasInjection = InjectionTokens.Any(t => host.Contains(t, StringComparison.Ordinal));
        ViewData["Result"] = hasInjection
            ? "Komut enjeksiyonu tespit edildi (simulated): ek komutlar calistirilabilirdi."
            : "Komut calisti (simulated): sadece ping islemi gorundu.";

        return View("~/Views/Labs/CommandInjection.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(string? host)
    {
        host ??= string.Empty;
        ViewData["Mode"] = "Secure";
        ViewData["Host"] = host;

        if (string.IsNullOrWhiteSpace(host))
        {
            ViewData["Result"] = "Reddedildi: host bos olamaz.";
            return View("~/Views/Labs/CommandInjection.cshtml");
        }

        if (!SafeHostRegex.IsMatch(host))
        {
            ViewData["Result"] = "Reddedildi: yalnizca hostname karakterleri kabul edilir.";
            return View("~/Views/Labs/CommandInjection.cshtml");
        }

        if (InjectionTokens.Any(t => host.Contains(t, StringComparison.Ordinal)))
        {
            ViewData["Result"] = "Reddedildi: injection karakter deseni tespit edildi.";
            return View("~/Views/Labs/CommandInjection.cshtml");
        }

        var command = "ping -c 1 <validated-host>";
        ViewData["Command"] = command;
        ViewData["Result"] = $"Guvenli komut calisti (simulated): {host}";
        return View("~/Views/Labs/CommandInjection.cshtml");
    }
}
