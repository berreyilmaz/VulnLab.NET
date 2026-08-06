using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace VulnLab.NET.Controllers;

[Route("labs/broken-auth")]
public class BrokenAuthLabController : Controller
{
    private static readonly ConcurrentDictionary<string, int> FailedAttempts = new(StringComparer.OrdinalIgnoreCase);
    private const int MaxAttempts = 3;
    private const string ValidUsername = "admin";
    private const string ValidPassword = "S3cure!Pass";

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/BrokenAuth.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(string? username, string? password)
    {
        username ??= string.Empty;
        password ??= string.Empty;

        ViewData["Mode"] = "Vulnerable";
        ViewData["Username"] = username;

        // KASITLI ZAFİYET: zayif parola + rate limit yok + ayirt edici hata mesaji
        if (string.Equals(username, ValidUsername, StringComparison.OrdinalIgnoreCase) &&
            (password == "admin" || password == "123456" || password == ValidPassword))
        {
            ViewData["Result"] = "Giris basarili (vulnerable): zayif parola kabul edildi.";
            return View("~/Views/Labs/BrokenAuth.cshtml");
        }

        if (!string.Equals(username, ValidUsername, StringComparison.OrdinalIgnoreCase))
        {
            ViewData["Result"] = "Hata: kullanici adi bulunamadi.";
        }
        else
        {
            ViewData["Result"] = "Hata: sifre yanlis.";
        }

        return View("~/Views/Labs/BrokenAuth.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(string? username, string? password)
    {
        username ??= string.Empty;
        password ??= string.Empty;

        ViewData["Mode"] = "Secure";
        ViewData["Username"] = username;

        var attempts = FailedAttempts.GetOrAdd(username, 0);
        if (attempts >= MaxAttempts)
        {
            ViewData["Result"] = "Hesap gecici kilitli: cok fazla basarisiz deneme.";
            return View("~/Views/Labs/BrokenAuth.cshtml");
        }

        var ok = string.Equals(username, ValidUsername, StringComparison.Ordinal) &&
                 password == ValidPassword;

        if (!ok)
        {
            FailedAttempts.AddOrUpdate(username, 1, (_, current) => current + 1);
            ViewData["Attempts"] = FailedAttempts[username];
            // GUVENLI: jenerik hata mesaji
            ViewData["Result"] = "Giris basarisiz: kullanici adi veya sifre hatali.";
            return View("~/Views/Labs/BrokenAuth.cshtml");
        }

        FailedAttempts.TryRemove(username, out _);
        ViewData["Result"] = "Giris basarili (secure): guclu parola ve rate limit aktif.";
        return View("~/Views/Labs/BrokenAuth.cshtml");
    }
}
