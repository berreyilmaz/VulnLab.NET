using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace VulnLab.NET.Controllers;

[Route("labs/ssrf")]
public class SsrfLabController : Controller
{
    private static readonly HashSet<string> AllowedHosts = new(StringComparer.OrdinalIgnoreCase)
    {
        "api.weather.com",
        "news.example.com"
    };

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Labs/Ssrf.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(string? targetUrl)
    {
        ViewData["Mode"] = "Vulnerable";
        ViewData["TargetUrl"] = targetUrl ?? string.Empty;
        ViewData["Result"] = ExecuteMockFetch(targetUrl, enforceSecurityChecks: false);
        return View("~/Views/Labs/Ssrf.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(string? targetUrl)
    {
        ViewData["Mode"] = "Secure";
        ViewData["TargetUrl"] = targetUrl ?? string.Empty;
        ViewData["Result"] = ExecuteMockFetch(targetUrl, enforceSecurityChecks: true);
        return View("~/Views/Labs/Ssrf.cshtml");
    }

    private static string ExecuteMockFetch(string? targetUrl, bool enforceSecurityChecks)
    {
        if (string.IsNullOrWhiteSpace(targetUrl))
        {
            return "URL bos olamaz.";
        }

        if (!Uri.TryCreate(targetUrl, UriKind.Absolute, out var uri))
        {
            return "URL gecersiz.";
        }

        if (enforceSecurityChecks)
        {
            if (!string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                return "Reddedildi: sadece HTTPS URL kabul edilir.";
            }

            if (!AllowedHosts.Contains(uri.Host))
            {
                return "Reddedildi: host allowlist disinda.";
            }

            if (IsInternalHost(uri.Host))
            {
                return "Reddedildi: internal/private adrese erisim engellendi.";
            }
        }

        return GetMockResponse(uri);
    }

    private static bool IsInternalHost(string host)
    {
        if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!IPAddress.TryParse(host, out var ip))
        {
            return false;
        }

        var bytes = ip.GetAddressBytes();
        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            return bytes[0] switch
            {
                10 => true,
                127 => true,
                172 when bytes[1] >= 16 && bytes[1] <= 31 => true,
                192 when bytes[1] == 168 => true,
                _ => false
            };
        }

        return IPAddress.IsLoopback(ip) || ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal;
    }

    private static string GetMockResponse(Uri uri)
    {
        var host = uri.Host.ToLowerInvariant();
        var path = uri.PathAndQuery.ToLowerInvariant();

        if (host is "localhost" or "127.0.0.1" && path.Contains("admin"))
        {
            return "INTERNAL DATA LEAK: admin panel response (simulated).";
        }

        if (host == "169.254.169.254")
        {
            return "INTERNAL DATA LEAK: cloud metadata token (simulated).";
        }

        return $"Dis servisten veri alindi (simulated): {uri}";
    }
}
