using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace VulnLab.NET.Controllers;

[Route("labs/jwt")]
public class JwtLabController : Controller
{
    private const string DemoSecret = "VulnLabSuperSecretKey";

    [HttpGet("")]
    public IActionResult Index()
    {
        var safeToken = BuildDemoToken("alice", "user");
        ViewData["SafeDemoToken"] = safeToken;
        ViewData["ForgedAdminToken"] = BuildForgedAdminToken(safeToken);
        return View("~/Views/Labs/Jwt.cshtml");
    }

    [HttpPost("vulnerable")]
    public IActionResult Vulnerable(string? token)
    {
        ViewData["Mode"] = "Vulnerable";
        ViewData["InputToken"] = token ?? string.Empty;
        var safeToken = BuildDemoToken("alice", "user");
        ViewData["SafeDemoToken"] = safeToken;
        ViewData["ForgedAdminToken"] = BuildForgedAdminToken(safeToken);

        var payload = ReadPayload(token);
        if (payload is null)
        {
            ViewData["Result"] = "Token parse edilemedi.";
            return View("~/Views/Labs/Jwt.cshtml");
        }

        var role = payload.RootElement.TryGetProperty("role", out var roleEl) ? roleEl.GetString() : "unknown";
        ViewData["ParsedPayload"] = payload.RootElement.ToString();
        ViewData["Result"] = role == "admin"
            ? "Erisim verildi (vulnerable): imza dogrulanmadan admin kabul edildi."
            : "Erisim sinirli: admin yetkisi yok.";
        return View("~/Views/Labs/Jwt.cshtml");
    }

    [HttpPost("secure")]
    public IActionResult Secure(string? token)
    {
        ViewData["Mode"] = "Secure";
        ViewData["InputToken"] = token ?? string.Empty;
        var safeToken = BuildDemoToken("alice", "user");
        ViewData["SafeDemoToken"] = safeToken;
        ViewData["ForgedAdminToken"] = BuildForgedAdminToken(safeToken);

        if (!TryValidateHs256Token(token, DemoSecret, out var payloadJson, out var error))
        {
            ViewData["Result"] = $"Token reddedildi: {error}";
            return View("~/Views/Labs/Jwt.cshtml");
        }

        using var payload = JsonDocument.Parse(payloadJson);
        var role = payload.RootElement.TryGetProperty("role", out var roleEl) ? roleEl.GetString() : "unknown";
        ViewData["ParsedPayload"] = payload.RootElement.ToString();
        ViewData["Result"] = role == "admin"
            ? "Erisim verildi: imza dogrulandi ve rol admin."
            : "Erisim sinirli: imza gecerli ama rol admin degil.";
        return View("~/Views/Labs/Jwt.cshtml");
    }

    private static JsonDocument? ReadPayload(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var parts = token.Split('.');
        if (parts.Length < 2)
        {
            return null;
        }

        try
        {
            var payloadBytes = Base64UrlDecode(parts[1]);
            return JsonDocument.Parse(payloadBytes);
        }
        catch
        {
            return null;
        }
    }

    private static bool TryValidateHs256Token(string? token, string secret, out string payloadJson, out string error)
    {
        payloadJson = string.Empty;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(token))
        {
            error = "Token bos.";
            return false;
        }

        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            error = "JWT formati gecersiz.";
            return false;
        }

        try
        {
            var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
            using var header = JsonDocument.Parse(headerJson);
            var alg = header.RootElement.TryGetProperty("alg", out var algEl) ? algEl.GetString() : null;
            if (!string.Equals(alg, "HS256", StringComparison.Ordinal))
            {
                error = "Sadece HS256 kabul edilir.";
                return false;
            }

            var signedData = $"{parts[0]}.{parts[1]}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var expectedSignature = Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(signedData)));
            if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(parts[2])))
            {
                error = "Imza gecersiz.";
                return false;
            }

            payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            return true;
        }
        catch
        {
            error = "Token parse/dogrulama hatasi.";
            return false;
        }
    }

    private static string BuildDemoToken(string username, string role)
    {
        var headerJson = """{"alg":"HS256","typ":"JWT"}""";
        var payloadJson = $"{{\"sub\":\"{username}\",\"role\":\"{role}\"}}";
        var header = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        var payload = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        var signedData = $"{header}.{payload}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(DemoSecret));
        var signature = Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(signedData)));
        return $"{header}.{payload}.{signature}";
    }

    private static string BuildForgedAdminToken(string safeToken)
    {
        var parts = safeToken.Split('.');
        if (parts.Length != 3)
        {
            return safeToken;
        }

        var forgedPayloadJson = """{"sub":"alice","role":"admin"}""";
        var forgedPayload = Base64UrlEncode(Encoding.UTF8.GetBytes(forgedPayloadJson));

        // Intentional forgery for lab: payload changes, original signature kept.
        return $"{parts[0]}.{forgedPayload}.{parts[2]}";
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var padded = input.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
        return Convert.FromBase64String(padded);
    }
}
